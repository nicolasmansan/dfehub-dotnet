[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string] $Path,

    [ValidateRange(0, 1)]
    [double] $MinimumLineRate = 0.90,

    [ValidateRange(0, 1)]
    [double] $MinimumBranchRate = 0.85
)

$ErrorActionPreference = 'Stop'

$resolvedPath = Resolve-Path -LiteralPath $Path
[xml] $report = Get-Content -LiteralPath $resolvedPath -Raw

$culture = [System.Globalization.CultureInfo]::InvariantCulture
$lineRate = [double]::Parse(
    $report.coverage.'line-rate',
    $culture)
$branchRate = [double]::Parse(
    $report.coverage.'branch-rate',
    $culture)

$linePercent = $lineRate * 100
$branchPercent = $branchRate * 100

$message = (
    (
        'Coverage: lines {0:N2}% (minimum {1:N2}%), branches {2:N2}% ' +
        '(minimum {3:N2}%).'
    ) -f
        $linePercent,
        ($MinimumLineRate * 100),
        $branchPercent,
        ($MinimumBranchRate * 100)
)

Write-Host $message

if ($lineRate -lt $MinimumLineRate) {
    throw "Line coverage is below the required threshold."
}

if ($branchRate -lt $MinimumBranchRate) {
    throw "Branch coverage is below the required threshold."
}
