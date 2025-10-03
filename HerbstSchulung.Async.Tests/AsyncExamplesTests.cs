using Xunit;

namespace HerbstSchulung.Async.Tests;

public class AsyncExamplesTests
{
    [Fact]
    public async Task FehlendesAwait_Verhalten()
    {
        await Assert.ThrowsAsync<Exception>(async () => await AsyncExamples.FehlendesAwait());
    }

    [Fact]
    public async Task LibraryCall_mit_ConfigureAwait_funktioniert()
    {
        var uri = new Uri("https://example.com");
        var result = await AsyncExamples.LibraryCall_mit_ConfigureAwait_Korrekt(uri);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task AsyncDisposable_Korrekt_entsorgt()
    {
        await AsyncExamples.AsyncDisposable_Korrekt();
    }
}
