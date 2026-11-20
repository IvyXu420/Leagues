namespace Leagues.Client;

public readonly struct LcuCredential(string token, int port)
{
    public string Token { get; } = token;
    public int Port { get; } = port;
}