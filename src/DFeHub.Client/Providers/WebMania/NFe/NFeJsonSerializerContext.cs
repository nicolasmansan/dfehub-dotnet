using System.Text.Json.Serialization;

namespace DFeHub.Providers.WebMania.NFe;

[JsonSourceGenerationOptions(
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower,
    NumberHandling = JsonNumberHandling.AllowReadingFromString,
    Converters =
    [
        typeof(NFeDecimalStringConverter),
        typeof(NFeDateTimeOffsetConverter),
    ])]
[JsonSerializable(typeof(NFeIssueRequest))]
[JsonSerializable(typeof(NFeReturnRequest))]
[JsonSerializable(typeof(NFeAdjustmentRequest))]
[JsonSerializable(typeof(NFeComplementRequest))]
[JsonSerializable(typeof(NFeCorrectionLetterRequest))]
[JsonSerializable(typeof(NFeRecipientManifestationRequest))]
[JsonSerializable(typeof(NFeIbsCbsEventRequest))]
[JsonSerializable(typeof(NFeIbsCbsEventCancellationRequest))]
[JsonSerializable(typeof(NFeFinancialReconciliationRequest))]
[JsonSerializable(typeof(NFeFinancialReconciliationCancellationRequest))]
[JsonSerializable(typeof(NFeCancellationRequest))]
[JsonSerializable(typeof(NFeNumberInvalidationRequest))]
[JsonSerializable(typeof(NFeTaxClassUpsertRequest))]
[JsonSerializable(typeof(NFeTaxClassDeleteRequest))]
[JsonSerializable(typeof(NFeReportRequest))]
[JsonSerializable(typeof(NFeCompanyUpdateRequest))]
[JsonSerializable(typeof(NFeAccessKeyQuery))]
[JsonSerializable(typeof(NFeUuidQuery))]
[JsonSerializable(typeof(NFeOrderIdQuery))]
[JsonSerializable(typeof(NFeDocumentResponse))]
[JsonSerializable(typeof(NFeDocumentResponse[]))]
[JsonSerializable(typeof(NFeEventResponse))]
[JsonSerializable(typeof(NFeCancellationResponse))]
[JsonSerializable(typeof(NFeNumberInvalidationResponse))]
[JsonSerializable(typeof(NFeSefazStatusResponse))]
[JsonSerializable(typeof(NFeCertificateStatusResponse))]
[JsonSerializable(typeof(NFeTaxClassResponse))]
[JsonSerializable(typeof(NFeTaxClassResponse[]))]
[JsonSerializable(typeof(NFeTaxClassDeleteResponse[]))]
[JsonSerializable(typeof(NFeReportResponse))]
[JsonSerializable(typeof(NFeCompanyUpdateResponse))]
[JsonSerializable(typeof(NFeNotification))]
internal sealed partial class NFeJsonSerializerContext : JsonSerializerContext;
