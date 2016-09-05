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
	$assemblyfileversion = $version.Split('-', 2)[0]
	$assemblyversion = $assemblyfileversion

    $outputDir = "$baseDir\Build\Output"
    $net35Path = "$baseDir\Insight.Database\bin\NET35"
    $net40Path = "$baseDir\Insight.Database\bin\NET40"
    $net45Path = "$baseDir\Insight.Database\bin\NET45"
    $monoPath = "$baseDir\Insight.Database\bin\Mono"

    $framework = "$env:systemroot\Microsoft.NET\Framework\v4.0.30319\"
    $msbuild = $framework + "msbuild.exe"
    $configuration = "Release"
    $nuget = "nuget.exe"
    $nunit = Get-ChildItem "$baseDir\packages" -Recurse -Include nunit-console.exe
    $nunitx86 = Get-ChildItem "$baseDir\packages" -Recurse -Include nunit-console-x86.exe
}

Task default -depends Build
Task Build -depends Build45,Build40,Build35,BuildMono
Task Test -depends Test45,Test40,Test35

function Replace-Version {
    param (
        [string] $Path
    )

    $x = (Get-Content $Path) |
		% { $_ -replace "\[assembly: AssemblyVersion\(`"(\d+\.?)*`"\)\]","[assembly: AssemblyVersion(`"$assemblyversion`")]" } |
		% { $_ -replace "\[assembly: AssemblyFileVersion\(`"(\d+\.?)*`"\)\]","[assembly: AssemblyFileVersion(`"$assemblyfileversion`")]" } |
		Out-String

	$x.Trim() | Out-File $Path 
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

function Do-Build35 {
	param (
		[string] $Project,
		[string] $ProjectFile
	)

	if ($ProjectFile -eq "") { $ProjectFile = $Project }

	$args = "/p:Configuration=$configuration /p:TargetFrameworkVersion=v3.5 /p:DefineConstants=""NODBASYNC;NODYNAMIC;NET35"" '/t:Clean;Build'"
	echo "Build Args: $args"

	Exec {
		Invoke-Expression "$msbuild $baseDir\$Project\$ProjectFile.csproj $args"
	}

    # copy the binaries to the net35 folder
    Copy-Item $baseDir\$Project\bin\Release\*.* $net35Path
}

Task Build35 {
    ReplaceVersions

    try {
        Wipe-Folder $net35Path

        # build the NET35 binaries
		Do-Build35 "Insight.Database"
		Do-Build35 "Insight.Database.Configuration"
		Do-Build35 "Insight.Database.Compatibility3x"
		Do-Build35 "Insight.Database.Providers.Default"
		Do-Build35 "Insight.Database.Providers.PostgreSQL"
		Do-Build35 "Insight.Tests"
		Do-Build35 "Insight.Tests.Compatibility3x"
    }
    finally {
        RestoreVersions
    }
}


function Do-Build40 {
	param (
		[string] $Project
	)

	$args = " /p:Configuration=$configuration /p:TargetFrameworkVersion=v4.0 /p:DefineConstants=""NODBASYNC;CODE_ANALYSIS"" '/t:Clean;Build'"
	echo "Build Args: $args"

	Exec {
        Invoke-Expression "$msbuild $baseDir\$Project\$Project.csproj $args"
	}

    # copy the binaries to the net40 folder
    Copy-Item $baseDir\$Project\bin\Release\*.* $net40Path
}

Task Build40 {
    ReplaceVersions

    try {
        # build the NET40 binaries
		Do-Build40 "Insight.Database"
		Do-Build40 "Insight.Database.Configuration"
		Do-Build40 "Insight.Database.Compatibility3x"
		Do-Build40 "Insight.Database.Json"
		Do-Build40 "Insight.Database.Providers.Default"
		Do-Build40 "Insight.Database.Providers.DB2"
		Do-Build40 "Insight.Database.Providers.Glimpse"
		Do-Build40 "Insight.Database.Providers.Oracle"
		Do-Build40 "Insight.Database.Providers.OracleManaged"
		Do-Build40 "Insight.Database.Providers.PostgreSQL"
		Do-Build40 "Insight.Tests"
		Do-Build40 "Insight.Tests.Compatibility3x"
    }
    finally {
        RestoreVersions
    }
}

Task Build45 {
    ReplaceVersions

    try {
        # build the NET45 binaries

		$args = "'/p:Configuration=$configuration' '/t:Clean;Build'"
		echo "Build Args: $args"

        Exec {
            Invoke-Expression "$msbuild $baseDir\Insight.sln $args"
        }
 
        # copy the binaries to the net45 folder
        Wipe-Folder $net45Path
        Copy-Item $baseDir\*.*\bin\Release\*.* $net45Path
	}
    finally {
        RestoreVersions
    }
}

Task BuildMono {
    ReplaceVersions

    try {
		$args = "/p:Configuration=$configuration /p:DefineConstants=""MONO"" '/t:Clean;Build'"
		echo "Build Args: $args"

        # build the binaries for mono
        Exec {
            Invoke-Expression "$msbuild $baseDir\Insight.Database\Insight.Database.csproj $args"
        }
 
        # copy the binaries to the mono folder
        Wipe-Folder $monoPath
        Copy-Item $baseDir\Insight.Database\bin\Release\*.* $monoPath
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

			try {
				$x = (Get-Content $nuspec) |
					% { $_ -replace "<dependency id=`"Insight.Database.Core`" version=`"(\d+\.?)*`">","<dependency id=`"Insight.Database.Core`" version=`"$version`">" } |
					Out-String
				$x.Trim() | Out-File $nuspec 

				Invoke-Expression "$nuget pack $nuspec -OutputDirectory $outputDir -Version $version -NoPackageAnalysis"
			}
			finally {
		        git checkout $nuspec
			}
		}
	}
}

Task Package -depends Test, PackageOnly {
}
