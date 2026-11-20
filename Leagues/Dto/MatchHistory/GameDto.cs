using System.Text.Json.Serialization;

namespace Leagues.Dto.MatchHistory;

public sealed record GameDto
{
    [JsonPropertyName("endOfGameResult")] public string EndOfGameResult { get; set; } = "";

    [JsonPropertyName("gameCreation")] public long GameCreation { get; set; }

    [JsonPropertyName("gameCreationDate")] public string GameCreationDate { get; set; } = "";

    [JsonPropertyName("gameDuration")] public int GameDuration { get; set; }

    [JsonPropertyName("gameId")] public long GameId { get; set; }

    [JsonPropertyName("gameMode")] public string GameMode { get; set; } = "";

    [JsonPropertyName("gameModeMutators")] public List<string> GameModeMutators { get; set; } = [];

    [JsonPropertyName("gameType")] public string GameType { get; set; } = "";

    [JsonPropertyName("gameVersion")] public string GameVersion { get; set; } = "";

    [JsonPropertyName("mapId")] public int MapId { get; set; }

    [JsonPropertyName("participantIdentities")]
    public List<ParticipantIdentityDto> ParticipantIdentities { get; set; } = [];

    /// <summary>
    /// Contains only the participant corresponding to the queried summoner
    /// in this Match History response.
    /// </summary>
    [JsonPropertyName("participants")]
    public List<ParticipantDto> Participants { get; set; } = [];

    [JsonPropertyName("platformId")] public string PlatformId { get; set; } = "";

    [JsonPropertyName("queueId")] public int QueueId { get; set; }

    [JsonPropertyName("seasonId")] public int SeasonId { get; set; }

    [JsonPropertyName("teams")] public List<TeamDto> Teams { get; set; } = [];
}