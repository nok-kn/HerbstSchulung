using Refit;

namespace HerbstSchulung.RestClients.Refit;

public interface IWebApi
{
    [Get("api/Greeting/say-hello-async")]
    Task<string> SayHelloAsync([Query] string name, CancellationToken cancellationToken = default);

    // ApiResponse enthält zusätzliche Informationen über die HTTP-Antwort
    [Get("api/Greeting/say-hello")]
    Task<ApiResponse<string>> SayHelloSyncAsync([Query] string name, CancellationToken cancellationToken = default);
}
