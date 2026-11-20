using Leagues.Client;
using Leagues.Dto;
using Leagues.Dto.MatchHistory;
using Leagues.Models;

namespace Leagues.Mappers;

public static class MatchMapper
{
    private static readonly Dictionary<string, List<MatchSummary>> Cache = [];

    public static async Task<List<MatchSummary>?> ToSummaries(string playerName, int begIndex, int endIndex)
    {
        // Fetch PUUID to cache the result.
        var summoner = await LcuNet.GetJsonAsync<SummonerDto>(
            LcuEndPoint.Summoner(playerName));
        if (summoner is null)
            return null;

        if (Cache.TryGetValue(summoner.Puuid, out var res))
            return res;

        // Not in the cache, then fetch them from the LCU API.
        var history = await LcuNet.GetJsonAsync<MatchHistoryDto>(LcuEndPoint.MatchHistory(playerName, begIndex, endIndex));
        if (history is null)
            return null;

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
        Cache.Add(summoner.Puuid, summaries);
        return summaries;
    }
}