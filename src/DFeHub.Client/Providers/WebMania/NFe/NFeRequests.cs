using System.Text.Json.Serialization;

#pragma warning disable CS1591

namespace DFeHub.Providers.WebMania.NFe;

/// <summary>
/// Requests issuance of an NF-e or NFC-e.
/// </summary>
public sealed record NFeIssueRequest
{
    [JsonPropertyName("ID")]
    public required string OrderId { get; init; }

    [JsonPropertyName("operacao")]
    public required NFeOperation Operation { get; init; }

    [JsonPropertyName("natureza_operacao")]
    public required string OperationNature { get; init; }

    [JsonPropertyName("modelo")]
    public required NFeModel Model { get; init; }

    [JsonPropertyName("finalidade")]
    public NFePurpose Purpose { get; init; } = NFePurpose.Normal;

    [JsonPropertyName("ambiente")]
    public required NFeEnvironment Environment { get; init; }

    [JsonPropertyName("tipo_credito")]
    public int? CreditType { get; init; }

    [JsonPropertyName("tipo_debito")]
    public int? DebitType { get; init; }

    [JsonPropertyName("url_notificacao")]
    public Uri? NotificationUrl { get; init; }

    [JsonPropertyName("data_emissao")]
    public DateTimeOffset? IssuedAt { get; init; }

    [JsonPropertyName("data_entrada_saida")]
    public DateTimeOffset? EnteredOrExitedAt { get; init; }

    [JsonPropertyName("data_previsao_entrega")]
    public DateOnly? ExpectedDeliveryDate { get; init; }

    [JsonPropertyName("data_servico")]
    public DateOnly? ServiceDate { get; init; }

    [JsonPropertyName("nfe_referenciada")]
    public IReadOnlyList<string>? ReferencedNFeAccessKeys { get; init; }

    [JsonPropertyName("cte_referenciada")]
    public IReadOnlyList<string>? ReferencedCTeAccessKeys { get; init; }

    [JsonPropertyName("ecf_referenciada")]
    public IReadOnlyList<NFeReferencedEcf>? ReferencedEcfReceipts { get; init; }

    [JsonPropertyName("nf_rural_referenciada")]
    public IReadOnlyList<NFeReferencedRuralInvoice>? ReferencedRuralInvoices
    {
        get;
        init;
    }

    [JsonPropertyName("calculo_impostos")]
    public bool? CalculateTaxesOnly { get; init; }

    [JsonPropertyName("previa_danfe")]
    public bool? PreviewDanfe { get; init; }

    [JsonPropertyName("cliente")]
    public required NFeCustomer Customer { get; init; }

    [JsonPropertyName("produtos")]
    public required IReadOnlyList<NFeProduct> Products { get; init; }

    [JsonPropertyName("pedido")]
    public required NFeOrder Order { get; init; }

    [JsonPropertyName("transporte")]
    public NFeTransport? Transport { get; init; }

    [JsonPropertyName("fatura")]
    public NFeBilling? Billing { get; init; }

    [JsonPropertyName("parcelas")]
    public IReadOnlyList<NFeInstallment>? Installments { get; init; }

    [JsonPropertyName("exportacao")]
    public NFeExport? Export { get; init; }

    [JsonPropertyName("agropecuario")]
    public NFeAgriculture? Agriculture { get; init; }
}

/// <summary>
/// Requests a simplified return or reversal invoice.
/// </summary>
public sealed record NFeReturnRequest
{
    [JsonPropertyName("chave")]
    public string? AccessKey { get; init; }

    [JsonPropertyName("uuid")]
    public Guid? Uuid { get; init; }

    [JsonPropertyName("natureza_operacao")]
    public required string OperationNature { get; init; }

    [JsonPropertyName("ambiente")]
    public required NFeEnvironment Environment { get; init; }

    [JsonPropertyName("classe_imposto")]
    public string? TaxClass { get; init; }

    [JsonPropertyName("codigo_cfop")]
    public required string CfopCode { get; init; }

    [JsonPropertyName("produtos")]
    public IReadOnlyList<int>? ProductNumbers { get; init; }

    [JsonPropertyName("quantidade")]
    public IReadOnlyList<decimal>? Quantities { get; init; }

    [JsonPropertyName("volume")]
    public string? VolumeCount { get; init; }

    [JsonPropertyName("informacoes_fisco")]
    public string? TaxAuthorityInformation { get; init; }

    [JsonPropertyName("informacoes_complementares")]
    public string? CustomerInformation { get; init; }

    [JsonPropertyName("url_notificacao")]
    public Uri? NotificationUrl { get; init; }
}

/// <summary>
/// Requests an adjustment invoice.
/// </summary>
public sealed record NFeAdjustmentRequest
{
    [JsonPropertyName("operacao")]
    public required NFeOperation Operation { get; init; }

    [JsonPropertyName("natureza_operacao")]
    public required string OperationNature { get; init; }

    [JsonPropertyName("codigo_cfop")]
    public required string CfopCode { get; init; }

    [JsonPropertyName("valor_icms")]
    public required decimal IcmsValue { get; init; }

    [JsonPropertyName("valor_icms_st")]
    public decimal? IcmsStValue { get; init; }

    [JsonPropertyName("ambiente")]
    public required NFeEnvironment Environment { get; init; }

    [JsonPropertyName("cliente")]
    public required NFeCustomer Customer { get; init; }

    [JsonPropertyName("situacao_tributaria")]
    public string? TaxSituation { get; init; }

    [JsonPropertyName("informacoes_fisco")]
    public string? TaxAuthorityInformation { get; init; }

    [JsonPropertyName("informacoes_complementares")]
    public string? CustomerInformation { get; init; }
}

/// <summary>
/// Requests a complementary invoice.
/// </summary>
public sealed record NFeComplementRequest
{
    [JsonPropertyName("chave")]
    public string? AccessKey { get; init; }

    [JsonPropertyName("uuid")]
    public Guid? Uuid { get; init; }

    [JsonPropertyName("operacao")]
    public required NFeOperation Operation { get; init; }

    [JsonPropertyName("natureza_operacao")]
    public required string OperationNature { get; init; }

    [JsonPropertyName("ambiente")]
    public required NFeEnvironment Environment { get; init; }

    [JsonPropertyName("cliente")]
    public required NFeCustomer Customer { get; init; }

    [JsonPropertyName("produtos")]
    public IReadOnlyList<NFeProduct>? Products { get; init; }

    [JsonPropertyName("impostos")]
    public NFeComplementTaxes? Taxes { get; init; }

    [JsonPropertyName("agropecuario")]
    public NFeAgriculture? Agriculture { get; init; }

    [JsonPropertyName("documento_adicao")]
    public NFeImportAdditionDocument? ImportDocument { get; init; }

    [JsonPropertyName("informacoes_fisco")]
    public string? TaxAuthorityInformation { get; init; }

    [JsonPropertyName("informacoes_complementares")]
    public string? CustomerInformation { get; init; }
}

/// <summary>
/// Describes taxes complemented at invoice level.
/// </summary>
public sealed record NFeComplementTaxes
{
    [JsonPropertyName("codigo_cfop")]
    public required string CfopCode { get; init; }

    [JsonPropertyName("situacao_tributaria")]
    public string? TaxSituation { get; init; }

    [JsonPropertyName("bc_icms")]
    public decimal? IcmsTaxBase { get; init; }

    [JsonPropertyName("valor_icms")]
    public decimal? IcmsValue { get; init; }

    [JsonPropertyName("valor_fcp")]
    public decimal? FcpValue { get; init; }

    [JsonPropertyName("bc_icms_st")]
    public decimal? IcmsStTaxBase { get; init; }

    [JsonPropertyName("valor_icms_st")]
    public decimal? IcmsStValue { get; init; }

    [JsonPropertyName("valor_fcp_st")]
    public decimal? FcpStValue { get; init; }

    [JsonPropertyName("aliquota_mva")]
    public decimal? MvaRate { get; init; }

    [JsonPropertyName("bc_ipi")]
    public decimal? IpiTaxBase { get; init; }

    [JsonPropertyName("valor_ipi")]
    public decimal? IpiValue { get; init; }

    [JsonPropertyName("bc_issqn")]
    public decimal? IssqnTaxBase { get; init; }

    [JsonPropertyName("valor_issqn")]
    public decimal? IssqnValue { get; init; }

    [JsonPropertyName("beneficio_fiscal")]
    public string? TaxBenefit { get; init; }

    [JsonPropertyName("ibs_cbs")]
    public NFeIbsCbsTax? IbsCbs { get; init; }
}

/// <summary>
/// Describes an import addition document for a complementary invoice.
/// </summary>
public sealed record NFeImportAdditionDocument
{
    [JsonPropertyName("ndoc_importacao")]
    public required string DocumentNumber { get; init; }

    [JsonPropertyName("ddoc_importacao")]
    public required DateOnly DocumentDate { get; init; }

    [JsonPropertyName("uf_desembaraco")]
    public required string CustomsClearanceState { get; init; }

    [JsonPropertyName("local_desembaraco")]
    public required string CustomsClearanceLocation { get; init; }

    [JsonPropertyName("data_desembaraco")]
    public required DateOnly CustomsClearanceDate { get; init; }

    [JsonPropertyName("via_transporte")]
    public required string TransportMode { get; init; }

    [JsonPropertyName("intermediacao")]
    public required string Intermediation { get; init; }

    [JsonPropertyName("fabricante")]
    public required string Manufacturer { get; init; }

    [JsonPropertyName("adicao")]
    public required string Addition { get; init; }

    [JsonPropertyName("seq_adicao")]
    public required string AdditionSequence { get; init; }

    public decimal? Afrmm { get; init; }
}

/// <summary>
/// Requests a correction letter.
/// </summary>
public sealed record NFeCorrectionLetterRequest
{
    [JsonPropertyName("chave")]
    public string? AccessKey { get; init; }

    [JsonPropertyName("uuid")]
    public Guid? Uuid { get; init; }

    [JsonPropertyName("correcao")]
    public required string Correction { get; init; }

    [JsonPropertyName("ambiente")]
    public required NFeEnvironment Environment { get; init; }

    [JsonPropertyName("evento")]
    public int EventSequence { get; init; } = 1;

    [JsonPropertyName("url_notificacao")]
    public Uri? NotificationUrl { get; init; }
}

/// <summary>
/// Requests recipient manifestation.
/// </summary>
public sealed record NFeRecipientManifestationRequest
{
    [JsonPropertyName("chave")]
    public required string AccessKey { get; init; }

    [JsonPropertyName("ambiente")]
    public required NFeEnvironment Environment { get; init; }

    [JsonIgnore]
    public NFeRecipientManifestation Manifestation { get; init; }

    [JsonPropertyName("evento")]
    public string EventCode =>
        ((int)Manifestation).ToString(
            System.Globalization.CultureInfo.InvariantCulture);

    [JsonPropertyName("justificativa")]
    public string? Justification { get; init; }
}

/// <summary>
/// Requests cancellation of an authorized invoice.
/// </summary>
public sealed record NFeCancellationRequest
{
    [JsonPropertyName("chave")]
    public string? AccessKey { get; init; }

    [JsonPropertyName("uuid")]
    public Guid? Uuid { get; init; }

    [JsonPropertyName("motivo")]
    public required string Reason { get; init; }

    [JsonPropertyName("nfce_referenciada")]
    public string? ReplacementNFCeAccessKey { get; init; }
}

/// <summary>
/// Requests invalidation of an unused NF-e/NFC-e number range.
/// </summary>
public sealed record NFeNumberInvalidationRequest
{
    [JsonPropertyName("sequencia")]
    public required string Sequence { get; init; }

    [JsonPropertyName("motivo")]
    public required string Reason { get; init; }

    [JsonPropertyName("ambiente")]
    public required NFeEnvironment Environment { get; init; }

    [JsonPropertyName("serie")]
    public required string Series { get; init; }

    [JsonPropertyName("modelo")]
    public required NFeModel Model { get; init; }
}

internal sealed record NFeAccessKeyQuery(
    [property: JsonPropertyName("chave")] string AccessKey);

internal sealed record NFeUuidQuery(
    [property: JsonPropertyName("uuid")] Guid Uuid);

internal sealed record NFeOrderIdQuery(
    [property: JsonPropertyName("ID")] string OrderId);
