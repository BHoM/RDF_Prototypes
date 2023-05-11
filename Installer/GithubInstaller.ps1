# Variables
$zipFileRelativePath = "BHoM\Assemblies\RDF_Prototypes.zip"
$zipFile = Join-Path -Path $env:ProgramData -ChildPath $zipFileRelativePath

# Download the zip file to the target location
Write-Host "Retrieving latest release from GitHub."
Invoke-WebRequest 'https://github.com/BHoM/RDF_Prototypes/releases/latest/download/RDF_Prototypes.zip' -OutFile $zipFile
Write-Host "Downloaded the zip file to $zipFile."

# Unzip the file into the BHoM\Assemblies directory
$targetFolderRelativePath = "BHoM\Assemblies"
$targetFolder = Join-Path -Path $env:ProgramData -ChildPath $targetFolderRelativePath
Expand-Archive -Path $zipFile -DestinationPath $targetFolder -Force
Write-Host "Extracted the zip file to $targetFolder."

# Delete the zip file after extraction
Remove-Item -Path $zipFile
Write-Host "Deleted the zip file."
Write-Host "Installation / Update successfull."