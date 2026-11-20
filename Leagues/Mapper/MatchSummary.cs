public readonly struct MatchSummary(
    long gameId,
    DateTimeOffset playedAt,
    TimeSpan duration,
    string gameMode,
    int championId,
    bool win,
    int kills,
    int deaths,
    int assists)
{
    public long GameId { get; } = gameId;
    public DateTimeOffset PlayedAt { get; } = playedAt;
    public TimeSpan Duration { get; } = duration;
    public string GameMode { get; } = gameMode;
    public int ChampionId { get; } = championId;
    public bool Win { get; } = win;
    public int Kills { get; } = kills;
    public int Deaths { get; } = deaths;
    public int Assists { get; } = assists;
}