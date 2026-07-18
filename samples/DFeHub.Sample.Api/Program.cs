using DFeHub.Providers.WebMania;
using DFeHub.Providers.WebMania.NFe;
using DFeHub.Providers.WebMania.NFSe;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebMania(
    builder.Configuration.GetSection(WebManiaOptions.SectionName));

var app = builder.Build();

app.MapGet(
    "/",
    (IWebManiaClient client) => Results.Ok(
        new
        {
            Package = "DFeHub.Client",
            Release = "PR 2 — NFS-e",
            Configured = new
            {
                NFe = client.NFe.IsConfigured,
                NFSe = client.NFSe.IsConfigured,
                CTe = client.CTe.IsConfigured,
                MDFe = client.MDFe.IsConfigured,
                NFCom = client.NFCom.IsConfigured,
                DCe = client.DCe.IsConfigured,
                Monitor = client.Monitor.IsConfigured,
                Validator = client.Validator.IsConfigured,
                B2B = client.B2B.IsConfigured,
            },
        }));

app.MapPost(
    "/nfe/emissao",
    async (
        NFeIssueRequest request,
        IWebManiaClient client,
        CancellationToken cancellationToken) =>
    {
        var response = await client.NFe.IssueAsync(
            request,
            cancellationToken);
        return Results.Ok(response);
    });

app.MapGet(
    "/nfe/{accessKey}",
    async (
        string accessKey,
        IWebManiaClient client,
        CancellationToken cancellationToken) =>
    {
        var response = await client.NFe.QueryByAccessKeyAsync(
            accessKey,
            cancellationToken);
        return Results.Ok(response);
    });

app.MapGet(
    "/nfse/status",
    async (
        IWebManiaClient client,
        CancellationToken cancellationToken) =>
    {
        var response = await client.NFSe.GetMunicipalityStatusAsync(
            cancellationToken);
        return Results.Ok(response);
    });

app.MapPost(
    "/nfse/emissao",
    async (
        NFSeIssueRequest request,
        IWebManiaClient client,
        CancellationToken cancellationToken) =>
    {
        var response = await client.NFSe.IssueAsync(
            request,
            cancellationToken);
        return Results.Ok(response);
    });

app.MapGet(
    "/nfse/{uuid:guid}",
    async (
        Guid uuid,
        IWebManiaClient client,
        CancellationToken cancellationToken) =>
    {
        var response = await client.NFSe.QueryByUuidAsync(
            uuid,
            cancellationToken);
        return Results.Ok(response);
    });

app.Run();
