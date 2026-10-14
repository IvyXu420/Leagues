using System.Text.Json;
using Leagues.Client;
using Leagues.Dto;
using static Leagues.Client.LcuConnection;
using static Leagues.Logging.Logging;

namespace Leagues.Services;

public sealed class ParticipantStats(
    string playerName,
    int summonerLevel,
    int profileIconId,
    double kda,
    double winRate
    )
{
    public string PlayerName { get; } = playerName;
    public int SummonerLevel { get; } = summonerLevel;
    public int ProfileIconId { get; } = profileIconId;
    public double Kda { get; } = kda;
    public double WinRate { get; } = winRate;
}

public static class ParticipantsStats
{
    public static async Task<IReadOnlyList<ParticipantStats>> LoadAsync(bool friendly,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (friendly)
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
        if (!Credential.TryGetCredentials(out var credentials))
            return [];

        using var client = LcuConnectionFactory.CreateHttpClient(LcuEndPoint.LiveClientDataPlayerList(), credentials);
        using var response = await client.GetAsync(LcuEndPoint.LiveClientDataPlayerList(), cancellationToken);
        if (!response.IsSuccessStatusCode)
            return [];

        using var stream = await response.Content.ReadAsStreamAsync();
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        var stats = await Task.WhenAll(document.RootElement.EnumerateArray()
            .Select(participant => ParseLiveDataParticipantAsync(participant, cancellationToken)));
        return [.. stats.OfType<ParticipantStats>()];
    }

    private static async Task<IReadOnlyList<ParticipantStats>> LoadStatsChampSelectAsync(
        CancellationToken cancellationToken)
    {
        using var conversationsResponse = await LcuHttpClient.GetAsync(
            LcuEndPoint.ChatConversations, cancellationToken);
        if (!conversationsResponse.IsSuccessStatusCode)
            return [];

        using var conversationsStream = await conversationsResponse.Content.ReadAsStreamAsync();
        using var conversations = await JsonDocument.ParseAsync(conversationsStream,
            cancellationToken: cancellationToken);
        var conversationId = conversations.RootElement[0].GetProperty("id").GetString()!;

        using var participantsResponse = await LcuHttpClient.GetAsync(
            LcuEndPoint.ChatConversationParticipants(conversationId), cancellationToken);
        if (!participantsResponse.IsSuccessStatusCode)
            return [];

        using var participantsStream = await participantsResponse.Content.ReadAsStreamAsync();
        using var participants = await JsonDocument.ParseAsync(participantsStream,
            cancellationToken: cancellationToken);
        var stats = await Task.WhenAll(participants.RootElement.EnumerateArray()
            .Select(participant => ParseChatParticipantAsync(participant, cancellationToken)));
        return [.. stats.OfType<ParticipantStats>()];
    }

    private static async Task<ParticipantStats?> ParseChatParticipantAsync(JsonElement participant,
        CancellationToken cancellationToken)
    {
        var puuid = participant.GetProperty("puuid").GetString()!;

        var playerName = participant.GetProperty("gameName").GetString()!;
        var tagLine = participant.GetProperty("gameTag").GetString();
        if (!string.IsNullOrWhiteSpace(tagLine))
            playerName = $"{playerName}#{tagLine}";

        var lol = participant.GetProperty("lol");
        int.TryParse(lol.GetProperty("level").GetString()!, out var level);
        var profileIconId = participant.GetProperty("icon").GetInt32();
        return await CreateSummonerStatsAsync(playerName, puuid, level,
            profileIconId, cancellationToken);
    }

    private static async Task<ParticipantStats?> ParseLiveDataParticipantAsync(JsonElement participant,
        CancellationToken cancellationToken)
    {
        try
        {
            var playerName = participant.GetProperty("summonerName").GetString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(playerName))
                return null;

            var puuid = await Uuid.FetchPlayerUuidAsync(playerName);
            if (puuid == null)
                return null;

            using var response = await LcuHttpClient.GetAsync(
                LcuEndPoint.SummonerOverView(puuid), cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                Logger.Warn($"Summoner overview failed for {playerName}: " +
                    $"{(int)response.StatusCode} {response.ReasonPhrase}");
                return null;
            }

            using var stream = await response.Content.ReadAsStreamAsync();
            var overview = await JsonSerializer.DeserializeAsync<LiveClientPlayerListDto>(
                stream, cancellationToken: cancellationToken);
            if (overview is null)
                return null;

            return await CreateSummonerStatsAsync(playerName, puuid, overview.SummonerLevel,
                overview.ProfileIconId, cancellationToken);
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
        string playerName, string puuid, int summonerLevel, int profileIconId,
        CancellationToken cancellationToken)
    {
        var historyResponse = await LcuHttpClient.GetAsync(LcuEndPoint.MatchHistory(puuid), cancellationToken);
        if (!historyResponse.IsSuccessStatusCode)
            return new(playerName, summonerLevel, profileIconId, 0, 0);

        using var historyStream = await historyResponse.Content.ReadAsStreamAsync();
        var history = await JsonSerializer.DeserializeAsync<MatchHistoryResponse>(historyStream,
            cancellationToken: cancellationToken);
        var games = history?.Games.Games ?? [];
        var playerGames = games.SelectMany(static game => game.Participants).ToList();
        var gameCount = playerGames.Count;
        var wins = playerGames.Count(static game => game.Stats.Win);
        var kills = playerGames.Sum(static game => game.Stats.Kills);
        var deaths = playerGames.Sum(static game => game.Stats.Deaths);
        var assists = playerGames.Sum(static game => game.Stats.Assists);
        var kda = deaths == 0 ? kills + assists : (double)(kills + assists) / deaths;
        return new(playerName, summonerLevel, profileIconId, kda,
            gameCount == 0 ? 0 : (double)wins / gameCount * 100);
    }
}