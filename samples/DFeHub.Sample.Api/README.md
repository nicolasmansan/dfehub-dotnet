# DFeHub.Sample.Api

Exemplo de registro global e uso do cliente NF-e em uma aplicação ASP.NET Core.

As credenciais não ficam no `appsettings.json`. Configure somente os grupos usados,
por exemplo:

```powershell
$env:Fiscal__WebMania__ApiV1__ConsumerKey = "sua-chave"
$env:Fiscal__WebMania__ApiV1__ConsumerSecret = "seu-segredo"
$env:Fiscal__WebMania__ApiV1__AccessToken = "seu-token"
$env:Fiscal__WebMania__ApiV1__AccessTokenSecret = "seu-token-secret"
dotnet run --project samples/DFeHub.Sample.Api
```

O endpoint `/` informa apenas quais grupos estão configurados. `GET /nfe/{chave}`
consulta uma nota e `POST /nfe/emissao` aceita um `NFeIssueRequest`. A emissão chama
a API real: durante o desenvolvimento, use `ambiente: 2` e credenciais de
homologação.
