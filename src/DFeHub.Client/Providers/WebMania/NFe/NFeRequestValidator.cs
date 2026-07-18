using DFeHub.Exceptions;
using DFeHub.Validation;

namespace DFeHub.Providers.WebMania.NFe;

internal static class NFeRequestValidator
{
    internal static void Validate(NFeIssueRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var failures = new List<WebManiaValidationFailure>();

        Required(request.OrderId, "OrderId", 1, 15, failures);
        Required(request.OperationNature, "OperationNature", 1, 60, failures);
        Defined(request.Operation, "Operation", failures);
        Defined(request.Model, "Model", failures);
        Defined(request.Purpose, "Purpose", failures);
        Defined(request.Environment, "Environment", failures);
        ValidateCustomer(request.Customer, "Customer", failures);

        if (request.Products is not { Count: >= 1 and <= 990 })
        {
            Add(
                failures,
                "Products",
                "At least one and at most 990 products are required.",
                "NFE_PRODUCTS_COUNT");
        }
        else
        {
            for (var index = 0; index < request.Products.Count; index++)
            {
                ValidateProduct(
                    request.Products[index],
                    $"Products[{index}]",
                    failures);
            }
        }

        if (request.Order is null)
        {
            Add(
                failures,
                "Order",
                "Order information is required.",
                "NFE_ORDER_REQUIRED");
        }
        else if (request.Order.Total < 0)
        {
            Add(
                failures,
                "Order.Total",
                "The order total cannot be negative.",
                "NFE_TOTAL_NEGATIVE");
        }

        if (request.Purpose == NFePurpose.Credit &&
            request.CreditType is not (>= 1 and <= 5))
        {
            Add(
                failures,
                "CreditType",
                "CreditType must be between 1 and 5 for a credit note.",
                "NFE_CREDIT_TYPE");
        }

        if (request.Purpose == NFePurpose.Debit &&
            request.DebitType is not (>= 1 and <= 8))
        {
            Add(
                failures,
                "DebitType",
                "DebitType must be between 1 and 8 for a debit note.",
                "NFE_DEBIT_TYPE");
        }

        ValidateAgriculture(request.Agriculture, "Agriculture", failures);
        ThrowIfAny(failures);
    }

    internal static void Validate(NFeReturnRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var failures = new List<WebManiaValidationFailure>();
        Identifier(request.AccessKey, request.Uuid, failures);
        Required(request.OperationNature, "OperationNature", 1, 60, failures);
        Required(request.CfopCode, "CfopCode", 4, 5, failures);
        Defined(request.Environment, "Environment", failures);

        if (request.Quantities is not null &&
            (request.ProductNumbers is null ||
             request.Quantities.Count != request.ProductNumbers.Count))
        {
            Add(
                failures,
                "Quantities",
                "Quantities must match the selected product count.",
                "NFE_RETURN_QUANTITIES");
        }

        ThrowIfAny(failures);
    }

    internal static void Validate(NFeAdjustmentRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var failures = new List<WebManiaValidationFailure>();
        Defined(request.Operation, "Operation", failures);
        Defined(request.Environment, "Environment", failures);
        Required(request.OperationNature, "OperationNature", 1, 60, failures);
        Required(request.CfopCode, "CfopCode", 4, 5, failures);
        ValidateCustomer(request.Customer, "Customer", failures);
        NonNegative(request.IcmsValue, "IcmsValue", failures);
        ThrowIfAny(failures);
    }

    internal static void Validate(NFeComplementRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var failures = new List<WebManiaValidationFailure>();
        Identifier(request.AccessKey, request.Uuid, failures);
        Defined(request.Operation, "Operation", failures);
        Defined(request.Environment, "Environment", failures);
        Required(request.OperationNature, "OperationNature", 1, 60, failures);
        ValidateCustomer(request.Customer, "Customer", failures);

        if (request.Products is not { Count: > 0 } && request.Taxes is null)
        {
            Add(
                failures,
                "Products",
                "Products or invoice-level taxes must be supplied.",
                "NFE_COMPLEMENT_CONTENT");
        }

        ValidateAgriculture(request.Agriculture, "Agriculture", failures);
        ThrowIfAny(failures);
    }

    internal static void Validate(NFeCorrectionLetterRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var failures = new List<WebManiaValidationFailure>();
        Identifier(request.AccessKey, request.Uuid, failures);
        Required(request.Correction, "Correction", 15, 1000, failures);
        Defined(request.Environment, "Environment", failures);
        Range(request.EventSequence, 1, 20, "EventSequence", failures);
        ThrowIfAny(failures);
    }

    internal static void Validate(NFeRecipientManifestationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var failures = new List<WebManiaValidationFailure>();
        AccessKey(request.AccessKey, "AccessKey", failures);
        Defined(request.Environment, "Environment", failures);
        Defined(request.Manifestation, "Manifestation", failures);

        if (request.Manifestation ==
                NFeRecipientManifestation.OperationNotPerformed)
        {
            Required(
                request.Justification,
                "Justification",
                15,
                255,
                failures);
        }

        ThrowIfAny(failures);
    }

    internal static void Validate(NFeIbsCbsEventRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var failures = new List<WebManiaValidationFailure>();
        AccessKey(request.AccessKey, "AccessKey", failures);
        Defined(request.Environment, "Environment", failures);
        Range(request.EventSequence, 1, 20, "EventSequence", failures);

        var requiresItems = request.EventCode is
            NFeIbsCbsEventCodes.ImportNotConvertedToExemption or
            NFeIbsCbsEventCodes.SupplierTransportLoss or
            NFeIbsCbsEventCodes.SupplyNotPerformed or
            NFeIbsCbsEventCodes.PresumedCreditRequest or
            NFeIbsCbsEventCodes.PersonalConsumption or
            NFeIbsCbsEventCodes.BuyerTransportLoss or
            NFeIbsCbsEventCodes.FixedAsset or
            NFeIbsCbsEventCodes.FuelCreditRequest or
            NFeIbsCbsEventCodes.ActivityCreditRequest;

        if (!IsKnownIbsCbsEvent(request.EventCode))
        {
            Add(
                failures,
                "EventCode",
                "EventCode is not an official IBS/CBS event.",
                "NFE_IBS_CBS_EVENT");
        }
        else if (requiresItems && request.Items is not { Count: > 0 })
        {
            Add(
                failures,
                "Items",
                "The selected event requires at least one item.",
                "NFE_IBS_CBS_ITEMS");
        }

        if (request.EventCode ==
                NFeIbsCbsEventCodes.ExpectedDeliveryDateUpdate &&
            request.ExpectedDeliveryDate is null)
        {
            Add(
                failures,
                "ExpectedDeliveryDate",
                "The expected delivery date is required for event 112150.",
                "NFE_IBS_CBS_DELIVERY_DATE");
        }

        if (request.EventCode == NFeIbsCbsEventCodes.DebitAcceptance &&
            request.AcceptanceIndicator is not (0 or 1))
        {
            Add(
                failures,
                "AcceptanceIndicator",
                "AcceptanceIndicator must be 0 or 1 for event 211128.",
                "NFE_IBS_CBS_ACCEPTANCE");
        }

        ThrowIfAny(failures);
    }

    internal static void Validate(NFeIbsCbsEventCancellationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request.Uuid == Guid.Empty)
        {
            ThrowSingle("Uuid", "A non-empty UUID is required.", "NFE_UUID");
        }
    }

    internal static void Validate(NFeFinancialReconciliationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var failures = new List<WebManiaValidationFailure>();
        Identifier(request.AccessKey, request.Uuid, failures);
        Range(request.EventSequence, 1, 99, "EventSequence", failures);

        var hasSingle = request.Payment is not null;
        var hasMany = request.Payments is { Count: > 0 };
        if (hasSingle == hasMany)
        {
            Add(
                failures,
                "Payment",
                "Supply either Payment or Payments, but not both.",
                "NFE_ECONF_PAYMENT");
        }
        else if (request.Payments is { Count: > 100 })
        {
            Add(
                failures,
                "Payments",
                "At most 100 payments may be supplied.",
                "NFE_ECONF_PAYMENT_COUNT");
        }

        ThrowIfAny(failures);
    }

    internal static void Validate(
        NFeFinancialReconciliationCancellationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request.Uuid == Guid.Empty)
        {
            ThrowSingle("Uuid", "A non-empty UUID is required.", "NFE_UUID");
        }
    }

    internal static void Validate(NFeCancellationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var failures = new List<WebManiaValidationFailure>();
        Identifier(request.AccessKey, request.Uuid, failures);
        Required(request.Reason, "Reason", 15, 255, failures);
        if (request.ReplacementNFCeAccessKey is not null)
        {
            AccessKey(
                request.ReplacementNFCeAccessKey,
                "ReplacementNFCeAccessKey",
                failures);
        }

        ThrowIfAny(failures);
    }

    internal static void Validate(NFeNumberInvalidationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var failures = new List<WebManiaValidationFailure>();
        Defined(request.Environment, "Environment", failures);
        Defined(request.Model, "Model", failures);
        Required(request.Sequence, "Sequence", 3, 19, failures);
        Required(request.Reason, "Reason", 15, 255, failures);
        Required(request.Series, "Series", 1, 3, failures);

        if (!string.IsNullOrWhiteSpace(request.Sequence) &&
            !System.Text.RegularExpressions.Regex.IsMatch(
                request.Sequence,
                @"^\d{1,9}-\d{1,9}$",
                System.Text.RegularExpressions.RegexOptions.CultureInvariant))
        {
            Add(
                failures,
                "Sequence",
                "Sequence must use the first-last numeric format.",
                "NFE_SEQUENCE_FORMAT");
        }

        ThrowIfAny(failures);
    }

    internal static void Validate(NFeTaxClassUpsertRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var failures = new List<WebManiaValidationFailure>();
        Required(request.Description, "Description", 1, 200, failures);
        Collection(request.Icms, "Icms", failures);
        Collection(request.Ipi, "Ipi", failures);
        Collection(request.Pis, "Pis", failures);
        Collection(request.Cofins, "Cofins", failures);
        ThrowIfAny(failures);
    }

    internal static void Validate(NFeTaxClassDeleteRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request.References is not { Count: > 0 })
        {
            ThrowSingle(
                "References",
                "At least one tax class reference is required.",
                "NFE_TAX_CLASS_REFERENCES");
        }
    }

    internal static void Validate(NFeReportRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var failures = new List<WebManiaValidationFailure>();
        Required(request.Model, "Model", 3, 5, failures);
        Required(request.Report, "Report", 3, 20, failures);
        if (request.EndDate < request.StartDate)
        {
            Add(
                failures,
                "EndDate",
                "EndDate cannot precede StartDate.",
                "NFE_REPORT_DATE_RANGE");
        }

        ThrowIfAny(failures);
    }

    internal static void Validate(NFeCompanyUpdateRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request == new NFeCompanyUpdateRequest())
        {
            ThrowSingle(
                string.Empty,
                "At least one company field must be supplied.",
                "NFE_COMPANY_EMPTY");
        }
    }

    internal static void ValidateAccessKey(string accessKey)
    {
        var failures = new List<WebManiaValidationFailure>();
        AccessKey(accessKey, "accessKey", failures);
        ThrowIfAny(failures);
    }

    internal static void ValidateOrderId(string orderId)
    {
        var failures = new List<WebManiaValidationFailure>();
        Required(orderId, "orderId", 1, 15, failures);
        ThrowIfAny(failures);
    }

    internal static void ValidateUuid(Guid uuid)
    {
        if (uuid == Guid.Empty)
        {
            ThrowSingle("uuid", "A non-empty UUID is required.", "NFE_UUID");
        }
    }

    internal static void ValidateDownloadUri(Uri documentUrl)
    {
        ArgumentNullException.ThrowIfNull(documentUrl);
        var isOfficialHost =
            documentUrl.IsAbsoluteUri &&
            (documentUrl.IdnHost.Equals(
                 "webmania.com.br",
                 StringComparison.OrdinalIgnoreCase) ||
             documentUrl.IdnHost.EndsWith(
                 ".webmania.com.br",
                 StringComparison.OrdinalIgnoreCase));

        if (!documentUrl.IsAbsoluteUri ||
            documentUrl.Scheme != Uri.UriSchemeHttps ||
            !isOfficialHost)
        {
            ThrowSingle(
                "documentUrl",
                "Authenticated downloads require an HTTPS Webmania host.",
                "NFE_DOWNLOAD_HOST");
        }
    }

    private static void ValidateCustomer(
        NFeCustomer? customer,
        string path,
        List<WebManiaValidationFailure> failures)
    {
        if (customer is null)
        {
            Add(
                failures,
                path,
                "Customer information is required.",
                "NFE_CUSTOMER_REQUIRED");
            return;
        }

        var identities = new[]
        {
            customer.Cpf,
            customer.Cnpj,
            customer.ForeignId,
        }.Count(value => !string.IsNullOrWhiteSpace(value));

        if (identities != 1)
        {
            Add(
                failures,
                path,
                "Exactly one customer identity must be supplied.",
                "NFE_CUSTOMER_IDENTITY");
        }
    }

    private static void ValidateProduct(
        NFeProduct product,
        string path,
        List<WebManiaValidationFailure> failures)
    {
        if (product is null)
        {
            Add(
                failures,
                path,
                "A product cannot be null.",
                "NFE_PRODUCT_NULL");
            return;
        }

        Required(product.Name, $"{path}.Name", 1, 120, failures);
        Required(product.Code, $"{path}.Code", 1, 60, failures);
        Required(product.Ncm, $"{path}.Ncm", 8, 10, failures);
        Required(product.Unit, $"{path}.Unit", 1, 6, failures);
        if (product.Quantity <= 0)
        {
            Add(
                failures,
                $"{path}.Quantity",
                "Product quantity must be greater than zero.",
                "NFE_PRODUCT_QUANTITY");
        }

        NonNegative(product.UnitPrice, $"{path}.UnitPrice", failures);
        NonNegative(product.Total, $"{path}.Total", failures);
    }

    private static void ValidateAgriculture(
        NFeAgriculture? agriculture,
        string path,
        List<WebManiaValidationFailure> failures)
    {
        if (agriculture?.Pesticides is { Count: > 0 } &&
            agriculture.TransitGuide is not null)
        {
            Add(
                failures,
                path,
                "Pesticides and TransitGuide cannot be supplied together.",
                "NFE_AGRICULTURE_EXCLUSIVE");
        }
    }

    private static bool IsKnownIbsCbsEvent(string? eventCode)
    {
        return eventCode is
            NFeIbsCbsEventCodes.FullPayment or
            NFeIbsCbsEventCodes.ImportNotConvertedToExemption or
            NFeIbsCbsEventCodes.SupplierTransportLoss or
            NFeIbsCbsEventCodes.SupplyNotPerformed or
            NFeIbsCbsEventCodes.ExpectedDeliveryDateUpdate or
            NFeIbsCbsEventCodes.PresumedCreditRequest or
            NFeIbsCbsEventCodes.PersonalConsumption or
            NFeIbsCbsEventCodes.BuyerTransportLoss or
            NFeIbsCbsEventCodes.DebitAcceptance or
            NFeIbsCbsEventCodes.FixedAsset or
            NFeIbsCbsEventCodes.FuelCreditRequest or
            NFeIbsCbsEventCodes.ActivityCreditRequest;
    }

    private static void Identifier(
        string? accessKey,
        Guid? uuid,
        List<WebManiaValidationFailure> failures)
    {
        if (string.IsNullOrWhiteSpace(accessKey) == (uuid is null))
        {
            Add(
                failures,
                "AccessKey",
                "Supply either AccessKey or Uuid, but not both.",
                "NFE_IDENTIFIER");
            return;
        }

        if (accessKey is not null)
        {
            AccessKey(accessKey, "AccessKey", failures);
        }
        else if (uuid == Guid.Empty)
        {
            Add(
                failures,
                "Uuid",
                "A non-empty UUID is required.",
                "NFE_UUID");
        }
    }

    private static void AccessKey(
        string? value,
        string path,
        List<WebManiaValidationFailure> failures)
    {
        if (value is null ||
            value.Length != 44 ||
            value.Any(character => !char.IsAsciiDigit(character)))
        {
            Add(
                failures,
                path,
                "An access key must contain exactly 44 digits.",
                "NFE_ACCESS_KEY");
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
                "NFE_STRING_LENGTH");
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
                "NFE_ENUM");
        }
    }

    private static void Collection<T>(
        IReadOnlyList<T>? value,
        string path,
        List<WebManiaValidationFailure> failures)
    {
        if (value is not { Count: > 0 })
        {
            Add(
                failures,
                path,
                "At least one scenario is required.",
                "NFE_TAX_CLASS_SCENARIO");
        }
    }

    private static void Range(
        int value,
        int minimum,
        int maximum,
        string path,
        List<WebManiaValidationFailure> failures)
    {
        if (value < minimum || value > maximum)
        {
            Add(
                failures,
                path,
                $"The value must be between {minimum} and {maximum}.",
                "NFE_RANGE");
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
                "NFE_NEGATIVE");
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
