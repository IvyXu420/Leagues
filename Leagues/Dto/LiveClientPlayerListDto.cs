using System.Text.Json.Serialization;

namespace Leagues.Dto;

public class LiveClientPlayerListDto
{
    [JsonPropertyName("accountId")]
    public long AccountId { get; set; }

    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonPropertyName("gameName")]
    public string GameName { get; set; } = string.Empty;

    [JsonPropertyName("internalName")]
    public string InternalName { get; set; } = string.Empty;

    [JsonPropertyName("nameChangeFlag")]
    public bool NameChangeFlag { get; set; }

    [JsonPropertyName("percentCompleteForNextLevel")]
    public int PercentCompleteForNextLevel { get; set; }

    [JsonPropertyName("privacy")]
    public string Privacy { get; set; } = string.Empty;

    [JsonPropertyName("profileIconId")]
    public int ProfileIconId { get; set; }

    [JsonPropertyName("puuid")]
    public string Puuid { get; set; } = string.Empty;

    [JsonPropertyName("rerollPoints")]
    public RerollPointsDto RerollPoints { get; set; } = new();

    [JsonPropertyName("summonerId")]
    public long SummonerId { get; set; }

    [JsonPropertyName("summonerLevel")]
    public int SummonerLevel { get; set; }

    [JsonPropertyName("tagLine")]
    public string TagLine { get; set; } = string.Empty;

    [JsonPropertyName("unnamed")]
    public bool Unnamed { get; set; }

    [JsonPropertyName("xpSinceLastLevel")]
    public int XpSinceLastLevel { get; set; }

    [JsonPropertyName("xpUntilNextLevel")]
    public int XpUntilNextLevel { get; set; }
}

public class RerollPointsDto
{
    [JsonPropertyName("currentPoints")]
    public int CurrentPoints { get; set; }

    [JsonPropertyName("maxRolls")]
    public int MaxRolls { get; set; }

    [JsonPropertyName("numberOfRolls")]
    public int NumberOfRolls { get; set; }

    [JsonPropertyName("pointsCostToRoll")]
    public int PointsCostToRoll { get; set; }

    [JsonPropertyName("pointsToReroll")]
    public int PointsToReroll { get; set; }
}