# Arquitetura da fundação

## Registro

`AddWebMania` registra opções, subclientes, `IHttpClientFactory`, handlers de
autenticação e o transporte interno. Todos os subclientes podem ser injetados
diretamente ou acessados pela fachada `IWebManiaClient`.

## Transporte

Existem quatro clientes HTTP nomeados:

| Nome interno | Endpoint | Credencial |
| --- | --- | --- |
| `DFeHub.WebMania.ApiV1` | `https://webmania.com.br/api/` | API 1.0 fiscal |
| `DFeHub.WebMania.B2B` | `https://webmania.com.br/api/` | API 1.0 B2B |
| `DFeHub.WebMania.ApiV2` | `https://api.webmania.com.br/` | Bearer |
| `DFeHub.WebMania.Validator` | `https://api.webmania.com.br/` | `X-Token` |

O transporte recebe `JsonTypeInfo<T>` gerado em compilação. Requests futuros serão
criados pelos clientes de produto; o transporte gerencia resposta, exceções,
streaming, métricas e tracing.

## Decisões de segurança

- Credenciais são validadas no handler imediatamente antes da rede.
- Nenhuma credencial é copiada para logs ou exceções.
- B2B possui grupo e pipeline próprios.
- Mutações não têm retry automático.
- Corpos de erro têm leitura limitada a 64 KiB e não são expostos integralmente.
- Downloads mantêm a resposta viva até `Dispose`/`DisposeAsync`.

Veja os ADRs em `docs/adr`.

