using System.Text.Json.Serialization;

#pragma warning disable CS1591

namespace DFeHub.Providers.WebMania.NFSe;

/// <summary>
/// Describes the common service data accepted by every NFS-e provider.
/// Provider-specific fields are exposed by the same type in
/// <c>NFSeProviderModels.cs</c>.
/// </summary>
public sealed partial record NFSeService
{
    [JsonPropertyName("discriminacao")]
    public required string Discrimination { get; init; }

    [JsonPropertyName("valor_servicos")]
    public required decimal ServiceValue { get; init; }

    [JsonPropertyName("classe_imposto")]
    public string? TaxClass { get; init; }

    [JsonPropertyName("impostos")]
    public NFSeTaxes? Taxes { get; init; }

    [JsonPropertyName("iss_retido")]
    public int? IssWithheld { get; init; }

    [JsonPropertyName("responsavel_retencao_iss")]
    public int? IssWithholdingResponsible { get; init; }

    [JsonPropertyName("deducoes")]
    public decimal? Deductions { get; init; }

    [JsonPropertyName("desconto_incondicionado")]
    public decimal? UnconditionalDiscount { get; init; }

    [JsonPropertyName("desconto_condicionado")]
    public decimal? ConditionalDiscount { get; init; }

    [JsonPropertyName("outras_retencoes")]
    public decimal? OtherWithholdings { get; init; }

    [JsonPropertyName("numero_processo")]
    public string? ProcessNumber { get; init; }

    [JsonPropertyName("intermediario")]
    public NFSeIntermediary? Intermediary { get; init; }
}

/// <summary>
/// Describes the payment terms associated with a service.
/// </summary>
public sealed record NFSePaymentCondition
{
    [JsonPropertyName("condicao")]
    public string? Condition { get; init; }

    [JsonPropertyName("qtd_parcela")]
    public int? InstallmentCount { get; init; }

    [JsonPropertyName("parcelas")]
    public IReadOnlyList<NFSePaymentInstallment>? Installments { get; init; }
}

/// <summary>
/// Describes one payment installment.
/// </summary>
public sealed record NFSePaymentInstallment
{
    [JsonPropertyName("data_parcela")]
    public required DateOnly DueDate { get; init; }

    [JsonPropertyName("valor")]
    public required decimal Value { get; init; }
}

/// <summary>
/// Describes a governmental entity participating in the service transaction.
/// </summary>
public sealed record NFSeGovernmentEntity
{
    [JsonPropertyName("tipo")]
    public required int Type { get; init; }

    [JsonPropertyName("classificacao")]
    public string? Classification { get; init; }
}

/// <summary>
/// Describes real-estate identification used by tax-reform operations.
/// Exactly one identification form should be populated.
/// </summary>
public sealed record NFSeRealEstate
{
    [JsonPropertyName("inscricao_imobiliaria")]
    public string? PropertyRegistration { get; init; }

    [JsonPropertyName("codigo_cib")]
    public string? CibCode { get; init; }

    [JsonPropertyName("endereco")]
    public NFSeRealEstateAddress? Address { get; init; }

    [JsonPropertyName("numero")]
    public string? Number { get; init; }

    [JsonPropertyName("complemento")]
    public string? AddressComplement { get; init; }

    [JsonPropertyName("bairro")]
    public string? District { get; init; }

    [JsonPropertyName("cep")]
    public string? PostalCode { get; init; }

    [JsonPropertyName("codigo_pais")]
    public string? CountryCode { get; init; }

    [JsonPropertyName("codigo_postal")]
    public string? ForeignPostalCode { get; init; }

    [JsonPropertyName("cidade")]
    public string? City { get; init; }

    [JsonPropertyName("estado_provincia")]
    public string? StateOrProvince { get; init; }
}

/// <summary>
/// Represents the documented string-or-structured-object shapes of
/// <c>imovel.endereco</c>.
/// </summary>
[JsonConverter(typeof(NFSeRealEstateAddressConverter))]
public sealed record NFSeRealEstateAddress
{
    public NFSeRealEstateAddress()
    {
    }

    public NFSeRealEstateAddress(string directAddress)
    {
        DirectAddress = directAddress;
    }

    public NFSeRealEstateAddress(NFSeStructuredAddress structuredAddress)
    {
        StructuredAddress = structuredAddress;
    }

    public string? DirectAddress { get; init; }

    public NFSeStructuredAddress? StructuredAddress { get; init; }

    public static implicit operator NFSeRealEstateAddress(string value) =>
        new(value);

    public static implicit operator NFSeRealEstateAddress(
        NFSeStructuredAddress value) => new(value);
}

/// <summary>
/// Describes reimbursed, transferred or refunded amounts considered in the tax
/// base.
/// </summary>
public sealed record NFSeReimbursement
{
    [JsonPropertyName("documentos")]
    public required IReadOnlyList<NFSeReimbursementDocument> Documents
    {
        get;
        init;
    }
}

/// <summary>
/// Describes a document supporting reimbursement or pass-through amounts.
/// </summary>
public sealed record NFSeReimbursementDocument
{
    [JsonPropertyName("tipo_reembolso")]
    public required int ReimbursementType { get; init; }

    [JsonPropertyName("descricao")]
    public string? Description { get; init; }

    [JsonPropertyName("valor")]
    public required decimal Value { get; init; }

    [JsonPropertyName("fornecedor_referenciado")]
    public NFSeReferencedSupplier? ReferencedSupplier { get; init; }

    [JsonPropertyName("dfe_nacional")]
    public NFSeNationalFiscalDocument? NationalFiscalDocument { get; init; }

    [JsonPropertyName("dfe_outro")]
    public NFSeOtherFiscalDocument? OtherFiscalDocument { get; init; }

    [JsonPropertyName("doc_outro")]
    public NFSeOtherDocument? OtherDocument { get; init; }

    [JsonPropertyName("data_emissao")]
    public required DateOnly IssuedOn { get; init; }

    [JsonPropertyName("data_competencia")]
    public required DateOnly CompetenceDate { get; init; }
}

/// <summary>
/// Describes an electronic fiscal document available in the national data
/// environment.
/// </summary>
public sealed record NFSeNationalFiscalDocument
{
    [JsonPropertyName("tipo_chave_dfe")]
    public required int KeyType { get; init; }

    [JsonPropertyName("descricao_tipo_chave_dfe")]
    public string? KeyTypeDescription { get; init; }

    [JsonPropertyName("chave_dfe")]
    public required string AccessKey { get; init; }
}

/// <summary>
/// Describes a fiscal document that is not available in the national
/// electronic environment.
/// </summary>
public sealed record NFSeOtherFiscalDocument
{
    [JsonPropertyName("codigo_municipio_emissor")]
    public required string IssuerCityCode { get; init; }

    [JsonPropertyName("numero_documento")]
    public required string DocumentNumber { get; init; }

    [JsonPropertyName("descricao_documento")]
    public required string DocumentDescription { get; init; }
}

/// <summary>
/// Describes a non-fiscal supporting document.
/// </summary>
public sealed record NFSeOtherDocument
{
    [JsonPropertyName("numero_documento")]
    public required string DocumentNumber { get; init; }

    [JsonPropertyName("descricao_documento")]
    public required string DocumentDescription { get; init; }
}

/// <summary>
/// Describes a National Standard ISS deduction or tax-base reduction.
/// Exactly one of value, percentage or documents should be populated.
/// </summary>
public sealed record NFSeDeductionReduction
{
    [JsonPropertyName("valor")]
    public decimal? Value { get; init; }

    [JsonPropertyName("percentual")]
    public decimal? Percentage { get; init; }

    [JsonPropertyName("documentos")]
    public IReadOnlyList<NFSeDeductionDocument>? Documents { get; init; }
}

/// <summary>
/// Describes one document supporting an ISS deduction or reduction.
/// </summary>
public sealed record NFSeDeductionDocument
{
    [JsonPropertyName("chave_nfse")]
    public string? NFSeAccessKey { get; init; }

    [JsonPropertyName("chave_nfe")]
    public string? NFeAccessKey { get; init; }

    [JsonPropertyName("nfse_municipal")]
    public NFSeMunicipalInvoiceReference? MunicipalInvoice { get; init; }

    [JsonPropertyName("nf_nfs")]
    public NFSePaperInvoiceReference? PaperInvoice { get; init; }

    [JsonPropertyName("fornecedor")]
    public NFSeDeductionSupplier? Supplier { get; init; }

    [JsonPropertyName("doc_fiscal")]
    public string? FiscalDocument { get; init; }

    [JsonPropertyName("doc_nao_fiscal")]
    public string? NonFiscalDocument { get; init; }

    [JsonPropertyName("tipo_deducao_reducao")]
    public required int DeductionType { get; init; }

    [JsonPropertyName("descricao_outras_deducoes")]
    public string? OtherDeductionsDescription { get; init; }

    [JsonPropertyName("data_emissao")]
    public required DateOnly IssuedOn { get; init; }

    [JsonPropertyName("valor_dedutivel")]
    public required decimal DeductibleValue { get; init; }

    [JsonPropertyName("valor_deducao")]
    public required decimal DeductionValue { get; init; }
}

/// <summary>
/// Identifies a municipal NFS-e outside the National Standard.
/// </summary>
public sealed record NFSeMunicipalInvoiceReference
{
    [JsonPropertyName("codigo_municipio")]
    public required string CityCode { get; init; }

    [JsonPropertyName("numero")]
    public required string Number { get; init; }

    [JsonPropertyName("codigo_verificacao")]
    public required string VerificationCode { get; init; }
}

/// <summary>
/// Identifies a non-electronic NF or NFS.
/// </summary>
public sealed record NFSePaperInvoiceReference
{
    [JsonPropertyName("numero")]
    public required string Number { get; init; }

    [JsonPropertyName("modelo")]
    public required string Model { get; init; }

    [JsonPropertyName("serie")]
    public required string Series { get; init; }
}

/// <summary>
/// Contains the union of construction fields published by the supported
/// providers.
/// </summary>
public sealed record NFSeConstruction
{
    /// <summary>
    /// Gets the work code used by Abrasf, the National Standard and São Paulo
    /// RTC.
    /// </summary>
    [JsonPropertyName("codigo_obra")]
    public string? WorkCode { get; init; }

    /// <summary>
    /// Gets the technical responsibility annotation used by Abrasf.
    /// </summary>
    [JsonPropertyName("art")]
    public string? Art { get; init; }

    /// <summary>
    /// Gets the fiscal property registration used by the National Standard.
    /// </summary>
    [JsonPropertyName("inscricao_imobiliaria_fiscal")]
    public string? FiscalPropertyRegistration { get; init; }

    /// <summary>
    /// Gets the work street address used by the National Standard and
    /// Sigissweb.
    /// </summary>
    [JsonPropertyName("endereco")]
    public string? Address { get; init; }

    /// <summary>
    /// Gets the work address number used by the National Standard and
    /// Sigissweb.
    /// </summary>
    [JsonPropertyName("numero")]
    public string? Number { get; init; }

    /// <summary>
    /// Gets the work address complement used by the National Standard and
    /// Sigissweb.
    /// </summary>
    [JsonPropertyName("complemento")]
    public string? AddressComplement { get; init; }

    /// <summary>
    /// Gets the work district used by the National Standard and Sigissweb.
    /// </summary>
    [JsonPropertyName("bairro")]
    public string? District { get; init; }

    /// <summary>
    /// Gets the work postal code used by the National Standard and Sigissweb.
    /// </summary>
    [JsonPropertyName("cep")]
    public string? PostalCode { get; init; }

    /// <summary>
    /// Gets the National Construction Registry number used by Sigissweb.
    /// </summary>
    [JsonPropertyName("cno")]
    public string? Cno { get; init; }

    /// <summary>
    /// Gets the work name used by Sigissweb.
    /// </summary>
    [JsonPropertyName("nome")]
    public string? Name { get; init; }

    /// <summary>
    /// Gets the work city used by Sigissweb.
    /// </summary>
    [JsonPropertyName("cidade")]
    public string? City { get; init; }

    /// <summary>
    /// Gets the work state used by Sigissweb.
    /// </summary>
    [JsonPropertyName("uf")]
    public string? State { get; init; }

    /// <summary>
    /// Gets the property registration used by Sigissweb.
    /// </summary>
    [JsonPropertyName("inscricao_imobiliaria")]
    public string? PropertyRegistration { get; init; }

    /// <summary>
    /// Gets the foreign-work indicator used by Sigissweb.
    /// </summary>
    [JsonPropertyName("exterior")]
    public string? ForeignIndicator { get; init; }

    /// <summary>
    /// Gets the work registration used by São Paulo RTC.
    /// </summary>
    [JsonPropertyName("matricula_obra")]
    public string? WorkRegistration { get; init; }

    /// <summary>
    /// Gets the encapsulation number used by São Paulo RTC.
    /// </summary>
    [JsonPropertyName("numero_encapsulamento")]
    public string? EncapsulationNumber { get; init; }

    /// <summary>
    /// Gets the work-location state used by São Paulo RTC.
    /// </summary>
    [JsonPropertyName("uf_local_obra")]
    public string? WorkLocationState { get; init; }

    /// <summary>
    /// Gets the work-location city used by São Paulo RTC.
    /// </summary>
    [JsonPropertyName("cidade_local_obra")]
    public string? WorkLocationCity { get; init; }
}

/// <summary>
/// Contains the union of event-activity fields published by the National
/// Standard, Sigissweb and São Paulo RTC providers.
/// </summary>
public sealed record NFSeEventActivity
{
    [JsonPropertyName("descricao_evento")]
    public string? Description { get; init; }

    [JsonPropertyName("data_inicio")]
    public DateOnly? StartDate { get; init; }

    [JsonPropertyName("data_final")]
    public DateOnly? EndDate { get; init; }

    [JsonPropertyName("identificacao")]
    public string? Identification { get; init; }

    [JsonPropertyName("endereco")]
    public string? Address { get; init; }

    [JsonPropertyName("numero")]
    public string? Number { get; init; }

    [JsonPropertyName("complemento")]
    public string? AddressComplement { get; init; }

    [JsonPropertyName("bairro")]
    public string? District { get; init; }

    [JsonPropertyName("cep")]
    public string? PostalCode { get; init; }

    /// <summary>
    /// Gets the event city used by Sigissweb and São Paulo RTC.
    /// </summary>
    [JsonPropertyName("cidade")]
    public string? City { get; init; }

    /// <summary>
    /// Gets the event state used by Sigissweb.
    /// </summary>
    [JsonPropertyName("uf")]
    public string? State { get; init; }

    /// <summary>
    /// Gets the service state used by Sigissweb.
    /// </summary>
    [JsonPropertyName("uf_prestacao")]
    public string? ServiceState { get; init; }

    /// <summary>
    /// Gets the event country used by Sigissweb.
    /// </summary>
    [JsonPropertyName("pais")]
    public string? Country { get; init; }

    /// <summary>
    /// Gets the foreign-event indicator used by Sigissweb.
    /// </summary>
    [JsonPropertyName("exterior")]
    public string? ForeignIndicator { get; init; }

    /// <summary>
    /// Gets the BACEN country code used by São Paulo RTC.
    /// </summary>
    [JsonPropertyName("codigo_pais")]
    public string? CountryCode { get; init; }

    /// <summary>
    /// Gets the foreign postal code used by São Paulo RTC.
    /// </summary>
    [JsonPropertyName("cep_exterior")]
    public string? ForeignPostalCode { get; init; }

    /// <summary>
    /// Gets the foreign state or province used by São Paulo RTC.
    /// </summary>
    [JsonPropertyName("estado")]
    public string? ForeignState { get; init; }
}
