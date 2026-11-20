using System.Text.Json.Serialization;

namespace Leagues.Dto.MatchHistory;

public sealed record GamesWrapper
{
    [JsonPropertyName("gameBeginDate")] public string GameBeginDate { get; set; } = "";

    [JsonPropertyName("gameCount")] public int GameCount { get; set; }

    [JsonPropertyName("gameEndDate")] public string GameEndDate { get; set; } = "";

    [JsonPropertyName("gameIndexBegin")] public int GameIndexBegin { get; set; }

    [JsonPropertyName("gameIndexEnd")] public int GameIndexEnd { get; set; }

    [JsonPropertyName("games")] public List<GameDto> Games { get; set; } = [];
}