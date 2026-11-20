using System.Text.Json.Serialization;

namespace Leagues.Dto;

/// <summary>
/// Champion's basic info retrived from the LCU API.
/// </summary>
public sealed record ChampionSummaryDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("alias")]
    public string Alias { get; set; } = string.Empty;

    [JsonPropertyName("contentId")]
    public string ContentId { get; set; } = string.Empty;

    [JsonPropertyName("squarePortraitPath")]
    public string SquarePortraitPath { get; set; } = string.Empty;

    [JsonPropertyName("roles")]
    public List<string> Roles { get; set; } = [];
}