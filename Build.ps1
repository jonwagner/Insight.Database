param (
    [string] $target = "default"
)

Invoke-psake .\Build.psake.ps1 -taskList $target
