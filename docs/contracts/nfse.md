# Matriz de contrato — NFS-e

- Fonte congelada: <https://webmania.com.br/docs/rest-api-nfse/>
- Versão da documentação: `3.1.1`
- Data do congelamento: `2026-07-18`
- Autenticação: API 2.0 Bearer
- Implementação: PR 2 / `0.3.0-alpha.1`

| Operação | Fonte | Versão/auth | Método e rota | Request | Response | Status | Fixture | Testes | Observações |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Status do município/provedor | [Status município](https://webmania.com.br/docs/rest-api-nfse/#status-municipio) | V2 / Bearer | `GET /2/nfse/status` | Sem corpo | `NFSeProviderStatusResponse` | Implementado | `status-response.json` | Contrato e cliente | Retorna provedor, versão, ambientes, autenticação, formas de emissão, funções, serviços e parâmetros disponíveis |
| Emissão de NFS-e ou lote RPS | [Emissão](https://webmania.com.br/docs/rest-api-nfse/#emissao) | V2 / Bearer | `POST /2/nfse/emissao` | `NFSeIssueRequest` | `NFSeDocumentResponse` | Implementado | `issue-request-national.json`, `issue-response-batch.json`, `issue-response-invoice.json` | Contrato, cliente e validação | Um a 50 RPS; agendamento é horário civil sem fuso e com segundos zerados; uma nota pode retornar `nfse` síncrona e várias notas retornam `lote_rps` assíncrono |
| Consulta de NFS-e, RPS ou lote | [Consultar nota](https://webmania.com.br/docs/rest-api-nfse/#consultar) | V2 / Bearer | `GET /2/nfse/consulta/{uuid}` | UUID na rota | `NFSeDocumentResponse` | Implementado | `query-response.json` | Contrato e cliente | O mesmo UUID identifica nota, RPS ou lote conforme o modelo retornado |
| Cancelamento de nota ou agendamento | [Cancelar](https://webmania.com.br/docs/rest-api-nfse/#cancelar) | V2 / Bearer | `PUT /2/nfse/cancelar` | `NFSeCancellationRequest` | `NFSeDocumentResponse` | Implementado | Coberto pelo wire test | Cliente e validação | O endpoint também cancela emissão agendada |
| Substituição | [Substituir](https://webmania.com.br/docs/rest-api-nfse/#substituir) | V2 / Bearer | `POST /2/nfse/substituir` | `NFSeSubstitutionRequest` | `NFSeDocumentResponse` | Implementado | Coberto pelo wire test | Cliente e validação | Envia um novo RPS e preserva a referência tipada da nota substituída |
| Manifestação de participação | [Manifestar](https://webmania.com.br/docs/rest-api-nfse/#manifestar) | V2 / Bearer | `POST /2/nfse/manifestar` | `NFSeManifestationRequest` | `NFSeDocumentResponse` | Implementado | Coberto pelo wire test | Cliente e validação | Exclusivo do Padrão Nacional; aceita chave de 50 dígitos ou UUID |
| Download autenticado de PDF NFS-e | [Downloads](https://webmania.com.br/docs/rest-api-nfse/#download) | V2 / Bearer | `GET /nfse/{uuid}` | URL retornada pela API | `FiscalDocumentFile` | Implementado | Coberto pelo teste de cliente | Cliente e segurança | Streaming autenticado; Bearer restrito a `https://api.webmania.com.br` e aos três caminhos oficiais |
| Download autenticado de PDF RPS | [Downloads](https://webmania.com.br/docs/rest-api-nfse/#download) | V2 / Bearer | `GET /darps/{uuid}` | URL retornada pela API | `FiscalDocumentFile` | Implementado | Coberto pelo teste de cliente | Cliente e segurança | Streaming autenticado |
| Download autenticado de XML | [Downloads](https://webmania.com.br/docs/rest-api-nfse/#download) | V2 / Bearer | `GET /xmlnfse/{uuid}` | URL retornada pela API | `FiscalDocumentFile` | Implementado | Coberto pelo teste de cliente | Cliente e segurança | Streaming autenticado |

## Provedores e variações congeladas

O contrato público possui um núcleo comum e propriedades opcionais tipadas no
objeto ao qual o provedor as adiciona. Não há um endpoint oficial separado para
listar todos os municípios: `/2/nfse/status` informa o provedor e os recursos do
município configurado para a empresa.

| Provedor/padrão | Variações tipadas |
| --- | --- |
| Abrasf `1.00` a `2.04` | Serviço, CNAE, tributação municipal, local de prestação, exigibilidade, incidência, NBS, itens e construção civil |
| Cecam | Serviço, local e CEP de prestação |
| DSF | CNAE, tipo de operação e tributação |
| Equiplano | Serviço, justificativa de deduções, descrição de impostos e tomador estrangeiro |
| Florianópolis | CFPS, identificador CNAE e situação tributária |
| Governa | CNAE, regime e forma de recolhimento |
| IPM | Serviço, situação tributária e natureza da operação |
| Osasco | Serviço e endereço da prestação de construção civil |
| Padrão Nacional | ISS nacional, comércio exterior, destinatário, imóvel, reembolso, dedução, construção, eventos e campos IBS/CBS |
| São Paulo (RTC) | Tributação, valores RTC, retenções, local de prestação, construção e eventos |
| SIGISS | Serviço e tributação |
| Sigissweb | NBS, intermediário, construção civil e eventos |
| Thema | Serviço, CNAE, tributação municipal, local e natureza da operação |

## Exclusões justificadas

- A documentação não publica uma rota de catálogo global de municípios ou
  provedores; o inventário usa o retorno de status da empresa configurada.
- O prestador não integra o payload de emissão da API 2.0 publicada: ele é a
  empresa associada ao Bearer token. Por isso apenas tomador, destinatário,
  intermediário e fornecedores referenciados aparecem nos DTOs de emissão.
- O sumário oficial cita Pública e Prescon, mas a versão 3.1.1 não publica
  campos próprios para esses provedores. Eles usam o núcleo comum até que um
  contrato específico seja documentado.
- O token temporário de download é produzido pelo algoritmo externo DFeToken e
  não é um endpoint NFS-e. O cliente aceita a URL assinada retornada pelo
  integrador, mas não manipula segredos de criptografia.

## Divergências registradas

- Na substituição, o texto introdutório menciona `uuid`, porém a tabela e o
  exemplo oficial usam `ambiente`, `codigo_verificacao`, `motivo` e um único
  `rps`. A implementação segue a tabela e o exemplo.
- A documentação chama `classe_imposto` de objeto e `ibs_cbs` de array em uma
  tabela, enquanto todos os exemplos os enviam como string e objeto,
  respectivamente. A implementação segue os exemplos.
- `quantidade_rps` e `numero_rps` aparecem como string e número em trechos
  diferentes. As respostas aceitam as duas representações.
- A consulta menciona o status `processamento`, enquanto emissão e notificações
  usam `processando`; status de resposta é preservado como texto para evolução
  aditiva.
- `log` é específico de cada prefeitura e não possui schema publicado. Ele é
  preservado como JSON somente nas respostas; requests continuam totalmente
  tipados.
- A seção de erros afirma que sempre existe `error`, mas o próprio exemplo de
  autenticação publica apenas `msg`. O envelope compartilhado aceita as duas
  formas e converte `msg` em `WebManiaAuthenticationException`.
- No Padrão Nacional, `tributacao_iss` é uma configuração explícita válida mesmo
  quando os campos opcionais de `impostos` não se aplicam. O validador não exige
  um objeto `impostos` vazio e mantém `classe_imposto` mutuamente exclusiva da
  configuração explícita.
