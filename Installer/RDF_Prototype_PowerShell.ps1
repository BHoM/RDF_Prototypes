# Variables
$sourceFolder = Get-Location
$targetFolderRelativePath = "BHoM\Assemblies"
$targetFolder = Join-Path -Path $env:ProgramData -ChildPath $targetFolderRelativePath

# Check if the target folder exists
if (Test-Path -Path $targetFolder) {
    # Copy files, excluding .bat and .ps1 files
    $overwriteAll = $false
    Get-ChildItem -Path $sourceFolder -Exclude *.bat, *.ps1 | ForEach-Object {
        $destinationFile = Join-Path -Path $targetFolder -ChildPath $_.Name
        if (Test-Path -Path $destinationFile) {
            if (-not $overwriteAll) {
                Write-Warning "File $($_.Name) already exists in the target folder."
                $userInput = Read-Host "Do you want to update and overwrite all existing files? (Y/N)"
                if ($userInput -eq "Y") {
                    $overwriteAll = $true
                }
            }
            if ($overwriteAll) {
                Copy-Item -Path $_.FullName -Destination $destinationFile -Force
            }
        } else {
            Copy-Item -Path $_.FullName -Destination $destinationFile
        }
    }

    Write-Output "RDF-Prototype has been installed successfully."
} else {
    Write-Warning "Target folder does not exist. Please check if BHoM is installed."
}

# Wait for a keypress before closing the window
Write-Output "Press any key to exit..."
$null = $host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")