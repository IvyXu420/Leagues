using System.Text.Json;
using Leagues.Models.Client;
using Leagues.Models.Dto;
using static Leagues.Models.Client.LcuConnection;
using static Leagues.Models.Logging.Logging;

namespace Leagues.Models.Services;

// TODO: simplify the code according to the actual JSON response

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

public static class MatchStats
{
    public static async Task<IReadOnlyList<SummonerStats>> LoadAsync(bool friends,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var sessionResponse = await LcuHttpClient.GetAsync(
                friends ? LcuEndPoint.ChampSelectSession : LcuEndPoint.GameflowSession,
                cancellationToken);
            if (!sessionResponse.IsSuccessStatusCode)
                return [];

            using var sessionStream = await sessionResponse.Content.ReadAsStreamAsync();
            using var session = await JsonDocument.ParseAsync(sessionStream, cancellationToken: cancellationToken);
            var participants = await ReadParticipantsAsync(session.RootElement, friends, cancellationToken);
            if (participants.Count == 0)
                return [];

            var stats = await Task.WhenAll(participants.Take(5)
                .Select(participant => LoadParticipantAsync(participant, cancellationToken)));
            return stats.Where(static stat => stat is not null).Cast<SummonerStats>().ToList();
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            Logger.Error($"Match stats fetch failed: {ex.Message}");
            return [];
        }
    }

    private static async Task<SummonerStats?> LoadParticipantAsync(JsonElement participant,
        CancellationToken cancellationToken)
    {
        if (!TryGetLong(participant, "summonerId", out var summonerId))
            return null;

        using var response = await LcuHttpClient.GetAsync(LcuEndPoint.Summoner(summonerId), cancellationToken);
        if (!response.IsSuccessStatusCode)
            return null;

        using var profileStream = await response.Content.ReadAsStreamAsync();
        using var profile = await JsonDocument.ParseAsync(profileStream, cancellationToken: cancellationToken);
        var profileRoot = profile.RootElement;
        var puuid = GetString(profileRoot, "puuid") ?? GetString(participant, "puuid");
        if (string.IsNullOrWhiteSpace(puuid))
            return null;
        var playerUuid = puuid!;

        var playerName = GetString(profileRoot, "gameName") ??
                         GetString(profileRoot, "displayName") ??
                         GetString(profileRoot, "summonerName") ??
                         GetString(participant, "summonerName") ?? "Unknown";
        var tagLine = GetString(profileRoot, "tagLine");
        if (!string.IsNullOrWhiteSpace(tagLine) && !playerName.Contains('#'))
            playerName = $"{playerName}#{tagLine}";

        var historyResponse = await LcuHttpClient.GetAsync(LcuEndPoint.MatchHistory(playerUuid), cancellationToken);
        if (!historyResponse.IsSuccessStatusCode)
            return CreateStats(profileRoot, summonerId, playerName, playerUuid, 0, 0);

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
        return CreateStats(profileRoot, summonerId, playerName, playerUuid, kda,
            gameCount == 0 ? 0 : (double)wins / gameCount * 100);
    }

    private static SummonerStats CreateStats(JsonElement profile, long summonerId, string name, string puuid,
        double kda, double winRate)
        => new(summonerId, name, GetInt(profile, "summonerLevel"), GetInt(profile, "profileIconId"), kda,
            winRate, puuid);

    private static async Task<List<JsonElement>> ReadParticipantsAsync(JsonElement root, bool friends,
        CancellationToken cancellationToken)
    {
        if (friends)
            return ReadArray(root, "myTeam");

        var enemy = ReadArray(root, "theirTeam");
        if (enemy.Count > 0)
            return enemy;

        if (root.TryGetProperty("gameData", out var gameData) && gameData.ValueKind == JsonValueKind.Object)
            root = gameData;
        if (root.TryGetProperty("teams", out var teams) && teams.ValueKind == JsonValueKind.Object)
            root = teams;

        var first = ReadArray(root, "teamOne");
        var second = ReadArray(root, "teamTwo");
        if (first.Count == 0 || second.Count == 0)
            return [];

        var currentSummonerId = await ReadCurrentSummonerIdAsync(cancellationToken);
        return first.Any(player => GetLong(player, "summonerId") == currentSummonerId) ? second : first;
    }

    private static async Task<long> ReadCurrentSummonerIdAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var response = await LcuHttpClient.GetAsync(LcuEndPoint.CurrentSummoner, cancellationToken);
            using var stream = await response.Content.ReadAsStreamAsync();
            using var document = JsonDocument.Parse(stream);
            return GetLong(document.RootElement, "summonerId");
        }
        catch
        {
            return 0;
        }
    }

    private static List<JsonElement> ReadArray(JsonElement root, string propertyName)
        => root.TryGetProperty(propertyName, out var array) && array.ValueKind == JsonValueKind.Array
            ? [.. array.EnumerateArray()]
            : [];

    private static string? GetString(JsonElement element, string propertyName)
        => element.TryGetProperty(propertyName, out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;

    private static int GetInt(JsonElement element, string propertyName)
        => element.TryGetProperty(propertyName, out var value) && value.TryGetInt32(out var result) ? result : 0;

    private static long GetLong(JsonElement element, string propertyName)
        => element.TryGetProperty(propertyName, out var value) && value.TryGetInt64(out var result) ? result : 0;

    private static bool TryGetLong(JsonElement element, string propertyName, out long result)
    {
        result = GetLong(element, propertyName);
        return result != 0;
    }
}