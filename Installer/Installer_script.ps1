Write-Host "Before running this script, please make sure any UI software that can run BHoM is closed (Rhino/Excel/Revit/etc.)`n`n"

# Set the variables for the repository and directory
$repository = "BHoM/RDF_Prototypes"
$directory = "InstallerDLLs"

# Set the output directory to save the downloaded files
$outputDirectoryRelativePath = "BHoM\Assemblies"
$outputDirectory = Join-Path -Path $env:ProgramData -ChildPath $outputDirectoryRelativePath

# Make sure the output directory exists
if (!(Test-Path -Path $outputDirectory)) {
    throw "The BHoM installation folder seems to be missing. Make sure to run the BHoM installer (http://bhom.xyz) before running this script."
}

# Get the list of files in the directory
$filesUrl = "https://api.github.com/repos/$repository/contents/$directory"
$files = Invoke-RestMethod -Uri $filesUrl

# Download each file
foreach ($file in $files) {
    $fileUrl = $file.download_url
    $filePath = Join-Path -Path $outputDirectory -ChildPath $file.name

    Write-Host "Downloading file: $($file.name)"
    try
    {
        Invoke-WebRequest -Uri $fileUrl -OutFile $filePath
    }
    catch 
    {
        Write-Error "Error overwriting file $($file.name). Make sure any UI running BHoM is closed (e.g. Rhino, Excel, Revit, etc.),`nand that you have permissions to write files in the BHoM installation folder. Error details:`n"
    }
}

Write-Host "`nInstallation / Update done.`n"