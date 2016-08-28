param (
    [string] $target = "default"
)

# If Invoke-psake does not work, you need to install Psake
# See building.txt in this dir

Invoke-psake .\Build.psake.ps1 -taskList $target
