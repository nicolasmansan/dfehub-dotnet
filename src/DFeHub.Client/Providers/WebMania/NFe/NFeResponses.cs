using System.Text.Json;
using System.Text.Json.Serialization;

#pragma warning disable CS1591

namespace DFeHub.Providers.WebMania.NFe;

/// <summary>
/// Represents an issued or queried NF-e/NFC-e.
/// </summary>
public sealed class NFeDocumentResponse
{
    public Guid Uuid { get; init; }

    public required string Status { get; init; }

    [JsonPropertyName("motivo")]
    public string? Reason { get; init; }

    [JsonPropertyName("nfe")]
    public string? Number { get; init; }

    [JsonPropertyName("serie")]
    public string? Series { get; init; }

    [JsonPropertyName("recibo")]
    public string? Receipt { get; init; }

    [JsonPropertyName("chave")]
    public string? AccessKey { get; init; }

    [JsonPropertyName("modelo")]
    public string? Model { get; init; }

    public bool? Epec { get; init; }

    public Uri? Xml { get; init; }

    public Uri? Danfe { get; init; }

    [JsonPropertyName("danfe_simples")]
    public Uri? SimplifiedDanfe { get; init; }

    [JsonPropertyName("danfe_etiqueta")]
    public Uri? LabelDanfe { get; init; }

    public JsonElement Log { get; init; }

    [JsonExtensionData]
    public IDictionary<string, JsonElement>? AdditionalData { get; set; }
}

/// <summary>
/// Represents a fiscal event response.
/// </summary>
public sealed class NFeEventResponse
{
    public Guid Uuid { get; init; }

    public required string Status { get; init; }

    [JsonPropertyName("cod_evento")]
    public string? EventCode { get; init; }

    [JsonPropertyName("evento")]
    public string? EventSequence { get; init; }

    [JsonPropertyName("modelo")]
    public string? Model { get; init; }

    [JsonPropertyName("protocolo_evento")]
    public string? EventProtocol { get; init; }

    public Uri? Xml { get; init; }

    public Uri? Dacce { get; init; }

    public JsonElement Log { get; init; }

    [JsonExtensionData]
    public IDictionary<string, JsonElement>? AdditionalData { get; set; }
}

/// <summary>
/// Represents cancellation of an NF-e/NFC-e.
/// </summary>
public sealed class NFeCancellationResponse
{
    public required string Status { get; init; }

    public Uri? Xml { get; init; }

    [JsonPropertyName("xml_cancelamento")]
    public Uri? CancellationXml { get; init; }

    public JsonElement Log { get; init; }

    [JsonExtensionData]
    public IDictionary<string, JsonElement>? AdditionalData { get; set; }
}

/// <summary>
/// Represents invalidation of an NF-e/NFC-e number range.
/// </summary>
public sealed class NFeNumberInvalidationResponse
{
    public required Uri Xml { get; init; }

    public JsonElement Log { get; init; }

    [JsonExtensionData]
    public IDictionary<string, JsonElement>? AdditionalData { get; set; }
}

/// <summary>
/// Represents current SEFAZ availability.
/// </summary>
public sealed class NFeSefazStatusResponse
{
    public required string Status { get; init; }
}

/// <summary>
/// Represents the remaining validity of the issuer certificate.
/// </summary>
public sealed class NFeCertificateStatusResponse
{
    [JsonPropertyName("expiration")]
    public required int DaysUntilExpiration { get; init; }
}

/// <summary>
/// Represents an asynchronous report export.
/// </summary>
public sealed class NFeReportResponse
{
    public Guid Uuid { get; init; }

    public required string Status { get; init; }

    [JsonPropertyName("data_inicio")]
    public required DateOnly StartDate { get; init; }

    [JsonPropertyName("data_final")]
    public required DateOnly EndDate { get; init; }

    [JsonPropertyName("modelo")]
    public required string Model { get; init; }

    [JsonPropertyName("relatorio")]
    public required string Report { get; init; }

    [JsonPropertyName("total")]
    public int? Total { get; init; }

    [JsonPropertyName("processado")]
    public int? Processed { get; init; }

    [JsonPropertyName("url")]
    public Uri? DownloadUrl { get; init; }

    [JsonPropertyName("expira")]
    public DateOnly? ExpiresOn { get; init; }

    [JsonExtensionData]
    public IDictionary<string, JsonElement>? AdditionalData { get; set; }
}

/// <summary>
/// Represents a tax class stored by Webmania.
/// </summary>
public sealed class NFeTaxClassResponse
{
    [JsonPropertyName("referencia")]
    public required string Reference { get; init; }

    [JsonPropertyName("descricao")]
    public required string Description { get; init; }

    [JsonPropertyName("data")]
    public string? CreatedAt { get; init; }

    public IReadOnlyList<NFeTaxClassIcms>? Icms { get; init; }

    public IReadOnlyList<NFeTaxClassContribution>? Ipi { get; init; }

    public IReadOnlyList<NFeTaxClassContribution>? Pis { get; init; }

    public IReadOnlyList<NFeTaxClassContribution>? Cofins { get; init; }

    public IReadOnlyList<NFeTaxClassIssqn>? Issqn { get; init; }

    [JsonPropertyName("ibs_cbs")]
    public IReadOnlyList<NFeTaxClassIbsCbs>? IbsCbs { get; init; }

    [JsonExtensionData]
    public IDictionary<string, JsonElement>? AdditionalData { get; set; }
}

/// <summary>
/// Represents deletion of one tax class.
/// </summary>
public sealed class NFeTaxClassDeleteResponse
{
    [JsonPropertyName("referencia")]
    public required string Reference { get; init; }

    [JsonPropertyName("msg")]
    public required string Message { get; init; }
}

/// <summary>
/// Represents a successful company update.
/// </summary>
public sealed class NFeCompanyUpdateResponse
{
    public required string Success { get; init; }
}

/// <summary>
/// Represents the provider callback sent for invoice status changes.
/// </summary>
public sealed class NFeNotification
{
    public Guid Uuid { get; init; }

    public required string Status { get; init; }

    [JsonPropertyName("motivo")]
    public string? Reason { get; init; }

    [JsonPropertyName("nfe")]
    public string? Number { get; init; }

    [JsonPropertyName("serie")]
    public string? Series { get; init; }

    [JsonPropertyName("recibo")]
    public string? Receipt { get; init; }

    [JsonPropertyName("chave")]
    public string? AccessKey { get; init; }

    [JsonPropertyName("modelo")]
    public string? Model { get; init; }

    public bool? Epec { get; init; }

    public Uri? Xml { get; init; }

    public Uri? Danfe { get; init; }

    public JsonElement Log { get; init; }

    [JsonPropertyName("data")]
    public JsonElement RequestData { get; init; }

    [JsonPropertyName("log_cancelamento")]
    public JsonElement CancellationLog { get; init; }

    [JsonExtensionData]
    public IDictionary<string, JsonElement>? AdditionalData { get; set; }
}
