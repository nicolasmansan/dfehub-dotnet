using DFeHub.Providers.WebMania.NFSe;

namespace DFeHub.Providers.WebMania;

/// <summary>
/// Provides typed access to NFS-e operations exposed by Webmania API 2.0.
/// </summary>
public interface INFSeClient : IWebManiaProductClient
{
    /// <summary>Gets the configured municipality provider status.</summary>
    Task<NFSeProviderStatusResponse> GetMunicipalityStatusAsync(
        CancellationToken cancellationToken = default);

    /// <summary>Issues an NFS-e or an RPS batch.</summary>
    Task<NFSeDocumentResponse> IssueAsync(
        NFSeIssueRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Queries an NFS-e, RPS or RPS batch by its Webmania UUID.</summary>
    Task<NFSeDocumentResponse> QueryByUuidAsync(
        Guid uuid,
        CancellationToken cancellationToken = default);

    /// <summary>Cancels an NFS-e or a scheduled issue.</summary>
    Task<NFSeDocumentResponse> CancelAsync(
        NFSeCancellationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Substitutes an issued NFS-e.</summary>
    Task<NFSeDocumentResponse> SubstituteAsync(
        NFSeSubstitutionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Registers an NFS-e participation manifestation.</summary>
    Task<NFSeDocumentResponse> ManifestAsync(
        NFSeManifestationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Downloads an authenticated Webmania document artifact.</summary>
    Task<FiscalDocumentFile> DownloadAsync(
        Uri documentUrl,
        CancellationToken cancellationToken = default);
}
