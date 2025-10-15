namespace HerbstSchulung.WebApi;

#pragma warning disable CS7022 // nur für Demozwecke

public class TestableProgram
{
    public static Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var startup = new TestableStartup(builder.Configuration, builder.Environment);
        startup.ConfigureServices(builder.Services);
        var app = builder.Build();
        startup.ConfigureMiddleware(app);
        return app.RunAsync();
    }
}

#pragma warning restore CS7022 // The entry point of the program is global code; ignoring entry point
