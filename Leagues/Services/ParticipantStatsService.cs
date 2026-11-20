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
        var participants = await LcuNet.GetJsonAsync<List<LiveDataParticipantDto>>(
            LcuNet.LiveClient, LcuEndPoint.LiveClientDataPlayerList(), cancellationToken
        ) ?? [];

        var championIdMap = await ChampionIdsMapService.GetMapAsync(cancellationToken);
        var stats = await Task.WhenAll(participants
            .Select(participant => ParseLiveDataParticipantAsync(
                participant, cancellationToken)));
        return [.. stats.OfType<ParticipantStats>()];
    }

    private static async Task<IReadOnlyList<ParticipantStats>> LoadStatsChampSelectAsync(
        CancellationToken cancellationToken)
    {
        var conversations = await LcuNet.GetJsonAsync<List<ChatConversationDto>>(
            LcuEndPoint.ChatConversations, cancellationToken);
        if (conversations is null || conversations.Count == 0)
        {
            Logger.Warn("No chat conversation is available for champion select.");
            return [];
        }

        var conversationId = conversations[0].Id;
        if (string.IsNullOrWhiteSpace(conversationId))
            return [];

        var participants = await LcuNet.GetJsonAsync<List<ChatParticipantDto>>(
            LcuEndPoint.ChatConversationParticipants(conversationId), cancellationToken) ?? [];
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
        CancellationToken cancellationToken)
    {
        try
        {
            var playerName = participant.SummonerName;
            var championName = participant.ChampionName;
            // Render the champion avatar in-game.
            var championIdMap = await ChampionIdsMapService.GetMapAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(playerName) ||
                !championIdMap.TryGetValue(championName, out var championId))
                return null;

            var summoner = await LcuNet.GetJsonAsync<SummonerDto>(
                LcuEndPoint.Summoner(playerName), cancellationToken);
            if (summoner == null)
                return null;
            var puuid = summoner.Puuid;

            var overview = await LcuNet.GetJsonAsync<SummonerDto>(
                LcuEndPoint.SummonerOverView(puuid), cancellationToken);
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
        var history = await LcuNet.GetJsonAsync<MatchHistoryDto>(LcuEndPoint.MatchHistory(puuid), cancellationToken);
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