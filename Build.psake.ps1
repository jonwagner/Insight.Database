Framework '4.5.2'
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

    $configuration = 'Release'
    $nuget = 'nuget.exe'
    $nunit = (Get-ChildItem $baseDir\packages nunit3-console.exe -Recurse)[0].fullname
}

Task default -depends Build
Task Build -depends Build45,Build40,Build35
Task Test -depends Test45,Test40,Test35

function Replace-Version {
    param (
        [string] $Path
    )
    $x = Get-Content -raw $Path | `
		% { $_ -replace "\[assembly: AssemblyVersion\(`"(\d+\.?)*`"\)\]","[assembly: AssemblyVersion(`"$assemblyversion`")]" } | `
		% { $_ -replace "\[assembly: AssemblyFileVersion\(`"(\d+\.?)*`"\)\]","[assembly: AssemblyFileVersion(`"$assemblyfileversion`")]" }

	[System.IO.File]::WriteAllText($Path, $x.Trim())
}

function ReplaceVersions {
    Get-ChildItem $baseDir AssemblyInfo.cs -Recurse | %{ Replace-Version $_.FullName }
}

function RestoreVersions {
    Get-ChildItem $baseDir AssemblyInfo.cs -Recurse | %{ git checkout $_.FullName }
}

function Wipe-Folder {
    param (
        [string] $Path
    )

    if (Test-Path $Path) {
        Remove-Item $Path -Recurse -Force -ErrorAction SilentlyContinue 
    }
    [System.IO.Directory]::CreateDirectory($Path) | Out-Null
}

function Do-Build {
    Param
    (
        [String]
        $Framework,

        [String]
        $OutputPath,

		[String]
		$FrameworkFolder,

        [String[]]
        $Projects,

        [String[]]
        $DefineConstants
    )

    $parameters = @('/m', '/t:Clean;Build', "/p:Configuration=$configuration", "/p:OutputPath=`"$OutputPath`"", "/p:FrameworkFolder=$FrameworkFolder")
    
    if ($Framework) {
        $parameters += "/p:TargetFrameworkVersion=v$Framework"
    }

    if ($DefineConstants) {
        $parameters += "/p:DefineConstants=`"$([String]::Join(';', $DefineConstants))`""
    }

    if ($VerbosePreference -eq 'SilentlyContinue') {
        $parameters += '/verbosity:quiet'
    }

    if ($Projects) {
        $projectFiles = $Projects | %{ "$baseDir\$_\$_.csproj" }
    } else {
        # Default to building the solution, if no projects are specified
        $projectFiles = @("$baseDir\Insight.sln")
    }

    Wipe-Folder $OutputPath 
    ReplaceVersions
    try {
        foreach ($project in $projectFiles) {
            Write-Output "Running build: msbuild `"$project`" $parameters"
            exec { msbuild "`"$project`"" @parameters  }
        }
    }
    finally {
        RestoreVersions
    }
}

Task Build35 {
    Do-Build -Framework 3.5 -OutputPath $net35Path -DefineConstants NODBASYNC,NODYNAMIC,NET35 -FrameworkFolder NET35 -Projects `
		Insight.Database,`
		Insight.Database.Configuration,`
		Insight.Database.Providers.Default,`
		Insight.Tests
}

Task Build40 {
    Do-Build -Framework 4.0 -OutputPath $net40Path -DefineConstants NODBASYNC,CODE_ANALYSIS,NET40 -FrameworkFolder NET40 -Projects `
		Insight.Database,`
		Insight.Database.Configuration,`
		Insight.Database.Json,`
		Insight.Database.Providers.Default,`
		Insight.Database.Providers.DB2,`
		Insight.Database.Providers.Glimpse,`
		Insight.Database.Providers.Oracle,`
		Insight.Database.Providers.OracleManaged,`
		Insight.Tests
}

Task Build45 {
    Do-Build -Framework 4.5 -OutputPath $net45Path -DefineConstants CODE_ANALYSIS -FrameworkFolder NET45
}

Task Test35Only { 
    exec { & $nunit $net35Path\Insight.Tests.dll }
}

Task Test35 -depends Build35, Test35Only { 
}

Task Test40Only { 
    exec { & $nunit $net40Path\Insight.Tests.dll }
}

Task Test40 -depends Build40, Test40Only { 
}

Task Test45Only {
	Get-ChildItem $baseDir\Insight.Tests* | %{
		exec { & $nunit "$net45Path\$($_.Name).dll" }
	}
}

Task Test45 -depends Build45, Test45Only { 
}

Task PackageOnly {
    Wipe-Folder $outputDir

	# package nuget
	Get-ChildItem $baseDir\Insight* |% { Get-ChildItem -Path $_ } |? Extension -eq .nuspec |% {
        $nuspec = $_.FullName

        $x = Get-Content $nuspec | %{ 
            $_ -replace "<dependency id=`"Insight.Database.Core`" version=`"(\d+\.?)*`">","<dependency id=`"Insight.Database.Core`" version=`"$version`">" 
        }

        try {
            Set-Content $nuspec $x.Trim()
            exec { & $nuget pack $nuspec -OutputDirectory $outputDir -Version $version -NoPackageAnalysis }
        }
        finally {
            git checkout $nuspec
        }
	}
}

Task Package -depends Test, PackageOnly {
}