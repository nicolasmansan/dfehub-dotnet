namespace DFeHub.Providers.WebMania.NFSe;

/// <summary>
/// Identifies the municipality provider environment used for NFS-e operations.
/// </summary>
public enum NFSeEnvironment
{
    /// <summary>
    /// Production environment.
    /// </summary>
    Production = 1,

    /// <summary>
    /// Certification environment without fiscal value.
    /// </summary>
    Certification = 2,
}

/// <summary>
/// Identifies the reason for cancelling or replacing an NFS-e.
/// </summary>
public enum NFSeCancellationReason
{
    /// <summary>
    /// The document contains an issuance error.
    /// </summary>
    IssuanceError = 1,

    /// <summary>
    /// The declared service was not provided.
    /// </summary>
    ServiceNotProvided = 2,

    /// <summary>
    /// The document duplicates another invoice.
    /// </summary>
    DuplicateInvoice = 4,
}

/// <summary>
/// Identifies the party that manifests participation in a national-standard
/// NFS-e.
/// </summary>
public enum NFSeManifestingParty
{
    /// <summary>
    /// The service taker.
    /// </summary>
    Taker = 1,

    /// <summary>
    /// The service intermediary.
    /// </summary>
    Intermediary = 2,
}

/// <summary>
/// Identifies an NFS-e participation manifestation event.
/// </summary>
public enum NFSeManifestationEvent
{
    /// <summary>
    /// Confirms participation in the transaction.
    /// </summary>
    Confirmation = 1,

    /// <summary>
    /// Rejects participation in the transaction.
    /// </summary>
    Rejection = 2,
}

/// <summary>
/// Identifies the reason for rejecting participation in an NFS-e.
/// </summary>
public enum NFSeManifestationRejectionReason
{
    /// <summary>
    /// The NFS-e is duplicated.
    /// </summary>
    DuplicateInvoice = 1,

    /// <summary>
    /// The NFS-e was already issued by the taker.
    /// </summary>
    InvoiceAlreadyIssuedByTaker = 2,

    /// <summary>
    /// The taxable event did not occur.
    /// </summary>
    TaxableEventDidNotOccur = 3,

    /// <summary>
    /// The document assigns tax responsibility incorrectly.
    /// </summary>
    TaxResponsibilityError = 4,

    /// <summary>
    /// A service value, deduction, description or taxable-event date is
    /// incorrect.
    /// </summary>
    ServiceInformationError = 5,

    /// <summary>
    /// Another reason described by the rejection justification.
    /// </summary>
    Other = 9,
}
