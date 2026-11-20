using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Text;

namespace Leagues.Client;

public static class LcuNet
{
    private static LcuCredential? clientCredentials;
    private static LcuCredential? liveClientCredentials;
    private static LcuCredential? webSocketCredentials;
    public static HttpClient Client
    {
        get
        {
            if (!Credential.TryGetCredentials(out var credentials))
                throw new InvalidOperationException("League client credentials are unavailable.");

            if (field is not null && clientCredentials is { } active &&
                active.Port == credentials.Port && active.Token == credentials.Token)
                return field;

            field?.Dispose();
            field = LcuNetFactory.CreateHttpClient($"https://127.0.0.1:{credentials.Port}/", credentials);
            clientCredentials = credentials;
            return field;
        }
    }

    public static HttpClient LiveClient
    {
        get
        {
            if (!Credential.TryGetCredentials(out var credentials))
                throw new InvalidOperationException("League client credentials are unavailable.");

            if (field is not null && liveClientCredentials is { } active &&
                active.Port == credentials.Port && active.Token == credentials.Token)
                return field;

            field?.Dispose();
            field = LcuNetFactory.CreateHttpClient("https://127.0.0.1:2999", credentials);
            liveClientCredentials = credentials;
            return field;
        }
    }
    private static ClientWebSocket? webSocket;
    public async static Task<ClientWebSocket> GetWebSocket()
    {
        if (!Credential.TryGetCredentials(out var credentials))
            throw new InvalidOperationException("League client credentials are unavailable.");

        if (webSocket is { State: WebSocketState.Open } && webSocketCredentials is { } active &&
            active.Port == credentials.Port && active.Token == credentials.Token)
            return webSocket;

        webSocket?.Dispose();
        webSocket = await LcuNetFactory.CreateWebSocketAsync($"wss://127.0.0.1:{credentials.Port}", credentials);
        webSocketCredentials = credentials;
        return webSocket;
    }
    private static class LcuNetFactory
    {
        public static HttpClient CreateHttpClient(string url, LcuCredential credentials)
        {
            var handler = new HttpClientHandler
            {
                UseProxy = false,
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            var client = new HttpClient(handler) { BaseAddress = new Uri(url) };
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes($"riot:{credentials.Token}")));

            return client;
        }

        public static async Task<ClientWebSocket> CreateWebSocketAsync(string url, LcuCredential credentials)
        {
            var clientWebSocket = new ClientWebSocket();
            ServicePointManager.ServerCertificateValidationCallback =
                (_, _, _, _) => true;
            var authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes($"riot:{credentials.Token}"));
            clientWebSocket.Options.SetRequestHeader("Authorization", $"Basic {authorization}");
            var uri = new Uri(url);

            await clientWebSocket.ConnectAsync(uri, CancellationToken.None);
            return clientWebSocket;
        }
    }
}