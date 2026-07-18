using System.Text.Json;
using System.Text.Json.Serialization;

#pragma warning disable CS1591

namespace DFeHub.Providers.WebMania.NFSe;

/// <summary>
/// Represents municipality-provider availability and supported NFS-e
/// capabilities.
/// </summary>
public sealed class NFSeProviderStatusResponse
{
    public bool Status { get; init; }

    [JsonPropertyName("modelo")]
    public required string Model { get; init; }

    [JsonPropertyName("versao")]
    public required string Version { get; init; }

    [JsonPropertyName("ambientes")]
    public IReadOnlyList<NFSeEnvironment> Environments { get; init; } = [];

    [JsonPropertyName("autenticacao")]
    public IReadOnlyList<string> Authentication { get; init; } = [];

    [JsonPropertyName("emissao")]
    public IReadOnlyList<string> Issuance { get; init; } = [];

    [JsonPropertyName("funcoes")]
    public IReadOnlyList<string> Functions { get; init; } = [];

    [JsonPropertyName("codigo_servico")]
    public IReadOnlyList<NFSeProviderServiceCode> ServiceCodes { get; init; } =
        [];

    [JsonPropertyName("parametros_disponiveis")]
    public Dictionary<string, string> AvailableParameters { get; init; } = [];

    [JsonExtensionData]
    public IDictionary<string, JsonElement>? AdditionalData { get; set; }
}

/// <summary>
/// Represents one service code advertised by a municipality provider.
/// </summary>
public sealed class NFSeProviderServiceCode
{
    [JsonPropertyName("codigo")]
    public required string Code { get; init; }

    [JsonPropertyName("descricao")]
    public required string Description { get; init; }

    [JsonPropertyName("codigo_tributacao")]
    public string? TaxCode { get; init; }

    [JsonPropertyName("aliquota_sugerida")]
    public NFSeSuggestedTaxRate? SuggestedRate { get; init; }
}

/// <summary>
/// Represents tax rates suggested for a municipality service code.
/// </summary>
public sealed class NFSeSuggestedTaxRate
{
    [JsonPropertyName("iss")]
    public decimal? Iss { get; init; }
}

/// <summary>
/// Represents an NFS-e, an RPS batch, a cancellation, a substitution or a
/// manifestation returned by the Webmania NFS-e API.
/// </summary>
public sealed class NFSeDocumentResponse
{
    public Guid Uuid { get; init; }

    [JsonPropertyName("modelo")]
    public string? Model { get; init; }

    public required string Status { get; init; }

    [JsonPropertyName("motivo")]
    public string? Reason { get; init; }

    [JsonPropertyName("numero")]
    [JsonConverter(typeof(NFSeStringOrNumberConverter))]
    public string? Number { get; init; }

    [JsonPropertyName("codigo_verificacao")]
    public string? VerificationCode { get; init; }

    [JsonPropertyName("serie_rps")]
    [JsonConverter(typeof(NFSeStringOrNumberConverter))]
    public string? RpsSeries { get; init; }

    [JsonPropertyName("numero_rps")]
    [JsonConverter(typeof(NFSeStringOrNumberConverter))]
    public string? RpsNumber { get; init; }

    public Uri? Xml { get; init; }

    [JsonPropertyName("pdf_nfse")]
    public Uri? NFSePdf { get; init; }

    [JsonPropertyName("pdf_nfse_status")]
    public string? NFSePdfStatus { get; init; }

    [JsonPropertyName("pdf_rps")]
    public Uri? RpsPdf { get; init; }

    [JsonPropertyName("numero_lote")]
    [JsonConverter(typeof(NFSeStringOrNumberConverter))]
    public string? BatchNumber { get; init; }

    [JsonPropertyName("serie_lote")]
    [JsonConverter(typeof(NFSeStringOrNumberConverter))]
    public string? BatchSeries { get; init; }

    [JsonPropertyName("quantidade_rps")]
    [JsonConverter(typeof(NFSeStringOrNumberConverter))]
    public string? RpsQuantity { get; init; }

    [JsonPropertyName("protocolo")]
    [JsonConverter(typeof(NFSeStringOrNumberConverter))]
    public string? Protocol { get; init; }

    [JsonPropertyName("info_nfse")]
    public IReadOnlyList<NFSeDocumentResponse>? Invoices { get; init; }

    [JsonPropertyName("nfse_substituida")]
    public NFSeReplacedDocument? ReplacedInvoice { get; init; }

    [JsonPropertyName("chave")]
    public string? AccessKey { get; init; }

    public JsonElement Log { get; init; }

    [JsonPropertyName("atualizado_em")]
    public DateTimeOffset? UpdatedAt { get; init; }

    [JsonExtensionData]
    public IDictionary<string, JsonElement>? AdditionalData { get; set; }
}

/// <summary>
/// Identifies the document replaced by a successful substitution.
/// </summary>
public sealed class NFSeReplacedDocument
{
    public Guid Uuid { get; init; }

    [JsonPropertyName("numero")]
    [JsonConverter(typeof(NFSeStringOrNumberConverter))]
    public string? Number { get; init; }

    [JsonPropertyName("codigo_verificacao")]
    public string? VerificationCode { get; init; }

    [JsonExtensionData]
    public IDictionary<string, JsonElement>? AdditionalData { get; set; }
}
