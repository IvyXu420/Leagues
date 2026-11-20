using System.Text.Json.Serialization;

namespace Leagues.Dto.LiveClient;

public sealed record LiveDataParticipantDto
{
    [JsonPropertyName("championName")]
    public string ChampionName { get; set; } = string.Empty;

    [JsonPropertyName("isBot")]
    public bool IsBot { get; set; }

    [JsonPropertyName("isDead")]
    public bool IsDead { get; set; }

    [JsonPropertyName("items")]
    public List<LiveDataItemDto> Items { get; set; } = [];

    [JsonPropertyName("level")]
    public int Level { get; set; }

    [JsonPropertyName("position")]
    public string Position { get; set; } = string.Empty;

    [JsonPropertyName("rawChampionName")]
    public string RawChampionName { get; set; } = string.Empty;

    [JsonPropertyName("rawSkinName")]
    public string RawSkinName { get; set; } = string.Empty;

    [JsonPropertyName("respawnTimer")]
    public double RespawnTimer { get; set; }

    [JsonPropertyName("riotId")]
    public string RiotId { get; set; } = string.Empty;

    [JsonPropertyName("riotIdGameName")]
    public string RiotIdGameName { get; set; } = string.Empty;

    [JsonPropertyName("riotIdTagLine")]
    public string RiotIdTagLine { get; set; } = string.Empty;

    [JsonPropertyName("runes")]
    public object? Runes { get; set; }

    [JsonPropertyName("scores")]
    public LiveDataScoresDto Scores { get; set; } = new();

    [JsonPropertyName("skinID")]
    public int SkinId { get; set; }

    [JsonPropertyName("skinName")]
    public string SkinName { get; set; } = string.Empty;

    [JsonPropertyName("summonerName")]
    public string SummonerName { get; set; } = string.Empty;

    [JsonPropertyName("summonerSpells")]
    public LiveDataSummonerSpellsDto SummonerSpells { get; set; } = new();

    [JsonPropertyName("team")]
    public string Team { get; set; } = string.Empty;
}