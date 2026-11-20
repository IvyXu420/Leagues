using System.Text.Json.Serialization;

namespace Leagues.Dto.LiveClient;

public sealed record LiveDataScoresDto
{
    [JsonPropertyName("assists")]
    public int Assists { get; set; }

    [JsonPropertyName("creepScore")]
    public int CreepScore { get; set; }

    [JsonPropertyName("deaths")]
    public int Deaths { get; set; }

    [JsonPropertyName("kills")]
    public int Kills { get; set; }

    [JsonPropertyName("wardScore")]
    public double WardScore { get; set; }
}