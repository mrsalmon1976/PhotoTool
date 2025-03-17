$ErrorActionPreference = "Stop"
Clear-Host
$root = $PSScriptRoot

Set-Location "$root\..\source\PhotoTool"

&dotnet publish -c Release -r win-x64 --self-contained true