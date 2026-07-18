# Matriz de contrato — NF-e, NFC-e, CC-e e MDe

- Congelamento: 2026-07-18
- Fonte primária: <https://webmania.com.br/docs/rest-api-nfe/>
- Fonte complementar: <https://www.postman.com/webmania-api/webmania/overview>
- Autenticação: API 1.0, com os quatro cabeçalhos `X-*`
- Implementação: PR 1 / `0.2.0-alpha.1`

| Operação | Método e rota | Request | Response | Status | Fixture/teste |
| --- | --- | --- | --- | --- | --- |
| Emitir NF-e/NFC-e | `POST /1/nfe/emissao/` | `NFeIssueRequest` | `NFeDocumentResponse` | Implementado | `issue-request.json`, `issue-response.json` |
| Emitir devolução | `POST /1/nfe/devolucao/` | `NFeReturnRequest` | `NFeDocumentResponse` | Implementado | Matriz de rotas |
| Emitir ajuste | `POST /1/nfe/ajuste/` | `NFeAdjustmentRequest` | `NFeDocumentResponse` | Implementado | Matriz de rotas |
| Emitir complemento | `POST /1/nfe/complementar/` | `NFeComplementRequest` | `NFeDocumentResponse` | Implementado | Matriz de rotas |
| Carta de correção | `POST /1/nfe/cartacorrecao/` | `NFeCorrectionLetterRequest` | `NFeEventResponse` | Implementado | Validação e matriz de rotas |
| Manifestação do destinatário | `POST /1/nfe/manifesta/` | `NFeRecipientManifestationRequest` | `NFeEventResponse` | Implementado | Serialização do evento |
| Registrar evento IBS/CBS | `POST /1/nfe/evento-ibs-cbs/` | `NFeIbsCbsEventRequest` | `NFeEventResponse` | Implementado | Catálogo dos 12 eventos |
| Cancelar evento IBS/CBS | `PUT /1/nfe/evento-ibs-cbs/cancelar/` | `NFeIbsCbsEventCancellationRequest` | `NFeEventResponse` | Implementado | Matriz de rotas |
| Registrar ECONF | `POST /1/nfe/conciliacao/` | `NFeFinancialReconciliationRequest` | `NFeEventResponse` | Implementado | Matriz de rotas |
| Cancelar ECONF | `PUT /1/nfe/conciliacao/cancelar/` | `NFeFinancialReconciliationCancellationRequest` | `NFeEventResponse` | Implementado | Matriz de rotas |
| Consultar por chave | `GET /1/nfe/consulta/` com corpo | `chave` | `NFeDocumentResponse` | Implementado | Teste de request |
| Consultar por UUID | `GET /1/nfe/consulta/` com corpo | `uuid` | `NFeDocumentResponse` | Implementado | Matriz de rotas |
| Consultar por ID | `GET /1/nfe/consulta/` com corpo | `ID` | `NFeDocumentResponse[]` | Implementado | Teste de múltiplos resultados |
| Cancelar NF-e/NFC-e | `PUT /1/nfe/cancelar/` | `NFeCancellationRequest` | `NFeCancellationResponse` | Implementado | Teste de request |
| Inutilizar numeração | `PUT /1/nfe/inutilizar/` | `NFeNumberInvalidationRequest` | `NFeNumberInvalidationResponse` | Implementado | Matriz de rotas |
| Consultar SEFAZ | `GET /1/nfe/sefaz/` | — | `NFeSefazStatusResponse` | Implementado | Teste sem corpo |
| Consultar certificado | `GET /1/nfe/certificado/` | — | `NFeCertificateStatusResponse` | Implementado | Matriz de rotas |
| Criar/alterar classe de imposto | `POST /1/nfe/classe-imposto/` | `NFeTaxClassUpsertRequest` | `NFeTaxClassResponse` | Implementado | Matriz de rotas |
| Listar classes de imposto | `GET /1/nfe/classe-imposto/` | — | `NFeTaxClassResponse[]` | Implementado | Matriz de rotas |
| Excluir classes de imposto | `DELETE /1/nfe/classe-imposto/` | `NFeTaxClassDeleteRequest` | `NFeTaxClassDeleteResponse[]` | Implementado | Matriz de rotas |
| Exportar relatório | `POST /1/nfe/relatorios/` | `NFeReportRequest` | `NFeReportResponse` | Implementado | Matriz de rotas |
| Atualizar empresa | `POST /1/nfe/empresa/` | `NFeCompanyUpdateRequest` | `NFeCompanyUpdateResponse` | Implementado | Matriz de rotas |
| Baixar XML/DANFE | URL HTTPS retornada pela Webmania | URL | `FiscalDocumentFile` | Implementado | Validação de host e streaming |

## Decisões de contrato

- Valores monetários usam `decimal` em memória e string JSON no wire format, sem
  perda de precisão.
- Datas de emissão usam `yyyy-MM-dd HH:mm:ss`, como exigido pela API 1.0.
- Consultas mantêm o corpo JSON em requisições `GET`, conforme o contrato oficial.
- Respostas extensíveis preservam campos ainda desconhecidos em `AdditionalData`.
- Downloads autenticados aceitam somente HTTPS em `webmania.com.br` ou subdomínios.
- `CancellationToken` é propagado em todas as operações.
- Antecipação e geração de token não possuem endpoint REST NF-e próprio na fonte
  congelada; por isso não integram `INFeClient`.

As fixtures usam dados sintéticos e não contêm credenciais nem documentos fiscais
reais.
