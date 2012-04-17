param($installPath, $toolsPath, $package, $project)

# this is the name of the package folder
$packageFolder = $package.ID + "." + $package.Version

# if there is a project file, then modify it
if ($project)
{
	Write-Host "Removing FxCop.targets from project"

	# This is the MSBuild targets file to add
	$targetsFile = [System.IO.Path]::Combine($toolsPath, 'FxCop.targets')

	# Need to load MSBuild assembly if it's not loaded yet.
	Add-Type -AssemblyName 'Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
	# Grab the loaded MSBuild project for the project
	$msbuild = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($project.FullName) | Select-Object -First 1

	# Make the path to the targets file relative.
	$projectUri = new-object Uri('file://' + $project.FullName)
	$targetUri = new-object Uri('file://' + $targetsFile)
	$relativePath = $projectUri.MakeRelativeUri($targetUri).ToString().Replace([System.IO.Path]::AltDirectorySeparatorChar, [System.IO.Path]::DirectorySeparatorChar)

	# Remove the import and save the project
	$msbuild.Xml.Imports | Where { $_.Project -eq $relativePath } | % { $msbuild.Xml.RemoveChild($_) }
	$project.Save()
}