param($installPath, $toolsPath, $package, $project)

# if there is a project file, then modify it
if ($project)
{
	# ***************************************************
	# Modify the build project to import the stylecop.targets file
	# ***************************************************
	# This is the MSBuild targets file to add
	Write-Host "Adding FxCop.targets"
	$targetsFile = [System.IO.Path]::Combine($toolsPath, 'FxCop.targets')

	# Need to load MSBuild assembly if it's not loaded yet.
	Add-Type -AssemblyName 'Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
	# Grab the loaded MSBuild project for the project
	$msbuild = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($project.FullName) | Select-Object -First 1

	# Make the path to the targets file relative.
	$projectUri = new-object Uri('file://' + $project.FullName)
	$targetUri = new-object Uri('file://' + $targetsFile)
	$relativePath = $projectUri.MakeRelativeUri($targetUri).ToString().Replace([System.IO.Path]::AltDirectorySeparatorChar, [System.IO.Path]::DirectorySeparatorChar)

	# Add the import and save the project
	$msbuild.Xml.AddImport($relativePath) | out-null
	$project.Save()

	# ***************************************************
	# Modify the per-project fxcop settings file to point at the project dll
	# ***************************************************
	Write-Host "Adding default target to FxCop.FxCop file"
	$fxCopFile = [System.IO.Path]::GetDirectoryName($project.filename) + "\FxCop.FxCop"
	$projectUri = new-object Uri('file://' + $project.FullName)
	$targetUri = new-object Uri('file://' + $fxCopFile)
	$relativePath = $projectUri.MakeRelativeUri($targetUri).ToString().Replace([System.IO.Path]::AltDirectorySeparatorChar, [System.IO.Path]::DirectorySeparatorChar)
	$settings = [xml](get-content($fxCopFile))
	$settings.SelectSingleNode("//Target").SetAttribute("Name", "`$(ProjectDir)/bin/Release/" + $project.name + ".dll")
	$settings.Save($fxCopFile)
}
