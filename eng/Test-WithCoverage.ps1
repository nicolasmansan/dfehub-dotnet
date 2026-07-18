[CmdletBinding()]
param(
    [switch] $NoBuild
)

$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$settings = Join-Path $PSScriptRoot 'coverage.runsettings'
$reportDirectory = Join-Path $repositoryRoot 'artifacts/coverage'
$testProjects = @(
    @{
        Name = 'unit'
        Path = 'tests/DFeHub.Client.UnitTests/DFeHub.Client.UnitTests.csproj'
    },
    @{
        Name = 'contract'
        Path = 'tests/DFeHub.Client.ContractTests/DFeHub.Client.ContractTests.csproj'
    }
)

Push-Location $repositoryRoot
try {
    if (-not $NoBuild) {
        dotnet build DFeHub.slnx --configuration Release
        if ($LASTEXITCODE -ne 0) {
            throw 'Build failed.'
        }
    }

    dotnet tool restore
    if ($LASTEXITCODE -ne 0) {
        throw 'Tool restore failed.'
    }

    foreach ($testProject in $testProjects) {
        $output = Join-Path `
            $reportDirectory `
            "$($testProject.Name).cobertura.xml"
        $command = (
            (
                'dotnet run --project {0} --configuration Release ' +
                '--no-build --no-restore'
            ) -f $testProject.Path
        )

        dotnet tool run dotnet-coverage collect `
            $command `
            --settings $settings `
            --output $output `
            --output-format cobertura `
            --nologo

        if ($LASTEXITCODE -ne 0) {
            throw "Tests or coverage failed for $($testProject.Path)."
        }
    }

    dotnet reportgenerator `
        "-reports:$reportDirectory/*.cobertura.xml" `
        "-targetdir:$reportDirectory" `
        '-reporttypes:Html;Cobertura;MarkdownSummaryGithub'

    if ($LASTEXITCODE -ne 0) {
        throw 'Coverage report generation failed.'
    }

    & (Join-Path $PSScriptRoot 'Validate-Coverage.ps1') `
        -Path (Join-Path $reportDirectory 'Cobertura.xml')
}
finally {
    Pop-Location
}
