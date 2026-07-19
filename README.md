# DFeHub.Client

SDK .NET 10 comunitário e não oficial para integração tipada e assíncrona com as
APIs fiscais e de faturamento da Webmania.

> Webmania é marca de seus respectivos titulares. Este projeto não possui
> afiliação, endosso ou suporte oficial da Webmania.

## Estado atual

Versão: `0.3.0-alpha.1` — NF-e, NFC-e, CC-e, MDe e NFS-e.

A PR 2 adiciona NFS-e síncrona e lote RPS, status municipal, consulta,
cancelamento, substituição, manifestação, agendamento e artefatos. O contrato
inclui Padrão Nacional, provedores municipais e os campos da reforma tributária.

| API | Cliente | Estado |
| --- | --- | --- |
| NF-e, NFC-e, CC-e e MDe | `INFeClient` | Implementado |
| NFS-e | `INFSeClient` | Implementado |
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

## NF-e e NFC-e

Os modelos usam nomes idiomáticos em C# e preservam o contrato da API 1.0 na
serialização. Um exemplo mínimo de emissão em homologação:

```csharp
using DFeHub.Providers.WebMania;
using DFeHub.Providers.WebMania.NFe;

public sealed class EmitirNota(IWebManiaClient webMania)
{
    public Task<NFeDocumentResponse> ExecuteAsync(
        NFeIssueRequest request,
        CancellationToken cancellationToken)
    {
        return webMania.NFe.IssueAsync(request, cancellationToken);
    }
}
```

O `NFeIssueRequest` tipa destinatário, produtos, impostos, pedido, pagamentos,
transporte, cobrança, referências, exportação e dados agropecuários. Para consultar
uma nota ou baixar o XML retornado:

```csharp
var nota = await webMania.NFe.QueryByAccessKeyAsync(
    chaveDe44Digitos,
    cancellationToken);

if (nota.Xml is not null)
{
    await using var arquivo = await webMania.NFe.DownloadAsync(
        nota.Xml,
        cancellationToken);
    await arquivo.Content.CopyToAsync(destino, cancellationToken);
}
```

Veja a [matriz completa de NF-e](docs/contracts/nfe.md) e o
[projeto de exemplo](samples/DFeHub.Sample.Api).

## NFS-e

`NFSeIssueRequest` aceita de um a 50 RPS. Uma emissão individual pode retornar a
NFS-e sincronamente; lotes e alguns municípios retornam processamento assíncrono.
Use a URL de notificação e reconcilie atualizações por `UpdatedAt`.
Agendamentos usam `DateTime` com `Kind.Unspecified`, segundos zerados, pois o
contrato transmite um horário civil sem fuso (`yyyy-MM-dd HH:mm:00`).

```csharp
using DFeHub.Providers.WebMania;
using DFeHub.Providers.WebMania.NFSe;

var resultado = await webMania.NFSe.IssueAsync(
    new NFSeIssueRequest
    {
        Environment = NFSeEnvironment.Certification,
        Rps =
        [
            new NFSeRps
            {
                Service = new NFSeService
                {
                    Discrimination = "Serviço de homologação",
                    ServiceValue = 15.00m,
                    TaxClass = "REF000000",
                },
                Taker = new NFSeParty
                {
                    Cpf = "00000000000",
                    FullName = "Tomador de homologação",
                },
            },
        ],
    },
    cancellationToken);

var atual = await webMania.NFSe.QueryByUuidAsync(
    resultado.Uuid,
    cancellationToken);
```

Antes de montar campos específicos, consulte
`GetMunicipalityStatusAsync`: o retorno informa o provedor, a versão, os ambientes,
as funções e os parâmetros aceitos. Use `TaxClass` para cálculo automático ou os
campos explícitos do provedor; no Padrão Nacional, `IssTaxation` pode ser usado sem
criar um objeto `Taxes` vazio. Veja a
[matriz completa de NFS-e](docs/contracts/nfse.md).

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
