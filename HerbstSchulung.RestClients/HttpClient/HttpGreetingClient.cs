namespace HerbstSchulung.RestClients.HttpClient;

public class HttpGreetingClient(System.Net.Http.HttpClient http) : IGreetingClient
{
    
    public async Task<string> SayHelloAsync(string name, CancellationToken cancellationToken = default)
    {
        var response = await http.GetAsync($"api/Greeting/say-hello-async?name={Uri.EscapeDataString(name)}", cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    public async Task<string> SayHelloSyncAsync(string name, CancellationToken cancellationToken = default)
    {
        var response = await http.GetAsync($"api/Greeting/say-hello?name={Uri.EscapeDataString(name)}", cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }
}
