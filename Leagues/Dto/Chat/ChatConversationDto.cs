using System.Text.Json.Serialization;

namespace Leagues.Dto.Chat;

public sealed record ChatConversationDto
{
    [JsonPropertyName("gameName")]
    public string GameName { get; set; } = "";

    [JsonPropertyName("gameTag")]
    public string GameTag { get; set; } = "";

    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("inviterId")]
    public string InviterId { get; set; } = "";

    [JsonPropertyName("isMuted")]
    public bool IsMuted { get; set; }

    [JsonPropertyName("lastMessage")]
    public object? LastMessage { get; set; }

    [JsonPropertyName("mucJwtDto")]
    public MucJwtDto MucJwtDto { get; set; } = new();

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("password")]
    public string Password { get; set; } = "";

    [JsonPropertyName("pid")]
    public string Pid { get; set; } = "";

    [JsonPropertyName("targetRegion")]
    public string TargetRegion { get; set; } = "";

    [JsonPropertyName("type")]
    public string Type { get; set; } = "";

    [JsonPropertyName("unreadMessageCount")]
    public int UnreadMessageCount { get; set; }
}

public sealed record MucJwtDto
{
    [JsonPropertyName("channelClaim")]
    public string ChannelClaim { get; set; } = "";

    [JsonPropertyName("domain")]
    public string Domain { get; set; } = "";

    [JsonPropertyName("jwt")]
    public string Jwt { get; set; } = "";

    [JsonPropertyName("targetRegion")]
    public string TargetRegion { get; set; } = "";
}