using System.Text.Json.Serialization;

namespace Leagues.Dto;

public record RerollPointsDto
{
    [JsonPropertyName("currentPoints")]
    public int CurrentPoints { get; set; }

    [JsonPropertyName("maxRolls")]
    public int MaxRolls { get; set; }

    [JsonPropertyName("numberOfRolls")]
    public int NumberOfRolls { get; set; }

    [JsonPropertyName("pointsCostToRoll")]
    public int PointsCostToRoll { get; set; }

    [JsonPropertyName("pointsToReroll")]
    public int PointsToReroll { get; set; }
}