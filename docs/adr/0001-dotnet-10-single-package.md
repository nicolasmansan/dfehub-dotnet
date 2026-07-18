# ADR 0001: .NET 10 e pacote único

- Status: aceito
- Data: 2026-07-17

## Contexto

O SDK precisa usar os padrões atuais do .NET e oferecer uma instalação simples para
nove APIs relacionadas.

## Decisão

O projeto terá target único `net10.0`, C# 14 e um pacote `DFeHub.Client`. A separação
por API será interna. O pacote será reavaliado depois de `1.0.0` caso tamanho,
cadência ou dependências demonstrem a necessidade de divisão.

## Consequências

- A API usa recursos e analisadores atuais.
- Consumidores em runtimes anteriores precisarão atualizar.
- Um único `AddWebMania` configura todos os produtos sem exigir todas as credenciais.

