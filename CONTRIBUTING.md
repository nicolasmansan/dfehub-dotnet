# Contribuindo

Obrigado por ajudar a construir o DFeHub.Client.

## Antes de começar

1. Procure uma issue existente.
2. Para endpoints novos ou divergências, informe a URL da documentação oficial.
3. Nunca publique credenciais, payloads fiscais reais ou certificados.
4. Mantenha cada PR limitada a uma API ou a uma mudança transversal acordada.

## Ambiente

- .NET SDK definido em `global.json`;
- Git;
- PowerShell 7 para os scripts de cobertura, também disponível no GitHub Actions.

## Fluxo local

```shell
dotnet restore DFeHub.slnx
dotnet format DFeHub.slnx --verify-no-changes --no-restore
dotnet build DFeHub.slnx --configuration Release --no-restore
dotnet test --project tests/DFeHub.Client.UnitTests/DFeHub.Client.UnitTests.csproj --configuration Release --no-build --no-restore
dotnet test --project tests/DFeHub.Client.ContractTests/DFeHub.Client.ContractTests.csproj --configuration Release --no-build --no-restore
```

Para gerar cobertura:

```powershell
./eng/Test-WithCoverage.ps1
```

## Regra para PRs de API

Cada PR de API deve incluir:

- matriz em `docs/contracts`;
- requests e responses tipados;
- métodos assíncronos com `CancellationToken`;
- source generation de JSON;
- validação determinística;
- erros mapeados;
- fixtures anonimizadas;
- testes unitários e de contrato;
- exemplo e documentação;
- cobertura sem regressão.

Não use `dynamic`, `JObject` ou `Dictionary<string, object>` na API pública. Preserve
CPF, CNPJ, CEP, chaves e códigos com zeros à esquerda como texto; use `decimal` para
valores fiscais.

## Commits e PRs

- Escreva commits pequenos e descritivos.
- Explique decisões de contrato e links oficiais na PR.
- Atualize `CHANGELOG.md` quando houver impacto para consumidores.
- Aceite as verificações de formatação, build, testes, cobertura e pacote.
- Não altere snapshots/fixtures sem revisar o diff.

Ao contribuir, você concorda que sua contribuição será licenciada sob a licença MIT
do projeto.

