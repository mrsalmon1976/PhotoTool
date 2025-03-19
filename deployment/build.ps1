$ErrorActionPreference = "Stop"
Clear-Host
$root = $PSScriptRoot

Set-Location "$root\..\source\PhotoTool" 

&dotnet publish -c Release -r win-x64 -o "$root\output" --self-contained true 