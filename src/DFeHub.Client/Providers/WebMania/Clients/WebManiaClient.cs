namespace DFeHub.Providers.WebMania;

internal sealed class WebManiaClient(
    INFeClient nfe,
    INFSeClient nfse,
    ICTeClient cte,
    IMDFeClient mdfe,
    INFComClient nfcom,
    IDCeClient dce,
    IMonitorFiscalClient monitor,
    IValidatorFiscalClient validator,
    IB2BClient b2b)
    : IWebManiaClient
{
    public INFeClient NFe { get; } = nfe;

    public INFSeClient NFSe { get; } = nfse;

    public ICTeClient CTe { get; } = cte;

    public IMDFeClient MDFe { get; } = mdfe;

    public INFComClient NFCom { get; } = nfcom;

    public IDCeClient DCe { get; } = dce;

    public IMonitorFiscalClient Monitor { get; } = monitor;

    public IValidatorFiscalClient Validator { get; } = validator;

    public IB2BClient B2B { get; } = b2b;
}

