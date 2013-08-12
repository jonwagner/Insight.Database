$psake.use_exit_on_error = $true

#########################################
# to build a new version
# 1. git tag 1.0.x
# 2. build package
#########################################

properties {
    $baseDir = $psake.build_script_dir

    $version = git describe --abbrev=0 --tags
    $changeset = (git log -1 $version --pretty=format:%H)
	$assemblyversion = $version.Split('-', 2)[0]

    $outputDir = "$baseDir\Build\Output"
    $net40Path = "$baseDir\Insight.Database\bin\NET40"

    $framework = "$env:systemroot\Microsoft.NET\Framework\v4.0.30319\"
    $msbuild = $framework + "msbuild.exe"
    $configuration = "Release"
    $nuget = "$baseDir\.nuget\nuget.exe"
    $nunit = Get-ChildItem "$baseDir\packages" -Recurse -Include nunit-console.exe
    $nunitx86 = Get-ChildItem "$baseDir\packages" -Recurse -Include nunit-console-x86.exe
}

Task default -depends Build
Task Build -depends Build40,Build45
Task Test -depends Test40,Test45

function Replace-Version {
    param (
        [string] $Path
    )

    (Get-Content $Path) |
		% { $_ -replace "\[assembly: AssemblyVersion\(`"(\d+\.?)*`"\)\]","[assembly: AssemblyVersion(`"$assemblyversion`")]" } |
		% { $_ -replace "\[assembly: AssemblyFileVersion\(`"(\d+\.?)*`"\)\]","[assembly: AssemblyFileVersion(`"$assemblyversion`")]" } |
		Set-Content $Path
}

function ReplaceVersions {
    Get-ChildItem $baseDir -Include AssemblyInfo.cs -Recurse |% { Replace-Version $_.FullName }
}

function RestoreVersions {
    Get-ChildItem $baseDir -Include AssemblyInfo.cs -Recurse |% {
        git checkout $_.FullName
    }
}

function Wipe-Folder {
    param (
        [string] $Path
    )

    if (Test-Path $Path) { Remove-Item $Path -Recurse }
    New-Item -Path $Path -ItemType Directory | Out-Null
}

Task Build40 {
    ReplaceVersions

    try {
        # build the NET40 binaries
        Exec {
            Invoke-Expression "$msbuild $baseDir\Insight.sln /p:Configuration=$configuration /p:TargetFrameworkVersion=v4.0 /p:DefineConstants=```"NET40``;``NODBASYNC``;CODE_ANALYSIS```" '/t:Clean;Build'"
        }

        # copy the binaries to the net40 folder
        Wipe-Folder $net40Path
        Copy-Item $baseDir\*.*\bin\Release\*.* $net40Path
    }
    finally {
        RestoreVersions
    }
}

Task Build45 {
    ReplaceVersions

    try {
        # build the NET45 binaries
        Exec {
            Invoke-Expression "$msbuild $baseDir\Insight.sln '/p:Configuration=$configuration' '/t:Clean;Build'"
        }
    }
    finally {
        RestoreVersions
    }
}

Task Test40 -depends Build40 { 
    Exec {
        Invoke-Expression "$nunit $net40Path\Insight.Tests.dll"
    }
}

Task Test45Only {
	Get-ChildItem $baseDir\Insight.Tests* |% {
	    Exec {
			if ($_.Name -eq 'Insight.Tests.Oracle') {
				# having trouble getting the 64-bit drivers installed on Windows 8, so use the 32-bit drivers
				Invoke-Expression "$nunitx86 $($_.FullName)\bin\$configuration\$($_.Name).dll"
			}
			else {
				Invoke-Expression "$nunit $($_.FullName)\bin\$configuration\$($_.Name).dll"
			}
		}
	}
}

Task Test45 -depends Build45, Test45Only { 
}

Task PackageOnly {
    Wipe-Folder $outputDir
 
    # package the snippets
    Exec {
        Invoke-Expression "$baseDir\Build\zip.exe $outputDir\InsightCodeSnippets.vsi $baseDir\Insight.Database\CodeSnippets\*.*"
    }

    # package nuget
	Get-ChildItem $baseDir\Insight* |% { Get-ChildItem -Path $_ } |? Extension -eq .nuspec |% {
		Exec {
			$nuspec = $_.FullName
			Invoke-Expression "$nuget pack $nuspec -OutputDirectory $outputDir -Version $version -NoPackageAnalysis"
		}
	}
}

Task Package -depends Test, PackageOnly {
}