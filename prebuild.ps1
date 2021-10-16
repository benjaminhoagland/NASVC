if($PSVersionTable.psversion.major -lt 7)
{
    Write-Output 'Powershell 7.0 or greater is required for this script.'
    $version = "$($psversiontable.psversion.major).$($psversiontable.psversion.minor)";
    Write-Output "This script was executed with Powershell version $version.";
    Write-Output "Exiting now."
    exit 1;
}

try 
{
    Write-Output 'Looking for installed service NASVC...'
    $foundService = Get-Service 'NASVC';
    if($null -ne $foundService)
    {
        Write-Output "Found service NASVC. Attempting to stop this service..."
        Write-Output 'Stopping service NASVC...'
        Stop-Service NASVC -ErrorAction SilentlyContinue;
        Write-Output 'Unregistering (removing) service NASVC...'
        Remove-Service NASVC; 
    }
    else
    {
        Write-Output 'Service not found. Continuing build...'
    }
}
catch 
{
    exit 1    
}

