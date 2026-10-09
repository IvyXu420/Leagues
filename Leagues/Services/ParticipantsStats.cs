using System.Text.Json;
using Leagues.Client;
using Leagues.Dto;
using static Leagues.Client.LcuConnection;
using static Leagues.Logging.Logging;

namespace Leagues.Services;

// TODO: simplify the code according to the actual JSON response
// session part seems like hallucination

public sealed class SummonerStats(
    long summonerId,
    string playerName,
    int summonerLevel,
    int profileIconId,
    double kda,
    double winRate,
    string puuid)
{
    public long SummonerId { get; } = summonerId;
    public string PlayerName { get; } = playerName;
    public int SummonerLevel { get; } = summonerLevel;
    public int ProfileIconId { get; } = profileIconId;
    public double Kda { get; } = kda;
    public double WinRate { get; } = winRate;
    public string Puuid { get; } = puuid;
}

public static class ParticipantsStats
{
    public static async Task<IReadOnlyList<SummonerStats>> LoadAsync(bool friends,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (friends)
                return await LoadChampSelectStatsAsync(cancellationToken);
            else
                return [];
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            Logger.Error($"Match stats fetch failed: {ex.Message}");
            return [];
        }
    }

    private static async Task<IReadOnlyList<SummonerStats>> LoadChampSelectStatsAsync(
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
            .Select(participant => LoadChatParticipantAsync(participant, cancellationToken)));
        return [.. stats.OfType<SummonerStats>()];
    }

    private static async Task<SummonerStats?> LoadChatParticipantAsync(JsonElement participant,
        CancellationToken cancellationToken)
    {
        var summonerId = participant.GetProperty("summonerId").GetInt64();
        var puuid = participant.GetProperty("puuid").GetString()!;

        var playerName = participant.GetProperty("gameName").GetString()!;
        var tagLine = participant.GetProperty("gameTag").GetString();
        if (!string.IsNullOrWhiteSpace(tagLine))
            playerName = $"{playerName}#{tagLine}";

        var lol = participant.GetProperty("lol");
        var summonerLevel = lol.GetProperty("level").GetInt32();
        var profileIconId = participant.GetProperty("icon").GetInt32();
        return await LoadHistoryStatsAsync(participant, summonerId, playerName, puuid, summonerLevel,
            profileIconId, cancellationToken);
    }

    private static async Task<SummonerStats> LoadHistoryStatsAsync(JsonElement profile, long summonerId,
        string playerName, string puuid, int summonerLevel, int profileIconId,
        CancellationToken cancellationToken)
    {
        var historyResponse = await LcuHttpClient.GetAsync(LcuEndPoint.MatchHistory(puuid), cancellationToken);
        if (!historyResponse.IsSuccessStatusCode)
            return CreateStats(summonerLevel, profileIconId, summonerId, playerName, puuid, 0, 0);

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
        return CreateStats(summonerLevel, profileIconId, summonerId, playerName, puuid, kda,
            gameCount == 0 ? 0 : (double)wins / gameCount * 100);
    }

    private static SummonerStats CreateStats(int summonerLevel, int profileIconId, long summonerId, string name,
        string puuid, double kda, double winRate)
        => new(summonerId, name, summonerLevel, profileIconId, kda, winRate, puuid);
}