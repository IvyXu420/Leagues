using System.Text.Json.Serialization;

namespace Leagues.Dto.MatchHistory;

public sealed record ParticipantDto
{
    [JsonPropertyName("championId")] public int ChampionId { get; set; }

    [JsonPropertyName("highestAchievedSeasonTier")]
    public string HighestAchievedSeasonTier { get; set; } = "";

    [JsonPropertyName("participantId")] public int ParticipantId { get; set; }

    [JsonPropertyName("spell1Id")] public int Spell1Id { get; set; }

    [JsonPropertyName("spell2Id")] public int Spell2Id { get; set; }

    [JsonPropertyName("stats")] public StatsDto Stats { get; set; } = new();

    [JsonPropertyName("teamId")] public int TeamId { get; set; }

    [JsonPropertyName("timeline")] public TimelineDto Timeline { get; set; } = new();
}