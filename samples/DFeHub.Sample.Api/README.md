# DFeHub.Sample.Api

Exemplo mínimo de registro global do SDK em uma aplicação ASP.NET Core.

As credenciais não ficam no `appsettings.json`. Configure somente os grupos usados,
por exemplo:

```powershell
$env:Fiscal__WebMania__ApiV2__AccessToken = "seu-token"
dotnet run --project samples/DFeHub.Sample.Api
```

O endpoint `/` informa apenas quais grupos estão configurados. Nenhuma credencial é
exibida e nenhuma operação fiscal é chamada durante a PR 0.

