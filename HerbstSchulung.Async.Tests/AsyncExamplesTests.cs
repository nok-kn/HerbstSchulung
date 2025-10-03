using Xunit;

namespace HerbstSchulung.Async.Tests;

public class AsyncExamplesTests
{
    [Fact]
    public async Task FireAndForget_Korrekt_propagiert_Fehler()
    {
        await Assert.ThrowsAsync<Exception>(async () => await AsyncExamples.FehlendesAwait());
    }

    [Fact]
    public async Task HttpResult_Asynchron_ohne_Blockierung()
    {
        var uri = new Uri("https://example.com");
        var result = await AsyncExamples.HttpResult_Asynchron_Korrekt(uri);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task LibraryCall_mit_ConfigureAwait_funktioniert()
    {
        var uri = new Uri("https://example.com");
        var result = await AsyncExamples.LibraryCall_mit_ConfigureAwait_Korrekt(uri);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task ValueTask_Loesung_hat_Ergebnis()
    {
        var v = await AsyncExamples.LiefereZahl_Task_Korrekt();
        Assert.Equal(42, v);
    }

    [Fact]
    public async Task AsyncDisposable_Korrekt_entsorgt()
    {
        await AsyncExamples.AsyncDisposable_Korrekt();
    }
}
