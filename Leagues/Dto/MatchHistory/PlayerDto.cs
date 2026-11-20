using System.Text.Json.Serialization;

namespace Leagues.Dto.MatchHistory;

public sealed record PlayerDto
{
    [JsonPropertyName("accountId")] public long AccountId { get; set; }

    [JsonPropertyName("currentAccountId")] public long CurrentAccountId { get; set; }

    [JsonPropertyName("currentPlatformId")]
    public string CurrentPlatformId { get; set; } = "";

    [JsonPropertyName("gameName")] public string GameName { get; set; } = "";

    [JsonPropertyName("matchHistoryUri")] public string MatchHistoryUri { get; set; } = "";

    [JsonPropertyName("platformId")] public string PlatformId { get; set; } = "";

    [JsonPropertyName("profileIcon")] public int ProfileIcon { get; set; }

    [JsonPropertyName("puuid")] public string Puuid { get; set; } = "";

    [JsonPropertyName("summonerId")] public long SummonerId { get; set; }

    [JsonPropertyName("summonerName")] public string SummonerName { get; set; } = "";

    [JsonPropertyName("tagLine")] public string TagLine { get; set; } = "";
}