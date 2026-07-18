using Microsoft.Extensions.Options;

namespace DFeHub.Providers.WebMania;

internal abstract class WebManiaProductClient(
    IOptionsMonitor<WebManiaOptions> options,
    WebManiaCredentialGroup credentialGroup)
    : IWebManiaProductClient
{
    public bool IsConfigured =>
        WebManiaCredentialValidator.IsConfigured(
            options.CurrentValue,
            credentialGroup);
}

internal sealed class NFSeClient(IOptionsMonitor<WebManiaOptions> options)
    : WebManiaProductClient(options, WebManiaCredentialGroup.ApiV2), INFSeClient;

internal sealed class CTeClient(IOptionsMonitor<WebManiaOptions> options)
    : WebManiaProductClient(options, WebManiaCredentialGroup.ApiV2), ICTeClient;

internal sealed class MDFeClient(IOptionsMonitor<WebManiaOptions> options)
    : WebManiaProductClient(options, WebManiaCredentialGroup.ApiV2), IMDFeClient;

internal sealed class NFComClient(IOptionsMonitor<WebManiaOptions> options)
    : WebManiaProductClient(options, WebManiaCredentialGroup.ApiV2), INFComClient;

internal sealed class DCeClient(IOptionsMonitor<WebManiaOptions> options)
    : WebManiaProductClient(options, WebManiaCredentialGroup.ApiV2), IDCeClient;

internal sealed class MonitorFiscalClient(
    IOptionsMonitor<WebManiaOptions> options)
    : WebManiaProductClient(options, WebManiaCredentialGroup.ApiV2),
        IMonitorFiscalClient;

internal sealed class ValidatorFiscalClient(
    IOptionsMonitor<WebManiaOptions> options)
    : WebManiaProductClient(options, WebManiaCredentialGroup.Validator),
        IValidatorFiscalClient;

internal sealed class B2BClient(IOptionsMonitor<WebManiaOptions> options)
    : WebManiaProductClient(options, WebManiaCredentialGroup.B2B), IB2BClient;
