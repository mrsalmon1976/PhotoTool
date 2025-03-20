$ErrorActionPreference = "Stop"
Clear-Host
$root = $PSScriptRoot
. "$root\lib\VersionUtils.ps1"

$setupScriptPath = "$root\PhotoTool.iss"
$innosetupPath = "C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
$sourcePath = "$root\..\source" 
$version = Read-Host -Prompt "What version are we building? [e.g. 2.3.0]"

# validate Inno Setup path
if ((Test-Path -Path $innosetupPath) -eq $false) {
    Write-Host "Inno Setup not found at $innosetupPath"
    Exit 1
}

# reset version numbers in files
UpdateInnoVersion -scriptPath $setupScriptPath -version $version
UpdateProjectVersion -filePath "$sourcePath\PhotoTool\PhotoTool.csproj" -version $version

&dotnet publish "$sourcePath\PhotoTool\PhotoTool.csproj" -c Release -r win-x64 -o "$root\output" --self-contained true 
&$innosetupPath /DVersion=$version $setupScriptPath
