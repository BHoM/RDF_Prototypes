# Variables
$zipFileRelativePath = "BHoM\Assemblies\RDF_Prototypes.zip"
$zipFile = Join-Path -Path $env:ProgramData -ChildPath $zipFileRelativePath

# Download the zip file to the target location
# This uses a third-party service to download just the required folder (download-directory.github.io).
# If the installer does not work properly, check that this service is available.
Write-Host "Retrieving latest compiled assemblies from GitHub."
Invoke-WebRequest 'https://download-directory.github.io/?url=https%3A%2F%2Fgithub.com%2FBHoM%2FRDF_Prototypes%2Ftree%2Fmain%2FInstallerDlls' -OutFile $zipFile
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