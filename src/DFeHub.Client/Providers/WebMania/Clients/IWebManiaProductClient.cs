namespace DFeHub.Providers.WebMania;

/// <summary>
/// Describes a Webmania product client.
/// </summary>
public interface IWebManiaProductClient
{
    /// <summary>
    /// Gets a value indicating whether all credentials required by this product
    /// are currently configured.
    /// </summary>
    bool IsConfigured { get; }
}

/// <summary>
/// Provides access to NF-e, NFC-e, CC-e and MDe operations.
/// </summary>
public interface INFeClient : IWebManiaProductClient;

/// <summary>
/// Provides access to NFS-e operations.
/// </summary>
public interface INFSeClient : IWebManiaProductClient;

/// <summary>
/// Provides access to CT-e and CT-e OS operations.
/// </summary>
public interface ICTeClient : IWebManiaProductClient;

/// <summary>
/// Provides access to MDF-e operations.
/// </summary>
public interface IMDFeClient : IWebManiaProductClient;

/// <summary>
/// Provides access to NFCom operations.
/// </summary>
public interface INFComClient : IWebManiaProductClient;

/// <summary>
/// Provides access to DC-e operations.
/// </summary>
public interface IDCeClient : IWebManiaProductClient;

/// <summary>
/// Provides access to Monitor Fiscal operations.
/// </summary>
public interface IMonitorFiscalClient : IWebManiaProductClient;

/// <summary>
/// Provides access to Validador Fiscal operations.
/// </summary>
public interface IValidatorFiscalClient : IWebManiaProductClient;

/// <summary>
/// Provides access to Gerencial B2B operations.
/// </summary>
public interface IB2BClient : IWebManiaProductClient;

