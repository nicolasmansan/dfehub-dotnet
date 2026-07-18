namespace DFeHub.Client.UnitTests.TestDoubles;

internal sealed class StubHttpClientFactory(HttpClient client)
    : IHttpClientFactory
{
    internal string? RequestedName { get; private set; }

    public HttpClient CreateClient(string name)
    {
        RequestedName = name;
        return client;
    }
}

