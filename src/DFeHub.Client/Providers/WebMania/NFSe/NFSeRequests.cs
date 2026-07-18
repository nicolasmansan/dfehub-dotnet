using System.Text.Json.Serialization;

#pragma warning disable CS1591

namespace DFeHub.Providers.WebMania.NFSe;

/// <summary>
/// Requests issuance of one NFS-e or an RPS batch.
/// </summary>
public sealed record NFSeIssueRequest
{
    [JsonPropertyName("ID")]
    public string? OrderId { get; init; }

    [JsonPropertyName("ambiente")]
    public required NFSeEnvironment Environment { get; init; }

    [JsonPropertyName("rps")]
    public required IReadOnlyList<NFSeRps> Rps { get; init; }

    [JsonPropertyName("url_notificacao")]
    public Uri? NotificationUrl { get; init; }

    /// <summary>
    /// Gets the optional wall-clock issue time, without a time zone and with
    /// minute precision.
    /// </summary>
    [JsonPropertyName("data_agendamento")]
    [JsonConverter(typeof(NFSeScheduledDateTimeConverter))]
    public DateTime? ScheduledAt { get; init; }
}

/// <summary>
/// Describes one provisional service receipt in an issuance or substitution
/// request.
/// </summary>
public sealed record NFSeRps
{
    [JsonPropertyName("servico")]
    public required NFSeService Service { get; init; }

    [JsonPropertyName("tomador")]
    public NFSeParty? Taker { get; init; }

    [JsonPropertyName("destinatario")]
    public NFSeRecipient? Recipient { get; init; }

    [JsonPropertyName("construcao_civil")]
    public NFSeConstruction? Construction { get; init; }

    [JsonPropertyName("atividades_evento")]
    public NFSeEventActivity? EventActivity { get; init; }
}

/// <summary>
/// Requests cancellation of an NFS-e or a scheduled issuance.
/// </summary>
public sealed record NFSeCancellationRequest
{
    [JsonPropertyName("uuid")]
    public required Guid Uuid { get; init; }

    [JsonPropertyName("motivo")]
    public required NFSeCancellationReason Reason { get; init; }
}

/// <summary>
/// Requests replacement of an issued NFS-e with a new RPS.
/// </summary>
public sealed record NFSeSubstitutionRequest
{
    [JsonPropertyName("ambiente")]
    public required NFSeEnvironment Environment { get; init; }

    [JsonPropertyName("codigo_verificacao")]
    public required string VerificationCode { get; init; }

    [JsonPropertyName("motivo")]
    public required NFSeCancellationReason Reason { get; init; }

    [JsonPropertyName("rps")]
    public required NFSeRps Rps { get; init; }
}

/// <summary>
/// Requests confirmation or rejection of participation in a
/// national-standard NFS-e.
/// </summary>
public sealed record NFSeManifestationRequest
{
    [JsonPropertyName("ambiente")]
    public required NFSeEnvironment Environment { get; init; }

    [JsonPropertyName("chave")]
    public string? AccessKey { get; init; }

    [JsonPropertyName("uuid")]
    public Guid? Uuid { get; init; }

    [JsonPropertyName("manifestador")]
    public required NFSeManifestingParty ManifestingParty { get; init; }

    [JsonPropertyName("evento")]
    public required NFSeManifestationEvent Event { get; init; }

    [JsonPropertyName("motivo_rejeicao")]
    public NFSeManifestationRejectionReason? RejectionReason { get; init; }

    [JsonPropertyName("justificativa_rejeicao")]
    public string? RejectionJustification { get; init; }
}
