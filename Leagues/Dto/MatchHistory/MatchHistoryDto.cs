using System.Text.Json.Serialization;

namespace Leagues.Dto.MatchHistory;

public sealed record MatchHistoryDto
{
    [JsonPropertyName("accountId")] public long AccountId { get; set; }

    [JsonPropertyName("games")] public GamesWrapper Games { get; set; } = new();

    [JsonPropertyName("platformId")] public string PlatformId { get; set; } = "";
}