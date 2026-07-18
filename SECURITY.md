# Política de segurança

## Versões suportadas

Enquanto o projeto estiver em alpha, somente a versão pré-release mais recente
receberá correções de segurança. Após `1.0.0`, a política será revista e publicada
nesta tabela.

## Relatando uma vulnerabilidade

Não abra uma issue pública para:

- vazamento de credenciais;
- exposição de payload fiscal;
- falhas que permitam autenticação indevida;
- dependências vulneráveis ainda não divulgadas.

Use o recurso **Private vulnerability reporting** do GitHub no repositório. Se ele
ainda não estiver habilitado, entre em contato de forma privada com o mantenedor
listado no perfil do repositório.

Inclua:

- versão afetada;
- impacto;
- passos mínimos de reprodução sem dados reais;
- mitigação conhecida, se houver.

Não inclua tokens, certificados, XMLs ou dados pessoais reais. O projeto buscará
confirmar o recebimento em até 5 dias úteis e fornecer uma avaliação inicial em até
10 dias úteis.

## Tratamento de dados

O SDK não registra credenciais nem payloads fiscais por padrão. Aplicações
consumidoras continuam responsáveis por sua configuração de logs, armazenamento,
controle de acesso e conformidade com a LGPD.

