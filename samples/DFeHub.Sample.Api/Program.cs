using DFeHub.Providers.WebMania;

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
            Foundation = "PR 0",
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

app.Run();
