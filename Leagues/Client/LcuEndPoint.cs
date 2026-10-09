using System.Globalization;
using System.Text;

namespace Leagues.Client;

public static class LcuEndPoint
{
    /// <summary>
    /// Returns the endpoint for fetching a summoner's information by their player name.
    /// </summary>
    /// <param name="playerName"></param>
    /// <returns></returns>
    public static string Summoner(string playerName)
    {
        // The player name copied from LOL client contains special chars,
        // [U+2066] Name [U+2069] # [U+2066] num [U+2069]
        var cleanedPlayerName = new string([
            .. playerName
                .Where(c =>
                {
                    var cat = CharUnicodeInfo.GetUnicodeCategory(c);
                    return cat != UnicodeCategory.Format && cat != UnicodeCategory.Control;
                })
        ]);

        cleanedPlayerName = cleanedPlayerName.Trim().Normalize(NormalizationForm.FormC);
        return $"lol-summoner/v1/summoners?name={Uri.EscapeDataString(cleanedPlayerName)}";
    }

    public static string CurrentSummoner
        => "lol-summoner/v1/current-summoner";

    public static string Summoner(long summonerId)
        => $"lol-summoner/v1/summoners/{summonerId}";

    public static string ChatConversations
        => "lol-chat/v1/conversations";

    public static string ChatConversationParticipants(string conversationId)
        => $"lol-chat/v1/conversations/{Uri.EscapeDataString(conversationId)}/participants";

    public static string ProfileIcon(int profileIconId)
        => $"lol-game-data/assets/v1/profile-icons/{profileIconId}.jpg";

    public static string MatchHistory(string uuid, int begIndex = 0, int endIndex = 20)
        => $"lol-match-history/v1/products/lol/{uuid}/matches?begIndex={begIndex}&endIndex={endIndex}";

    public static string GameDetails(long gameId)
        => $"lol-match-history/v1/games/{gameId}";

    public static string ChampionAvatar(int championId)
        => $"lol-game-data/assets/v1/champion-icons/{championId}.png";
}