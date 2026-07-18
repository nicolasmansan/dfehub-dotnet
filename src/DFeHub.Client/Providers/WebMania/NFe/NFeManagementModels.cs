using System.Text.Json.Serialization;

#pragma warning disable CS1591

namespace DFeHub.Providers.WebMania.NFe;

/// <summary>
/// Requests creation or replacement of an NF-e/NFC-e tax class.
/// </summary>
public sealed record NFeTaxClassUpsertRequest
{
    [JsonPropertyName("referencia")]
    public string? Reference { get; init; }

    [JsonPropertyName("descricao")]
    public required string Description { get; init; }

    public required IReadOnlyList<NFeTaxClassIcms> Icms { get; init; }

    public required IReadOnlyList<NFeTaxClassContribution> Ipi { get; init; }

    public required IReadOnlyList<NFeTaxClassContribution> Pis { get; init; }

    public required IReadOnlyList<NFeTaxClassContribution> Cofins { get; init; }

    public IReadOnlyList<NFeTaxClassIssqn>? Issqn { get; init; }

    [JsonPropertyName("ibs_cbs")]
    public IReadOnlyList<NFeTaxClassIbsCbs>? IbsCbs { get; init; }

    [JsonPropertyName("informacoes_fisco")]
    public string? TaxAuthorityInformation { get; init; }

    [JsonPropertyName("informacoes_complementares")]
    public string? CustomerInformation { get; init; }
}

/// <summary>
/// Describes an ICMS scenario in a tax class.
/// </summary>
public sealed record NFeTaxClassIcms
{
    [JsonPropertyName("tipo_tributacao")]
    public required string TaxRegime { get; init; }

    [JsonPropertyName("cenario")]
    public required string Scenario { get; init; }

    [JsonPropertyName("tipo_pessoa")]
    public required string PersonType { get; init; }

    [JsonPropertyName("nao_contribuinte")]
    public bool? NonContributor { get; init; }

    [JsonPropertyName("codigo_cfop")]
    public required string CfopCode { get; init; }

    [JsonPropertyName("situacao_tributaria")]
    public required string TaxSituation { get; init; }

    [JsonPropertyName("aliquota_importacao")]
    public decimal? ImportRate { get; init; }

    [JsonPropertyName("aliquota_credito")]
    public decimal? CreditRate { get; init; }

    [JsonPropertyName("aliquota_carga_efetiva")]
    public decimal? EffectiveTaxLoadRate { get; init; }

    [JsonPropertyName("aliquota_mva")]
    public IReadOnlyList<NFeTaxClassStateRate>? MvaRates { get; init; }

    [JsonPropertyName("aliquota_icms")]
    public IReadOnlyList<NFeTaxClassStateRate>? IcmsRates { get; init; }

    [JsonPropertyName("aliquota_icms_st")]
    public IReadOnlyList<NFeTaxClassStateRate>? IcmsStRates { get; init; }

    [JsonPropertyName("aliquota_fcp")]
    public IReadOnlyList<NFeTaxClassStateRate>? FcpRates { get; init; }

    [JsonPropertyName("aliquota_fcp_st")]
    public IReadOnlyList<NFeTaxClassStateRate>? FcpStRates { get; init; }

    [JsonPropertyName("aliquota_diferimento")]
    public decimal? DeferralRate { get; init; }

    [JsonPropertyName("aliquota_diferimento_fcp")]
    public decimal? FcpDeferralRate { get; init; }

    [JsonPropertyName("aliquota_reducao")]
    public decimal? TaxBaseReductionRate { get; init; }

    [JsonPropertyName("aliquota_reducao_st")]
    public decimal? StTaxBaseReductionRate { get; init; }

    [JsonPropertyName("motivo_desoneracao")]
    public string? ExemptionReason { get; init; }

    [JsonPropertyName("deducao_desoneracao")]
    public string? DeductExemption { get; init; }

    [JsonPropertyName("motivo_desoneracao_st")]
    public string? StExemptionReason { get; init; }

    [JsonPropertyName("beneficio_fiscal")]
    public IReadOnlyList<NFeTaxClassStateBenefit>? TaxBenefits { get; init; }

    [JsonPropertyName("credito_presumido")]
    public IReadOnlyList<NFeTaxClassPresumedCredit>? PresumedCredits { get; init; }
}

/// <summary>
/// Describes a state-specific tax class rate.
/// </summary>
public sealed record NFeTaxClassStateRate
{
    [JsonPropertyName("estado")]
    public required string State { get; init; }

    [JsonPropertyName("valor")]
    public required decimal Value { get; init; }
}

/// <summary>
/// Describes a state-specific tax benefit.
/// </summary>
public sealed record NFeTaxClassStateBenefit
{
    [JsonPropertyName("estado")]
    public required string State { get; init; }

    [JsonPropertyName("codigo")]
    public required string Code { get; init; }
}

/// <summary>
/// Describes state-specific presumed credit in a tax class.
/// </summary>
public sealed record NFeTaxClassPresumedCredit
{
    [JsonPropertyName("estado")]
    public required string State { get; init; }

    [JsonPropertyName("codigo_beneficio_fiscal")]
    public required string TaxBenefitCode { get; init; }

    [JsonPropertyName("aliquota")]
    public required decimal Rate { get; init; }
}

/// <summary>
/// Describes an IPI, PIS or COFINS tax class scenario.
/// </summary>
public sealed record NFeTaxClassContribution
{
    [JsonPropertyName("cenario")]
    public required string Scenario { get; init; }

    [JsonPropertyName("tipo_pessoa")]
    public required string PersonType { get; init; }

    [JsonPropertyName("situacao_tributaria")]
    public required string TaxSituation { get; init; }

    [JsonPropertyName("codigo_enquadramento")]
    public string? LegalFrameworkCode { get; init; }

    [JsonPropertyName("aliquota")]
    public decimal? Rate { get; init; }
}

/// <summary>
/// Describes an ISSQN tax class scenario.
/// </summary>
public sealed record NFeTaxClassIssqn
{
    [JsonPropertyName("cenario")]
    public required string Scenario { get; init; }

    [JsonPropertyName("tipo_pessoa")]
    public required string PersonType { get; init; }

    [JsonPropertyName("codigo_cfop")]
    public required string CfopCode { get; init; }

    [JsonPropertyName("exigibilidade")]
    public required string Enforceability { get; init; }

    [JsonPropertyName("item_servico")]
    public required string ServiceItem { get; init; }

    [JsonPropertyName("incentivo_fiscal")]
    public required string TaxIncentive { get; init; }

    [JsonPropertyName("aliquota")]
    public required decimal Rate { get; init; }

    [JsonPropertyName("municipio")]
    public string? City { get; init; }

    [JsonPropertyName("codigo_servico")]
    public string? ServiceCode { get; init; }
}

/// <summary>
/// Describes an IBS/CBS tax class scenario.
/// </summary>
public sealed record NFeTaxClassIbsCbs
{
    [JsonPropertyName("cenario")]
    public required string Scenario { get; init; }

    [JsonPropertyName("tipo_pessoa")]
    public required string PersonType { get; init; }

    [JsonPropertyName("situacao_tributaria")]
    public required string TaxSituation { get; init; }

    [JsonPropertyName("classificacao_tributaria")]
    public required string TaxClassification { get; init; }

    [JsonPropertyName("situacao_tributaria_regular")]
    public string? RegularTaxSituation { get; init; }

    [JsonPropertyName("classificacao_tributaria_regular")]
    public string? RegularTaxClassification { get; init; }

    [JsonPropertyName("ibs_estadual")]
    public NFeIbsCbsJurisdiction? StateIbs { get; init; }

    [JsonPropertyName("ibs_municipal")]
    public NFeIbsCbsJurisdiction? CityIbs { get; init; }

    public NFeIbsCbsJurisdiction? Cbs { get; init; }

    [JsonPropertyName("credito_presumido")]
    public NFeIbsCbsPresumedCredit? PresumedCredit { get; init; }
}

/// <summary>
/// Requests deletion of one or more tax classes.
/// </summary>
public sealed record NFeTaxClassDeleteRequest
{
    [JsonPropertyName("referencia")]
    public required IReadOnlyList<string> References { get; init; }
}

/// <summary>
/// Requests asynchronous export of fiscal documents.
/// </summary>
public sealed record NFeReportRequest
{
    [JsonPropertyName("data_inicio")]
    public required DateOnly StartDate { get; init; }

    [JsonPropertyName("data_final")]
    public required DateOnly EndDate { get; init; }

    [JsonPropertyName("numero_inicio")]
    public string? StartNumber { get; init; }

    [JsonPropertyName("numero_final")]
    public string? EndNumber { get; init; }

    [JsonPropertyName("modelo")]
    public required string Model { get; init; }

    [JsonPropertyName("relatorio")]
    public required string Report { get; init; }

    [JsonPropertyName("separar_pdf")]
    public int? SplitPdf { get; init; }

    [JsonPropertyName("status")]
    public string? Status { get; init; }

    [JsonPropertyName("ordem")]
    public string? Order { get; init; }

    [JsonPropertyName("url_notificacao")]
    public Uri? NotificationUrl { get; init; }
}

/// <summary>
/// Requests an update to issuer company data.
/// </summary>
public sealed record NFeCompanyUpdateRequest
{
    [JsonPropertyName("tipo_tributacao")]
    public string? TaxRegime { get; init; }

    [JsonPropertyName("regime_tributario")]
    public string? CorporateTaxRegime { get; init; }

    public string? Cnpj { get; init; }

    [JsonPropertyName("razao_social")]
    public string? CorporateName { get; init; }

    public string? Cpf { get; init; }

    [JsonPropertyName("nome_completo")]
    public string? FullName { get; init; }

    [JsonPropertyName("nome_fantasia")]
    public string? TradeName { get; init; }

    [JsonPropertyName("ie")]
    public string? StateRegistration { get; init; }

    [JsonPropertyName("im")]
    public string? CityRegistration { get; init; }

    [JsonPropertyName("unidade_empresa")]
    public string? CompanyUnit { get; init; }

    public string? Email { get; init; }

    [JsonPropertyName("telefone")]
    public string? Phone { get; init; }

    [JsonPropertyName("conta_bancaria_banco")]
    public string? BankCode { get; init; }

    [JsonPropertyName("conta_bancaria_agencia")]
    public string? BankBranch { get; init; }

    [JsonPropertyName("conta_bancaria_numero")]
    public string? BankAccount { get; init; }

    [JsonPropertyName("conta_bancaria_digito")]
    public string? BankAccountDigit { get; init; }

    [JsonPropertyName("contabilidade")]
    public string? AccountantDocuments { get; init; }

    [JsonPropertyName("url_notificacao")]
    public Uri? NotificationUrl { get; init; }

    [JsonPropertyName("logomarca")]
    public Uri? LogoUrl { get; init; }

    [JsonPropertyName("cep")]
    public string? PostalCode { get; init; }

    [JsonPropertyName("endereco")]
    public string? Address { get; init; }

    [JsonPropertyName("numero")]
    public string? AddressNumber { get; init; }

    [JsonPropertyName("complemento")]
    public string? AddressComplement { get; init; }

    [JsonPropertyName("bairro")]
    public string? District { get; init; }

    [JsonPropertyName("cidade")]
    public string? City { get; init; }

    [JsonPropertyName("uf")]
    public string? State { get; init; }

    [JsonPropertyName("nfe_serie")]
    public int? NFeSeries { get; init; }

    [JsonPropertyName("nfe_numero")]
    public int? NextNFeNumber { get; init; }

    [JsonPropertyName("nfe_numero_dev")]
    public int? NextCertificationNFeNumber { get; init; }

    [JsonPropertyName("nfce_serie")]
    public int? NFCeSeries { get; init; }

    [JsonPropertyName("nfce_numero")]
    public int? NextNFCeNumber { get; init; }

    [JsonPropertyName("nfce_numero_dev")]
    public int? NextCertificationNFCeNumber { get; init; }

    [JsonPropertyName("nfce_id_csc")]
    public string? NFCeCscId { get; init; }

    [JsonPropertyName("nfce_codigo_csc")]
    public string? NFCeCscCode { get; init; }

    [JsonPropertyName("nfce_id_csc_dev")]
    public string? CertificationNFCeCscId { get; init; }

    [JsonPropertyName("nfce_codigo_csc_dev")]
    public string? CertificationNFCeCscCode { get; init; }

    [JsonPropertyName("informacoes_fisco")]
    public string? TaxAuthorityInformation { get; init; }

    [JsonPropertyName("cnae_issqn")]
    public string? MainCnae { get; init; }

    [JsonPropertyName("certificado")]
    public string? CertificateBase64 { get; init; }

    [JsonPropertyName("certificado_senha")]
    public string? CertificatePassword { get; init; }

    [JsonPropertyName("orientacao_danfe")]
    public string? DanfeOrientation { get; init; }

    [JsonPropertyName("microcervejaria")]
    public bool? Microbrewery { get; init; }

    [JsonPropertyName("exclusao_icms_pis_cofins")]
    public bool? ExcludeIcmsFromPisCofins { get; init; }

    [JsonPropertyName("exclusao_difal_pis_cofins")]
    public bool? ExcludeDifalFromPisCofins { get; init; }

    [JsonPropertyName("deduzir_desconto_ipi")]
    public bool? DeductDiscountFromIpi { get; init; }

    [JsonPropertyName("base_calculo_ipi_sem_frete")]
    public bool? ExcludeFreightFromIpiTaxBase { get; init; }

    [JsonPropertyName("desativar_epec")]
    public string? DisableEpec { get; init; }

    [JsonPropertyName("ocultar_total_etiqueta")]
    public string? HideLabelTotal { get; init; }
}
