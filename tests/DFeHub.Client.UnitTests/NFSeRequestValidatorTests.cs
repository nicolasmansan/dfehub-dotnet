using DFeHub.Exceptions;
using DFeHub.Providers.WebMania.NFSe;

namespace DFeHub.Client.UnitTests;

public sealed class NFSeRequestValidatorTests
{
    private const string AccessKey =
        "00000000000000000000000000000000000000000000000000";

    [Fact]
    public void Issue_accepts_tax_class_and_manual_tax_shapes()
    {
        NFSeRequestValidator.Validate(ValidIssue());
        NFSeRequestValidator.Validate(
            ValidIssue() with
            {
                Rps =
                [
                    Rps() with
                    {
                        Service = Service() with
                        {
                            TaxClass = null,
                            Taxes = new NFSeTaxes { Iss = 2.00m },
                        },
                    },
                ],
            });
        NFSeRequestValidator.Validate(
            ValidIssue() with
            {
                Rps =
                [
                    Rps() with
                    {
                        Service = Service() with
                        {
                            TaxClass = null,
                            IssTaxation = 1,
                            IssWithheld = 2,
                        },
                    },
                ],
            });
    }

    [Fact]
    public void Issue_collects_environment_identifier_and_count_failures()
    {
        var exception = Invalid(
            () => NFSeRequestValidator.Validate(
                ValidIssue() with
                {
                    OrderId = string.Empty,
                    Environment = (NFSeEnvironment)99,
                    Rps = [],
                }));

        AssertCodes(
            exception,
            "NFSE_ENUM",
            "NFSE_STRING_LENGTH",
            "NFSE_RPS_COUNT");
    }

    [Fact]
    public void Issue_validates_service_and_exclusive_tax_configuration()
    {
        var exception = Invalid(
            () => NFSeRequestValidator.Validate(
                ValidIssue() with
                {
                    Rps =
                    [
                        Rps() with
                        {
                            Service = Service() with
                            {
                                Discrimination = string.Empty,
                                ServiceValue = -1,
                                Taxes = new NFSeTaxes { Iss = 2.00m },
                            },
                        },
                        new NFSeRps { Service = null! },
                    ],
                }));

        AssertCodes(
            exception,
            "NFSE_STRING_LENGTH",
            "NFSE_NEGATIVE",
            "NFSE_TAX_CONFIGURATION",
            "NFSE_SERVICE_REQUIRED");

        AssertCodes(
            Invalid(
                () => NFSeRequestValidator.Validate(
                    ValidIssue() with
                    {
                        Rps = [Rps() with { Service = Service() with
                            {
                                TaxClass = null,
                            } }],
                    })),
            "NFSE_TAX_CONFIGURATION");

        AssertCodes(
            Invalid(
                () => NFSeRequestValidator.Validate(
                    ValidIssue() with
                    {
                        Rps =
                        [
                            Rps() with
                            {
                                Service = Service() with
                                {
                                    TaxClass = null,
                                    Taxes = new NFSeTaxes(),
                                },
                            },
                        ],
                    })),
            "NFSE_TAXES_EMPTY",
            "NFSE_TAX_CONFIGURATION");
    }

    [Fact]
    public void Issue_rejects_null_rps_and_more_than_fifty_entries()
    {
        AssertCodes(
            Invalid(
                () => NFSeRequestValidator.Validate(
                    ValidIssue() with { Rps = [null!] })),
            "NFSE_RPS_REQUIRED");

        AssertCodes(
            Invalid(
                () => NFSeRequestValidator.Validate(
                    ValidIssue() with
                    {
                        Rps = Enumerable.Repeat(Rps(), 51).ToArray(),
                    })),
            "NFSE_RPS_COUNT");
    }

    [Fact]
    public void Issue_validates_schedule_as_minute_precision_wall_clock()
    {
        NFSeRequestValidator.Validate(
            ValidIssue() with
            {
                ScheduledAt = new DateTime(
                    2026,
                    7,
                    20,
                    14,
                    30,
                    0,
                    DateTimeKind.Unspecified),
            });

        var exception = Invalid(
            () => NFSeRequestValidator.Validate(
                ValidIssue() with
                {
                    ScheduledAt = new DateTime(
                        2026,
                        7,
                        20,
                        14,
                        30,
                        1,
                        DateTimeKind.Utc),
                }));

        AssertCodes(
            exception,
            "NFSE_SCHEDULE_KIND",
            "NFSE_SCHEDULE_PRECISION");
    }

    [Fact]
    public void Party_requires_one_identity_and_its_corresponding_name()
    {
        NFSeRequestValidator.Validate(ValidIssue());
        NFSeRequestValidator.Validate(
            ValidIssue() with
            {
                Rps =
                [
                    Rps() with
                    {
                        Taker = new NFSeParty
                        {
                            ForeignId = "PASSPORT-1",
                            ForeignName = "Foreign customer",
                        },
                    },
                ],
            });

        var exception = Invalid(
            () => NFSeRequestValidator.Validate(
                ValidIssue() with
                {
                    Rps =
                    [
                        Rps() with
                        {
                            Taker = new NFSeParty
                            {
                                Cpf = "12345678909",
                                Cnpj = "12345678000190",
                            },
                        },
                    ],
                }));

        AssertCodes(exception, "NFSE_PARTY_IDENTITY");
    }

    [Fact]
    public void Recipient_requires_matching_indicator_and_one_address_group()
    {
        NFSeRequestValidator.Validate(
            ValidIssue() with
            {
                Rps =
                [
                    Rps() with
                    {
                        Service = Service() with { RecipientIndicator = 1 },
                        Recipient = new NFSeRecipient
                        {
                            Cnpj = "12345678000190",
                            Name = "Filial de Homologação",
                            Address = new NFSeStructuredAddress
                            {
                                Street = "Rua Fiscal",
                                NationalAddress = new NFSeNationalAddress
                                {
                                    IbgeCityCode = "3550308",
                                    PostalCode = "01310100",
                                },
                            },
                        },
                    },
                ],
            });

        var exception = Invalid(
            () => NFSeRequestValidator.Validate(
                ValidIssue() with
                {
                    Rps =
                    [
                        Rps() with
                        {
                            Service = Service() with
                            {
                                RecipientIndicator = 0,
                            },
                            Recipient = new NFSeRecipient
                            {
                                Cpf = "12345678909",
                                Name = "Destinatário",
                                Address = new NFSeStructuredAddress(),
                            },
                        },
                    ],
                }));

        AssertCodes(
            exception,
            "NFSE_RECIPIENT_UNEXPECTED",
            "NFSE_RECIPIENT_ADDRESS");
    }

    [Fact]
    public void National_groups_enforce_documented_union_shapes()
    {
        NFSeRequestValidator.Validate(
            ValidIssue() with
            {
                Rps =
                [
                    Rps() with
                    {
                        Service = Service() with
                        {
                            OperationIndicatorCode = "020301",
                            RealEstate = new NFSeRealEstate
                            {
                                PropertyRegistration = "123456",
                            },
                        },
                    },
                ],
            });

        var exception = Invalid(
            () => NFSeRequestValidator.Validate(
                ValidIssue() with
                {
                    Rps =
                    [
                        Rps() with
                        {
                            Service = Service() with
                            {
                                GovernmentEntity =
                                    new NFSeGovernmentEntity { Type = 9 },
                                DeductionReduction =
                                    new NFSeDeductionReduction
                                    {
                                        Value = 10,
                                        Percentage = 5,
                                    },
                                Reimbursement = new NFSeReimbursement
                                {
                                    Documents = [],
                                },
                                OperationIndicatorCode = "020301",
                                RealEstate = new NFSeRealEstate
                                {
                                    PropertyRegistration = "123456",
                                    CibCode = "12345678",
                                },
                            },
                            Construction = new NFSeConstruction
                            {
                                WorkCode = "OBRA-1",
                                FiscalPropertyRegistration = "IMOVEL-1",
                            },
                        },
                    ],
                }));

        AssertCodes(
            exception,
            "NFSE_STRING_LENGTH",
            "NFSE_DEDUCTION_OPTION",
            "NFSE_REIMBURSEMENT_DOCUMENTS",
            "NFSE_REAL_ESTATE_FORM",
            "NFSE_REAL_ESTATE_UNEXPECTED",
            "NFSE_CONSTRUCTION_FORM");
    }

    [Fact]
    public void Deep_document_groups_accept_valid_shapes()
    {
        NFSeRequestValidator.Validate(
            ValidIssue() with
            {
                Rps =
                [
                    Rps() with
                    {
                        Service = Service() with
                        {
                            GovernmentEntity = new NFSeGovernmentEntity
                            {
                                Type = 1,
                                Classification = "Administração direta",
                            },
                            Reimbursement = new NFSeReimbursement
                            {
                                Documents =
                                [
                                    new NFSeReimbursementDocument
                                    {
                                        ReimbursementType = 1,
                                        Description = "Reembolso",
                                        Value = 10,
                                        IssuedOn = new DateOnly(2026, 7, 1),
                                        CompetenceDate =
                                            new DateOnly(2026, 7, 1),
                                        NationalFiscalDocument =
                                            new NFSeNationalFiscalDocument
                                            {
                                                KeyType = 1,
                                                AccessKey = "CHAVE",
                                            },
                                    },
                                ],
                            },
                            DeductionReduction = new NFSeDeductionReduction
                            {
                                Documents =
                                [
                                    new NFSeDeductionDocument
                                    {
                                        NFSeAccessKey = "CHAVE",
                                        DeductionType = 1,
                                        IssuedOn = new DateOnly(2026, 7, 1),
                                        DeductibleValue = 10,
                                        DeductionValue = 5,
                                    },
                                ],
                            },
                        },
                    },
                ],
            });
    }

    [Fact]
    public void Deep_document_groups_reject_ambiguous_and_invalid_items()
    {
        var exception = Invalid(
            () => NFSeRequestValidator.Validate(
                ValidIssue() with
                {
                    Rps =
                    [
                        Rps() with
                        {
                            Service = Service() with
                            {
                                GovernmentEntity =
                                    new NFSeGovernmentEntity { Type = 0 },
                                Reimbursement = new NFSeReimbursement
                                {
                                    Documents =
                                    [
                                        null!,
                                        new NFSeReimbursementDocument
                                        {
                                            ReimbursementType = 99,
                                            Value = -1,
                                            IssuedOn =
                                                new DateOnly(2026, 7, 1),
                                            CompetenceDate =
                                                new DateOnly(2026, 7, 1),
                                        },
                                        new NFSeReimbursementDocument
                                        {
                                            ReimbursementType = 1,
                                            Value = 1,
                                            IssuedOn =
                                                new DateOnly(2026, 7, 1),
                                            CompetenceDate =
                                                new DateOnly(2026, 7, 1),
                                            NationalFiscalDocument =
                                                new NFSeNationalFiscalDocument
                                                {
                                                    KeyType = 1,
                                                    AccessKey = "A",
                                                },
                                            OtherDocument =
                                                new NFSeOtherDocument
                                                {
                                                    DocumentNumber = "1",
                                                    DocumentDescription =
                                                        "Outro",
                                                },
                                        },
                                    ],
                                },
                                DeductionReduction =
                                    new NFSeDeductionReduction
                                    {
                                        Documents =
                                        [
                                            null!,
                                            new NFSeDeductionDocument
                                            {
                                                NFSeAccessKey = "A",
                                                NFeAccessKey = "B",
                                                DeductionType = 0,
                                                IssuedOn =
                                                    new DateOnly(2026, 7, 1),
                                                DeductibleValue = -1,
                                                DeductionValue = 2,
                                            },
                                            new NFSeDeductionDocument
                                            {
                                                FiscalDocument = "DOC",
                                                DeductionType = 99,
                                                IssuedOn =
                                                    new DateOnly(2026, 7, 1),
                                                DeductibleValue = 5,
                                                DeductionValue = 6,
                                            },
                                        ],
                                    },
                            },
                        },
                    ],
                }));

        AssertCodes(
            exception,
            "NFSE_GOVERNMENT_ENTITY_TYPE",
            "NFSE_REIMBURSEMENT_DOCUMENT",
            "NFSE_STRING_LENGTH",
            "NFSE_NEGATIVE",
            "NFSE_DEDUCTION_DOCUMENT",
            "NFSE_DEDUCTION_TYPE",
            "NFSE_DEDUCTION_VALUE");
    }

    [Fact]
    public void Real_estate_and_construction_cover_all_structural_forms()
    {
        NFSeRequestValidator.Validate(
            ValidIssue() with
            {
                Rps =
                [
                    Rps() with
                    {
                        Service = Service() with
                        {
                            OperationIndicatorCode = "020101",
                            RealEstate = new NFSeRealEstate
                            {
                                Address = "Rua Fiscal",
                                Number = "100",
                                District = "Centro",
                                PostalCode = "01310100",
                            },
                        },
                    },
                    Rps() with
                    {
                        Service = Service() with
                        {
                            OperationIndicatorCode = "020201",
                            RealEstate = new NFSeRealEstate
                            {
                                Address = new NFSeStructuredAddress
                                {
                                    Street = "Rua Fiscal",
                                    ForeignAddress = new NFSeForeignAddress
                                    {
                                        CountryCode = "2496",
                                        City = "New York",
                                    },
                                },
                            },
                        },
                    },
                    Rps() with
                    {
                        Construction = new NFSeConstruction
                        {
                            Address = "Rua da Obra",
                            Number = "10",
                            District = "Centro",
                            PostalCode = "01310100",
                            WorkLocationState = "SP",
                            WorkLocationCity = "São Paulo",
                        },
                    },
                ],
            });

        var exception = Invalid(
            () => NFSeRequestValidator.Validate(
                ValidIssue() with
                {
                    Rps =
                    [
                        Rps() with
                        {
                            Service = Service() with
                            {
                                OperationIndicatorCode = "020101",
                                RealEstate = new NFSeRealEstate
                                {
                                    Address =
                                        new NFSeRealEstateAddress(),
                                },
                            },
                        },
                        Rps() with
                        {
                            Construction = new NFSeConstruction(),
                        },
                        Rps() with
                        {
                            Construction = new NFSeConstruction
                            {
                                Address = "Rua incompleta",
                                WorkLocationState = "SP",
                            },
                        },
                    ],
                }));

        AssertCodes(
            exception,
            "NFSE_REAL_ESTATE_FORM",
            "NFSE_REAL_ESTATE_ADDRESS",
            "NFSE_CONSTRUCTION_EMPTY",
            "NFSE_CONSTRUCTION_ADDRESS",
            "NFSE_CONSTRUCTION_LOCATION");
    }

    [Fact]
    public void Cancellation_and_substitution_validate_fiscal_identifiers()
    {
        NFSeRequestValidator.Validate(
            new NFSeCancellationRequest
            {
                Uuid = Guid.NewGuid(),
                Reason = NFSeCancellationReason.IssuanceError,
            });
        NFSeRequestValidator.Validate(
            new NFSeSubstitutionRequest
            {
                Environment = NFSeEnvironment.Certification,
                VerificationCode = "ABC-123",
                Reason = NFSeCancellationReason.DuplicateInvoice,
                Rps = Rps(),
            });

        AssertCodes(
            Invalid(
                () => NFSeRequestValidator.Validate(
                    new NFSeCancellationRequest
                    {
                        Uuid = Guid.Empty,
                        Reason = (NFSeCancellationReason)3,
                    })),
            "NFSE_UUID",
            "NFSE_ENUM");
        AssertCodes(
            Invalid(
                () => NFSeRequestValidator.Validate(
                    new NFSeSubstitutionRequest
                    {
                        Environment = (NFSeEnvironment)0,
                        VerificationCode = string.Empty,
                        Reason = (NFSeCancellationReason)3,
                        Rps = null!,
                    })),
            "NFSE_ENUM",
            "NFSE_VERIFICATION_CODE",
            "NFSE_RPS_REQUIRED");
    }

    [Fact]
    public void Manifestation_validates_identifier_and_rejection_fields()
    {
        NFSeRequestValidator.Validate(
            new NFSeManifestationRequest
            {
                Environment = NFSeEnvironment.Certification,
                AccessKey = AccessKey,
                ManifestingParty = NFSeManifestingParty.Taker,
                Event = NFSeManifestationEvent.Confirmation,
            });
        NFSeRequestValidator.Validate(
            new NFSeManifestationRequest
            {
                Environment = NFSeEnvironment.Production,
                Uuid = Guid.NewGuid(),
                ManifestingParty = NFSeManifestingParty.Intermediary,
                Event = NFSeManifestationEvent.Rejection,
                RejectionReason = NFSeManifestationRejectionReason.Other,
                RejectionJustification =
                    "Participação rejeitada pelo intermediário.",
            });

        AssertCodes(
            Invalid(
                () => NFSeRequestValidator.Validate(
                    new NFSeManifestationRequest
                    {
                        Environment = NFSeEnvironment.Certification,
                        AccessKey = "123",
                        ManifestingParty = (NFSeManifestingParty)0,
                        Event = NFSeManifestationEvent.Rejection,
                    })),
            "NFSE_ACCESS_KEY",
            "NFSE_ENUM",
            "NFSE_REJECTION_REASON");
        AssertCodes(
            Invalid(
                () => NFSeRequestValidator.Validate(
                    new NFSeManifestationRequest
                    {
                        Environment = NFSeEnvironment.Certification,
                        Uuid = Guid.NewGuid(),
                        ManifestingParty = NFSeManifestingParty.Taker,
                        Event = NFSeManifestationEvent.Confirmation,
                        RejectionReason =
                            NFSeManifestationRejectionReason.DuplicateInvoice,
                    })),
            "NFSE_CONFIRMATION_REJECTION_FIELDS");
    }

    [Fact]
    public void Query_and_download_identifiers_are_validated()
    {
        NFSeRequestValidator.ValidateUuid(Guid.NewGuid());
        NFSeRequestValidator.ValidateDownloadUri(
            new Uri(
                $"https://api.webmania.com.br/nfse/{Guid.NewGuid():D}"));

        AssertCodes(
            Invalid(() => NFSeRequestValidator.ValidateUuid(Guid.Empty)),
            "NFSE_UUID");
        AssertCodes(
            Invalid(
                () => NFSeRequestValidator.ValidateDownloadUri(
                    new Uri("http://api.webmania.com.br/nfse/documento"))),
            "NFSE_DOWNLOAD_HOST");
        AssertCodes(
            Invalid(
                () => NFSeRequestValidator.ValidateDownloadUri(
                    new Uri(
                        $"https://files.webmania.com.br/nfse/" +
                        $"{Guid.NewGuid():D}"))),
            "NFSE_DOWNLOAD_HOST");
        AssertCodes(
            Invalid(
                () => NFSeRequestValidator.ValidateDownloadUri(
                    new Uri(
                        $"https://api.webmania.com.br/outro/" +
                        $"{Guid.NewGuid():D}"))),
            "NFSE_DOWNLOAD_HOST");
        Assert.Throws<ArgumentNullException>(
            () => NFSeRequestValidator.ValidateDownloadUri(null!));
    }

    private static NFSeIssueRequest ValidIssue()
    {
        return new NFSeIssueRequest
        {
            OrderId = "PEDIDO-1",
            Environment = NFSeEnvironment.Certification,
            Rps = [Rps()],
        };
    }

    private static NFSeRps Rps()
    {
        return new NFSeRps
        {
            Service = Service(),
            Taker = new NFSeParty
            {
                Cnpj = "12345678000190",
                CorporateName = "Empresa de Homologação",
            },
        };
    }

    private static NFSeService Service()
    {
        return new NFSeService
        {
            Discrimination = "Licenciamento de software",
            ServiceValue = 150.00m,
            TaxClass = "REF000000",
        };
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
}
