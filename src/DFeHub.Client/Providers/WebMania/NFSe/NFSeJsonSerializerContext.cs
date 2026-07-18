using System.Text.Json;
using System.Text.Json.Serialization;

namespace DFeHub.Providers.WebMania.NFSe;

[JsonSourceGenerationOptions(
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower,
    NumberHandling = JsonNumberHandling.AllowReadingFromString,
    Converters =
    [
        typeof(NFSeDecimalStringConverter),
        typeof(NFSeDateOnlyConverter),
    ])]
[JsonSerializable(typeof(NFSeIssueRequest))]
[JsonSerializable(typeof(NFSeRps))]
[JsonSerializable(typeof(NFSeCancellationRequest))]
[JsonSerializable(typeof(NFSeSubstitutionRequest))]
[JsonSerializable(typeof(NFSeManifestationRequest))]
[JsonSerializable(typeof(NFSeParty))]
[JsonSerializable(typeof(NFSeRecipient))]
[JsonSerializable(typeof(NFSeStructuredAddress))]
[JsonSerializable(typeof(NFSeNationalAddress))]
[JsonSerializable(typeof(NFSeForeignAddress))]
[JsonSerializable(typeof(NFSeIntermediary))]
[JsonSerializable(typeof(NFSeReferencedSupplier))]
[JsonSerializable(typeof(NFSeDeductionSupplier))]
[JsonSerializable(typeof(NFSeService))]
[JsonSerializable(typeof(NFSePaymentCondition))]
[JsonSerializable(typeof(NFSePaymentInstallment))]
[JsonSerializable(typeof(NFSeGovernmentEntity))]
[JsonSerializable(typeof(NFSeRealEstate))]
[JsonSerializable(typeof(NFSeRealEstateAddress))]
[JsonSerializable(typeof(NFSeReimbursement))]
[JsonSerializable(typeof(NFSeReimbursementDocument))]
[JsonSerializable(typeof(NFSeNationalFiscalDocument))]
[JsonSerializable(typeof(NFSeOtherFiscalDocument))]
[JsonSerializable(typeof(NFSeOtherDocument))]
[JsonSerializable(typeof(NFSeDeductionReduction))]
[JsonSerializable(typeof(NFSeDeductionDocument))]
[JsonSerializable(typeof(NFSeMunicipalInvoiceReference))]
[JsonSerializable(typeof(NFSePaperInvoiceReference))]
[JsonSerializable(typeof(NFSeConstruction))]
[JsonSerializable(typeof(NFSeEventActivity))]
[JsonSerializable(typeof(NFSeTaxes))]
[JsonSerializable(typeof(NFSeIbsCbsTax))]
[JsonSerializable(typeof(NFSeIbsCbsJurisdiction))]
[JsonSerializable(typeof(NFSeIbsCbsPresumedCredit))]
[JsonSerializable(typeof(NFSeIbsCbsCreditTransfer))]
[JsonSerializable(typeof(NFSeStringOrIntValue))]
[JsonSerializable(typeof(NFSeAbrasfItem))]
[JsonSerializable(typeof(NFSeProviderStatusResponse))]
[JsonSerializable(typeof(NFSeProviderServiceCode))]
[JsonSerializable(typeof(NFSeSuggestedTaxRate))]
[JsonSerializable(typeof(NFSeDocumentResponse))]
[JsonSerializable(typeof(NFSeReplacedDocument))]
[JsonSerializable(typeof(NFSeRps[]))]
[JsonSerializable(typeof(NFSePaymentInstallment[]))]
[JsonSerializable(typeof(NFSeReimbursementDocument[]))]
[JsonSerializable(typeof(NFSeDeductionDocument[]))]
[JsonSerializable(typeof(NFSeAbrasfItem[]))]
[JsonSerializable(typeof(NFSeProviderServiceCode[]))]
[JsonSerializable(typeof(NFSeDocumentResponse[]))]
[JsonSerializable(typeof(NFSeEnvironment[]))]
[JsonSerializable(typeof(string[]))]
[JsonSerializable(typeof(Dictionary<string, string>))]
[JsonSerializable(typeof(Dictionary<string, JsonElement>))]
internal sealed partial class NFSeJsonSerializerContext : JsonSerializerContext;
