using Microsoft.Extensions.DependencyInjection;
using Refit;
using HerbstSchulung.RestClients.Refit;
using HerbstSchulung.RestClients.HttpClient;


namespace HerbstSchulung.RestClients;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGreetingHttpClient(this IServiceCollection services, string baseAddress)
    {
        services.AddHttpClient<IGreetingClient, HttpGreetingClient>(client =>
        {
            client.BaseAddress = new Uri(baseAddress);
        });
        return services;
    }

    public static IServiceCollection AddGreetingRefitClient(this IServiceCollection services, string baseAddress)
    {
        services
            .AddRefitClient<IWebApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseAddress));

        return services;
    }
}
