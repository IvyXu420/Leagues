using System.Text.Json.Serialization;

namespace Leagues.Dto;

public sealed class ChatParticipantDto
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

public sealed class ChatParticipantLolDto
{
    [JsonPropertyName("level")]
    public string Level { get; set; } = string.Empty;
}

public sealed class LiveDataParticipantDto
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

public sealed class LiveDataItemDto
{
    [JsonPropertyName("canUse")]
    public bool CanUse { get; set; }

    [JsonPropertyName("consumable")]
    public bool Consumable { get; set; }

    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonPropertyName("itemID")]
    public int ItemId { get; set; }

    [JsonPropertyName("price")]
    public int Price { get; set; }

    [JsonPropertyName("rawDescription")]
    public string RawDescription { get; set; } = string.Empty;

    [JsonPropertyName("rawDisplayName")]
    public string RawDisplayName { get; set; } = string.Empty;

    [JsonPropertyName("slot")]
    public int Slot { get; set; }
}

public sealed class LiveDataScoresDto
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

public sealed class LiveDataSummonerSpellsDto
{
    [JsonPropertyName("summonerSpellOne")]
    public LiveDataSummonerSpellDto SummonerSpellOne { get; set; } = new();

    [JsonPropertyName("summonerSpellTwo")]
    public LiveDataSummonerSpellDto SummonerSpellTwo { get; set; } = new();
}

public sealed class LiveDataSummonerSpellDto
{
    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonPropertyName("rawDescription")]
    public string RawDescription { get; set; } = string.Empty;

    [JsonPropertyName("rawDisplayName")]
    public string RawDisplayName { get; set; } = string.Empty;
}