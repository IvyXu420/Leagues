using System.Text.Json.Serialization;

namespace Leagues.Dto.Chat;

public sealed record ChatParticipantLolDto
{
    [JsonPropertyName("level")]
    public string Level { get; set; } = string.Empty;
}