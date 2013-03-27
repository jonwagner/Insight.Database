﻿$psake.use_exit_on_error = $true

#########################################
# to build a new version
# 1. git tag 1.0.x
# 2. build package
#########################################

properties {
    $baseDir = $psake.build_script_dir

    $version = git describe --abbrev=0 --tags
    $changeset = (git log -1 $version --pretty=format:%H)

    # assemblyversion is the version of the assembly that this build is compatible with
    # change this when there is a breaking change to the assembly
    $assemblyversion = (Get-Content $baseDir\assemblyversion.txt)

    $outputDir = "$baseDir\Build\Output"
    $net40Path = "$baseDir\Insight.Database\bin\NET40"

    $framework = "$env:systemroot\Microsoft.NET\Framework\v4.0.30319\"
    $msbuild = $framework + "msbuild.exe"
    $configuration = "Release"
    $nuget = "$baseDir\.nuget\nuget.exe"
    $nunit = Get-ChildItem "$baseDir\packages" -Recurse -Include nunit-console.exe
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
		% { $_ -replace "\[assembly: AssemblyFileVersion\(`"(\d+\.?)*`"\)\]","[assembly: AssemblyFileVersion(`"$version`")]" } |
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
            Invoke-Expression "$msbuild $baseDir\Insight.sln /p:Configuration=$configuration /p:TargetFrameworkVersion=v4.0 /p:DefineConstants=```"NODBASYNC``;CODE_ANALYSIS```" '/t:Clean;Build'"
        }

        # copy the binaries to the net40 folder
        Wipe-Folder $net40Path
        Copy-Item $baseDir\Insight.Database\bin\Release\*.* $net40Path
        Copy-Item $baseDir\Insight.Tests\bin\Release\*.* $net40Path
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

Task Test45 -depends Build45 { 
    Exec {
        Invoke-Expression "$nunit $baseDir\Insight.Tests\bin\$configuration\Insight.Tests.dll"
    }
}

Task Package -depends Test {
    Wipe-Folder $outputDir
 
    # package the snippets
    Exec {
        Invoke-Expression "$baseDir\Build\zip.exe $outputDir\InsightCodeSnippets.vsi $baseDir\Insight.Database\CodeSnippets\*.*"
    }

    # package nuget
    Exec {
        Invoke-Expression "$nuget pack $baseDir\Insight.Database\Insight.Database.nuspec -OutputDirectory $outputDir -Version $version"
    }
}
