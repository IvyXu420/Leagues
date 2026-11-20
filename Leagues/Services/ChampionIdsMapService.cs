using Leagues.Client;
using Leagues.Dto;

namespace Leagues.Services;

public static class ChampionIdsMapService
{
    private static IReadOnlyDictionary<string, int>? championIds;

    public static async Task<IReadOnlyDictionary<string, int>> GetMapAsync(
        CancellationToken cancellationToken)
    {
        if (championIds is not null)
            return championIds;

        var champions = await LcuNet.GetJsonAsync<List<ChampionSummaryDto>>(
            LcuEndPoint.ChampionSummary, cancellationToken
        );
        var result = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        if (champions is null)
            return result;

        foreach (var champion in champions)
        {
            if (!string.IsNullOrWhiteSpace(champion.Name))
                result[champion.Name] = champion.Id;
        }

        championIds = result;
        return championIds;
    }
}