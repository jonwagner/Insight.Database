param($installPath, $toolsPath, $package, $project)

$solutionPath = [System.IO.Directory]::GetParent([System.IO.Path]::GetDirectoryName($installPath)).FullName
Write-Host "Installing AutoBuild files to $solutionPath..."

# always put a new copy of autobuild out there
Copy-Item "$toolsPath\autobuild.proj" "$solutionPath"
$autobuild = [xml](Get-Content("$solutionPath\autobuild.proj"))
$import = $autobuild.CreateElement("Import", "http://schemas.microsoft.com/developer/msbuild/2003")
$import.SetAttribute("Project", "$toolsPath\Build\MSBuild.Community.Tasks.Targets")
$autobuild.Project.AppendChild($import)
$import = $autobuild.CreateElement("Import", "http://schemas.microsoft.com/developer/msbuild/2003")
$import.SetAttribute("Project", "$toolsPath\Build\MSBuild.Mercurial.Tasks")
$autobuild.Project.AppendChild($import)
$autobuild.Save("$solutionPath\autobuild.proj")

# these files can get modified by the user
if (!(Test-Path("$solutionPath\autobuild.bat")))	{ Copy-Item "$toolsPath\autobuild.bat" "$solutionPath" }
if (!(Test-Path("$solutionPath\localbuild.bat")))	{ Copy-Item "$toolsPath\localbuild.bat" "$solutionPath" }
if (!(Test-Path("$solutionPath\testbuild.bat")))	{ Copy-Item "$toolsPath\testbuild.bat" "$solutionPath" }
if (!(Test-Path("$solutionPath\nuget.exe")))		{ Copy-Item "$toolsPath\nuget.exe" "$solutionPath" }
if (!(Test-Path("$solutionPath\version.txt")))		{ Copy-Item "$toolsPath\version.txt" "$solutionPath" }

Write-Host "AutoBuild has been installed."
Write-Host "Next Steps:"
Write-Host "1) Update version.txt for your initial version number"
Write-Host "2) If you want autobuild to publish when complete, add /p:PublishFolder=<yourpublishfolder> to autobuild.bat"
Write-Host ""
