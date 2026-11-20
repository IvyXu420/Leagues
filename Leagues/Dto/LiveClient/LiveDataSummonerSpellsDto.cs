using System.Text.Json.Serialization;

namespace Leagues.Dto.LiveClient;

public sealed record LiveDataSummonerSpellsDto
{
    [JsonPropertyName("summonerSpellOne")]
    public LiveDataSummonerSpellDto SummonerSpellOne { get; set; } = new();

    [JsonPropertyName("summonerSpellTwo")]
    public LiveDataSummonerSpellDto SummonerSpellTwo { get; set; } = new();
}