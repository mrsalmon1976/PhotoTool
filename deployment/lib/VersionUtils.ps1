function UpdateInnoVersion($scriptPath, $version) {

    # Read the content of the file
    $content = Get-Content -Path $scriptPath -Raw
    
    # Replace text using regular expression
    # Example: Replace any line that contains "pattern" with "new line text"
    $newContent = $content -replace ".*#define Version ""\d.\d.\d"".*", "#define Version ""$version"""
    
    # Write the modified content back to the file
    $newContent | Set-Content -Path $scriptPath
}

function UpdateProjectVersion
{
	param([string]$filePath, [string]$version)

	if (!(Test-Path -Path $filePath)) {
		throw "$filePath does not exist - unable to update project file"
	}

	$doc = New-Object System.Xml.XmlDocument
	$doc.Load($filePath)
	UpdateXmlNodeIfExists -xmlDoc $doc -xpath "//PropertyGroup/Version" -newValue $version
	UpdateXmlNodeIfExists -xmlDoc $doc -xpath "//PropertyGroup/AssemblyVersion" -newValue $version
	UpdateXmlNodeIfExists -xmlDoc $doc -xpath "//PropertyGroup/FileVersion" -newValue $version
	UpdateXmlNodeIfExists -xmlDoc $doc -xpath "//package/metadata/version" -newValue $version
	$doc.Save($filePath)
}

function UpdateXmlNodeIfExists
{
	param($xmlDoc, $xpath, $newValue)
	$node = $xmlDoc.SelectSingleNode($xpath)
	if ($null -ne $node)
	{
		$node.InnerText = $newValue
	}
}