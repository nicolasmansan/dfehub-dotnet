namespace DFeHub.Providers.WebMania;

/// <summary>
/// Provides a single entry point for all Webmania product clients.
/// </summary>
public interface IWebManiaClient
{
    /// <summary>
    /// Gets the NF-e, NFC-e, CC-e and MDe client.
    /// </summary>
    INFeClient NFe { get; }

    /// <summary>
    /// Gets the NFS-e client.
    /// </summary>
    INFSeClient NFSe { get; }

    /// <summary>
    /// Gets the CT-e and CT-e OS client.
    /// </summary>
    ICTeClient CTe { get; }

    /// <summary>
    /// Gets the MDF-e client.
    /// </summary>
    IMDFeClient MDFe { get; }

    /// <summary>
    /// Gets the NFCom client.
    /// </summary>
    INFComClient NFCom { get; }

    /// <summary>
    /// Gets the DC-e client.
    /// </summary>
    IDCeClient DCe { get; }

    /// <summary>
    /// Gets the Monitor Fiscal client.
    /// </summary>
    IMonitorFiscalClient Monitor { get; }

    /// <summary>
    /// Gets the Validador Fiscal client.
    /// </summary>
    IValidatorFiscalClient Validator { get; }

    /// <summary>
    /// Gets the Gerencial B2B client.
    /// </summary>
    IB2BClient B2B { get; }
}

