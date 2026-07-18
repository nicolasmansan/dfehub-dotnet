using System.Text.Json.Serialization;

#pragma warning disable CS1591

namespace DFeHub.Providers.WebMania.NFSe;

/// <summary>
/// Contains the municipal, federal and tax-reform rates applicable to a
/// service.
/// </summary>
public sealed record NFSeTaxes
{
    [JsonPropertyName("iss")]
    public decimal? Iss { get; init; }

    /// <summary>
    /// Gets the partial-exemption Simples Nacional ISS rate supported by
    /// CONAM.
    /// </summary>
    [JsonPropertyName("iss_simples_nacional")]
    public decimal? SimplesNacionalIss { get; init; }

    /// <summary>
    /// Gets the ISS tax-base reduction used by the National Standard
    /// provider.
    /// </summary>
    [JsonPropertyName("reducao")]
    public decimal? Reduction { get; init; }

    /// <summary>
    /// Gets the PIS/COFINS tax situation used by the National Standard
    /// provider.
    /// </summary>
    [JsonPropertyName("cst_pis_cofins")]
    public string? PisCofinsTaxSituation { get; init; }

    public decimal? Pis { get; init; }

    public decimal? Cofins { get; init; }

    public decimal? Inss { get; init; }

    [JsonPropertyName("ir")]
    public decimal? IncomeTax { get; init; }

    public decimal? Csll { get; init; }

    /// <summary>
    /// Gets the employer contribution rate used by the National Standard
    /// provider.
    /// </summary>
    [JsonPropertyName("cp")]
    public decimal? EmployerContribution { get; init; }

    [JsonPropertyName("ibs_cbs")]
    public NFSeIbsCbsTax? IbsCbs { get; init; }

    /// <summary>
    /// Gets a description of the tax amounts used by Equiplano.
    /// </summary>
    [JsonPropertyName("descricao_impostos")]
    public string? TaxDescription { get; init; }
}

/// <summary>
/// Describes IBS and CBS taxation introduced by the Brazilian tax reform.
/// </summary>
public sealed record NFSeIbsCbsTax
{
    [JsonPropertyName("situacao_tributaria")]
    public required string TaxSituation { get; init; }

    [JsonPropertyName("classificacao_tributaria")]
    public required string TaxClassification { get; init; }

    [JsonPropertyName("situacao_tributaria_regular")]
    public string? RegularTaxSituation { get; init; }

    [JsonPropertyName("classificacao_tributaria_regular")]
    public string? RegularTaxClassification { get; init; }

    [JsonPropertyName("ibs_estadual")]
    public NFSeIbsCbsJurisdiction? StateIbs { get; init; }

    [JsonPropertyName("ibs_municipal")]
    public NFSeIbsCbsJurisdiction? CityIbs { get; init; }

    [JsonPropertyName("cbs")]
    public NFSeIbsCbsJurisdiction? Cbs { get; init; }

    [JsonPropertyName("credito_presumido")]
    public NFSeIbsCbsPresumedCredit? PresumedCredit { get; init; }

    [JsonPropertyName("transferencia_credito")]
    public NFSeIbsCbsCreditTransfer? CreditTransfer { get; init; }
}

/// <summary>
/// Describes deferral and returned-value information for state IBS, municipal
/// IBS or CBS.
/// </summary>
public sealed record NFSeIbsCbsJurisdiction
{
    [JsonPropertyName("aliquota_diferimento")]
    public decimal? DeferralRate { get; init; }

    [JsonPropertyName("valor_devolvido")]
    public decimal? ReturnedValue { get; init; }
}

/// <summary>
/// Describes presumed IBS and CBS credit.
/// </summary>
public sealed record NFSeIbsCbsPresumedCredit
{
    [JsonPropertyName("classificacao")]
    public required string Classification { get; init; }

    [JsonPropertyName("aliquota_ibs")]
    public decimal? IbsRate { get; init; }

    [JsonPropertyName("aliquota_cbs")]
    public decimal? CbsRate { get; init; }
}

/// <summary>
/// Describes IBS and CBS credit transferred by the transaction.
/// </summary>
public sealed record NFSeIbsCbsCreditTransfer
{
    [JsonPropertyName("valor_ibs")]
    public required decimal IbsValue { get; init; }

    [JsonPropertyName("valor_cbs")]
    public required decimal CbsValue { get; init; }
}
