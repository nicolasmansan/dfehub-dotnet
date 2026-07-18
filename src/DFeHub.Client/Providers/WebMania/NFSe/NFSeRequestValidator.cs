using DFeHub.Exceptions;
using DFeHub.Validation;

namespace DFeHub.Providers.WebMania.NFSe;

internal static class NFSeRequestValidator
{
    internal static void Validate(NFSeIssueRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var failures = new List<WebManiaValidationFailure>();

        Defined(request.Environment, "Environment", failures);
        OptionalLength(request.OrderId, "OrderId", 1, 15, failures);
        NotificationUrl(request.NotificationUrl, failures);
        Schedule(request.ScheduledAt, failures);

        if (request.Rps is not { Count: >= 1 and <= 50 })
        {
            Add(
                failures,
                "Rps",
                "At least one and at most 50 RPS entries are required.",
                "NFSE_RPS_COUNT");
        }
        else
        {
            for (var index = 0; index < request.Rps.Count; index++)
            {
                ValidateRps(request.Rps[index], $"Rps[{index}]", failures);
            }
        }

        ThrowIfAny(failures);
    }

    internal static void Validate(NFSeCancellationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var failures = new List<WebManiaValidationFailure>();
        NonEmptyUuid(request.Uuid, "Uuid", failures);
        Defined(request.Reason, "Reason", failures);
        ThrowIfAny(failures);
    }

    internal static void Validate(NFSeSubstitutionRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var failures = new List<WebManiaValidationFailure>();
        Defined(request.Environment, "Environment", failures);
        NonBlank(
            request.VerificationCode,
            "VerificationCode",
            "A verification code is required.",
            "NFSE_VERIFICATION_CODE",
            failures);
        Defined(request.Reason, "Reason", failures);
        ValidateRps(request.Rps, "Rps", failures);
        ThrowIfAny(failures);
    }

    internal static void Validate(NFSeManifestationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var failures = new List<WebManiaValidationFailure>();
        Defined(request.Environment, "Environment", failures);
        Identifier(request.AccessKey, request.Uuid, failures);
        Defined(request.ManifestingParty, "ManifestingParty", failures);
        Defined(request.Event, "Event", failures);

        if (request.Event == NFSeManifestationEvent.Rejection)
        {
            if (request.RejectionReason is null)
            {
                Add(
                    failures,
                    "RejectionReason",
                    "A rejection reason is required for a rejection event.",
                    "NFSE_REJECTION_REASON");
            }
            else
            {
                Defined(
                    request.RejectionReason.Value,
                    "RejectionReason",
                    failures);

                if (request.RejectionReason ==
                    NFSeManifestationRejectionReason.Other)
                {
                    Required(
                        request.RejectionJustification,
                        "RejectionJustification",
                        15,
                        255,
                        failures);
                }
                else
                {
                    OptionalLength(
                        request.RejectionJustification,
                        "RejectionJustification",
                        15,
                        255,
                        failures);
                }
            }
        }
        else if (request.Event == NFSeManifestationEvent.Confirmation &&
                 (request.RejectionReason is not null ||
                  request.RejectionJustification is not null))
        {
            Add(
                failures,
                "RejectionReason",
                "A confirmation cannot contain rejection fields.",
                "NFSE_CONFIRMATION_REJECTION_FIELDS");
        }

        ThrowIfAny(failures);
    }

    internal static void ValidateUuid(Guid uuid)
    {
        if (uuid == Guid.Empty)
        {
            ThrowSingle("uuid", "A non-empty UUID is required.", "NFSE_UUID");
        }
    }

    internal static void ValidateDownloadUri(Uri documentUrl)
    {
        ArgumentNullException.ThrowIfNull(documentUrl);
        var isOfficialHost =
            documentUrl.IsAbsoluteUri &&
            documentUrl.IdnHost.Equals(
                "api.webmania.com.br",
                StringComparison.OrdinalIgnoreCase);
        var pathSegments = documentUrl.IsAbsoluteUri
            ? documentUrl.AbsolutePath.Split(
                '/',
                StringSplitOptions.RemoveEmptyEntries)
            : [];
        var isOfficialArtifact =
            pathSegments.Length == 2 &&
            pathSegments[0] is "nfse" or "xmlnfse" or "darps" &&
            Guid.TryParse(pathSegments[1], out var uuid) &&
            uuid != Guid.Empty;

        if (!documentUrl.IsAbsoluteUri ||
            documentUrl.Scheme != Uri.UriSchemeHttps ||
            !documentUrl.IsDefaultPort ||
            !isOfficialHost ||
            !isOfficialArtifact)
        {
            ThrowSingle(
                "documentUrl",
                "Authenticated downloads require an official HTTPS NFS-e " +
                "artifact URL.",
                "NFSE_DOWNLOAD_HOST");
        }
    }

    private static void ValidateRps(
        NFSeRps? rps,
        string path,
        List<WebManiaValidationFailure> failures)
    {
        if (rps is null)
        {
            Add(
                failures,
                path,
                "RPS information is required.",
                "NFSE_RPS_REQUIRED");
            return;
        }

        ValidateService(rps.Service, $"{path}.Service", failures);
        ValidateTaker(rps.Taker, $"{path}.Taker", failures);
        ValidateRecipient(rps.Recipient, $"{path}.Recipient", failures);
        ValidateConstruction(
            rps.Construction,
            $"{path}.Construction",
            failures);

        if (rps.Service?.RecipientIndicator == 1 && rps.Recipient is null)
        {
            Add(
                failures,
                $"{path}.Recipient",
                "Recipient information is required when RecipientIndicator is 1.",
                "NFSE_RECIPIENT_REQUIRED");
        }

        if (rps.Service?.RecipientIndicator == 0 && rps.Recipient is not null)
        {
            Add(
                failures,
                $"{path}.Recipient",
                "Recipient information requires RecipientIndicator 1.",
                "NFSE_RECIPIENT_UNEXPECTED");
        }

        if (rps.Service?.RecipientIndicator is not null and not (0 or 1))
        {
            Add(
                failures,
                $"{path}.Service.RecipientIndicator",
                "RecipientIndicator must be 0 or 1.",
                "NFSE_INDICATOR");
        }

        var requiresRealEstate =
            rps.Service?.OperationIndicatorCode?.StartsWith(
                "02",
                StringComparison.Ordinal) == true &&
            rps.Construction is null;
        if (requiresRealEstate && rps.Service?.RealEstate is null)
        {
            Add(
                failures,
                $"{path}.Service.RealEstate",
                "Real-estate data is required for this operation indicator.",
                "NFSE_REAL_ESTATE_REQUIRED");
        }
        else if (!requiresRealEstate && rps.Service?.RealEstate is not null)
        {
            Add(
                failures,
                $"{path}.Service.RealEstate",
                "Real-estate data is only accepted for a non-construction " +
                "operation indicator beginning with 02.",
                "NFSE_REAL_ESTATE_UNEXPECTED");
        }
    }

    private static void ValidateService(
        NFSeService? service,
        string path,
        List<WebManiaValidationFailure> failures)
    {
        if (service is null)
        {
            Add(
                failures,
                path,
                "Service information is required.",
                "NFSE_SERVICE_REQUIRED");
            return;
        }

        Required(
            service.Discrimination,
            $"{path}.Discrimination",
            1,
            2000,
            failures);
        NonNegative(service.ServiceValue, $"{path}.ServiceValue", failures);

        var hasTaxClass = !string.IsNullOrWhiteSpace(service.TaxClass);
        var hasTaxes = HasTaxValues(service.Taxes);
        var hasNationalIss = service.IssTaxation is not null;

        if (service.Taxes is not null && !hasTaxes)
        {
            Add(
                failures,
                $"{path}.Taxes",
                "The taxes object must contain at least one documented value.",
                "NFSE_TAXES_EMPTY");
        }

        if ((hasTaxClass &&
             (service.Taxes is not null || hasNationalIss)) ||
            (!hasTaxClass && !hasTaxes && !hasNationalIss))
        {
            Add(
                failures,
                path,
                "Supply TaxClass alone, or an explicit municipal/national tax " +
                "configuration.",
                "NFSE_TAX_CONFIGURATION");
        }

        if (service.IssTaxation is not null and not (1 or 2 or 4))
        {
            Add(
                failures,
                $"{path}.IssTaxation",
                "IssTaxation must be 1, 2 or 4.",
                "NFSE_INDICATOR");
        }

        if (service.IssWithheld is not null and not (1 or 2))
        {
            Add(
                failures,
                $"{path}.IssWithheld",
                "IssWithheld must be 1 or 2.",
                "NFSE_INDICATOR");
        }

        if (service.IssWithholdingResponsible is not null and not (1 or 2))
        {
            Add(
                failures,
                $"{path}.IssWithholdingResponsible",
                "IssWithholdingResponsible must be 1 or 2.",
                "NFSE_INDICATOR");
        }

        if (service.IssWithheld == 1 &&
            service.IssWithholdingResponsible is null)
        {
            Add(
                failures,
                $"{path}.IssWithholdingResponsible",
                "The withholding responsible party is required when ISS is " +
                "withheld.",
                "NFSE_ISS_WITHHOLDING_RESPONSIBLE");
        }

        ValidateGovernmentEntity(
            service.GovernmentEntity,
            $"{path}.GovernmentEntity",
            failures);
        ValidateRealEstate(
            service.RealEstate,
            $"{path}.RealEstate",
            failures);
        ValidateReimbursement(
            service.Reimbursement,
            $"{path}.Reimbursement",
            failures);
        ValidateDeductionReduction(
            service.DeductionReduction,
            $"{path}.DeductionReduction",
            failures);
    }

    private static void ValidateTaker(
        NFSeParty? taker,
        string path,
        List<WebManiaValidationFailure> failures)
    {
        if (taker is null)
        {
            return;
        }

        var identities = new[]
        {
            taker.Cpf,
            taker.Cnpj,
            taker.ForeignId,
            taker.ForeignDocument,
        }.Count(value => !string.IsNullOrWhiteSpace(value));

        if (identities != 1)
        {
            Add(
                failures,
                path,
                "Exactly one taker identity must be supplied.",
                "NFSE_PARTY_IDENTITY");
        }

        if (!string.IsNullOrWhiteSpace(taker.Cpf))
        {
            Required(taker.FullName, $"{path}.FullName", 2, 150, failures);
        }

        if (!string.IsNullOrWhiteSpace(taker.Cnpj))
        {
            Required(
                taker.CorporateName,
                $"{path}.CorporateName",
                2,
                150,
                failures);
        }

        if (!string.IsNullOrWhiteSpace(taker.ForeignId))
        {
            Required(
                taker.ForeignName,
                $"{path}.ForeignName",
                2,
                150,
                failures);
        }

        if (!string.IsNullOrWhiteSpace(taker.ForeignDocument) &&
            string.IsNullOrWhiteSpace(taker.FullName) &&
            string.IsNullOrWhiteSpace(taker.CorporateName) &&
            string.IsNullOrWhiteSpace(taker.ForeignName))
        {
            Add(
                failures,
                path,
                "A taker name is required with ForeignDocument.",
                "NFSE_PARTY_NAME");
        }
    }

    private static void ValidateRecipient(
        NFSeRecipient? recipient,
        string path,
        List<WebManiaValidationFailure> failures)
    {
        if (recipient is null)
        {
            return;
        }

        var identities = new[]
        {
            recipient.Cpf,
            recipient.Cnpj,
            recipient.ForeignId,
        }.Count(value => !string.IsNullOrWhiteSpace(value));

        if (identities != 1)
        {
            Add(
                failures,
                path,
                "Exactly one recipient identity must be supplied.",
                "NFSE_RECIPIENT_IDENTITY");
        }

        Required(recipient.Name, $"{path}.Name", 1, 255, failures);

        if (recipient.Address is { } address)
        {
            ValidateStructuredAddress(
                address,
                $"{path}.Address",
                "NFSE_RECIPIENT_ADDRESS",
                failures);
        }
    }

    private static void NotificationUrl(
        Uri? notificationUrl,
        List<WebManiaValidationFailure> failures)
    {
        if (notificationUrl is not null &&
            (!notificationUrl.IsAbsoluteUri ||
             (notificationUrl.Scheme != Uri.UriSchemeHttp &&
              notificationUrl.Scheme != Uri.UriSchemeHttps)))
        {
            Add(
                failures,
                "NotificationUrl",
                "NotificationUrl must be an absolute HTTP or HTTPS URL.",
                "NFSE_NOTIFICATION_URL");
        }
    }

    private static void Schedule(
        DateTime? scheduledAt,
        List<WebManiaValidationFailure> failures)
    {
        if (scheduledAt is null)
        {
            return;
        }

        if (scheduledAt.Value.Kind != DateTimeKind.Unspecified)
        {
            Add(
                failures,
                "ScheduledAt",
                "ScheduledAt must be a wall-clock time without a time zone.",
                "NFSE_SCHEDULE_KIND");
        }

        if (scheduledAt.Value.Ticks % TimeSpan.TicksPerMinute != 0)
        {
            Add(
                failures,
                "ScheduledAt",
                "ScheduledAt must use zero seconds and fractional seconds.",
                "NFSE_SCHEDULE_PRECISION");
        }
    }

    private static bool HasTaxValues(NFSeTaxes? taxes)
    {
        return taxes is not null &&
            (taxes.Iss.HasValue ||
             taxes.SimplesNacionalIss.HasValue ||
             taxes.Reduction.HasValue ||
             !string.IsNullOrWhiteSpace(taxes.PisCofinsTaxSituation) ||
             taxes.Pis.HasValue ||
             taxes.Cofins.HasValue ||
             taxes.Inss.HasValue ||
             taxes.IncomeTax.HasValue ||
             taxes.Csll.HasValue ||
             taxes.EmployerContribution.HasValue ||
             taxes.IbsCbs is not null ||
             !string.IsNullOrWhiteSpace(taxes.TaxDescription));
    }

    private static void ValidateGovernmentEntity(
        NFSeGovernmentEntity? entity,
        string path,
        List<WebManiaValidationFailure> failures)
    {
        if (entity is null)
        {
            return;
        }

        if (entity.Type is not (1 or 2 or 3 or 4 or 9))
        {
            Add(
                failures,
                $"{path}.Type",
                "Government entity type must be 1, 2, 3, 4 or 9.",
                "NFSE_GOVERNMENT_ENTITY_TYPE");
        }

        if (entity.Type == 9)
        {
            Required(
                entity.Classification,
                $"{path}.Classification",
                1,
                100,
                failures);
        }
        else
        {
            OptionalLength(
                entity.Classification,
                $"{path}.Classification",
                1,
                100,
                failures);
        }
    }

    private static void ValidateRealEstate(
        NFSeRealEstate? realEstate,
        string path,
        List<WebManiaValidationFailure> failures)
    {
        if (realEstate is null)
        {
            return;
        }

        var hasDirectAddressFields =
            !string.IsNullOrWhiteSpace(realEstate.Address?.DirectAddress) ||
            !string.IsNullOrWhiteSpace(realEstate.Number) ||
            !string.IsNullOrWhiteSpace(realEstate.AddressComplement) ||
            !string.IsNullOrWhiteSpace(realEstate.District) ||
            !string.IsNullOrWhiteSpace(realEstate.PostalCode) ||
            !string.IsNullOrWhiteSpace(realEstate.CountryCode) ||
            !string.IsNullOrWhiteSpace(realEstate.ForeignPostalCode) ||
            !string.IsNullOrWhiteSpace(realEstate.City) ||
            !string.IsNullOrWhiteSpace(realEstate.StateOrProvince);
        var hasStructuredAddress =
            realEstate.Address?.StructuredAddress is not null;
        var formCount = new[]
        {
            !string.IsNullOrWhiteSpace(realEstate.PropertyRegistration),
            !string.IsNullOrWhiteSpace(realEstate.CibCode),
            hasDirectAddressFields,
            hasStructuredAddress,
        }.Count(value => value);

        if (formCount != 1)
        {
            Add(
                failures,
                path,
                "Real-estate data requires exactly one identification form.",
                "NFSE_REAL_ESTATE_FORM");
        }

        if (realEstate.Address is not null &&
            !hasDirectAddressFields &&
            !hasStructuredAddress)
        {
            Add(
                failures,
                $"{path}.Address",
                "The real-estate address has no wire value.",
                "NFSE_REAL_ESTATE_ADDRESS");
        }

        if (hasDirectAddressFields &&
            string.IsNullOrWhiteSpace(realEstate.Address?.DirectAddress))
        {
            Add(
                failures,
                $"{path}.Address",
                "A direct real-estate address requires its street.",
                "NFSE_REAL_ESTATE_ADDRESS");
        }

        if (realEstate.Address?.StructuredAddress is { } structuredAddress)
        {
            ValidateStructuredAddress(
                structuredAddress,
                $"{path}.Address",
                "NFSE_REAL_ESTATE_ADDRESS",
                failures);
        }
    }

    private static void ValidateStructuredAddress(
        NFSeStructuredAddress address,
        string path,
        string code,
        List<WebManiaValidationFailure> failures)
    {
        var locationGroupCount = new[]
        {
            address.NationalAddress is not null,
            address.ForeignAddress is not null,
        }.Count(value => value);

        if (locationGroupCount != 1)
        {
            Add(
                failures,
                path,
                "Exactly one national or foreign address group is required.",
                code);
        }
    }

    private static void ValidateReimbursement(
        NFSeReimbursement? reimbursement,
        string path,
        List<WebManiaValidationFailure> failures)
    {
        if (reimbursement is null)
        {
            return;
        }

        if (reimbursement.Documents is not { Count: >= 1 and <= 100 })
        {
            Add(
                failures,
                $"{path}.Documents",
                "Reimbursement requires between one and 100 documents.",
                "NFSE_REIMBURSEMENT_DOCUMENTS");
            return;
        }

        for (var index = 0; index < reimbursement.Documents.Count; index++)
        {
            var document = reimbursement.Documents[index];
            var documentPath = $"{path}.Documents[{index}]";
            if (document is null)
            {
                Add(
                    failures,
                    documentPath,
                    "A reimbursement document is required.",
                    "NFSE_REIMBURSEMENT_DOCUMENT");
                continue;
            }

            var documentGroupCount = new[]
            {
                document.NationalFiscalDocument is not null,
                document.OtherFiscalDocument is not null,
                document.OtherDocument is not null,
            }.Count(value => value);
            if (documentGroupCount != 1)
            {
                Add(
                    failures,
                    documentPath,
                    "Exactly one reimbursement document group is required.",
                    "NFSE_REIMBURSEMENT_DOCUMENT");
            }

            if (document.ReimbursementType == 99)
            {
                Required(
                    document.Description,
                    $"{documentPath}.Description",
                    1,
                    150,
                    failures);
            }

            NonNegative(document.Value, $"{documentPath}.Value", failures);
        }
    }

    private static void ValidateDeductionReduction(
        NFSeDeductionReduction? reduction,
        string path,
        List<WebManiaValidationFailure> failures)
    {
        if (reduction is null)
        {
            return;
        }

        var optionCount = new[]
        {
            reduction.Value.HasValue,
            reduction.Percentage.HasValue,
            reduction.Documents is not null,
        }.Count(value => value);
        if (optionCount != 1)
        {
            Add(
                failures,
                path,
                "Deduction/reduction requires exactly one option.",
                "NFSE_DEDUCTION_OPTION");
        }

        if (reduction.Value is { } value)
        {
            NonNegative(value, $"{path}.Value", failures);
        }

        if (reduction.Percentage is { } percentage &&
            (percentage < 0 || percentage > 100))
        {
            Add(
                failures,
                $"{path}.Percentage",
                "The deduction percentage must be between 0 and 100.",
                "NFSE_DEDUCTION_PERCENTAGE");
        }

        if (reduction.Documents is null)
        {
            return;
        }

        if (reduction.Documents is not { Count: >= 1 and <= 1000 })
        {
            Add(
                failures,
                $"{path}.Documents",
                "Deduction/reduction requires between one and 1,000 " +
                "documents.",
                "NFSE_DEDUCTION_DOCUMENTS");
            return;
        }

        for (var index = 0; index < reduction.Documents.Count; index++)
        {
            ValidateDeductionDocument(
                reduction.Documents[index],
                $"{path}.Documents[{index}]",
                failures);
        }
    }

    private static void ValidateDeductionDocument(
        NFSeDeductionDocument? document,
        string path,
        List<WebManiaValidationFailure> failures)
    {
        if (document is null)
        {
            Add(
                failures,
                path,
                "A deduction document is required.",
                "NFSE_DEDUCTION_DOCUMENT");
            return;
        }

        var identifierCount = new[]
        {
            !string.IsNullOrWhiteSpace(document.NFSeAccessKey),
            !string.IsNullOrWhiteSpace(document.NFeAccessKey),
            document.MunicipalInvoice is not null,
            document.PaperInvoice is not null,
            !string.IsNullOrWhiteSpace(document.FiscalDocument),
            !string.IsNullOrWhiteSpace(document.NonFiscalDocument),
        }.Count(value => value);
        if (identifierCount != 1)
        {
            Add(
                failures,
                path,
                "Exactly one deduction document identifier is required.",
                "NFSE_DEDUCTION_DOCUMENT");
        }

        if (document.DeductionType is not
            (1 or 2 or 3 or 4 or 5 or 6 or 7 or 8 or 99))
        {
            Add(
                failures,
                $"{path}.DeductionType",
                "The deduction type is not supported.",
                "NFSE_DEDUCTION_TYPE");
        }

        if (document.DeductionType == 99)
        {
            Required(
                document.OtherDeductionsDescription,
                $"{path}.OtherDeductionsDescription",
                1,
                150,
                failures);
        }

        NonNegative(
            document.DeductibleValue,
            $"{path}.DeductibleValue",
            failures);
        NonNegative(
            document.DeductionValue,
            $"{path}.DeductionValue",
            failures);
        if (document.DeductionValue > document.DeductibleValue)
        {
            Add(
                failures,
                $"{path}.DeductionValue",
                "DeductionValue cannot exceed DeductibleValue.",
                "NFSE_DEDUCTION_VALUE");
        }
    }

    private static void ValidateConstruction(
        NFSeConstruction? construction,
        string path,
        List<WebManiaValidationFailure> failures)
    {
        if (construction is null)
        {
            return;
        }

        var hasAddress =
            !string.IsNullOrWhiteSpace(construction.Address) ||
            !string.IsNullOrWhiteSpace(construction.Number) ||
            !string.IsNullOrWhiteSpace(construction.AddressComplement) ||
            !string.IsNullOrWhiteSpace(construction.District) ||
            !string.IsNullOrWhiteSpace(construction.PostalCode);
        var nationalFormCount = new[]
        {
            !string.IsNullOrWhiteSpace(
                construction.FiscalPropertyRegistration),
            !string.IsNullOrWhiteSpace(construction.WorkCode),
            hasAddress,
        }.Count(value => value);

        if (nationalFormCount > 1)
        {
            Add(
                failures,
                path,
                "National construction identification forms are mutually " +
                "exclusive.",
                "NFSE_CONSTRUCTION_FORM");
        }

        if (hasAddress &&
            (string.IsNullOrWhiteSpace(construction.Address) ||
             string.IsNullOrWhiteSpace(construction.Number) ||
             string.IsNullOrWhiteSpace(construction.District) ||
             string.IsNullOrWhiteSpace(construction.PostalCode)))
        {
            Add(
                failures,
                path,
                "A construction address requires street, number, district " +
                "and postal code.",
                "NFSE_CONSTRUCTION_ADDRESS");
        }

        var hasAnyValue =
            nationalFormCount > 0 ||
            !string.IsNullOrWhiteSpace(construction.Art) ||
            !string.IsNullOrWhiteSpace(construction.Cno) ||
            !string.IsNullOrWhiteSpace(construction.Name) ||
            !string.IsNullOrWhiteSpace(construction.City) ||
            !string.IsNullOrWhiteSpace(construction.State) ||
            !string.IsNullOrWhiteSpace(
                construction.PropertyRegistration) ||
            !string.IsNullOrWhiteSpace(construction.ForeignIndicator) ||
            !string.IsNullOrWhiteSpace(construction.WorkRegistration) ||
            !string.IsNullOrWhiteSpace(construction.EncapsulationNumber) ||
            !string.IsNullOrWhiteSpace(construction.WorkLocationState) ||
            !string.IsNullOrWhiteSpace(construction.WorkLocationCity);
        if (!hasAnyValue)
        {
            Add(
                failures,
                path,
                "Construction data must contain at least one provider value.",
                "NFSE_CONSTRUCTION_EMPTY");
        }

        if (string.IsNullOrWhiteSpace(construction.WorkLocationState) !=
            string.IsNullOrWhiteSpace(construction.WorkLocationCity))
        {
            Add(
                failures,
                path,
                "WorkLocationState and WorkLocationCity must be supplied " +
                "together.",
                "NFSE_CONSTRUCTION_LOCATION");
        }
    }

    private static void Identifier(
        string? accessKey,
        Guid? uuid,
        List<WebManiaValidationFailure> failures)
    {
        var hasAccessKey = accessKey is not null;
        var hasUuid = uuid is not null;
        if (hasAccessKey == hasUuid)
        {
            Add(
                failures,
                "AccessKey",
                "Supply either AccessKey or Uuid, but not both.",
                "NFSE_IDENTIFIER");
            return;
        }

        if (hasAccessKey)
        {
            AccessKey(accessKey, "AccessKey", failures);
        }
        else if (uuid == Guid.Empty)
        {
            Add(
                failures,
                "Uuid",
                "A non-empty UUID is required.",
                "NFSE_UUID");
        }
    }

    private static void AccessKey(
        string? value,
        string path,
        List<WebManiaValidationFailure> failures)
    {
        if (value is null ||
            value.Length != 50 ||
            value.Any(character => !char.IsAsciiDigit(character)))
        {
            Add(
                failures,
                path,
                "An NFS-e access key must contain exactly 50 digits.",
                "NFSE_ACCESS_KEY");
        }
    }

    private static void NonEmptyUuid(
        Guid value,
        string path,
        List<WebManiaValidationFailure> failures)
    {
        if (value == Guid.Empty)
        {
            Add(
                failures,
                path,
                "A non-empty UUID is required.",
                "NFSE_UUID");
        }
    }

    private static void NonBlank(
        string? value,
        string path,
        string message,
        string code,
        List<WebManiaValidationFailure> failures)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            Add(failures, path, message, code);
        }
    }

    private static void Required(
        string? value,
        string path,
        int minimumLength,
        int maximumLength,
        List<WebManiaValidationFailure> failures)
    {
        if (string.IsNullOrWhiteSpace(value) ||
            value.Length < minimumLength ||
            value.Length > maximumLength)
        {
            Add(
                failures,
                path,
                $"The value must contain between {minimumLength} and " +
                $"{maximumLength} characters.",
                "NFSE_STRING_LENGTH");
        }
    }

    private static void OptionalLength(
        string? value,
        string path,
        int minimumLength,
        int maximumLength,
        List<WebManiaValidationFailure> failures)
    {
        if (value is not null)
        {
            Required(value, path, minimumLength, maximumLength, failures);
        }
    }

    private static void Defined<TEnum>(
        TEnum value,
        string path,
        List<WebManiaValidationFailure> failures)
        where TEnum : struct, Enum
    {
        if (!Enum.IsDefined(value))
        {
            Add(
                failures,
                path,
                "The value is not supported by the official contract.",
                "NFSE_ENUM");
        }
    }

    private static void NonNegative(
        decimal value,
        string path,
        List<WebManiaValidationFailure> failures)
    {
        if (value < 0)
        {
            Add(
                failures,
                path,
                "The value cannot be negative.",
                "NFSE_NEGATIVE");
        }
    }

    private static void Add(
        List<WebManiaValidationFailure> failures,
        string path,
        string message,
        string code)
    {
        failures.Add(new WebManiaValidationFailure(path, message, code));
    }

    private static void ThrowSingle(string path, string message, string code)
    {
        throw new WebManiaValidationException(
            [new WebManiaValidationFailure(path, message, code)]);
    }

    private static void ThrowIfAny(
        List<WebManiaValidationFailure> failures)
    {
        if (failures.Count > 0)
        {
            throw new WebManiaValidationException(failures);
        }
    }
}
