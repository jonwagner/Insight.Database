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
    $net35Path = "$baseDir\Insight.Database\bin\NET35"
    $net40Path = "$baseDir\Insight.Database\bin\NET40"
    $net45Path = "$baseDir\Insight.Database\bin\NET45"

    $framework = "$env:systemroot\Microsoft.NET\Framework\v4.0.30319\"
    $msbuild = $framework + "msbuild.exe"
    $configuration = "Release"
    $nuget = "$baseDir\.nuget\nuget.exe"
    $nunit = Get-ChildItem "$baseDir\packages" -Recurse -Include nunit-console.exe
    $nunitx86 = Get-ChildItem "$baseDir\packages" -Recurse -Include nunit-console-x86.exe
}

Task default -depends Build
Task Build -depends Build45,Build40,Build35
Task Test -depends Test45,Test40,Test35

function Replace-Version {
    param (
        [string] $Path
    )

    (Get-Content $Path) |
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

Task Build35 {
    ReplaceVersions

    try {
        # build the NET35 binaries
        Exec {
            Invoke-Expression "$msbuild $baseDir\Insight.Database\Insight.Database.csproj /p:Configuration=$configuration /p:TargetFrameworkVersion=v3.5 /p:DefineConstants=```"NODBASYNC``;NODYNAMIC``;NET35```" '/t:Clean;Build'"
		}
        Exec {
            Invoke-Expression "$msbuild $baseDir\Insight.Database.Providers.Default\Insight.Database.Providers.Default.csproj /p:Configuration=$configuration /p:TargetFrameworkVersion=v3.5 /p:DefineConstants=```"NODBASYNC``;NODYNAMIC``;NET35```" '/t:Clean;Build'"
		}
        Exec {
            Invoke-Expression "$msbuild $baseDir\Insight.Database.Compatibility3x\Insight.Database.Compatibility3x.csproj /p:Configuration=$configuration /p:TargetFrameworkVersion=v3.5 /p:DefineConstants=```"NODBASYNC``;NODYNAMIC``;NET35```" '/t:Clean;Build'"
		}
		Exec {
            Invoke-Expression "$msbuild $baseDir\Insight.Database.Providers.PostgreSQL\Insight.Database.Providers.PostgreSQL.NET35.csproj /p:Configuration=$configuration /p:TargetFrameworkVersion=v3.5 /p:DefineConstants=```"NODBASYNC``;NODYNAMIC``;NET35```" '/t:Clean;Build'"
        }
		Exec {
		    Invoke-Expression "$msbuild $baseDir\Insight.Tests\Insight.Tests.csproj /p:Configuration=$configuration /p:TargetFrameworkVersion=v3.5 /p:DefineConstants=```"NODBASYNC``;NODYNAMIC``;NET35```" '/t:Clean;Build'"
        }
		Exec {
		    Invoke-Expression "$msbuild $baseDir\Insight.Tests.Compatibility3x\Insight.Tests.Compatibility3x.csproj /p:Configuration=$configuration /p:TargetFrameworkVersion=v3.5 /p:DefineConstants=```"NODBASYNC``;NODYNAMIC``;NET35```" '/t:Clean;Build'"
        }

        # copy the binaries to the net35 folder
        Wipe-Folder $net35Path
        Copy-Item $baseDir\Insight.Database\bin\Release\*.* $net35Path
        Copy-Item $baseDir\Insight.Database.Providers.Default\bin\Release\*.* $net35Path
        Copy-Item $baseDir\Insight.Database.Compatibility3x\bin\Release\*.* $net35Path
        Copy-Item $baseDir\Insight.Database.Json\bin\Release\*.* $net35Path
        Copy-Item $baseDir\Insight.Database.Providers.PostgreSQL\bin\Release\*.* $net35Path
        Copy-Item $baseDir\Insight.Tests\bin\Release\*.* $net35Path
        Copy-Item $baseDir\Insight.Tests.Compatibility3x\bin\Release\*.* $net35Path
    }
    finally {
        RestoreVersions
    }
}

Task Build40 {
    ReplaceVersions

    try {
        # build the NET40 binaries
        Exec {
            Invoke-Expression "$msbuild $baseDir\Insight.Database\Insight.Database.csproj /p:Configuration=$configuration /p:TargetFrameworkVersion=v4.0 /p:DefineConstants=```"NODBASYNC``;CODE_ANALYSIS```" '/t:Clean;Build'"
        }
        Exec {
            Invoke-Expression "$msbuild $baseDir\Insight.Database.Providers.Default\Insight.Database.Providers.Default.csproj /p:Configuration=$configuration /p:TargetFrameworkVersion=v4.0 /p:DefineConstants=```"NODBASYNC``;CODE_ANALYSIS```" '/t:Clean;Build'"
        }
        Exec {
            Invoke-Expression "$msbuild $baseDir\Insight.Database.Compatibility3x\Insight.Database.Compatibility3x.csproj /p:Configuration=$configuration /p:TargetFrameworkVersion=v4.0 /p:DefineConstants=```"NODBASYNC``;CODE_ANALYSIS```" '/t:Clean;Build'"
        }
		Exec {
		    Invoke-Expression "$msbuild $baseDir\Insight.Database.Providers.Glimpse\Insight.Database.Providers.Glimpse.csproj /p:Configuration=$configuration /p:TargetFrameworkVersion=v4.0 /p:DefineConstants=```"NODBASYNC``;CODE_ANALYSIS```" '/t:Clean;Build'"
        }
		Exec {
		    Invoke-Expression "$msbuild $baseDir\Insight.Database.Providers.PostgreSQL\Insight.Database.Providers.PostgreSQL.csproj /p:Configuration=$configuration /p:TargetFrameworkVersion=v4.0 /p:DefineConstants=```"NODBASYNC``;CODE_ANALYSIS```" '/t:Clean;Build'"
        }
		Exec {
		    Invoke-Expression "$msbuild $baseDir\Insight.Tests\Insight.Tests.csproj /p:Configuration=$configuration /p:TargetFrameworkVersion=v4.0 /p:DefineConstants=```"NODBASYNC``;CODE_ANALYSIS```" '/t:Clean;Build'"
        }
		Exec {
		    Invoke-Expression "$msbuild $baseDir\Insight.Tests.Compatibility3x\Insight.Tests.Compatibility3x.csproj /p:Configuration=$configuration /p:TargetFrameworkVersion=v4.0 /p:DefineConstants=```"NODBASYNC``;CODE_ANALYSIS```" '/t:Clean;Build'"
        }

        # copy the binaries to the net40 folder
        Wipe-Folder $net40Path
        Copy-Item $baseDir\Insight.Database\bin\Release\*.* $net40Path
        Copy-Item $baseDir\Insight.Database.Providers.Default\bin\Release\*.* $net40Path
        Copy-Item $baseDir\Insight.Database.Compatibility3x\bin\Release\*.* $net40Path
        Copy-Item $baseDir\Insight.Database.Json\bin\Release\*.* $net40Path
        Copy-Item $baseDir\Insight.Database.Providers.Glimpse\bin\Release\*.* $net40Path
        Copy-Item $baseDir\Insight.Database.Providers.PostgreSQL\bin\Release\*.* $net40Path
        Copy-Item $baseDir\Insight.Tests\bin\Release\*.* $net40Path
        Copy-Item $baseDir\Insight.Tests.Compatibility3x\bin\Release\*.* $net40Path
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
 
        # copy the binaries to the net45 folder
        Wipe-Folder $net45Path
        Copy-Item $baseDir\*.*\bin\Release\*.* $net45Path
   }
    finally {
        RestoreVersions
    }
}

Task Test35 -depends Build35 { 
    Exec {
        Invoke-Expression "$nunit $net35Path\Insight.Tests.dll"
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
			if ($_.Name -eq 'Insight.Tests.SQLite') {
				Invoke-Expression "$nunit $baseDir\$($_.Name)\bin\Release\$($_.Name).dll"
			}
			else {
				Invoke-Expression "$nunit $net45Path\$($_.Name).dll"
			}
		}
	}
}

Task Test45 -depends Build45, Test45Only { 
}

Task PackageOnly {
    Wipe-Folder $outputDir
 
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