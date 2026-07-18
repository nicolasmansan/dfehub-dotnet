using DFeHub.Exceptions;
using DFeHub.Providers.WebMania.NFe;

namespace DFeHub.Client.UnitTests;

public sealed class NFeRequestValidatorTests
{
    private const string AccessKey =
        "35260712345678000190550010000001231000001234";

    [Fact]
    public void Issue_accepts_normal_credit_and_debit_purposes()
    {
        NFeRequestValidator.Validate(ValidIssue());
        NFeRequestValidator.Validate(
            ValidIssue() with
            {
                Purpose = NFePurpose.Credit,
                CreditType = 3,
            });
        NFeRequestValidator.Validate(
            ValidIssue() with
            {
                Purpose = NFePurpose.Debit,
                DebitType = 4,
            });
    }

    [Fact]
    public void Issue_collects_header_customer_content_and_purpose_failures()
    {
        var exception = Invalid(
            () => NFeRequestValidator.Validate(
                ValidIssue() with
                {
                    OrderId = string.Empty,
                    OperationNature = new string('x', 61),
                    Operation = (NFeOperation)99,
                    Model = (NFeModel)99,
                    Purpose = NFePurpose.Credit,
                    CreditType = null,
                    Environment = (NFeEnvironment)99,
                    Customer = null!,
                    Products = [],
                    Order = null!,
                    Agriculture = InvalidAgriculture(),
                }));

        AssertCodes(
            exception,
            "NFE_STRING_LENGTH",
            "NFE_ENUM",
            "NFE_CUSTOMER_REQUIRED",
            "NFE_PRODUCTS_COUNT",
            "NFE_ORDER_REQUIRED",
            "NFE_CREDIT_TYPE",
            "NFE_AGRICULTURE_EXCLUSIVE");
    }

    [Fact]
    public void Issue_rejects_invalid_products_order_and_debit_type()
    {
        var invalidProduct = Product() with
        {
            Name = string.Empty,
            Code = string.Empty,
            Ncm = "1",
            Unit = string.Empty,
            Quantity = 0,
            UnitPrice = -1,
            Total = -1,
        };
        var exception = Invalid(
            () => NFeRequestValidator.Validate(
                ValidIssue() with
                {
                    Purpose = NFePurpose.Debit,
                    DebitType = 9,
                    Products = [invalidProduct],
                    Order = Order() with { Total = -1 },
                }));

        AssertCodes(
            exception,
            "NFE_STRING_LENGTH",
            "NFE_PRODUCT_QUANTITY",
            "NFE_NEGATIVE",
            "NFE_TOTAL_NEGATIVE",
            "NFE_DEBIT_TYPE");
    }

    [Fact]
    public void Issue_rejects_null_product_and_more_than_990_products()
    {
        AssertCodes(
            Invalid(
                () => NFeRequestValidator.Validate(
                    ValidIssue() with { Products = [null!] })),
            "NFE_PRODUCT_NULL");

        AssertCodes(
            Invalid(
                () => NFeRequestValidator.Validate(
                    ValidIssue() with
                    {
                        Products = Enumerable
                            .Repeat(Product(), 991)
                            .ToArray(),
                    })),
            "NFE_PRODUCTS_COUNT");
    }

    [Fact]
    public void Issue_requires_exactly_one_customer_identity()
    {
        AssertCodes(
            Invalid(
                () => NFeRequestValidator.Validate(
                    ValidIssue() with
                    {
                        Customer = Customer() with { Cpf = null },
                    })),
            "NFE_CUSTOMER_IDENTITY");

        AssertCodes(
            Invalid(
                () => NFeRequestValidator.Validate(
                    ValidIssue() with
                    {
                        Customer = Customer() with
                        {
                            Cnpj = "12345678000190",
                        },
                    })),
            "NFE_CUSTOMER_IDENTITY");

        NFeRequestValidator.Validate(
            ValidIssue() with
            {
                Customer = Customer() with
                {
                    Cpf = null,
                    ForeignId = "FOREIGN-1",
                },
            });
    }

    [Fact]
    public void Return_validates_identifier_and_quantity_alignment()
    {
        var valid = new NFeReturnRequest
        {
            AccessKey = AccessKey,
            OperationNature = "Devolução",
            Environment = NFeEnvironment.Certification,
            CfopCode = "5202",
        };
        NFeRequestValidator.Validate(valid);
        NFeRequestValidator.Validate(
            valid with
            {
                AccessKey = null,
                Uuid = Guid.NewGuid(),
                ProductNumbers = [1, 2],
                Quantities = [1, 2],
            });

        AssertCodes(
            Invalid(
                () => NFeRequestValidator.Validate(
                    valid with
                    {
                        ProductNumbers = null,
                        Quantities = [1],
                    })),
            "NFE_RETURN_QUANTITIES");
        AssertCodes(
            Invalid(
                () => NFeRequestValidator.Validate(
                    valid with
                    {
                        ProductNumbers = [1],
                        Quantities = [1, 2],
                    })),
            "NFE_RETURN_QUANTITIES");
    }

    [Fact]
    public void Identifier_requires_one_nonempty_value()
    {
        var valid = new NFeReturnRequest
        {
            OperationNature = "Devolução",
            Environment = NFeEnvironment.Certification,
            CfopCode = "5202",
        };

        AssertCodes(
            Invalid(() => NFeRequestValidator.Validate(valid)),
            "NFE_IDENTIFIER");
        AssertCodes(
            Invalid(
                () => NFeRequestValidator.Validate(
                    valid with
                    {
                        AccessKey = AccessKey,
                        Uuid = Guid.NewGuid(),
                    })),
            "NFE_IDENTIFIER");
        AssertCodes(
            Invalid(
                () => NFeRequestValidator.Validate(
                    valid with { Uuid = Guid.Empty })),
            "NFE_UUID");
    }

    [Fact]
    public void Adjustment_and_complement_validate_fiscal_content()
    {
        NFeRequestValidator.Validate(
            new NFeAdjustmentRequest
            {
                Operation = NFeOperation.Outbound,
                OperationNature = "Ajuste",
                CfopCode = "5949",
                IcmsValue = 0,
                Environment = NFeEnvironment.Certification,
                Customer = Customer(),
            });

        AssertCodes(
            Invalid(
                () => NFeRequestValidator.Validate(
                    new NFeAdjustmentRequest
                    {
                        Operation = NFeOperation.Outbound,
                        OperationNature = "Ajuste",
                        CfopCode = "5949",
                        IcmsValue = -1,
                        Environment = NFeEnvironment.Certification,
                        Customer = Customer(),
                    })),
            "NFE_NEGATIVE");

        var complement = new NFeComplementRequest
        {
            AccessKey = AccessKey,
            Operation = NFeOperation.Outbound,
            OperationNature = "Complemento",
            Environment = NFeEnvironment.Certification,
            Customer = Customer(),
        };
        AssertCodes(
            Invalid(() => NFeRequestValidator.Validate(complement)),
            "NFE_COMPLEMENT_CONTENT");
        NFeRequestValidator.Validate(
            complement with
            {
                Products = [Product()],
            });
        NFeRequestValidator.Validate(
            complement with
            {
                Taxes = new NFeComplementTaxes { CfopCode = "5949" },
            });
    }

    [Fact]
    public void Correction_and_manifestation_validate_ranges_and_enums()
    {
        NFeRequestValidator.Validate(
            new NFeCorrectionLetterRequest
            {
                Uuid = Guid.NewGuid(),
                Correction = "Correção suficientemente longa.",
                Environment = NFeEnvironment.Certification,
                EventSequence = 20,
            });
        AssertCodes(
            Invalid(
                () => NFeRequestValidator.Validate(
                    new NFeCorrectionLetterRequest
                    {
                        AccessKey = AccessKey,
                        Correction = "Correção suficientemente longa.",
                        Environment = NFeEnvironment.Certification,
                        EventSequence = 21,
                    })),
            "NFE_RANGE");

        NFeRequestValidator.Validate(
            new NFeRecipientManifestationRequest
            {
                AccessKey = AccessKey,
                Environment = NFeEnvironment.Certification,
                Manifestation =
                    NFeRecipientManifestation.OperationNotPerformed,
                Justification = "Operação recusada pelo destinatário.",
            });
        AssertCodes(
            Invalid(
                () => NFeRequestValidator.Validate(
                    new NFeRecipientManifestationRequest
                    {
                        AccessKey = AccessKey,
                        Environment = NFeEnvironment.Certification,
                        Manifestation = (NFeRecipientManifestation)0,
                    })),
            "NFE_ENUM");
    }

    [Theory]
    [InlineData(NFeIbsCbsEventCodes.FullPayment, false)]
    [InlineData(NFeIbsCbsEventCodes.ImportNotConvertedToExemption, true)]
    [InlineData(NFeIbsCbsEventCodes.SupplierTransportLoss, true)]
    [InlineData(NFeIbsCbsEventCodes.SupplyNotPerformed, true)]
    [InlineData(NFeIbsCbsEventCodes.ExpectedDeliveryDateUpdate, false)]
    [InlineData(NFeIbsCbsEventCodes.PresumedCreditRequest, true)]
    [InlineData(NFeIbsCbsEventCodes.PersonalConsumption, true)]
    [InlineData(NFeIbsCbsEventCodes.BuyerTransportLoss, true)]
    [InlineData(NFeIbsCbsEventCodes.DebitAcceptance, false)]
    [InlineData(NFeIbsCbsEventCodes.FixedAsset, true)]
    [InlineData(NFeIbsCbsEventCodes.FuelCreditRequest, true)]
    [InlineData(NFeIbsCbsEventCodes.ActivityCreditRequest, true)]
    public void All_official_ibs_cbs_events_are_valid(
        string eventCode,
        bool requiresItems)
    {
        NFeRequestValidator.Validate(
            new NFeIbsCbsEventRequest
            {
                AccessKey = AccessKey,
                Environment = NFeEnvironment.Certification,
                EventCode = eventCode,
                Items = requiresItems
                    ? [new NFeIbsCbsEventItem { Item = 1 }]
                    : null,
                ExpectedDeliveryDate =
                    eventCode == NFeIbsCbsEventCodes.ExpectedDeliveryDateUpdate
                        ? new DateOnly(2026, 8, 1)
                        : null,
                AcceptanceIndicator =
                    eventCode == NFeIbsCbsEventCodes.DebitAcceptance
                        ? 1
                        : null,
            });
    }

    [Fact]
    public void Ibs_cbs_events_validate_conditional_fields()
    {
        var request = new NFeIbsCbsEventRequest
        {
            AccessKey = AccessKey,
            Environment = NFeEnvironment.Certification,
            EventCode = "999999",
        };
        AssertCodes(
            Invalid(() => NFeRequestValidator.Validate(request)),
            "NFE_IBS_CBS_EVENT");
        AssertCodes(
            Invalid(
                () => NFeRequestValidator.Validate(
                    request with
                    {
                        EventCode =
                            NFeIbsCbsEventCodes.SupplierTransportLoss,
                    })),
            "NFE_IBS_CBS_ITEMS");
        AssertCodes(
            Invalid(
                () => NFeRequestValidator.Validate(
                    request with
                    {
                        EventCode =
                            NFeIbsCbsEventCodes.ExpectedDeliveryDateUpdate,
                    })),
            "NFE_IBS_CBS_DELIVERY_DATE");
        AssertCodes(
            Invalid(
                () => NFeRequestValidator.Validate(
                    request with
                    {
                        EventCode = NFeIbsCbsEventCodes.DebitAcceptance,
                        AcceptanceIndicator = 2,
                        EventSequence = 0,
                    })),
            "NFE_IBS_CBS_ACCEPTANCE",
            "NFE_RANGE");
    }

    [Fact]
    public void Event_cancellations_require_nonempty_uuid()
    {
        NFeRequestValidator.Validate(
            new NFeIbsCbsEventCancellationRequest { Uuid = Guid.NewGuid() });
        NFeRequestValidator.Validate(
            new NFeFinancialReconciliationCancellationRequest
            {
                Uuid = Guid.NewGuid(),
            });

        AssertCodes(
            Invalid(
                () => NFeRequestValidator.Validate(
                    new NFeIbsCbsEventCancellationRequest
                    {
                        Uuid = Guid.Empty,
                    })),
            "NFE_UUID");
        AssertCodes(
            Invalid(
                () => NFeRequestValidator.Validate(
                    new NFeFinancialReconciliationCancellationRequest
                    {
                        Uuid = Guid.Empty,
                    })),
            "NFE_UUID");
    }

    [Fact]
    public void Financial_reconciliation_requires_exactly_one_payment_shape()
    {
        var payment = Payment();
        var valid = new NFeFinancialReconciliationRequest
        {
            AccessKey = AccessKey,
            Payment = payment,
        };
        NFeRequestValidator.Validate(valid);
        NFeRequestValidator.Validate(
            valid with
            {
                AccessKey = null,
                Uuid = Guid.NewGuid(),
                Payment = null,
                Payments = [payment],
            });

        AssertCodes(
            Invalid(
                () => NFeRequestValidator.Validate(
                    valid with { Payment = null })),
            "NFE_ECONF_PAYMENT");
        AssertCodes(
            Invalid(
                () => NFeRequestValidator.Validate(
                    valid with { Payments = [payment] })),
            "NFE_ECONF_PAYMENT");
        AssertCodes(
            Invalid(
                () => NFeRequestValidator.Validate(
                    valid with
                    {
                        Payment = null,
                        Payments = Enumerable
                            .Repeat(payment, 101)
                            .ToArray(),
                    })),
            "NFE_ECONF_PAYMENT_COUNT");
    }

    [Fact]
    public void Cancellation_validates_reason_and_replacement_key()
    {
        NFeRequestValidator.Validate(
            new NFeCancellationRequest
            {
                Uuid = Guid.NewGuid(),
                Reason = "Cancelamento solicitado pelo cliente.",
                ReplacementNFCeAccessKey = AccessKey,
            });

        AssertCodes(
            Invalid(
                () => NFeRequestValidator.Validate(
                    new NFeCancellationRequest
                    {
                        AccessKey = AccessKey,
                        Reason = "curto",
                        ReplacementNFCeAccessKey = "123",
                    })),
            "NFE_STRING_LENGTH",
            "NFE_ACCESS_KEY");
    }

    [Fact]
    public void Invalidation_validates_sequence_and_required_fields()
    {
        NFeRequestValidator.Validate(
            new NFeNumberInvalidationRequest
            {
                Sequence = "1-99",
                Reason = "Numeração não utilizada no sistema.",
                Environment = NFeEnvironment.Certification,
                Series = "1",
                Model = NFeModel.NFe,
            });

        var exception = Invalid(
            () => NFeRequestValidator.Validate(
                new NFeNumberInvalidationRequest
                {
                    Sequence = "invalid",
                    Reason = string.Empty,
                    Environment = (NFeEnvironment)0,
                    Series = string.Empty,
                    Model = (NFeModel)0,
                }));
        AssertCodes(
            exception,
            "NFE_SEQUENCE_FORMAT",
            "NFE_STRING_LENGTH",
            "NFE_ENUM");
    }

    [Fact]
    public void Tax_class_operations_validate_required_collections()
    {
        NFeRequestValidator.Validate(ValidTaxClass());
        NFeRequestValidator.Validate(
            new NFeTaxClassDeleteRequest { References = ["PADRAO"] });

        AssertCodes(
            Invalid(
                () => NFeRequestValidator.Validate(
                    ValidTaxClass() with
                    {
                        Description = string.Empty,
                        Icms = [],
                        Ipi = [],
                        Pis = [],
                        Cofins = [],
                    })),
            "NFE_STRING_LENGTH",
            "NFE_TAX_CLASS_SCENARIO");
        AssertCodes(
            Invalid(
                () => NFeRequestValidator.Validate(
                    new NFeTaxClassDeleteRequest { References = [] })),
            "NFE_TAX_CLASS_REFERENCES");
    }

    [Fact]
    public void Reports_and_company_updates_validate_request_shape()
    {
        NFeRequestValidator.Validate(
            new NFeReportRequest
            {
                StartDate = new DateOnly(2026, 7, 1),
                EndDate = new DateOnly(2026, 7, 18),
                Model = "nfe",
                Report = "xml",
            });
        AssertCodes(
            Invalid(
                () => NFeRequestValidator.Validate(
                    new NFeReportRequest
                    {
                        StartDate = new DateOnly(2026, 7, 18),
                        EndDate = new DateOnly(2026, 7, 1),
                        Model = string.Empty,
                        Report = string.Empty,
                    })),
            "NFE_REPORT_DATE_RANGE",
            "NFE_STRING_LENGTH");

        NFeRequestValidator.Validate(
            new NFeCompanyUpdateRequest { Email = "fiscal@example.test" });
        AssertCodes(
            Invalid(
                () => NFeRequestValidator.Validate(
                    new NFeCompanyUpdateRequest())),
            "NFE_COMPANY_EMPTY");
    }

    [Fact]
    public void Query_identifiers_are_validated()
    {
        NFeRequestValidator.ValidateAccessKey(AccessKey);
        NFeRequestValidator.ValidateOrderId("PEDIDO-1");
        NFeRequestValidator.ValidateUuid(Guid.NewGuid());

        AssertCodes(
            Invalid(() => NFeRequestValidator.ValidateAccessKey("44A")),
            "NFE_ACCESS_KEY");
        AssertCodes(
            Invalid(() => NFeRequestValidator.ValidateOrderId(string.Empty)),
            "NFE_STRING_LENGTH");
        AssertCodes(
            Invalid(() => NFeRequestValidator.ValidateUuid(Guid.Empty)),
            "NFE_UUID");
    }

    [Fact]
    public void Download_uri_handles_apex_subdomain_relative_and_null_values()
    {
        NFeRequestValidator.ValidateDownloadUri(
            new Uri("https://webmania.com.br/documento.xml"));
        NFeRequestValidator.ValidateDownloadUri(
            new Uri("https://NFE.WEBMANIA.COM.BR/documento.xml"));

        AssertCodes(
            Invalid(
                () => NFeRequestValidator.ValidateDownloadUri(
                    new Uri("documento.xml", UriKind.Relative))),
            "NFE_DOWNLOAD_HOST");
        Assert.Throws<ArgumentNullException>(
            () => NFeRequestValidator.ValidateDownloadUri(null!));
    }

    private static WebManiaValidationException Invalid(Action action)
    {
        return Assert.Throws<WebManiaValidationException>(action);
    }

    private static void AssertCodes(
        WebManiaValidationException exception,
        params string[] expected)
    {
        foreach (var code in expected)
        {
            Assert.Contains(
                exception.Failures,
                failure => failure.Code == code);
        }
    }

    private static NFeIssueRequest ValidIssue()
    {
        return new NFeIssueRequest
        {
            OrderId = "PEDIDO-1",
            Operation = NFeOperation.Outbound,
            OperationNature = "Venda",
            Model = NFeModel.NFe,
            Environment = NFeEnvironment.Certification,
            Customer = Customer(),
            Products = [Product()],
            Order = Order(),
        };
    }

    private static NFeCustomer Customer()
    {
        return new NFeCustomer
        {
            Cpf = "12345678909",
            FullName = "Cliente",
            Address = "Rua Fiscal",
            Number = "1",
            District = "Centro",
            City = "Curitiba",
            State = "PR",
        };
    }

    private static NFeProduct Product()
    {
        return new NFeProduct
        {
            Name = "Produto",
            Code = "SKU-1",
            Ncm = "61091000",
            Quantity = 1,
            Unit = "UN",
            Origin = 0,
            UnitPrice = 10,
            Total = 10,
        };
    }

    private static NFeOrder Order()
    {
        return new NFeOrder
        {
            BuyerPresence = 1,
            FreightMode = 9,
            Total = 10,
        };
    }

    private static NFeAgriculture InvalidAgriculture()
    {
        return new NFeAgriculture
        {
            Pesticides =
            [
                new NFePesticidePrescription
                {
                    Number = "1",
                    TechnicianCpf = "12345678909",
                },
            ],
            TransitGuide = new NFeTransitGuide
            {
                Type = 1,
                State = "PR",
                Number = "1",
            },
        };
    }

    private static NFeReconciliationPayment Payment()
    {
        return new NFeReconciliationPayment
        {
            PaymentIndicator = 0,
            PaymentMethod = "01",
            Value = 10,
            Date = new DateOnly(2026, 7, 18),
        };
    }

    private static NFeTaxClassUpsertRequest ValidTaxClass()
    {
        var contribution = new NFeTaxClassContribution
        {
            Scenario = "padrao",
            PersonType = "juridica",
            TaxSituation = "01",
        };
        return new NFeTaxClassUpsertRequest
        {
            Description = "Classe padrão",
            Icms =
            [
                new NFeTaxClassIcms
                {
                    TaxRegime = "normal",
                    Scenario = "padrao",
                    PersonType = "juridica",
                    CfopCode = "5102",
                    TaxSituation = "00",
                },
            ],
            Ipi = [contribution],
            Pis = [contribution],
            Cofins = [contribution],
        };
    }
}
