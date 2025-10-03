using Xunit;

namespace HerbstSchulung.Async.Tests;

public class AsyncChallengesTests
{
    [Fact(DisplayName = "L�sung1: LadeTextAsync_Loesung liefert Inhalt")]
    public async Task Loesung1_Test()
    {
        var uri = new Uri("https://example.com");
        var s = await AsyncSolutions.LadeTextAsync_Loesung(uri);
        Assert.NotNull(s);
    }

    [Fact(DisplayName = "L�sung2: LadeTextSynchron_Loesung ist asynchron")]
    public async Task Loesung2_Test()
    {
        var uri = new Uri("https://example.com");
        var s = await AsyncSolutions.LadeTextSynchron_Loesung(uri);
        Assert.NotNull(s);
    }

    [Theory(DisplayName = "L�sung3: BerechneAsync_Loesung ok f�r synchron/asynchron")]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Loesung3_Test(bool synchron)
    {
        var r = await AsyncSolutions.BerechneAsync_Loesung(synchron);
        Assert.Equal(7, r);
    }

    [Fact(DisplayName = "L�sung4: AsyncStream.DisposeAsync ist idempotent")]
    public async Task Loesung4_Test()
    {
        await using var s = new AsyncSolutions.AsyncStream();
        await s.DisposeAsync();
        await s.DisposeAsync();
    }

    [Fact(DisplayName = "L�sung5: PipelineAsync_Loesung honoriert Cancellation")]
    public async Task Loesung5_Cancellation_Test()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            await AsyncSolutions.PipelineAsync_Loesung(
                ct => Task.FromResult(1),
                (x, ct) => Task.FromResult(2),
                cts.Token));
    }

    [Fact(DisplayName = "L�sung5: PipelineAsync_Loesung addiert Ergebnisse")]
    public async Task Loesung5_Success_Test()
    {
        var sum = await AsyncSolutions.PipelineAsync_Loesung(
            ct => Task.FromResult(3),
            (x, ct) => Task.FromResult(x + 1),
            CancellationToken.None);
        Assert.Equal(3 + 4, sum);
    }
}
