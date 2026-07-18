# DFeHub.Client

SDK .NET 10 comunitário e não oficial para integração tipada e assíncrona com as
APIs fiscais e de faturamento da Webmania.

> Webmania é marca de seus respectivos titulares. Este projeto não possui
> afiliação, endosso ou suporte oficial da Webmania.

## Estado atual

Versão: `0.1.0-alpha.1` — fundação do SDK.

A PR 0 entrega configuração, autenticação, transporte HTTP, erros, streaming,
telemetria, testes e empacotamento. Operações de emissão e consulta serão adicionadas
uma API por PR; portanto, esta alpha ainda não emite documentos fiscais.

| API | Cliente | Estado |
| --- | --- | --- |
| NF-e, NFC-e, CC-e e MDe | `INFeClient` | Fundação |
| NFS-e | `INFSeClient` | Fundação |
| CT-e e CT-e OS | `ICTeClient` | Fundação |
| MDF-e | `IMDFeClient` | Fundação |
| NFCom | `INFComClient` | Fundação |
| DC-e | `IDCeClient` | Fundação |
| Monitor Fiscal | `IMonitorFiscalClient` | Fundação |
| Validador Fiscal | `IValidatorFiscalClient` | Fundação |
| Gerencial B2B | `IB2BClient` | Fundação |

Consulte o [plano completo](PLANO_DE_IMPLEMENTACAO.md) para a ordem das próximas
PRs.

## Requisitos

- .NET SDK 10;
- uma aplicação compatível com `net10.0`;
- credenciais apenas para as APIs efetivamente utilizadas.

## Instalação

Quando a primeira alpha estiver publicada:

```shell
dotnet add package DFeHub.Client --prerelease
```

Para desenvolvimento local, referencie
`src/DFeHub.Client/DFeHub.Client.csproj`.

## Configuração

O pacote possui um único ponto de entrada:

```csharp
using DFeHub.Providers.WebMania;

builder.Services.AddWebMania(
    builder.Configuration.GetSection(WebManiaOptions.SectionName));
```

Configure segredos com variáveis de ambiente, Secret Manager ou um cofre. Exemplos
de nomes:

```text
Fiscal__WebMania__ApiV1__ConsumerKey
Fiscal__WebMania__ApiV1__ConsumerSecret
Fiscal__WebMania__ApiV1__AccessToken
Fiscal__WebMania__ApiV1__AccessTokenSecret

Fiscal__WebMania__ApiV2__AccessToken
Fiscal__WebMania__Validator__Token

Fiscal__WebMania__B2B__ConsumerKey
Fiscal__WebMania__B2B__ConsumerSecret
Fiscal__WebMania__B2B__AccessToken
Fiscal__WebMania__B2B__AccessTokenSecret
```

Também é possível usar um delegate:

```csharp
builder.Services.AddWebMania(options =>
{
    options.ApiV2.AccessToken =
        builder.Configuration["WEBMANIA_API_V2_ACCESS_TOKEN"];
    options.Timeout = TimeSpan.FromSeconds(60);
});
```

Credenciais ausentes não impedem a inicialização. Cada grupo é validado somente
quando seu subcliente envia uma operação. `IsConfigured` permite verificar o estado
sem revelar valores:

```csharp
public sealed class FiscalHealthCheck(IWebManiaClient client)
{
    public bool IsNFSeConfigured() => client.NFSe.IsConfigured;
}
```

## Autenticação

| Grupo | Produtos | Contrato |
| --- | --- | --- |
| `ApiV1` | NF-e, NFC-e, CC-e e MDe | Quatro cabeçalhos `X-*` |
| `ApiV2` | NFS-e, CT-e, MDF-e, NFCom, DC-e e Monitor | Bearer |
| `Validator` | Validador Fiscal | `X-Token` |
| `B2B` | Gerencial B2B | Quatro cabeçalhos `X-*` próprios |

As credenciais B2B nunca reutilizam implicitamente as credenciais fiscais da API
1.0.

## Segurança e observabilidade

- Não há retry automático para operações fiscais mutáveis.
- Credenciais e payloads fiscais não são escritos em logs.
- Traces usam o `ActivitySource` `DFeHub.WebMania`.
- Métricas usam o `Meter` `DFeHub.WebMania`.
- Erros HTTP são convertidos em exceções derivadas de `WebManiaException`.
- Downloads são transmitidos por `FiscalDocumentFile` e devem ser descartados.

Consulte [SECURITY.md](SECURITY.md) antes de enviar um relato de vulnerabilidade.

## Desenvolvimento

```shell
dotnet restore DFeHub.slnx
dotnet build DFeHub.slnx --configuration Release --no-restore
dotnet test --project tests/DFeHub.Client.UnitTests/DFeHub.Client.UnitTests.csproj \
  --configuration Release --no-build --no-restore
dotnet test --project tests/DFeHub.Client.ContractTests/DFeHub.Client.ContractTests.csproj \
  --configuration Release --no-build --no-restore
dotnet pack src/DFeHub.Client/DFeHub.Client.csproj \
  --configuration Release --no-build
```

No PowerShell, use crase para continuação de linha ou execute cada comando em uma
linha.

As metas de cobertura são 90% de linhas e 85% de branches. Veja
[CONTRIBUTING.md](CONTRIBUTING.md).

## Licença

[MIT](LICENSE).

