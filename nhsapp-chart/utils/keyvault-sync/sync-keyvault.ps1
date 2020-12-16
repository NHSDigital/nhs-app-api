#! /usr/bin/env pwsh
$ErrorActionPreference = "Stop"

Set-Item Env:\SuppressAzurePowerShellBreakingChangeWarnings "true"

if (-Not (Get-Module -ListAvailable -Name powershell-yaml -ErrorAction SilentlyContinue)) {
  Write-Warning "Installing YAML powershell module"
  
  Set-PSRepository -Name "PSGallery" -InstallationPolicy Trusted
  Install-Module powershell-yaml
}

Import-Module powershell-yaml

$scriptPath = $PSScriptRoot
$configPath = Join-Path $scriptPath "config.yml"

function Export-Secret {
  param($VaultName, $SecretName)

  Write-Host "Exporting secret '${SecretName}' from vault '${VaultName}'"

  $secret = Get-AzKeyVaultSecret -VaultName $VaultName -Name $SecretName

  return $secret
}

function Sync-Secret {
  param($Config, $Secret)

  Write-Host -ForegroundColor Yellow "Attempting to add secret '$($Secret.Name)' to destination keyvault '$($Config.source)'"

  if (Get-AzKeyVaultSecret -VaultName $Config.dest -Name $Secret.Name -ErrorAction SilentlyContinue) {
    if (-Not $Config.overwriteExisting) {
      throw "Secret '$($Secret.Name)' already exists in destination keyvault '$($config.dest)'"
    }
  }

  try {
    $secretParams = @{
      VaultName =  $Config.dest;
      Name =  $Secret.Name;
      Expires =  $Secret.Expires;
      NotBefore =  $Secret.NotBefore;
      ContentType =  $Secret.ContentType;
      Tags =  $Secret.Tags;
      SecretValue =  $Secret.SecretValue
    }

    Set-AzKeyVaultSecret @secretParams -WhatIf:$Config.dryRun
  
    Write-Host -ForegroundColor Green "Secret '$($Secret.Name)' added to destination keyvault '$($Config.dest)'"
  } catch {
    if (-Not $Config.ignoreAddSecretErrors) {
      throw $_.Exception
    }

    Write-Host -ForegroundColor Red "Error adding secret '$($Secret.Name)' to destination keyvault '$($Config.dest)':`n$($_.Exception.Message)"
  }
}

function Sync-Secrets {
  param($Config)

  Write-Host "Syncing secrets from $($Config.source) to $($Config.dest)"

  $secrets = $Config.secrets

  if ($secrets.GetType().Name -eq "String" -and $secrets.ToLower().Trim() -eq "all") {
    $secrets = Get-AzKeyVaultSecret -VaultName $Config.source | Select-Object -ExpandProperty Name
  }

  foreach ($secretName in $secrets) {
    $secret = Export-Secret -VaultName $Config.source -SecretName $secretName

    Sync-Secret -Config $Config -Secret $secret
  }
}

function Export-Cert {
  param($VaultName, $CertName)

  Write-Host "Exporting certificate '${CertName}' from source keyvault '${VaultName}'"

  $exportParams = @{
    VaultName = $VaultName;
    SecretName = $CertName
  }

  $kvSecret = Export-Secret @exportParams | Select-Object -ExpandProperty SecretValue | ConvertFrom-SecureString -AsPlainText
  $kvSecretBytes = [System.Convert]::FromBase64String($kvSecret)

  $certCollection = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2Collection
  $certCollection.Import($kvSecretBytes, $null, [System.Security.Cryptography.X509Certificates.X509KeyStorageFlags]::Exportable)

  return $certCollection
}

function Sync-Cert {
  param($Config, $Cert)

  if (Get-AzKeyVaultCertificate -VaultName $Config.dest -Name $Cert.Name -ErrorAction SilentlyContinue) {
    if (-Not $Config.overwriteExisting) {
      throw "Certificate '$($Cert.Name)' already exists in destination keyvault '$($config.dest)'"
    }
  }

  $certCollection = Export-Cert -VaultName $Config.source -CertName $Cert.Name 

  Write-Host -ForegroundColor Yellow "Attempting to import certificate '$($Cert.Name)' to destination keyvault '$($Config.source)'"

  try {
    $importParams = @{
      VaultName = $Config.dest;
      Name = $Cert.Name;
      CertificateCollection = $certCollection
    }

    Import-AzKeyVaultCertificate @importParams -WhatIf:$Config.dryRun

    Write-Host -ForegroundColor Green "Certificate '$($Cert.Name)' synced to destination keyvault '$($Config.dest)'"
  } catch {
    if (-Not $Config.ignoreCertImportErrors) {
      throw $_.Exception
    }

    Write-Host -ForegroundColor Red "Error importing cert '$($Cert.Name)' into destination keyvault '$($Config.dest)':`n$($_.Exception.Message)"
  }
}

function Sync-Certs {
  param($Config)

  Write-Host "Syncing certificates from $($Config.source) to $($Config.dest)"

  $certs = $Config.certs

  if ($certs.GetType().Name -eq "String" -and $certs.ToLower().Trim() -eq "all") {
    $certs = Get-AzKeyVaultCertificate -VaultName $Config.source | Select-Object -ExpandProperty Name
  }

  foreach ($certName in $certs) {
    $cert = Get-AzKeyVaultCertificate -VaultName $Config.source -Name $certName

    Sync-Cert -Config $Config -Cert $cert
  }
}

function Main {
  Write-Host "Loading config from path: ${configPath}"

  $config = Get-Content -Raw -Path $configPath | ConvertFrom-Yaml

  Write-Warning "You will now be asked to sign into Azure"
  Connect-AzAccount -UseDeviceAuthentication

  Sync-Certs -Config $config
  Sync-Secrets -Config $config
}

Main
