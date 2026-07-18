using DFeHub.Providers.WebMania.NFe;

namespace DFeHub.Providers.WebMania;

/// <summary>
/// Provides typed access to NF-e, NFC-e, CC-e, MDe, IBS/CBS and ECONF
/// operations exposed by Webmania API 1.0.
/// </summary>
public interface INFeClient : IWebManiaProductClient
{
    /// <summary>Issues an NF-e or NFC-e.</summary>
    Task<NFeDocumentResponse> IssueAsync(
        NFeIssueRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Issues a simplified return or reversal invoice.</summary>
    Task<NFeDocumentResponse> IssueReturnAsync(
        NFeReturnRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Issues an adjustment invoice.</summary>
    Task<NFeDocumentResponse> IssueAdjustmentAsync(
        NFeAdjustmentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Issues a complementary invoice.</summary>
    Task<NFeDocumentResponse> IssueComplementAsync(
        NFeComplementRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Registers a correction letter.</summary>
    Task<NFeEventResponse> IssueCorrectionLetterAsync(
        NFeCorrectionLetterRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Registers a recipient manifestation event.</summary>
    Task<NFeEventResponse> ManifestRecipientAsync(
        NFeRecipientManifestationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Registers an IBS/CBS fiscal event.</summary>
    Task<NFeEventResponse> RegisterIbsCbsEventAsync(
        NFeIbsCbsEventRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Cancels an IBS/CBS fiscal event.</summary>
    Task<NFeEventResponse> CancelIbsCbsEventAsync(
        NFeIbsCbsEventCancellationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Registers a financial reconciliation event (ECONF).</summary>
    Task<NFeEventResponse> RegisterFinancialReconciliationAsync(
        NFeFinancialReconciliationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Cancels a financial reconciliation event (ECONF).</summary>
    Task<NFeEventResponse> CancelFinancialReconciliationAsync(
        NFeFinancialReconciliationCancellationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Gets the current SEFAZ availability.</summary>
    Task<NFeSefazStatusResponse> GetSefazStatusAsync(
        CancellationToken cancellationToken = default);

    /// <summary>Gets the remaining issuer certificate validity.</summary>
    Task<NFeCertificateStatusResponse> GetCertificateStatusAsync(
        CancellationToken cancellationToken = default);

    /// <summary>Queries an invoice by its 44-digit access key.</summary>
    Task<NFeDocumentResponse> QueryByAccessKeyAsync(
        string accessKey,
        CancellationToken cancellationToken = default);

    /// <summary>Queries an invoice by its Webmania UUID.</summary>
    Task<NFeDocumentResponse> QueryByUuidAsync(
        Guid uuid,
        CancellationToken cancellationToken = default);

    /// <summary>Queries all invoices associated with an order identifier.</summary>
    Task<IReadOnlyList<NFeDocumentResponse>> QueryByOrderIdAsync(
        string orderId,
        CancellationToken cancellationToken = default);

    /// <summary>Cancels an authorized NF-e or NFC-e.</summary>
    Task<NFeCancellationResponse> CancelAsync(
        NFeCancellationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Invalidates an unused NF-e or NFC-e number range.</summary>
    Task<NFeNumberInvalidationResponse> InvalidateNumbersAsync(
        NFeNumberInvalidationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Creates or replaces a tax class.</summary>
    Task<NFeTaxClassResponse> UpsertTaxClassAsync(
        NFeTaxClassUpsertRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Lists the configured tax classes.</summary>
    Task<IReadOnlyList<NFeTaxClassResponse>> ListTaxClassesAsync(
        CancellationToken cancellationToken = default);

    /// <summary>Deletes one or more tax classes.</summary>
    Task<IReadOnlyList<NFeTaxClassDeleteResponse>> DeleteTaxClassesAsync(
        NFeTaxClassDeleteRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Starts an asynchronous fiscal document report export.</summary>
    Task<NFeReportResponse> ExportReportAsync(
        NFeReportRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Updates issuer company data.</summary>
    Task<NFeCompanyUpdateResponse> UpdateCompanyAsync(
        NFeCompanyUpdateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Downloads an authenticated Webmania document artifact.</summary>
    Task<FiscalDocumentFile> DownloadAsync(
        Uri documentUrl,
        CancellationToken cancellationToken = default);
}
