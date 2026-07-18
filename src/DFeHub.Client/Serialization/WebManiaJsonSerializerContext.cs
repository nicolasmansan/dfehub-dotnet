using System.Text.Json.Serialization;

namespace DFeHub.Serialization;

[JsonSourceGenerationOptions(
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
[JsonSerializable(typeof(WebManiaErrorPayload))]
internal sealed partial class WebManiaJsonSerializerContext
    : JsonSerializerContext;

