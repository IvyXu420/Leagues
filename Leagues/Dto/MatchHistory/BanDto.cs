using System.Text.Json.Serialization;

namespace Leagues.Dto.MatchHistory;

public sealed record BanDto
{
    [JsonPropertyName("championId")] public int ChampionId { get; set; }

    [JsonPropertyName("pickTurn")] public int PickTurn { get; set; }
}