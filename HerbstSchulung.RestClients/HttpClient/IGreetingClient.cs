namespace HerbstSchulung.RestClients.HttpClient;

public interface IGreetingClient
{
    Task<string> SayHelloAsync(string name, CancellationToken cancellationToken = default);
    Task<string> SayHelloSyncAsync(string name, CancellationToken cancellationToken = default);
}
