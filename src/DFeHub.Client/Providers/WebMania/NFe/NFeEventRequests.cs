using System.Text.Json.Serialization;

#pragma warning disable CS1591

namespace DFeHub.Providers.WebMania.NFe;

/// <summary>
/// Requests registration of an IBS/CBS event.
/// </summary>
public sealed record NFeIbsCbsEventRequest
{
    [JsonPropertyName("chave")]
    public required string AccessKey { get; init; }

    [JsonPropertyName("ambiente")]
    public required NFeEnvironment Environment { get; init; }

    [JsonPropertyName("cod_evento")]
    public required string EventCode { get; init; }

    [JsonPropertyName("evento")]
    public int EventSequence { get; init; } = 1;

    [JsonPropertyName("url_notificacao")]
    public Uri? NotificationUrl { get; init; }

    [JsonPropertyName("itens")]
    public IReadOnlyList<NFeIbsCbsEventItem>? Items { get; init; }

    [JsonPropertyName("data_previsao_entrega")]
    public DateOnly? ExpectedDeliveryDate { get; init; }

    [JsonPropertyName("indicador_aceitacao")]
    public int? AcceptanceIndicator { get; init; }
}

/// <summary>
/// Describes an item in an IBS/CBS event.
/// </summary>
public sealed record NFeIbsCbsEventItem
{
    [JsonPropertyName("tipo_autor")]
    public int? AuthorType { get; init; }

    [JsonPropertyName("item")]
    public required int Item { get; init; }

    [JsonPropertyName("valor_ibs")]
    public decimal? IbsValue { get; init; }

    [JsonPropertyName("valor_cbs")]
    public decimal? CbsValue { get; init; }

    [JsonPropertyName("valor_credito_ibs")]
    public decimal? IbsCreditValue { get; init; }

    [JsonPropertyName("valor_credito_cbs")]
    public decimal? CbsCreditValue { get; init; }

    [JsonPropertyName("base_calculo")]
    public decimal? TaxBase { get; init; }

    [JsonPropertyName("controle_estoque")]
    public NFeIbsCbsStockControl? StockControl { get; init; }

    [JsonPropertyName("credito_presumido")]
    public NFeIbsCbsEventPresumedCredit? PresumedCredit { get; init; }

    [JsonPropertyName("dfe_referenciado")]
    public NFeReferencedDocumentItem? ReferencedDocument { get; init; }
}

/// <summary>
/// Contains the quantity and credit fields used by IBS/CBS stock events.
/// </summary>
public sealed record NFeIbsCbsStockControl
{
    [JsonPropertyName("quantidade")]
    public decimal? Quantity { get; init; }

    [JsonPropertyName("unidade")]
    public string? Unit { get; init; }

    [JsonPropertyName("quantidade_perecimento")]
    public decimal? LostQuantity { get; init; }

    [JsonPropertyName("unidade_perecimento")]
    public string? LostUnit { get; init; }

    [JsonPropertyName("valor_ibs_estorno")]
    public decimal? IbsReversalValue { get; init; }

    [JsonPropertyName("valor_cbs_estorno")]
    public decimal? CbsReversalValue { get; init; }

    [JsonPropertyName("quantidade_nao_fornecida")]
    public decimal? NotSuppliedQuantity { get; init; }

    [JsonPropertyName("unidade_nao_fornecida")]
    public string? NotSuppliedUnit { get; init; }

    [JsonPropertyName("quantidade_consumo")]
    public decimal? PersonalConsumptionQuantity { get; init; }

    [JsonPropertyName("unidade_consumo")]
    public string? PersonalConsumptionUnit { get; init; }

    [JsonPropertyName("quantidade_imobilizada")]
    public decimal? FixedAssetQuantity { get; init; }

    [JsonPropertyName("unidade_imobilizada")]
    public string? FixedAssetUnit { get; init; }

    [JsonPropertyName("quantidade_combustivel")]
    public decimal? FuelQuantity { get; init; }

    [JsonPropertyName("unidade_combustivel")]
    public string? FuelUnit { get; init; }
}

/// <summary>
/// Describes presumed credit requested by event 211110.
/// </summary>
public sealed record NFeIbsCbsEventPresumedCredit
{
    [JsonPropertyName("classificacao")]
    public required string Classification { get; init; }

    [JsonPropertyName("aliquota_ibs")]
    public required decimal IbsRate { get; init; }

    [JsonPropertyName("aliquota_cbs")]
    public required decimal CbsRate { get; init; }

    [JsonPropertyName("valor_ibs")]
    public required decimal IbsValue { get; init; }

    [JsonPropertyName("valor_cbs")]
    public required decimal CbsValue { get; init; }
}

/// <summary>
/// Requests cancellation of an authorized IBS/CBS event.
/// </summary>
public sealed record NFeIbsCbsEventCancellationRequest
{
    public required Guid Uuid { get; init; }

    [JsonPropertyName("ambiente")]
    public NFeEnvironment? Environment { get; init; }

    [JsonPropertyName("url_notificacao")]
    public Uri? NotificationUrl { get; init; }
}

/// <summary>
/// Requests a financial reconciliation event (ECONF).
/// </summary>
public sealed record NFeFinancialReconciliationRequest
{
    [JsonPropertyName("chave")]
    public string? AccessKey { get; init; }

    public Guid? Uuid { get; init; }

    [JsonPropertyName("pagamento")]
    public NFeReconciliationPayment? Payment { get; init; }

    [JsonPropertyName("pagamentos")]
    public IReadOnlyList<NFeReconciliationPayment>? Payments { get; init; }

    [JsonPropertyName("ambiente")]
    public NFeEnvironment? Environment { get; init; }

    [JsonPropertyName("evento")]
    public int EventSequence { get; init; } = 1;

    [JsonPropertyName("url_notificacao")]
    public Uri? NotificationUrl { get; init; }
}

/// <summary>
/// Describes one payment in an ECONF event.
/// </summary>
public sealed record NFeReconciliationPayment
{
    [JsonPropertyName("indicador_pagamento")]
    public required int PaymentIndicator { get; init; }

    [JsonPropertyName("forma_pagamento")]
    public required string PaymentMethod { get; init; }

    [JsonPropertyName("desc_pagamento")]
    public string? PaymentDescription { get; init; }

    [JsonPropertyName("valor_pagamento")]
    public required decimal Value { get; init; }

    [JsonPropertyName("data_pagamento")]
    public required DateOnly Date { get; init; }

    [JsonPropertyName("cnpj_transacional_pag")]
    public string? TransactionalCnpj { get; init; }

    [JsonPropertyName("uf_pagamento")]
    public string? PaymentState { get; init; }

    [JsonPropertyName("cnpj_instituicao_financeira")]
    public string? FinancialInstitutionCnpj { get; init; }

    [JsonPropertyName("bandeira")]
    public string? CardBrand { get; init; }

    [JsonPropertyName("autorizacao")]
    public string? Authorization { get; init; }

    [JsonPropertyName("cnpj_beneficiario")]
    public string? BeneficiaryCnpj { get; init; }

    [JsonPropertyName("uf_beneficiario")]
    public string? BeneficiaryState { get; init; }
}

/// <summary>
/// Requests cancellation of an ECONF event.
/// </summary>
public sealed record NFeFinancialReconciliationCancellationRequest
{
    public required Guid Uuid { get; init; }

    [JsonPropertyName("url_notificacao")]
    public Uri? NotificationUrl { get; init; }
}
