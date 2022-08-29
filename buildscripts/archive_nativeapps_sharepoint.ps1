param(
  [Parameter(Mandatory)]
  $CertificateFilePath,
  [Parameter(Mandatory)]
  $FileToUploadRootFolder,
  [Parameter(Mandatory)]
  $SharePointFolderPath)

#! /usr/bin/env pwsh
$ErrorActionPreference = "Stop"

if (-Not (Get-Module -ListAvailable -Name PnP.PowerShell -ErrorAction SilentlyContinue)) {
  Write-Warning "Installing YAML powershell module"

  Set-PSRepository -Name "PSGallery" -InstallationPolicy Trusted
  Install-Module PnP.PowerShell -Scope "CurrentUser" -Verbose -Force
}

Import-Module PnP.PowerShell

function Main {
  $spSiteUrl = "$($env:SP_SITE_URL)"
  $clientId = $($env:SP_CLIENT_ID)
  $tenant = $($env:SP_TENANT)

  Write-Host "Sharepoint Site Url:" $spSiteUrl ", Sharepoint Folder Path:" $SharePointFolderPath ", Native app file(s) to upload root folder:" $FileToUploadRootFolder
  Write-Host "ClientId:" $clientId ", Tenant:" $tenant ", CertificatePath:" $CertificateFilePath

  try {
    $connection = Connect-PnPOnline -Url $spSiteUrl -ClientId $clientId -Tenant $tenant -CertificatePath $CertificateFilePath
    if (-not (Get-PnPContext)) {
      Write-Host "Error connecting to SharePoint Online, unable to establish context"
      throw
    }
  }
  catch {
    Write-Host "Error connecting to SharePoint Online: $($_.Exception.Message)"
    throw
  }

  try {
    $nativeAppsToUploadList = Get-ChildItem -Path $FileToUploadRootFolder

    foreach ($file in $nativeAppsToUploadList)
    {
      Write-Host "Uploading native app: " $file.FullName "to SharePoint folder: " $SharePointFolderPath
      Add-PnPFile -Path $file.FullName -Folder $SharePointFolderPath
    }
  }
  catch {
    Write-Host "Error uploading files to SharePoint Online: $($_.Exception.Message)"
  }
}

Main