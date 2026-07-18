using System.Text.Json.Serialization;

namespace DFeHub.Client.UnitTests;

[JsonSerializable(typeof(TestResponse))]
internal sealed partial class TestSerializationContext : JsonSerializerContext;

internal sealed record TestResponse(string Id, string Status);

