# Generates a VisualTeensy quick-setup project without the GUI, using the Debug build of vtCore.
# Run with Windows PowerShell 5.1, pwsh 7 ships a conflicting Newtonsoft.Json:
#   powershell -NoProfile -ExecutionPolicy Bypass -File tools/vt_generate.ps1 -ProjectPath C:\temp\vtTest -Board "Teensy 4.1"
param(
    [Parameter(Mandatory)][string]$ProjectPath,
    [string]$ArduinoBase = (Join-Path $env:LOCALAPPDATA 'Arduino15'),
    [string]$Board = 'Teensy 4.1'
)
$ErrorActionPreference = 'Stop'
$bin = Join-Path $PSScriptRoot '..\VisualTeensy\bin\Debug' | Resolve-Path
foreach ($dll in 'Newtonsoft.Json.dll', 'log4net.dll', 'LibGit2Sharp.dll', 'vtCore.dll') {
    $name = [IO.Path]::GetFileNameWithoutExtension($dll)
    if ([AppDomain]::CurrentDomain.GetAssemblies().GetName().Name -contains $name) { continue }  # pwsh 7 ships its own Newtonsoft.Json 13
    Add-Type -Path (Join-Path $bin $dll)
}

$setup = [vtCore.SetupData]::getDefault()
$setup.arduinoBase = $ArduinoBase
if ($setup.arduinoBaseError) { throw "Arduino folder: $($setup.arduinoBaseError)" }

"core     : $($setup.arduinoCoreBase)"
"compiler : $($setup.arduinoCompiler)"
"tools    : $($setup.arduinoTools)"

$setup.projectBaseDefault = Split-Path $ProjectPath
$setup.uplTyBase.path = [vtCore.Helpers]::findTyToolsFolder()
$setup.makeExeBase.path = "$bin"
$setup.additionalFiles = [System.Collections.Generic.List[string]]::new()
"tytools  : $($setup.uplTyBase.path)"

$libManager = [vtCore.Factory]::makeLibManager($setup)
$project = [vtCore.Factory]::makeProject($setup, $libManager)
$project.newProject()
$project.path = $ProjectPath

$cfg = $project.selectedConfiguration
$cfg.selectedBoard = $cfg.boards | Where-Object name -eq $Board | Select-Object -First 1
if (-not $cfg.selectedBoard) { throw "Board '$Board' not found in $($setup.arduinoBoardsTxt)" }
"board    : $($cfg.selectedBoard.name)"

foreach ($task in [vtCore.CodeGenerator]::getTasks($project, $libManager, $setup)) {
    "{0,-30} {1}" -f $task.title, $task.status
    $task.action.Invoke().GetAwaiter().GetResult() | Out-Null
}
