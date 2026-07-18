using System.Text.Json.Serialization;

#pragma warning disable CS1591

namespace DFeHub.Providers.WebMania.NFe;

/// <summary>
/// Contains item-level taxes accepted by the NF-e API.
/// </summary>
public sealed record NFeTaxes
{
    public NFeIcmsTax? Icms { get; init; }

    public NFeIpiTax? Ipi { get; init; }

    public NFeContributionTax? Pis { get; init; }

    public NFeContributionTax? Cofins { get; init; }

    public NFeIssqnTax? Issqn { get; init; }

    [JsonPropertyName("retencao_tributos")]
    public NFeWithholdingTaxes? Withholding { get; init; }

    [JsonPropertyName("importacao")]
    public NFeImportTax? Import { get; init; }

    [JsonPropertyName("ibs_cbs")]
    public NFeIbsCbsTax? IbsCbs { get; init; }
}

/// <summary>
/// Describes ICMS for Simples Nacional or the normal regime.
/// </summary>
public sealed record NFeIcmsTax
{
    [JsonPropertyName("codigo_cfop")]
    public required string CfopCode { get; init; }

    [JsonPropertyName("situacao_tributaria")]
    public required string TaxSituation { get; init; }

    [JsonPropertyName("aliquota")]
    public decimal? Rate { get; init; }

    [JsonPropertyName("aliquota_fcp")]
    public decimal? FcpRate { get; init; }

    [JsonPropertyName("aliquota_interestadual")]
    public decimal? InterstateRate { get; init; }

    [JsonPropertyName("aliquota_importacao")]
    public decimal? ImportRate { get; init; }

    [JsonPropertyName("industria")]
    public string? Industry { get; init; }

    [JsonPropertyName("majoracao")]
    public decimal? Increase { get; init; }

    [JsonPropertyName("difal_antecipacao")]
    public bool? AnticipatedDifal { get; init; }

    [JsonPropertyName("codigo_beneficio_reducao_bc")]
    public string? TaxBaseReductionBenefitCode { get; init; }

    [JsonPropertyName("aliquota_mva")]
    public decimal? MvaRate { get; init; }

    [JsonPropertyName("pauta_fiscal")]
    public decimal? FiscalPrice { get; init; }

    [JsonPropertyName("valor_operacao")]
    public decimal? OperationValue { get; init; }

    [JsonPropertyName("bc_icms")]
    public decimal? TaxBase { get; init; }

    [JsonPropertyName("valor_icms")]
    public decimal? TaxValue { get; init; }

    [JsonPropertyName("bc_icms_st")]
    public decimal? StTaxBase { get; init; }

    [JsonPropertyName("valor_icms_st")]
    public decimal? StTaxValue { get; init; }

    [JsonPropertyName("aliquota_diferimento")]
    public decimal? DeferralRate { get; init; }

    [JsonPropertyName("credito_presumido")]
    public NFeIcmsPresumedCredit? PresumedCredit { get; init; }
}

/// <summary>
/// Describes ICMS presumed credit.
/// </summary>
public sealed record NFeIcmsPresumedCredit
{
    [JsonPropertyName("codigo_beneficio_fiscal")]
    public required string TaxBenefitCode { get; init; }

    [JsonPropertyName("aliquota")]
    public required decimal Rate { get; init; }

    [JsonPropertyName("valor")]
    public decimal? Value { get; init; }
}

/// <summary>
/// Describes IPI.
/// </summary>
public sealed record NFeIpiTax
{
    [JsonPropertyName("situacao_tributaria")]
    public required string TaxSituation { get; init; }

    [JsonPropertyName("codigo_enquadramento")]
    public required string LegalFrameworkCode { get; init; }

    [JsonPropertyName("aliquota")]
    public decimal? Rate { get; init; }

    [JsonPropertyName("percentual_devolvido")]
    public decimal? ReturnedPercentage { get; init; }

    [JsonPropertyName("ipi_devolvido")]
    public decimal? ReturnedValue { get; init; }

    [JsonPropertyName("codigo_selo")]
    public string? SealCode { get; init; }

    [JsonPropertyName("qtd_selo")]
    public string? SealQuantity { get; init; }

    [JsonPropertyName("bc_ipi")]
    public decimal? TaxBase { get; init; }
}

/// <summary>
/// Describes PIS or COFINS.
/// </summary>
public sealed record NFeContributionTax
{
    [JsonPropertyName("situacao_tributaria")]
    public required string TaxSituation { get; init; }

    [JsonPropertyName("aliquota")]
    public decimal? Rate { get; init; }

    [JsonPropertyName("majoracao")]
    public decimal? Increase { get; init; }

    [JsonPropertyName("bc_pis")]
    public decimal? PisTaxBase { get; init; }

    [JsonPropertyName("bc_cofins")]
    public decimal? CofinsTaxBase { get; init; }
}

/// <summary>
/// Describes ISSQN.
/// </summary>
public sealed record NFeIssqnTax
{
    [JsonPropertyName("codigo_cfop")]
    public required string CfopCode { get; init; }

    [JsonPropertyName("exigibilidade")]
    public required int Enforceability { get; init; }

    [JsonPropertyName("item_servico")]
    public required string ServiceItem { get; init; }

    [JsonPropertyName("incentivo_fiscal")]
    public required string TaxIncentive { get; init; }

    [JsonPropertyName("aliquota")]
    public required decimal Rate { get; init; }

    [JsonPropertyName("deducao")]
    public decimal? Deduction { get; init; }

    [JsonPropertyName("retencoes")]
    public decimal? OtherWithholdings { get; init; }

    [JsonPropertyName("desconto_incondicionado")]
    public decimal? UnconditionalDiscount { get; init; }

    [JsonPropertyName("desconto_condicionado")]
    public decimal? ConditionalDiscount { get; init; }

    [JsonPropertyName("iss_retido")]
    public decimal? WithheldIss { get; init; }

    [JsonPropertyName("municipio")]
    public string? City { get; init; }

    [JsonPropertyName("codigo_servico")]
    public string? ServiceCode { get; init; }

    [JsonPropertyName("processo")]
    public string? ProcessNumber { get; init; }
}

/// <summary>
/// Describes federal and social-security withholdings.
/// </summary>
public sealed record NFeWithholdingTaxes
{
    [JsonPropertyName("valor_pis")]
    public decimal? Pis { get; init; }

    [JsonPropertyName("valor_cofins")]
    public decimal? Cofins { get; init; }

    [JsonPropertyName("valor_csll")]
    public decimal? Csll { get; init; }

    [JsonPropertyName("bc_irrf")]
    public decimal? IrrfTaxBase { get; init; }

    [JsonPropertyName("valor_irrf")]
    public decimal? Irrf { get; init; }

    [JsonPropertyName("bc_previdencia")]
    public decimal? SocialSecurityTaxBase { get; init; }

    [JsonPropertyName("valor_previdencia")]
    public decimal? SocialSecurity { get; init; }
}

/// <summary>
/// Describes import taxes.
/// </summary>
public sealed record NFeImportTax
{
    [JsonPropertyName("aliquota")]
    public required decimal Rate { get; init; }

    public required decimal Iof { get; init; }
}

/// <summary>
/// Describes IBS and CBS data introduced by the tax reform.
/// </summary>
public sealed record NFeIbsCbsTax
{
    [JsonPropertyName("situacao_tributaria")]
    public required string TaxSituation { get; init; }

    [JsonPropertyName("classificacao_tributaria")]
    public required string TaxClassification { get; init; }

    [JsonPropertyName("situacao_tributaria_regular")]
    public string? RegularTaxSituation { get; init; }

    [JsonPropertyName("classificacao_tributaria_regular")]
    public string? RegularTaxClassification { get; init; }

    [JsonPropertyName("base_calculo")]
    public decimal? TaxBase { get; init; }

    [JsonPropertyName("ibs_estadual")]
    public NFeIbsCbsJurisdiction? StateIbs { get; init; }

    [JsonPropertyName("ibs_municipal")]
    public NFeIbsCbsJurisdiction? CityIbs { get; init; }

    public NFeIbsCbsJurisdiction? Cbs { get; init; }

    [JsonPropertyName("tributacao_monofasica")]
    public NFeIbsCbsMonophase? Monophase { get; init; }

    [JsonPropertyName("credito_presumido")]
    public NFeIbsCbsPresumedCredit? PresumedCredit { get; init; }

    [JsonPropertyName("transferencia_credito")]
    public NFeIbsCbsValues? CreditTransfer { get; init; }

    [JsonPropertyName("ajuste_competencia")]
    public NFeIbsCbsCompetenceAdjustment? CompetenceAdjustment { get; init; }

    [JsonPropertyName("estorno_credito")]
    public NFeIbsCbsValues? CreditReversal { get; init; }
}

/// <summary>
/// Describes a jurisdiction-specific IBS/CBS calculation.
/// </summary>
public sealed record NFeIbsCbsJurisdiction
{
    [JsonPropertyName("aliquota_diferimento")]
    public decimal? DeferralRate { get; init; }

    [JsonPropertyName("valor_devolvido")]
    public decimal? ReturnedValue { get; init; }
}

/// <summary>
/// Describes monophase IBS/CBS taxation.
/// </summary>
public sealed record NFeIbsCbsMonophase
{
    [JsonPropertyName("quantidade_bc")]
    public decimal? TaxBaseQuantity { get; init; }

    [JsonPropertyName("aliquota_ibs")]
    public decimal? IbsRate { get; init; }

    [JsonPropertyName("aliquota_cbs")]
    public decimal? CbsRate { get; init; }

    [JsonPropertyName("quantidade_bc_retencao")]
    public decimal? WithholdingTaxBaseQuantity { get; init; }

    [JsonPropertyName("aliquota_ibs_retencao")]
    public decimal? WithholdingIbsRate { get; init; }

    [JsonPropertyName("aliquota_cbs_retencao")]
    public decimal? WithholdingCbsRate { get; init; }

    [JsonPropertyName("quantidade_bc_retido")]
    public decimal? PreviouslyWithheldTaxBaseQuantity { get; init; }

    [JsonPropertyName("aliquota_ibs_retido")]
    public decimal? PreviouslyWithheldIbsRate { get; init; }

    [JsonPropertyName("aliquota_cbs_retido")]
    public decimal? PreviouslyWithheldCbsRate { get; init; }

    [JsonPropertyName("aliquota_diferimento_ibs")]
    public decimal? IbsDeferralRate { get; init; }

    [JsonPropertyName("aliquota_diferimento_cbs")]
    public decimal? CbsDeferralRate { get; init; }
}

/// <summary>
/// Describes presumed IBS/CBS credit.
/// </summary>
public sealed record NFeIbsCbsPresumedCredit
{
    [JsonPropertyName("classificacao")]
    public required string Classification { get; init; }

    [JsonPropertyName("bc")]
    public decimal? TaxBase { get; init; }

    [JsonPropertyName("aliquota_ibs")]
    public required decimal IbsRate { get; init; }

    [JsonPropertyName("aliquota_cbs")]
    public required decimal CbsRate { get; init; }
}

/// <summary>
/// Describes a pair of IBS and CBS values.
/// </summary>
public sealed record NFeIbsCbsValues
{
    [JsonPropertyName("valor_ibs")]
    public required decimal Ibs { get; init; }

    [JsonPropertyName("valor_cbs")]
    public required decimal Cbs { get; init; }
}

/// <summary>
/// Describes an IBS/CBS competence adjustment.
/// </summary>
public sealed record NFeIbsCbsCompetenceAdjustment
{
    [JsonPropertyName("competencia_apuracao")]
    public required string TaxPeriod { get; init; }

    [JsonPropertyName("valor_ibs")]
    public required decimal Ibs { get; init; }

    [JsonPropertyName("valor_cbs")]
    public required decimal Cbs { get; init; }
}
