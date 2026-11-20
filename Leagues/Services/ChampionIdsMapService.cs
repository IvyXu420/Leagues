using System.Text.Json;
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

        using var response = await LcuNet.Client.GetAsync(
            LcuEndPoint.ChampionSummary, cancellationToken);
        if (!response.IsSuccessStatusCode)
            return new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        using var stream = await response.Content.ReadAsStreamAsync();
        var champions = await JsonSerializer.DeserializeAsync<List<ChampionSummaryDto>>(
            stream, cancellationToken: cancellationToken) ?? [];
        var result = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var champion in champions)
        {
            if (!string.IsNullOrWhiteSpace(champion.Name))
                result[champion.Name] = champion.Id;
        }

        championIds = result;
        return championIds;
    }
}