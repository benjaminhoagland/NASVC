try 
{
    Write-Output "Registering new service NASVC..."
    New-Service -Name NASVC -BinaryPathName 'C:\NodeAlive\NASVC\NASVC\bin\Debug\nasvc.exe' -DisplayName 'NodeAlive Scheduler Service' -Description 'A service that runs in the background for NodeAlive.'; 
    Write-Output "Starting service NASVC..."
    Start-Service NASVC; 
    Write-Output "Checking status of service NASVC..."
    Get-Service NASVC;
    if($(Get-Service NASVC).Status -ne 'Running')
    {
        Write-Output 'Service NASVC failed to start during post-build. Exiting now.';
        exit 1;
    }
}
catch   
{
    exit 1;
}