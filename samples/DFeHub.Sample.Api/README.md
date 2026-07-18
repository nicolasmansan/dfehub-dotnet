# DFeHub.Sample.Api

Exemplo de registro global e uso dos clientes NF-e e NFS-e em uma aplicação
ASP.NET Core.

As credenciais não ficam no `appsettings.json`. Configure somente os grupos usados,
por exemplo:

```powershell
$env:Fiscal__WebMania__ApiV1__ConsumerKey = "sua-chave"
$env:Fiscal__WebMania__ApiV1__ConsumerSecret = "seu-segredo"
$env:Fiscal__WebMania__ApiV1__AccessToken = "seu-token"
$env:Fiscal__WebMania__ApiV1__AccessTokenSecret = "seu-token-secret"
$env:Fiscal__WebMania__ApiV2__AccessToken = "seu-bearer-token"
dotnet run --project samples/DFeHub.Sample.Api
```

O endpoint `/` informa apenas quais grupos estão configurados. `GET /nfe/{chave}`
consulta uma NF-e e `POST /nfe/emissao` aceita um `NFeIssueRequest`.
`GET /nfse/status` consulta os recursos do município,
`GET /nfse/{uuid}` consulta uma NFS-e ou lote e `POST /nfse/emissao` aceita um
`NFSeIssueRequest`. As emissões chamam a API real: durante o desenvolvimento, use
`ambiente: 2` apenas quando o status municipal confirmar homologação e configure
credenciais próprias desse ambiente.
