# ADR 0003: JSON source-generated, exceções e observabilidade segura

- Status: aceito
- Data: 2026-07-17

## Contexto

O SDK deve ser tipado, compatível com trimming/AOT e diagnosticável sem expor dados
fiscais.

## Decisão

Cada API terá `JsonSerializerContext` próprio. Respostas não bem-sucedidas serão
convertidas em exceções tipadas. Logs, activities e métricas conterão apenas
produto, operação, status e duração, nunca payload ou credenciais.

## Consequências

- Todo novo DTO precisa ser incluído no contexto de sua API.
- Fixtures de contrato protegem o wire format.
- Diagnóstico pode ser habilitado com baixa exposição de dados.

