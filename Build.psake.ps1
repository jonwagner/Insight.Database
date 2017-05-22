Framework '4.5.2'
$psake.use_exit_on_error = $true

#########################################
# to build a new version
# 1. git tag 1.0.x
# 2. build package
#########################################

properties {
    $baseDir = $psake.build_script_dir
    $outputDir = "$baseDir\Build\Output"
    $configuration = 'Release'
}

Task default -depends Build

function Wipe-Folder {
    param (
        [string] $Path
    )

    if (Test-Path $Path) {
        Remove-Item $Path -Recurse -Force -ErrorAction SilentlyContinue 
    }
    [System.IO.Directory]::CreateDirectory($Path) | Out-Null
}

Task Clean {
	Get-ChildItem $baseDir\Insight*\*.csproj | %{ dotnet clean $_ -c $configuration  }
}

Task Restore {
	Get-ChildItem $baseDir\Insight*\*.csproj | %{ dotnet restore $_ }
}

Task Build -depends Restore {
	Get-ChildItem $baseDir\Insight*\*.csproj | %{ dotnet build $_ -c $configuration }
}

Task Test {
	Get-ChildItem $baseDir\Insight.Tests*\*.csproj | %{ dotnet test $_ -c $configuration }
}

Task TestOnly {
	Get-ChildItem $baseDir\Insight.Tests*\*.csproj | %{ dotnet test $_ -c $configuration --no-build }
}

Task PackageOnly {
    Wipe-Folder $outputDir
	Get-ChildItem $baseDir\Insight.Database*\**\**\*.nupkg | Remove-Item
	Get-ChildItem $baseDir\Insight.Database*\*.csproj | %{ dotnet pack $_ -c $configuration }
	Get-ChildItem $baseDir\Insight.Database*\**\**\*.$version.nupkg | Copy-Item -Destination $outputDir
}

Task Package -depends Clean, Build, Test, PackageOnly {
}
