using System.Text.Json.Serialization;

#pragma warning disable CS1591

namespace DFeHub.Providers.WebMania.NFSe;

/// <summary>
/// Describes a Brazilian or foreign service taker.
/// </summary>
public sealed record NFSeParty
{
    public string? Cpf { get; init; }

    [JsonPropertyName("nome_completo")]
    public string? FullName { get; init; }

    public string? Cnpj { get; init; }

    [JsonPropertyName("razao_social")]
    public string? CorporateName { get; init; }

    [JsonPropertyName("im")]
    public string? MunicipalRegistration { get; init; }

    public string? Nif { get; init; }

    [JsonPropertyName("endereco")]
    public string? Address { get; init; }

    [JsonPropertyName("numero")]
    public string? Number { get; init; }

    [JsonPropertyName("complemento")]
    public string? AddressComplement { get; init; }

    [JsonPropertyName("bairro")]
    public string? District { get; init; }

    [JsonPropertyName("cidade")]
    public string? City { get; init; }

    [JsonPropertyName("uf")]
    public string? State { get; init; }

    [JsonPropertyName("cep")]
    public string? PostalCode { get; init; }

    public string? Email { get; init; }

    [JsonPropertyName("telefone")]
    public string? Phone { get; init; }

    [JsonPropertyName("nome_estrangeiro")]
    public string? ForeignName { get; init; }

    [JsonPropertyName("id_estrangeiro")]
    public string? ForeignId { get; init; }

    [JsonPropertyName("endereco_completo")]
    public string? FullForeignAddress { get; init; }

    [JsonPropertyName("nome_pais")]
    public string? CountryName { get; init; }

    [JsonPropertyName("codigo_pais")]
    public string? CountryCode { get; init; }

    /// <summary>
    /// Gets the taker's state registration used by the Equiplano provider.
    /// </summary>
    [JsonPropertyName("ie")]
    public string? StateRegistration { get; init; }

    /// <summary>
    /// Gets the foreign document code used by the Equiplano provider.
    /// </summary>
    [JsonPropertyName("documento_estrangeiro")]
    public string? ForeignDocument { get; init; }

    /// <summary>
    /// Gets the foreign city used by the Equiplano provider.
    /// </summary>
    [JsonPropertyName("cidade_estrangeira")]
    public string? ForeignCity { get; init; }

    /// <summary>
    /// Gets the country name used by the Equiplano provider.
    /// </summary>
    [JsonPropertyName("pais")]
    public string? Country { get; init; }
}

/// <summary>
/// Describes a service recipient when it differs from the service taker.
/// </summary>
public sealed record NFSeRecipient
{
    public string? Cpf { get; init; }

    public string? Cnpj { get; init; }

    [JsonPropertyName("id_estrangeiro")]
    public string? ForeignId { get; init; }

    public string? Nif { get; init; }

    [JsonPropertyName("indicador_nao_nif")]
    public int? MissingNifReason { get; init; }

    [JsonPropertyName("nome")]
    public string? Name { get; init; }

    [JsonPropertyName("endereco")]
    public NFSeStructuredAddress? Address { get; init; }

    public string? Email { get; init; }

    [JsonPropertyName("telefone")]
    public string? Phone { get; init; }
}

/// <summary>
/// Describes a structured national or foreign address.
/// </summary>
public sealed record NFSeStructuredAddress
{
    [JsonPropertyName("logradouro")]
    public string? Street { get; init; }

    [JsonPropertyName("numero")]
    public string? Number { get; init; }

    [JsonPropertyName("complemento")]
    public string? AddressComplement { get; init; }

    [JsonPropertyName("bairro")]
    public string? District { get; init; }

    [JsonPropertyName("endereco_nacional")]
    public NFSeNationalAddress? NationalAddress { get; init; }

    [JsonPropertyName("endereco_exterior")]
    public NFSeForeignAddress? ForeignAddress { get; init; }
}

/// <summary>
/// Describes the municipality and postal code of a Brazilian address.
/// </summary>
public sealed record NFSeNationalAddress
{
    [JsonPropertyName("codigo_municipio_ibge")]
    public string? IbgeCityCode { get; init; }

    [JsonPropertyName("cep")]
    public string? PostalCode { get; init; }
}

/// <summary>
/// Describes the country and locality of a foreign address.
/// </summary>
public sealed record NFSeForeignAddress
{
    [JsonPropertyName("codigo_pais")]
    public string? CountryCode { get; init; }

    [JsonPropertyName("codigo_postal")]
    public string? PostalCode { get; init; }

    [JsonPropertyName("cidade")]
    public string? City { get; init; }

    [JsonPropertyName("estado_provincia")]
    public string? StateOrProvince { get; init; }
}

/// <summary>
/// Describes an intermediary involved in providing the service.
/// </summary>
public sealed record NFSeIntermediary
{
    public string? Cpf { get; init; }

    [JsonPropertyName("nome_completo")]
    public string? FullName { get; init; }

    public string? Cnpj { get; init; }

    [JsonPropertyName("razao_social")]
    public string? CorporateName { get; init; }

    [JsonPropertyName("im")]
    public string? MunicipalRegistration { get; init; }

    [JsonPropertyName("cidade")]
    public string? City { get; init; }

    [JsonPropertyName("uf")]
    public string? State { get; init; }

    /// <summary>
    /// Gets the intermediary's state registration for Sigissweb.
    /// </summary>
    [JsonPropertyName("ie")]
    public string? StateRegistration { get; init; }

    /// <summary>
    /// Gets the intermediary's street address for Sigissweb.
    /// </summary>
    [JsonPropertyName("endereco")]
    public string? Address { get; init; }

    /// <summary>
    /// Gets the intermediary's postal code for Sigissweb.
    /// </summary>
    [JsonPropertyName("cep")]
    public string? PostalCode { get; init; }

    /// <summary>
    /// Gets the intermediary's district for Sigissweb.
    /// </summary>
    [JsonPropertyName("bairro")]
    public string? District { get; init; }

    /// <summary>
    /// Gets the intermediary's address number for Sigissweb.
    /// </summary>
    [JsonPropertyName("numero")]
    public string? Number { get; init; }

    /// <summary>
    /// Gets the intermediary's address complement for Sigissweb.
    /// </summary>
    [JsonPropertyName("complemento")]
    public string? AddressComplement { get; init; }
}

/// <summary>
/// Identifies the supplier referenced by a reimbursement document.
/// </summary>
public sealed record NFSeReferencedSupplier
{
    public string? Cpf { get; init; }

    public string? Cnpj { get; init; }

    [JsonPropertyName("id_estrangeiro")]
    public string? ForeignId { get; init; }

    [JsonPropertyName("nome")]
    public string? Name { get; init; }

    [JsonPropertyName("indicador_nao_nif")]
    public int? MissingNifReason { get; init; }
}

/// <summary>
/// Describes the supplier associated with a deduction document.
/// </summary>
public sealed record NFSeDeductionSupplier
{
    public string? Cpf { get; init; }

    public string? Cnpj { get; init; }

    [JsonPropertyName("id_estrangeiro")]
    public string? ForeignId { get; init; }

    [JsonPropertyName("motivo_sem_nif")]
    public int? MissingNifReason { get; init; }

    [JsonPropertyName("razao_social")]
    public string? CorporateName { get; init; }

    [JsonPropertyName("nome_completo")]
    public string? FullName { get; init; }

    [JsonPropertyName("nome_estrangeiro")]
    public string? ForeignName { get; init; }

    public string? Caepf { get; init; }

    [JsonPropertyName("im")]
    public string? MunicipalRegistration { get; init; }

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

    [JsonPropertyName("codigo_municipio")]
    public string? CityCode { get; init; }

    [JsonPropertyName("codigo_pais")]
    public string? CountryCode { get; init; }

    [JsonPropertyName("cep_exterior")]
    public string? ForeignPostalCode { get; init; }

    [JsonPropertyName("cidade")]
    public string? City { get; init; }

    [JsonPropertyName("estado")]
    public string? State { get; init; }

    [JsonPropertyName("telefone")]
    public string? Phone { get; init; }

    public string? Email { get; init; }
}
