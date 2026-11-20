using System.Text.Json.Serialization;

namespace Leagues.Dto.Chat;

public sealed record ChatParticipantDto
{
    [JsonPropertyName("puuid")]
    public string Puuid { get; set; } = string.Empty;

    [JsonPropertyName("gameName")]
    public string GameName { get; set; } = string.Empty;

    [JsonPropertyName("gameTag")]
    public string? GameTag { get; set; }

    [JsonPropertyName("lol")]
    public ChatParticipantLolDto Lol { get; set; } = new();

    [JsonPropertyName("icon")]
    public int Icon { get; set; }
}