# ADR 0002: isolamento de autenticação e ausência de retry em mutações

- Status: aceito
- Data: 2026-07-17

## Contexto

As APIs usam três esquemas de autenticação, e B2B reutiliza o formato da API 1.0 com
credenciais diferentes. Repetir automaticamente uma emissão ou evento pode produzir
efeito fiscal duplicado.

## Decisão

Serão usados quatro clientes HTTP nomeados e handlers separados. B2B nunca herdará
credenciais fiscais. Operações mutáveis não terão retry automático. Consultas
idempotentes poderão receber resiliência explícita em PR futura.

## Consequências

- A configuração é mais explícita.
- É possível usar somente um produto.
- Falhas transitórias de mutações ficam visíveis à aplicação consumidora.

