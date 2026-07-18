using System.Text.Json.Serialization;

#pragma warning disable CS1591

namespace DFeHub.Providers.WebMania.NFe;

/// <summary>
/// Contains totals, payment and purchasing information for an invoice.
/// </summary>
public sealed record NFeOrder
{
    [JsonPropertyName("presenca")]
    public required int BuyerPresence { get; init; }

    [JsonPropertyName("intermediador")]
    public int? IntermediaryIndicator { get; init; }

    [JsonPropertyName("cnpj_intermediador")]
    public string? IntermediaryCnpj { get; init; }

    [JsonPropertyName("id_intermediador")]
    public string? IntermediaryId { get; init; }

    [JsonPropertyName("modalidade_frete")]
    public required int FreightMode { get; init; }

    [JsonPropertyName("frete")]
    public decimal? Freight { get; init; }

    [JsonPropertyName("desconto")]
    public decimal? Discount { get; init; }

    [JsonPropertyName("total")]
    public required decimal Total { get; init; }

    [JsonPropertyName("despesas_acessorias")]
    public decimal? AdditionalExpenses { get; init; }

    [JsonPropertyName("despesas_aduaneiras")]
    public decimal? CustomsExpenses { get; init; }

    [JsonPropertyName("informacoes_fisco")]
    public string? TaxAuthorityInformation { get; init; }

    [JsonPropertyName("informacoes_complementares")]
    public string? CustomerInformation { get; init; }

    [JsonPropertyName("observacoes_contribuinte")]
    public IReadOnlyList<NFeTaxpayerObservation>? TaxpayerObservations { get; init; }

    [JsonPropertyName("pedido_compra")]
    public string? PurchaseOrder { get; init; }

    [JsonPropertyName("contrato")]
    public string? Contract { get; init; }

    [JsonPropertyName("nota_empenho")]
    public string? CommitmentNote { get; init; }

    [JsonPropertyName("pagamento")]
    public IReadOnlyList<int>? PaymentTiming { get; init; }

    [JsonPropertyName("forma_pagamento")]
    public IReadOnlyList<string>? PaymentMethods { get; init; }

    [JsonPropertyName("desc_pagamento")]
    public IReadOnlyList<string>? PaymentDescriptions { get; init; }

    [JsonPropertyName("data_pagamento")]
    public IReadOnlyList<DateOnly>? PaymentDates { get; init; }

    [JsonPropertyName("cnpj_transacional_pag")]
    public IReadOnlyList<string>? TransactionalCnpjs { get; init; }

    [JsonPropertyName("uf_estabelecimento_pag")]
    public IReadOnlyList<string>? PaymentStates { get; init; }

    [JsonPropertyName("tipo_integracao")]
    public IReadOnlyList<int>? IntegrationTypes { get; init; }

    [JsonPropertyName("valor_pagamento")]
    public IReadOnlyList<decimal>? PaymentValues { get; init; }

    [JsonPropertyName("cnpj_credenciadora")]
    public IReadOnlyList<string>? AcquirerCnpjs { get; init; }

    [JsonPropertyName("bandeira")]
    public IReadOnlyList<string>? CardBrands { get; init; }

    [JsonPropertyName("autorizacao")]
    public IReadOnlyList<string>? Authorizations { get; init; }

    [JsonPropertyName("cnpj_beneficiario")]
    public IReadOnlyList<string>? BeneficiaryCnpjs { get; init; }

    [JsonPropertyName("id_terminal_pag")]
    public IReadOnlyList<string>? TerminalIds { get; init; }
}

/// <summary>
/// Contains a free-form taxpayer observation with a named field.
/// </summary>
public sealed record NFeTaxpayerObservation
{
    [JsonPropertyName("campo")]
    public required string Field { get; init; }

    [JsonPropertyName("texto")]
    public required string Text { get; init; }
}

/// <summary>
/// Describes transport information for an invoice.
/// </summary>
public sealed record NFeTransport
{
    [JsonPropertyName("volume")]
    public string? VolumeCount { get; init; }

    [JsonPropertyName("especie")]
    public string? PackageKind { get; init; }

    [JsonPropertyName("peso_bruto")]
    public decimal? GrossWeight { get; init; }

    [JsonPropertyName("peso_liquido")]
    public decimal? NetWeight { get; init; }

    [JsonPropertyName("marca")]
    public string? Brand { get; init; }

    [JsonPropertyName("numeracao")]
    public string? Numbering { get; init; }

    [JsonPropertyName("lacres")]
    public string? Seals { get; init; }

    [JsonPropertyName("transportadora")]
    public NFeCarrier? Carrier { get; init; }

    [JsonPropertyName("reboque")]
    public NFeTrailer? Trailer { get; init; }

    [JsonPropertyName("entrega")]
    public NFeLocation? DeliveryLocation { get; init; }

    [JsonPropertyName("retirada")]
    public NFeLocation? PickupLocation { get; init; }
}

/// <summary>
/// Describes the carrier used in an invoice.
/// </summary>
public sealed record NFeCarrier
{
    public string? Cnpj { get; init; }

    [JsonPropertyName("razao_social")]
    public string? CorporateName { get; init; }

    [JsonPropertyName("ie")]
    public string? StateRegistration { get; init; }

    public string? Cpf { get; init; }

    [JsonPropertyName("nome_completo")]
    public string? FullName { get; init; }

    [JsonPropertyName("endereco")]
    public string? Address { get; init; }

    [JsonPropertyName("uf")]
    public string? State { get; init; }

    [JsonPropertyName("cidade")]
    public string? City { get; init; }

    [JsonPropertyName("cep")]
    public string? PostalCode { get; init; }

    [JsonPropertyName("placa")]
    public string? LicensePlate { get; init; }

    [JsonPropertyName("uf_veiculo")]
    public string? VehicleState { get; init; }

    public string? Rntc { get; init; }

    [JsonPropertyName("seguro")]
    public decimal? Insurance { get; init; }
}

/// <summary>
/// Describes a transport trailer.
/// </summary>
public sealed record NFeTrailer
{
    [JsonPropertyName("placa")]
    public required string LicensePlate { get; init; }

    [JsonPropertyName("uf_veiculo")]
    public required string VehicleState { get; init; }

    public string? Rntc { get; init; }

    [JsonPropertyName("vagao")]
    public string? Wagon { get; init; }

    [JsonPropertyName("balsa")]
    public string? Ferry { get; init; }
}

/// <summary>
/// Describes an invoice issued for receivables.
/// </summary>
public sealed record NFeBilling
{
    [JsonPropertyName("numero")]
    public required string Number { get; init; }

    [JsonPropertyName("valor")]
    public required decimal Value { get; init; }

    [JsonPropertyName("desconto")]
    public decimal? Discount { get; init; }

    [JsonPropertyName("valor_liquido")]
    public required decimal NetValue { get; init; }
}

/// <summary>
/// Describes one installment.
/// </summary>
public sealed record NFeInstallment
{
    [JsonPropertyName("vencimento")]
    public required DateOnly DueDate { get; init; }

    [JsonPropertyName("valor")]
    public required decimal Value { get; init; }
}

/// <summary>
/// Describes shipment-level export data.
/// </summary>
public sealed record NFeExport
{
    [JsonPropertyName("uf_embarque")]
    public required string ShipmentState { get; init; }

    [JsonPropertyName("local_embarque")]
    public required string ShipmentLocation { get; init; }

    [JsonPropertyName("local_despacho")]
    public string? DispatchLocation { get; init; }
}

/// <summary>
/// Contains agricultural data for an invoice.
/// </summary>
public sealed record NFeAgriculture
{
    [JsonPropertyName("defensivos")]
    public IReadOnlyList<NFePesticidePrescription>? Pesticides { get; init; }

    [JsonPropertyName("guia_transito")]
    public NFeTransitGuide? TransitGuide { get; init; }
}

/// <summary>
/// Describes an agricultural pesticide prescription.
/// </summary>
public sealed record NFePesticidePrescription
{
    [JsonPropertyName("numero_receituario")]
    public required string Number { get; init; }

    [JsonPropertyName("cpf_responsavel_tecnico")]
    public required string TechnicianCpf { get; init; }
}

/// <summary>
/// Describes an agricultural transit guide.
/// </summary>
public sealed record NFeTransitGuide
{
    [JsonPropertyName("tipo")]
    public required int Type { get; init; }

    [JsonPropertyName("uf")]
    public required string State { get; init; }

    [JsonPropertyName("serie")]
    public string? Series { get; init; }

    [JsonPropertyName("numero")]
    public required string Number { get; init; }
}
