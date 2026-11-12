param([string]$UnityEditorPath = 'C:/Program Files/Unity/Hub/Editor/6000.2.8f1/Editor')
$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path $PSScriptRoot -Parent
Push-Location $projectRoot
try {
    $runtimeRoot = Join-Path $UnityEditorPath 'Data/NetCoreRuntime'
    $shared = Get-ChildItem (Join-Path $runtimeRoot 'shared/Microsoft.NETCore.App') -Directory | Sort-Object Name -Descending | Select-Object -First 1
    $compiler = Join-Path $UnityEditorPath 'Data/DotNetSdkRoslyn/csc.dll'
    New-Item -ItemType Directory -Force Logs | Out-Null
    $arguments = @('/nologo', '/target:exe', '/nostdlib+', '/out:Logs/BuffmageLogicChecks.dll')
    Get-ChildItem $shared.FullName -Filter '*.dll' | Where-Object { $_.Name -match '^(System\.|Microsoft\.CSharp|netstandard|mscorlib)' -and $_.Name -notmatch '\.Native\.' } | ForEach-Object { $arguments += '/r:"' + $_.FullName + '"' }
    $arguments += '/r:"' + (Join-Path $UnityEditorPath 'Data/Managed/UnityEngine/UnityEngine.CoreModule.dll') + '"'
    $arguments += 'Assets/Scripts/ActionTimingRules.cs', 'Assets/Scripts/TimingEventResult.cs', 'Assets/Scripts/Classes/HeroAvailability.cs', 'Assets/Scripts/SkillCooldownLedger.cs', 'Tests/BuffmageLogicChecks.cs'
    $arguments | Set-Content Logs/BuffmageLogicChecks.rsp
    & (Join-Path $runtimeRoot 'dotnet.exe') $compiler '@Logs/BuffmageLogicChecks.rsp'
    if ($LASTEXITCODE -ne 0) { throw 'Buffmage logic checks did not compile.' }
    @{ runtimeOptions = @{ tfm = 'net6.0'; framework = @{ name = 'Microsoft.NETCore.App'; version = $shared.Name } } } | ConvertTo-Json -Depth 5 | Set-Content Logs/BuffmageLogicChecks.runtimeconfig.json
    & (Join-Path $runtimeRoot 'dotnet.exe') Logs/BuffmageLogicChecks.dll
    if ($LASTEXITCODE -ne 0) { throw 'Buffmage logic checks failed.' }
}
finally { Pop-Location }
