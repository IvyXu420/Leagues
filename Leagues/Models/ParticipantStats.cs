namespace Leagues.Models;

/// <summary>
/// A general participant statistics class,
/// creating from the Chat API and LiveClient API.
/// displayed on the Stats page in-game.
/// </summary>
/// <param name="playerName"></param>
/// <param name="summonerLevel"></param>
/// <param name="avatarIconId"></param>
/// <param name="kda"></param>
/// <param name="winRate"></param>
public readonly struct ParticipantStats(
    string playerName,
    int summonerLevel,
    int avatarIconId,
    double kda,
    double winRate
    )
{
    public string PlayerName { get; } = playerName;
    public int SummonerLevel { get; } = summonerLevel;
    public int AvatarIconId { get; } = avatarIconId;
    public double Kda { get; } = kda;
    public double WinRate { get; } = winRate;
}