namespace DFeHub.Client.UnitTests.TestDoubles;

internal sealed class TrackingStream(byte[] bytes) : MemoryStream(bytes)
{
    internal bool WasDisposed { get; private set; }

    internal bool WasDisposedAsync { get; private set; }

    protected override void Dispose(bool disposing)
    {
        WasDisposed = true;
        base.Dispose(disposing);
    }

    public override async ValueTask DisposeAsync()
    {
        WasDisposedAsync = true;
        await base.DisposeAsync();
    }
}

