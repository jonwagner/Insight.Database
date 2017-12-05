$psake.use_exit_on_error = $true

#########################################
# to build a new version
# 1. git tag 1.0.x
# 2. build package
#########################################

exec { & $env:VS150COMNTOOLS\vsmsbuildcmd.bat }

properties {
    $baseDir = $psake.build_script_dir
    $outputDir = "$baseDir\Build\Output"
    $configuration = 'Release'
	$nunit = "$($env:USERPROFILE)\.nuget\packages\nunit.consolerunner\3.6.1\tools\nunit3-console.exe"
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
	exec { dotnet clean -c $configuration Insight.sln }
}

Task Restore {
	exec { dotnet restore Insight.sln }
}

Task Build -depends Restore {
	exec { dotnet build -c $configuration Insight.sln }
}

Task BuildCore {
	exec { dotnet build -c $configuration Insight.Database\Insight.Database.csproj -f netstandard2.0 }
}

Task TestCore {
	Get-ChildItem Insight.Tests\Insight.Tests.csproj | %{ exec { dotnet test $_ -c $configuration -f netcoreapp2.0 } }
}

Task Test -depends Build, TestOnly {
}

Task Test40 {
	Get-ChildItem $baseDir\Insight.Tests*\*.csproj | %{ 
		$testdll = $($_.Directory.ToString() + "\bin\Release\net40\" + $_.Name.Replace(".csproj", ".dll"))
		if (Test-Path ($testdll)) {
			exec { & $nunit $testdll }
		}
	}
}

Task TestOnly {
	Get-ChildItem $baseDir\Insight.Tests*\*.csproj | %{ exec { dotnet test $_ -c $configuration --no-build } }
}

Task PackageOnly {
    Wipe-Folder $outputDir
	Get-ChildItem $baseDir\Insight.Database*\**\**\*.nupkg | Remove-Item
	Get-ChildItem $baseDir\Insight.Database*\*.csproj | %{ exec { dotnet pack $_ -c $configuration } }
	Get-ChildItem $baseDir\Insight.Database*\**\**\*.nupkg | Copy-Item -Destination $outputDir
}

Task Package -depends Clean, Build, Test, Test40, PackageOnly {
}
