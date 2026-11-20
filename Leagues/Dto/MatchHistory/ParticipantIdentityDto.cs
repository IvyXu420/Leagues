using System.Text.Json.Serialization;

namespace Leagues.Dto.MatchHistory;

public sealed record ParticipantIdentityDto
{
    [JsonPropertyName("participantId")] public int ParticipantId { get; set; }

    [JsonPropertyName("player")] public PlayerDto Player { get; set; } = new();
}