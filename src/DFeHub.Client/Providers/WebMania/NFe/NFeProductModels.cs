using System.Text.Json.Serialization;

#pragma warning disable CS1591

namespace DFeHub.Providers.WebMania.NFe;

/// <summary>
/// Describes one product or service in an NF-e/NFC-e issuance.
/// </summary>
public sealed record NFeProduct
{
    [JsonPropertyName("ID")]
    public string? OrderId { get; init; }

    [JsonPropertyName("item")]
    public int? OrderItem { get; init; }

    [JsonPropertyName("nome")]
    public required string Name { get; init; }

    [JsonPropertyName("codigo")]
    public required string Code { get; init; }

    public required string Ncm { get; init; }

    [JsonPropertyName("quantidade")]
    public required decimal Quantity { get; init; }

    [JsonPropertyName("quantidade_tributavel")]
    public decimal? TaxableQuantity { get; init; }

    [JsonPropertyName("unidade")]
    public required string Unit { get; init; }

    [JsonPropertyName("unidade_tributavel")]
    public string? TaxableUnit { get; init; }

    [JsonPropertyName("peso")]
    public decimal? Weight { get; init; }

    [JsonPropertyName("origem")]
    public required int Origin { get; init; }

    [JsonPropertyName("desconto")]
    public decimal? Discount { get; init; }

    [JsonPropertyName("subtotal")]
    public required decimal UnitPrice { get; init; }

    [JsonPropertyName("subtotal_tributavel")]
    public decimal? TaxableUnitPrice { get; init; }

    [JsonPropertyName("total")]
    public required decimal Total { get; init; }

    [JsonPropertyName("ind_valor_total")]
    public int? IncludedInTotal { get; init; }

    [JsonPropertyName("classe_imposto")]
    public string? TaxClass { get; init; }

    [JsonPropertyName("impostos")]
    public NFeTaxes? Taxes { get; init; }

    public string? Cest { get; init; }

    [JsonPropertyName("beneficio_fiscal")]
    public string? TaxBenefit { get; init; }

    [JsonPropertyName("informacoes_adicionais")]
    public string? AdditionalInformation { get; init; }

    [JsonPropertyName("ind_escala")]
    public string? RelevantScale { get; init; }

    [JsonPropertyName("cnpj_fabricante")]
    public string? ManufacturerCnpj { get; init; }

    public string? Gtin { get; init; }

    [JsonPropertyName("gtin_tributavel")]
    public string? TaxableGtin { get; init; }

    [JsonPropertyName("cod_barras")]
    public string? Barcode { get; init; }

    public string? Nve { get; init; }

    public string? Nrecopi { get; init; }

    [JsonPropertyName("ativo_permanente")]
    public bool? PermanentAsset { get; init; }

    [JsonPropertyName("veiculo_usado")]
    public decimal? UsedVehicleValue { get; init; }

    [JsonPropertyName("ex_ipi")]
    public string? IpiException { get; init; }

    [JsonPropertyName("codigo_cfop")]
    public string? CfopCode { get; init; }

    [JsonPropertyName("dfe_referenciado")]
    public NFeReferencedDocumentItem? ReferencedDocument { get; init; }

    [JsonPropertyName("exportacao")]
    public NFeProductExport? Export { get; init; }

    [JsonPropertyName("importacao")]
    public NFeProductImport? Import { get; init; }

    [JsonPropertyName("rastreabilidade")]
    public IReadOnlyList<NFeTraceability>? Traceability { get; init; }

    [JsonPropertyName("combustiveis")]
    public NFeFuel? Fuel { get; init; }

    [JsonPropertyName("medicamento")]
    public NFeMedicine? Medicine { get; init; }

    [JsonPropertyName("armamentos")]
    public NFeWeapon? Weapon { get; init; }

    [JsonPropertyName("veiculos_novos")]
    public NFeNewVehicle? NewVehicle { get; init; }
}

/// <summary>
/// Links an item to a previous fiscal document.
/// </summary>
public sealed record NFeReferencedDocumentItem
{
    [JsonPropertyName("chave")]
    public required string AccessKey { get; init; }

    public int? Item { get; init; }
}

/// <summary>
/// Contains product-level export data.
/// </summary>
public sealed record NFeProductExport
{
    public string? Drawback { get; init; }

    [JsonPropertyName("reg_exportacao")]
    public string? ExportRegistration { get; init; }

    [JsonPropertyName("nfe_exportacao")]
    public string? ExportNFeAccessKey { get; init; }

    [JsonPropertyName("qtd_exportacao")]
    public decimal? ExportedQuantity { get; init; }
}

/// <summary>
/// Contains product-level import data.
/// </summary>
public sealed record NFeProductImport
{
    [JsonPropertyName("ndoc_importacao")]
    public required string DocumentNumber { get; init; }

    [JsonPropertyName("ddoc_importacao")]
    public required DateOnly DocumentDate { get; init; }

    [JsonPropertyName("local_desembaraco")]
    public required string CustomsClearanceLocation { get; init; }

    [JsonPropertyName("uf_desembaraco")]
    public required string CustomsClearanceState { get; init; }

    [JsonPropertyName("data_desembaraco")]
    public required DateOnly CustomsClearanceDate { get; init; }

    [JsonPropertyName("via_transporte")]
    public required string TransportMode { get; init; }

    [JsonPropertyName("intermediacao")]
    public required string Intermediation { get; init; }

    [JsonPropertyName("adicao")]
    public required string AdditionNumber { get; init; }

    [JsonPropertyName("seq_adicao")]
    public required string AdditionSequence { get; init; }

    [JsonPropertyName("fabricante")]
    public required string ManufacturerCode { get; init; }

    [JsonPropertyName("desc_di")]
    public decimal? Discount { get; init; }

    public decimal? Afrmm { get; init; }

    [JsonPropertyName("cnpj_terceiro")]
    public string? ThirdPartyCnpj { get; init; }

    [JsonPropertyName("cpf_terceiro")]
    public string? ThirdPartyCpf { get; init; }

    [JsonPropertyName("uf_terceiro")]
    public string? ThirdPartyState { get; init; }

    [JsonPropertyName("cod_exportador")]
    public required string ExporterCode { get; init; }

    public string? Nfci { get; init; }
}

/// <summary>
/// Describes a traceable product batch.
/// </summary>
public sealed record NFeTraceability
{
    [JsonPropertyName("lote")]
    public required string Batch { get; init; }

    [JsonPropertyName("quantidade")]
    public required decimal Quantity { get; init; }

    [JsonPropertyName("data_fabricacao")]
    public required DateOnly ManufacturingDate { get; init; }

    [JsonPropertyName("data_validade")]
    public required DateOnly ExpirationDate { get; init; }

    [JsonPropertyName("codigo_agregacao")]
    public string? AggregationCode { get; init; }
}

/// <summary>
/// Describes fuel-specific fields.
/// </summary>
public sealed record NFeFuel
{
    [JsonPropertyName("codigo_anp")]
    public required string AnpCode { get; init; }

    [JsonPropertyName("descricao_anp")]
    public string? AnpDescription { get; init; }

    [JsonPropertyName("uf_consumo")]
    public required string ConsumptionState { get; init; }

    [JsonPropertyName("percentual_glp")]
    public decimal? GlpPercentage { get; init; }

    [JsonPropertyName("percentual_gnn")]
    public decimal? NationalGasPercentage { get; init; }

    [JsonPropertyName("percentual_gni")]
    public decimal? ImportedGasPercentage { get; init; }

    [JsonPropertyName("percentual_biodiesel")]
    public decimal? BiodieselPercentage { get; init; }

    [JsonPropertyName("partida")]
    public decimal? StartingValue { get; init; }

    public string? Codif { get; init; }

    [JsonPropertyName("qtd_temperatura")]
    public decimal? TemperatureQuantity { get; init; }

    [JsonPropertyName("bc_cide")]
    public decimal? CideTaxBase { get; init; }

    [JsonPropertyName("valor_cide")]
    public decimal? CideValue { get; init; }

    [JsonPropertyName("bico")]
    public int? Nozzle { get; init; }

    [JsonPropertyName("bomba")]
    public int? Pump { get; init; }

    [JsonPropertyName("tanque")]
    public int? Tank { get; init; }

    [JsonPropertyName("encerrante_inicio")]
    public decimal? InitialMeter { get; init; }

    [JsonPropertyName("encerrante_final")]
    public decimal? FinalMeter { get; init; }

    [JsonPropertyName("origem")]
    public IReadOnlyList<NFeFuelOrigin>? Origins { get; init; }
}

/// <summary>
/// Describes the source of a fuel fraction.
/// </summary>
public sealed record NFeFuelOrigin
{
    [JsonPropertyName("indicador_importacao")]
    public required string ImportIndicator { get; init; }

    [JsonPropertyName("uf_origem")]
    public required string State { get; init; }

    [JsonPropertyName("percentual_originario")]
    public required decimal Percentage { get; init; }
}

/// <summary>
/// Describes medicine-specific fields.
/// </summary>
public sealed record NFeMedicine
{
    [JsonPropertyName("codigo_anvisa")]
    public required string AnvisaCode { get; init; }

    public required decimal Pmc { get; init; }
}

/// <summary>
/// Describes weapon-specific fields.
/// </summary>
public sealed record NFeWeapon
{
    [JsonPropertyName("tipo")]
    public required int Type { get; init; }

    [JsonPropertyName("serie_arma")]
    public required string WeaponSerial { get; init; }

    [JsonPropertyName("serie_cano")]
    public required string BarrelSerial { get; init; }

    [JsonPropertyName("descricao")]
    public required string Description { get; init; }
}

/// <summary>
/// Describes a new vehicle.
/// </summary>
public sealed record NFeNewVehicle
{
    [JsonPropertyName("tipo_operacao")]
    public required int OperationType { get; init; }

    [JsonPropertyName("chassi")]
    public required string Chassis { get; init; }

    [JsonPropertyName("cor")]
    public required string ColorCode { get; init; }

    [JsonPropertyName("cor_descricao")]
    public required string ColorDescription { get; init; }

    public required string Cv { get; init; }

    [JsonPropertyName("cilindrada")]
    public required string EngineCapacity { get; init; }

    [JsonPropertyName("peso_liquido")]
    public required decimal NetWeight { get; init; }

    [JsonPropertyName("peso_bruto")]
    public required decimal GrossWeight { get; init; }

    [JsonPropertyName("serie")]
    public required string Series { get; init; }

    [JsonPropertyName("tipo_combustivel")]
    public required string FuelType { get; init; }

    [JsonPropertyName("numero_motor")]
    public required string EngineNumber { get; init; }

    public required decimal Cmt { get; init; }

    [JsonPropertyName("dist")]
    public required string Wheelbase { get; init; }

    [JsonPropertyName("ano_modelo")]
    public required string ModelYear { get; init; }

    [JsonPropertyName("ano_fabricacao")]
    public required string ManufacturingYear { get; init; }

    [JsonPropertyName("tipo_pintura")]
    public required string PaintType { get; init; }

    [JsonPropertyName("tipo_veiculo")]
    public required string VehicleType { get; init; }

    [JsonPropertyName("especie_veiculo")]
    public required int VehicleKind { get; init; }

    public required string Vin { get; init; }

    [JsonPropertyName("condicao_veiculo")]
    public required int VehicleCondition { get; init; }

    [JsonPropertyName("marca_modelo")]
    public required string BrandModel { get; init; }

    [JsonPropertyName("cor_denatran")]
    public required string DenatranColor { get; init; }

    [JsonPropertyName("lotacao")]
    public required int Capacity { get; init; }

    [JsonPropertyName("restricao")]
    public required int Restriction { get; init; }
}
