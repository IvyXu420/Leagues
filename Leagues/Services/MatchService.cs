using Leagues.Client;
using Leagues.Logging;

namespace Leagues.Services;

public static class MatchService
{
    public static async Task<bool> Accept()
    {
        try
        {
            using var response = await LcuNet.Client.PostAsync("lol-matchmaking/v1/ready-check/accept", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Logger.Error($"Accept request failed: {ex.Message}");
            return false;
        }
    }

    public static async Task<bool> Decline()
    {
        try
        {
            using var response = await LcuNet.Client.PostAsync("lol-matchmaking/v1/ready-check/decline", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Logger.Error($"Decline request failed: {ex.Message}");
            return false;
        }
    }
}