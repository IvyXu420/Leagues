using System.Text.Json;
using Leagues.Client;
using Leagues.Dto;
using Leagues.Dto.Chat;
using Leagues.Dto.LiveClient;
using Leagues.Dto.MatchHistory;
using Leagues.Logging;
using Leagues.Models;

namespace Leagues.Services;

public static class ParticipantStatsService
{
    public static async Task<IReadOnlyList<ParticipantStats>> LoadAsync(bool ingame,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ingame)
                return await LoadStatsChampSelectAsync(cancellationToken);
            return await LoadStatsLiveDataAsync(cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            Logger.Error($"Match stats fetch failed: {ex.Message}");
            return [];
        }
    }

    private static async Task<IReadOnlyList<ParticipantStats>> LoadStatsLiveDataAsync
        (CancellationToken cancellationToken)
    {
        using var response = await LcuNet.Client.GetAsync(
            LcuEndPoint.LiveClientDataPlayerList(), cancellationToken);
        if (!response.IsSuccessStatusCode)
            return [];

        using var stream = await response.Content.ReadAsStreamAsync();
        var participants = await JsonSerializer.DeserializeAsync<List<LiveDataParticipantDto>>(
            stream, cancellationToken: cancellationToken) ?? [];
        var championIdMap = await ChampionIdsMapService.GetMapAsync(cancellationToken);
        var stats = await Task.WhenAll(participants
            .Select(participant => ParseLiveDataParticipantAsync(
                participant, championIdMap, cancellationToken)));
        return [.. stats.OfType<ParticipantStats>()];
    }

    /// <summary>
    /// Load participants' stats during the select phase.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private static async Task<IReadOnlyList<ParticipantStats>> LoadStatsChampSelectAsync(
        CancellationToken cancellationToken)
    {
        using var conversationsResponse = await LcuNet.Client.GetAsync(
            LcuEndPoint.ChatConversations, cancellationToken);
        if (!conversationsResponse.IsSuccessStatusCode)
            return [];
        using var conversationsStream = await conversationsResponse.Content.ReadAsStreamAsync();
        using var conversations = await JsonDocument.ParseAsync(conversationsStream,
            cancellationToken: cancellationToken);
        if (conversations.RootElement.ValueKind != JsonValueKind.Array ||
            conversations.RootElement.GetArrayLength() == 0 ||
            !conversations.RootElement[0].TryGetProperty("id", out var conversationIdElement) ||
            conversationIdElement.ValueKind != JsonValueKind.String)
        {
            Logger.Warn("No chat conversation is available for champion select.");
            return [];
        }

        var conversationId = conversationIdElement.GetString();
        if (string.IsNullOrWhiteSpace(conversationId))
            return [];

        using var participantsResponse = await LcuNet.Client.GetAsync(
            LcuEndPoint.ChatConversationParticipants(conversationId!), cancellationToken);
        if (!participantsResponse.IsSuccessStatusCode)
            return [];
        using var participantsStream = await participantsResponse.Content.ReadAsStreamAsync();
        var participants = await JsonSerializer.DeserializeAsync<List<ChatParticipantDto>>(
            participantsStream, cancellationToken: cancellationToken) ?? [];
        var stats = await Task.WhenAll(participants
            .Select(participant => ParseChatParticipantAsync(participant, cancellationToken)));
        return [.. stats.OfType<ParticipantStats>()];
    }

    private static async Task<ParticipantStats?> ParseChatParticipantAsync(
        ChatParticipantDto participant,
        CancellationToken cancellationToken)
    {
        var puuid = participant.Puuid;

        var playerName = participant.GameName;
        var tagLine = participant.GameTag;
        playerName = $"{playerName}#{tagLine}";

        int.TryParse(participant.Lol.Level, out var level);
        return await CreateSummonerStatsAsync(playerName, puuid, level,
            participant.Icon, cancellationToken);
    }

    private static async Task<ParticipantStats?> ParseLiveDataParticipantAsync(
        LiveDataParticipantDto participant,
        IReadOnlyDictionary<string, int> championIdMap,
        CancellationToken cancellationToken)
    {
        try
        {
            var playerName = participant.SummonerName;
            var championName = participant.ChampionName;
            if (string.IsNullOrWhiteSpace(playerName) ||
                !championIdMap.TryGetValue(championName, out var championId))
                return null;

            var puuid = await Uuid.FetchPlayerUuidAsync(playerName);
            if (puuid == null)
                return null;

            using var overviewResponse = await LcuNet.Client.GetAsync(
                LcuEndPoint.SummonerOverView(puuid), cancellationToken);
            if (!overviewResponse.IsSuccessStatusCode)
                return null;
            using var overviewStream = await overviewResponse.Content.ReadAsStreamAsync();
            var overview = await JsonSerializer.DeserializeAsync<SummonerDto>(
                overviewStream, cancellationToken: cancellationToken);
            if (overview is null)
                return null;

            return await CreateSummonerStatsAsync(playerName, puuid, overview.SummonerLevel,
                championId, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Logger.Warn($"Failed to load one live participant: {ex.Message}");
            return null;
        }
    }

    private static async Task<ParticipantStats> CreateSummonerStatsAsync(
        string playerName, string puuid, int summonerLevel, int avatarIconId,
        CancellationToken cancellationToken)
    {
        using var historyResponse = await LcuNet.Client.GetAsync(
            LcuEndPoint.MatchHistory(puuid), cancellationToken);
        if (!historyResponse.IsSuccessStatusCode)
            return new(playerName, summonerLevel, avatarIconId, 0, 0);
        using var historyStream = await historyResponse.Content.ReadAsStreamAsync();
        var history = await JsonSerializer.DeserializeAsync<MatchHistoryDto>(
            historyStream, cancellationToken: cancellationToken);
        var games = history?.Games.Games ?? [];
        var playerGames = games.SelectMany(static game => game.Participants).ToList();
        var gameCount = playerGames.Count;
        var wins = playerGames.Count(static game => game.Stats.Win);
        var kills = playerGames.Sum(static game => game.Stats.Kills);
        var deaths = playerGames.Sum(static game => game.Stats.Deaths);
        var assists = playerGames.Sum(static game => game.Stats.Assists);
        var kda = deaths == 0 ? kills + assists : (double)(kills + assists) / deaths;
        return new(playerName, summonerLevel, avatarIconId, kda,
            gameCount == 0 ? 0 : (double)wins / gameCount * 100);
    }
}