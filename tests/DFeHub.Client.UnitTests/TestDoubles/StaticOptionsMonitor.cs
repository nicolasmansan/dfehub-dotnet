using Microsoft.Extensions.Options;

namespace DFeHub.Client.UnitTests.TestDoubles;

internal sealed class StaticOptionsMonitor<TOptions>
    : IOptionsMonitor<TOptions>
{
    private readonly TOptions value;

    internal StaticOptionsMonitor(TOptions value)
    {
        this.value = value;
    }

    public TOptions CurrentValue => value;

    public TOptions Get(string? name) => value;

    public IDisposable? OnChange(Action<TOptions, string?> listener)
    {
        return NullDisposable.Instance;
    }

    private sealed class NullDisposable : IDisposable
    {
        internal static readonly NullDisposable Instance = new();

        public void Dispose()
        {
        }
    }
}
