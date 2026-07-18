# Changelog

As mudanças relevantes deste projeto serão documentadas neste arquivo. O formato é
baseado em [Keep a Changelog](https://keepachangelog.com/pt-BR/1.1.0/) e o
versionamento segue [Semantic Versioning](https://semver.org/lang/pt-BR/).

## [Não publicado]

### Planejado

- PR 2: NFS-e.
- PRs seguintes conforme `PLANO_DE_IMPLEMENTACAO.md`.

## [0.2.0-alpha.1] - 2026-07-18

### Adicionado

- Cliente `INFeClient` para NF-e, NFC-e, CC-e e MDe na API 1.0.
- Emissão normal, devolução, ajuste e complemento.
- Consulta por chave, UUID e ID do pedido, cancelamento e inutilização.
- Carta de correção e as quatro manifestações do destinatário.
- Registro e cancelamento dos 12 eventos IBS/CBS e de eventos ECONF.
- Gestão de classes de imposto, relatórios e dados da empresa.
- Download autenticado e em streaming de XML, DANFE e demais artefatos.
- Modelos fiscais tipados, validação determinística e serialização source-generated.
- Fixtures e testes de contrato compatíveis com evolução aditiva das respostas.

## [0.1.0-alpha.1] - 2026-07-17

### Adicionado

- Solução .NET 10 e pacote `DFeHub.Client`.
- Registro global `AddWebMania`.
- Opções separadas para API 1.0, API 2.0, Validador e B2B.
- Fachada `IWebManiaClient` e nove subclientes.
- Pipelines de autenticação com `IHttpClientFactory`.
- Transporte JSON source-generated, erros tipados e downloads em streaming.
- Logs, métricas e tracing sem payloads sensíveis.
- Testes unitários, de contrato, cobertura e smoke test do pacote.
- Workflows de CI, CodeQL e publicação OIDC.

[Não publicado]: https://github.com/nicolasmansan/dfehub-dotnet/compare/v0.2.0-alpha.1...HEAD
[0.2.0-alpha.1]: https://github.com/nicolasmansan/dfehub-dotnet/releases/tag/v0.2.0-alpha.1
[0.1.0-alpha.1]: https://github.com/nicolasmansan/dfehub-dotnet/releases/tag/v0.1.0-alpha.1
