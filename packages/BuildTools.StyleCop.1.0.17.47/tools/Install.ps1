param($installPath, $toolsPath, $package, $project)

# if there is a project file, then modify it
if ($project)
{
	# ***************************************************
	# Modify the build project to import the stylecop.targets file
	# ***************************************************
	# This is the MSBuild targets file to add
	$targetsFile = [System.IO.Path]::Combine($toolsPath, 'StyleCop.targets')
	$settingsFile = [System.IO.Path]::Combine($toolsPath, 'Settings.StyleCop')

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
	# Modify the per-project stylecop settings file to point at the global settings
	# ***************************************************
	$targetUri = new-object Uri('file://' + $settingsFile)
	$relativePath = $projectUri.MakeRelativeUri($targetUri).ToString().Replace([System.IO.Path]::AltDirectorySeparatorChar, [System.IO.Path]::DirectorySeparatorChar)
	$settings = [xml](get-content([System.IO.Path]::GetDirectoryName($project.filename) + "\Settings.StyleCop"))
	$settings.SelectSingleNode("//StringProperty[`@Name='LinkedSettingsFile']").InnerText = $relativePath
	$settings.Save([System.IO.Path]::GetDirectoryName($project.filename) + "\Settings.StyleCop")
}
