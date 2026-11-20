using System.Text.Json.Serialization;

namespace Leagues.Dto.MatchHistory;

public sealed record TeamDto
{
    [JsonPropertyName("bans")] public List<BanDto> Bans { get; set; } = [];

    [JsonPropertyName("baronKills")] public int BaronKills { get; set; }

    [JsonPropertyName("dominionVictoryScore")]
    public int DominionVictoryScore { get; set; }

    [JsonPropertyName("dragonKills")] public int DragonKills { get; set; }

    [JsonPropertyName("firstBaron")] public bool FirstBaron { get; set; }

    [JsonPropertyName("firstBlood")] public bool FirstBlood { get; set; }

    [JsonPropertyName("firstDargon")] public bool FirstDargon { get; set; }

    [JsonPropertyName("firstInhibitor")] public bool FirstInhibitor { get; set; }

    [JsonPropertyName("firstTower")] public bool FirstTower { get; set; }

    [JsonPropertyName("hordeKills")] public int HordeKills { get; set; }

    [JsonPropertyName("inhibitorKills")] public int InhibitorKills { get; set; }

    [JsonPropertyName("riftHeraldKills")] public int RiftHeraldKills { get; set; }

    [JsonPropertyName("teamId")] public int TeamId { get; set; }

    [JsonPropertyName("towerKills")] public int TowerKills { get; set; }

    [JsonPropertyName("vilemawKills")] public int VilemawKills { get; set; }

    [JsonPropertyName("win")] public string Win { get; set; } = "";
}