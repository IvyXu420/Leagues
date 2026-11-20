using System.Text.Json;
using Leagues.Client;
using Leagues.Dto.MatchHistory;
using Leagues.Models;
using Leagues.Services;

namespace Leagues.Mappers;

public static class MatchMapper
{
    private static readonly Dictionary<string, List<MatchSummary>> Cache = [];

    public static async Task<List<MatchSummary>?> ToSummaries(string playerName, int begIndex, int endIndex)
    {
        var json = await MatchService.QueryAsync(playerName, begIndex, endIndex);
        if (json == null)
            return null;

        var playerUuid = await Uuid.FetchPlayerUuidAsync(playerName);
        if (playerUuid == null)
            return null;

        if (Cache.TryGetValue(playerUuid, out var res))
            return res;

        var history = JsonSerializer.Deserialize<MatchHistoryDto>(json);
        var summaries = history!.Games.Games
            .Select(game =>
            {
                var participant = game.Participants[0];
                return new MatchSummary(
                    gameId: game.GameId,
                    playedAt: DateTimeOffset.FromUnixTimeMilliseconds(game.GameCreation),
                    duration: TimeSpan.FromSeconds(game.GameDuration),
                    gameMode: game.GameMode,
                    championId: participant.ChampionId,
                    win: participant.Stats.Win,
                    kills: participant.Stats.Kills,
                    deaths: participant.Stats.Deaths,
                    assists: participant.Stats.Assists);
            })
            .ToList();
        Cache.Add(playerUuid, summaries);
        return summaries;
    }
}