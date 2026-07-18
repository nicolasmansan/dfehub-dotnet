using System.Text.Json.Serialization;

#pragma warning disable CS1591

namespace DFeHub.Providers.WebMania.NFe;

/// <summary>
/// Describes a Brazilian or foreign NF-e/NFC-e recipient.
/// </summary>
public sealed record NFeCustomer
{
    public string? Cpf { get; init; }

    [JsonPropertyName("nome_completo")]
    public string? FullName { get; init; }

    public string? Cnpj { get; init; }

    [JsonPropertyName("razao_social")]
    public string? CorporateName { get; init; }

    [JsonPropertyName("ie")]
    public string? StateRegistration { get; init; }

    public string? Suframa { get; init; }

    [JsonPropertyName("substituto_tributario")]
    public string? TaxSubstituteRegistration { get; init; }

    [JsonPropertyName("consumidor_final")]
    public int? FinalConsumer { get; init; }

    [JsonPropertyName("contribuinte")]
    public int? IcmsContributor { get; init; }

    [JsonPropertyName("microcervejaria")]
    public bool? Microbrewery { get; init; }

    [JsonPropertyName("id_estrangeiro")]
    public string? ForeignId { get; init; }

    [JsonPropertyName("nome_estrangeiro")]
    public string? ForeignName { get; init; }

    [JsonPropertyName("cod_pais")]
    public string? CountryCode { get; init; }

    [JsonPropertyName("nome_pais")]
    public string? CountryName { get; init; }

    [JsonPropertyName("endereco")]
    public required string Address { get; init; }

    [JsonPropertyName("complemento")]
    public string? AddressComplement { get; init; }

    [JsonPropertyName("numero")]
    public required string Number { get; init; }

    [JsonPropertyName("bairro")]
    public required string District { get; init; }

    [JsonPropertyName("cidade")]
    public required string City { get; init; }

    [JsonPropertyName("uf")]
    public string? State { get; init; }

    [JsonPropertyName("cep")]
    public string? PostalCode { get; init; }

    [JsonPropertyName("telefone")]
    public string? Phone { get; init; }

    [JsonPropertyName("email")]
    public string? Email { get; init; }
}

/// <summary>
/// Describes a delivery or pickup location.
/// </summary>
public sealed record NFeLocation
{
    public string? Cnpj { get; init; }

    [JsonPropertyName("razao_social")]
    public string? CorporateName { get; init; }

    [JsonPropertyName("ie")]
    public string? StateRegistration { get; init; }

    public string? Cpf { get; init; }

    [JsonPropertyName("nome_completo")]
    public string? FullName { get; init; }

    [JsonPropertyName("nome_estrangeiro")]
    public string? ForeignName { get; init; }

    [JsonPropertyName("cod_pais")]
    public string? CountryCode { get; init; }

    [JsonPropertyName("pais")]
    public string? Country { get; init; }

    [JsonPropertyName("uf")]
    public required string State { get; init; }

    [JsonPropertyName("cep")]
    public string? PostalCode { get; init; }

    [JsonPropertyName("endereco")]
    public required string Address { get; init; }

    [JsonPropertyName("numero")]
    public required string Number { get; init; }

    [JsonPropertyName("complemento")]
    public string? AddressComplement { get; init; }

    [JsonPropertyName("bairro")]
    public required string District { get; init; }

    [JsonPropertyName("cidade")]
    public required string City { get; init; }

    [JsonPropertyName("telefone")]
    public string? Phone { get; init; }

    [JsonPropertyName("email")]
    public string? Email { get; init; }
}

/// <summary>
/// Describes a referenced paper ECF receipt.
/// </summary>
public sealed record NFeReferencedEcf
{
    [JsonPropertyName("modelo")]
    public required string Model { get; init; }

    [JsonPropertyName("n_ecf")]
    public required int EcfNumber { get; init; }

    [JsonPropertyName("n_coo")]
    public required int OperationCounter { get; init; }
}

/// <summary>
/// Describes a referenced rural producer invoice.
/// </summary>
public sealed record NFeReferencedRuralInvoice
{
    [JsonPropertyName("uf")]
    public required string State { get; init; }

    [JsonPropertyName("data")]
    public required DateOnly Date { get; init; }

    public string? Cpf { get; init; }

    public string? Cnpj { get; init; }

    [JsonPropertyName("ie")]
    public required string StateRegistration { get; init; }

    [JsonPropertyName("modelo")]
    public required string Model { get; init; }

    [JsonPropertyName("serie")]
    public required int Series { get; init; }

    [JsonPropertyName("numero")]
    public required int Number { get; init; }
}
