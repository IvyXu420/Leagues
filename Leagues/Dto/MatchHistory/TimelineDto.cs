using System.Text.Json.Serialization;

namespace Leagues.Dto.MatchHistory;

public sealed record TimelineDto
{
    [JsonPropertyName("creepsPerMinDeltas")]
    public Dictionary<string, double> CreepsPerMinDeltas { get; set; } = [];

    [JsonPropertyName("csDiffPerMinDeltas")]
    public Dictionary<string, double> CsDiffPerMinDeltas { get; set; } = [];

    [JsonPropertyName("damageTakenDiffPerMinDeltas")]
    public Dictionary<string, double> DamageTakenDiffPerMinDeltas { get; set; } = [];

    [JsonPropertyName("damageTakenPerMinDeltas")]
    public Dictionary<string, double> DamageTakenPerMinDeltas { get; set; } = [];

    [JsonPropertyName("goldPerMinDeltas")] public Dictionary<string, double> GoldPerMinDeltas { get; set; } = [];

    [JsonPropertyName("lane")] public string Lane { get; set; } = "";

    [JsonPropertyName("participantId")] public int ParticipantId { get; set; }

    [JsonPropertyName("role")] public string Role { get; set; } = "";

    [JsonPropertyName("xpDiffPerMinDeltas")]
    public Dictionary<string, double> XpDiffPerMinDeltas { get; set; } = [];

    [JsonPropertyName("xpPerMinDeltas")] public Dictionary<string, double> XpPerMinDeltas { get; set; } = [];
}