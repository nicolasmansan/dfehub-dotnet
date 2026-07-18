using System.Text.Json;
using System.Text.Json.Serialization;

namespace DFeHub.Serialization;

internal sealed class WebManiaErrorPayload
{
    [JsonPropertyName("error")]
    public JsonElement Error { get; init; }

    [JsonPropertyName("errors")]
    public JsonElement Errors { get; init; }

    [JsonPropertyName("message")]
    public string? Message { get; init; }

    [JsonPropertyName("mensagem")]
    public string? Mensagem { get; init; }

    [JsonPropertyName("code")]
    public string? Code { get; init; }

    [JsonPropertyName("codigo")]
    public string? Codigo { get; init; }
}

