using System.Globalization;
using System.Text.Json.Serialization;

#pragma warning disable CS1591

namespace DFeHub.Providers.WebMania.NFSe;

/// <summary>
/// Represents a provider field that is a JSON number for some providers and a
/// JSON string for others.
/// </summary>
[JsonConverter(typeof(NFSeStringOrIntValueConverter))]
public readonly record struct NFSeStringOrIntValue
{
    public NFSeStringOrIntValue(int value)
    {
        IntegerValue = value;
        StringValue = null;
    }

    public NFSeStringOrIntValue(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        IntegerValue = null;
        StringValue = value;
    }

    public int? IntegerValue { get; }

    public string? StringValue { get; }

    public bool IsInteger => IntegerValue.HasValue;

    public static NFSeStringOrIntValue FromInt32(int value) => new(value);

    public static NFSeStringOrIntValue FromString(string value) => new(value);

    public static implicit operator NFSeStringOrIntValue(int value) =>
        FromInt32(value);

    public static implicit operator NFSeStringOrIntValue(string value) =>
        FromString(value);

    public override string ToString() =>
        IntegerValue?.ToString(CultureInfo.InvariantCulture) ??
        StringValue ??
        string.Empty;
}

/// <summary>
/// Extends <see cref="NFSeService"/> with the optional fields published by
/// individual municipality providers and by the National Standard.
/// </summary>
public sealed partial record NFSeService
{
    /// <summary>
    /// Gets the provider service code.
    /// </summary>
    /// <remarks>
    /// Used by Abrasf, Cecam, Equiplano, IPM, Osasco, the National Standard,
    /// São Paulo, SIGISS and Thema.
    /// </remarks>
    [JsonPropertyName("codigo_servico")]
    public string? ServiceCode { get; init; }

    /// <summary>
    /// Gets the CNAE code.
    /// </summary>
    /// <remarks>
    /// Used by Abrasf, Cecam, DSF, Governa and Thema.
    /// </remarks>
    [JsonPropertyName("codigo_cnae")]
    public string? CnaeCode { get; init; }

    /// <summary>
    /// Gets the municipality-specific taxation code.
    /// </summary>
    /// <remarks>
    /// Used by Abrasf, the National Standard and Thema.
    /// </remarks>
    [JsonPropertyName("codigo_tributacao_municipio")]
    public string? MunicipalTaxCode { get; init; }

    /// <summary>
    /// Gets the state where the service is provided.
    /// </summary>
    /// <remarks>
    /// Used by Abrasf, Osasco, the National Standard, São Paulo RTC and Thema.
    /// </remarks>
    [JsonPropertyName("uf_local_prestacao")]
    public string? ServiceLocationState { get; init; }

    /// <summary>
    /// Gets the city where the service is provided.
    /// </summary>
    /// <remarks>
    /// Used by Abrasf, Osasco, the National Standard, São Paulo RTC and Thema.
    /// </remarks>
    [JsonPropertyName("cidade_local_prestacao")]
    public string? ServiceLocationCity { get; init; }

    /// <summary>
    /// Gets the operation-nature code.
    /// </summary>
    /// <remarks>
    /// This field is numeric for Abrasf and Thema and a string for IPM.
    /// </remarks>
    [JsonPropertyName("natureza_operacao")]
    public NFSeStringOrIntValue? OperationNature { get; init; }

    /// <summary>
    /// Gets the ISS enforceability code used by Abrasf 2.00 or later.
    /// </summary>
    [JsonPropertyName("exigibilidade_iss")]
    public int? IssEnforceability { get; init; }

    /// <summary>
    /// Gets the provider competence date.
    /// </summary>
    /// <remarks>
    /// Used by Abrasf 2.00 or later, the National Standard and São Paulo.
    /// </remarks>
    [JsonPropertyName("data_competencia")]
    public DateOnly? CompetenceDate { get; init; }

    /// <summary>
    /// Gets the tax-incidence location used by Abrasf 2.00 or later.
    /// </summary>
    [JsonPropertyName("local_incidencia")]
    public int? TaxIncidenceLocation { get; init; }

    /// <summary>
    /// Gets the Brazilian Service Nomenclature code.
    /// </summary>
    /// <remarks>
    /// Used by Abrasf 2.03, the National Standard and Sigissweb.
    /// </remarks>
    [JsonPropertyName("codigo_nbs")]
    public string? NbsCode { get; init; }

    /// <summary>
    /// Gets complementary provider information.
    /// </summary>
    /// <remarks>
    /// Used by Abrasf 2.04 and the National Standard.
    /// </remarks>
    [JsonPropertyName("informacoes_complementares")]
    public string? AdditionalInformation { get; init; }

    /// <summary>
    /// Gets the detailed service items supported by Abrasf 2.04 in
    /// Uberlândia.
    /// </summary>
    [JsonPropertyName("itens")]
    public IReadOnlyList<NFSeAbrasfItem>? Items { get; init; }

    /// <summary>
    /// Gets the service-location category used by Cecam.
    /// </summary>
    [JsonPropertyName("local_prestacao")]
    public string? ServiceLocation { get; init; }

    /// <summary>
    /// Gets the service-location postal code.
    /// </summary>
    /// <remarks>Used by Cecam and Osasco.</remarks>
    [JsonPropertyName("cep_local_prestacao")]
    public string? ServiceLocationPostalCode { get; init; }

    /// <summary>
    /// Gets the provider operation type.
    /// </summary>
    /// <remarks>
    /// This field is a letter code for DSF and a numeric tax-reform code for
    /// the National Standard.
    /// </remarks>
    [JsonPropertyName("tipo_operacao")]
    public NFSeStringOrIntValue? OperationType { get; init; }

    /// <summary>
    /// Gets the provider taxation type used by DSF and São Paulo.
    /// </summary>
    [JsonPropertyName("tipo_tributacao")]
    public string? TaxationType { get; init; }

    /// <summary>
    /// Gets the deduction justification required by Equiplano.
    /// </summary>
    [JsonPropertyName("justificativa_deducoes")]
    public string? DeductionJustification { get; init; }

    /// <summary>
    /// Gets the Fiscal Code for Provision of Services used by Florianópolis.
    /// </summary>
    [JsonPropertyName("cfps")]
    public string? Cfps { get; init; }

    /// <summary>
    /// Gets the service identifier used by Florianópolis.
    /// </summary>
    [JsonPropertyName("id_cnae")]
    public string? CnaeId { get; init; }

    /// <summary>
    /// Gets the provider tax-situation code.
    /// </summary>
    /// <remarks>Used by Florianópolis and IPM.</remarks>
    [JsonPropertyName("situacao_tributaria")]
    public string? TaxSituation { get; init; }

    /// <summary>
    /// Gets the collection regime used by Governa.
    /// </summary>
    [JsonPropertyName("regime_recolhimento")]
    public string? CollectionRegime { get; init; }

    /// <summary>
    /// Gets the collection method used by Governa.
    /// </summary>
    [JsonPropertyName("forma_recolhimento")]
    public string? CollectionMethod { get; init; }

    /// <summary>
    /// Gets the service-location address used by Osasco.
    /// </summary>
    [JsonPropertyName("endereco_local_prestacao")]
    public string? ServiceLocationAddress { get; init; }

    /// <summary>
    /// Gets the municipal tax-benefit identifier used by the National
    /// Standard.
    /// </summary>
    [JsonPropertyName("identificador_beneficio_municipal")]
    public string? MunicipalBenefitIdentifier { get; init; }

    /// <summary>
    /// Gets the ISS taxation indicator used by the National Standard.
    /// </summary>
    [JsonPropertyName("tributacao_iss")]
    public int? IssTaxation { get; init; }

    /// <summary>
    /// Gets the immunity type used by the National Standard.
    /// </summary>
    [JsonPropertyName("tipo_imunidade")]
    public int? ImmunityType { get; init; }

    /// <summary>
    /// Gets the PIS/COFINS/CSLL withholding indicator used by the National
    /// Standard and São Paulo.
    /// </summary>
    [JsonPropertyName("pis_cofins_retido")]
    public int? PisCofinsWithheld { get; init; }

    /// <summary>
    /// Gets the contributor's internal code used by the National Standard.
    /// </summary>
    [JsonPropertyName("codigo_interno")]
    public string? InternalCode { get; init; }

    /// <summary>
    /// Gets the approximate tax rate used by the National Standard for
    /// Simples Nacional.
    /// </summary>
    [JsonPropertyName("aliquota_tributos_aproximados")]
    public decimal? ApproximateTaxRate { get; init; }

    /// <summary>
    /// Gets the NFS-e issuance purpose introduced by the tax reform.
    /// </summary>
    [JsonPropertyName("finalidade")]
    public int? Purpose { get; init; }

    /// <summary>
    /// Gets the final-consumer indicator introduced by the tax reform.
    /// </summary>
    [JsonPropertyName("consumidor_final")]
    public int? FinalConsumer { get; init; }

    /// <summary>
    /// Gets the tax-reform supply-operation indicator code.
    /// </summary>
    [JsonPropertyName("cod_indicador_operacao")]
    public string? OperationIndicatorCode { get; init; }

    /// <summary>
    /// Gets the Manaus Free Trade Zone / Free Trade Area indicator.
    /// </summary>
    [JsonPropertyName("indicador_zfm_alc")]
    public int? ZfmAlcIndicator { get; init; }

    /// <summary>
    /// Gets whether a recipient different from the taker is present.
    /// </summary>
    [JsonPropertyName("indicador_destinatario")]
    public int? RecipientIndicator { get; init; }

    /// <summary>
    /// Gets real-estate information introduced by the tax reform.
    /// </summary>
    [JsonPropertyName("imovel")]
    public NFSeRealEstate? RealEstate { get; init; }

    /// <summary>
    /// Gets complementary NFS-e information introduced by the tax reform.
    /// </summary>
    [JsonPropertyName("outras_informacoes")]
    public string? OtherInformation { get; init; }

    /// <summary>
    /// Gets the payment condition introduced by the tax reform.
    /// </summary>
    [JsonPropertyName("condicao_pagamento")]
    public NFSePaymentCondition? PaymentCondition { get; init; }

    /// <summary>
    /// Gets access keys of NFS-e documents referenced by a substitute or
    /// complementary National Standard invoice.
    /// </summary>
    [JsonPropertyName("nfses_referenciadas")]
    public IReadOnlyList<string>? ReferencedNFSeAccessKeys { get; init; }

    /// <summary>
    /// Gets the governmental entity participating in the transaction.
    /// </summary>
    [JsonPropertyName("ente_governamental")]
    public NFSeGovernmentEntity? GovernmentEntity { get; init; }

    /// <summary>
    /// Gets reimbursement and pass-through documents introduced by the tax
    /// reform.
    /// </summary>
    [JsonPropertyName("reembolso_repasse")]
    public NFSeReimbursement? Reimbursement { get; init; }

    /// <summary>
    /// Gets the deduction or reduction block used by the National Standard.
    /// </summary>
    [JsonPropertyName("deducao_reducao")]
    public NFSeDeductionReduction? DeductionReduction { get; init; }

    /// <summary>
    /// Gets the cross-border service mode used by the National Standard.
    /// </summary>
    [JsonPropertyName("modo_prestacao")]
    public int? CrossBorderServiceMode { get; init; }

    /// <summary>
    /// Gets the relationship between the parties used by the National
    /// Standard.
    /// </summary>
    [JsonPropertyName("vinculo_prestacao")]
    public int? PartyRelationship { get; init; }

    /// <summary>
    /// Gets the transaction currency code used by the National Standard.
    /// </summary>
    [JsonPropertyName("tipo_moeda")]
    public string? CurrencyCode { get; init; }

    /// <summary>
    /// Gets the service value in foreign currency used by the National
    /// Standard.
    /// </summary>
    [JsonPropertyName("valor_servico_moeda")]
    public decimal? ForeignCurrencyServiceValue { get; init; }

    /// <summary>
    /// Gets the export-support mechanism used by the service provider in the
    /// National Standard.
    /// </summary>
    [JsonPropertyName("mecanismo_apoio_comex_prestador")]
    public string? ProviderForeignTradeSupportMechanism { get; init; }

    /// <summary>
    /// Gets the export-support mechanism used by the taker in the National
    /// Standard.
    /// </summary>
    [JsonPropertyName("mecanismo_apoio_comex_tomador")]
    public string? TakerForeignTradeSupportMechanism { get; init; }

    /// <summary>
    /// Gets the temporary movement of goods indicator used by the National
    /// Standard.
    /// </summary>
    [JsonPropertyName("movimento_temporario_bens")]
    public int? TemporaryMovementOfGoods { get; init; }

    /// <summary>
    /// Gets the import declaration number used by the National Standard.
    /// </summary>
    [JsonPropertyName("numero_di")]
    public string? ImportDeclarationNumber { get; init; }

    /// <summary>
    /// Gets the export registration number used by the National Standard.
    /// </summary>
    [JsonPropertyName("numero_re")]
    public string? ExportRegistrationNumber { get; init; }

    /// <summary>
    /// Gets whether the National Standard NFS-e is shared with MDIC.
    /// </summary>
    [JsonPropertyName("mdic")]
    public int? ShareWithMdic { get; init; }

    /// <summary>
    /// Gets the initial amount charged by São Paulo RTC.
    /// </summary>
    [JsonPropertyName("valor_inicial_cobrado")]
    public decimal? InitialChargedValue { get; init; }

    /// <summary>
    /// Gets the final amount charged by São Paulo RTC.
    /// </summary>
    [JsonPropertyName("valor_final_cobrado")]
    public decimal? FinalChargedValue { get; init; }

    /// <summary>
    /// Gets the fine amount used by São Paulo RTC.
    /// </summary>
    [JsonPropertyName("valor_multa")]
    public decimal? FineValue { get; init; }

    /// <summary>
    /// Gets the interest amount used by São Paulo RTC.
    /// </summary>
    [JsonPropertyName("valor_juros")]
    public decimal? InterestValue { get; init; }

    /// <summary>
    /// Gets the IPI amount used by São Paulo RTC.
    /// </summary>
    [JsonPropertyName("valor_ipi")]
    public decimal? IpiValue { get; init; }

    /// <summary>
    /// Gets the installment or advance payment indicator used by São Paulo
    /// RTC.
    /// </summary>
    [JsonPropertyName("pagamento_parcelado_antecipado")]
    public int? InstallmentOrAdvancePayment { get; init; }

    /// <summary>
    /// Gets the NCM code used by São Paulo RTC.
    /// </summary>
    [JsonPropertyName("ncm")]
    public string? Ncm { get; init; }

    /// <summary>
    /// Gets the service country code used by São Paulo RTC.
    /// </summary>
    [JsonPropertyName("codigo_pais_prestacao")]
    public string? ServiceCountryCode { get; init; }

    /// <summary>
    /// Gets the amount received for the service used by São Paulo.
    /// </summary>
    [JsonPropertyName("valor_recebido")]
    public decimal? ReceivedValue { get; init; }

    /// <summary>
    /// Gets the approximate tax burden used by São Paulo.
    /// </summary>
    [JsonPropertyName("carga_tributaria")]
    public decimal? TaxBurden { get; init; }

    /// <summary>
    /// Gets the source of the approximate tax burden used by São Paulo.
    /// </summary>
    [JsonPropertyName("fonte_carga_tributaria")]
    public string? TaxBurdenSource { get; init; }

    /// <summary>
    /// Gets the taxation indicator used by SIGISS.
    /// </summary>
    [JsonPropertyName("tributacao")]
    public int? Taxation { get; init; }
}

/// <summary>
/// Describes an item accepted by Abrasf 2.04 for Uberlândia.
/// </summary>
public sealed record NFSeAbrasfItem
{
    [JsonPropertyName("descricao")]
    public required string Description { get; init; }

    [JsonPropertyName("quantidade")]
    public required decimal Quantity { get; init; }

    [JsonPropertyName("valor_unitario")]
    public required decimal UnitValue { get; init; }

    [JsonPropertyName("item_tributavel")]
    public required string TaxableItem { get; init; }
}
