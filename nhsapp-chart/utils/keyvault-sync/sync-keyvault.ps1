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

  Write-Host "Syncing secrets from $($Config.source) to $($Config.dest)"

  if (Get-AzKeyVaultSecret -VaultName $Config.dest -Name $Secret.Name -ErrorAction SilentlyContinue) {
    if (-Not $Config.overwriteExisting) {
      throw "Secret '$($Secret.Name)' already exists in destination keyvault '$($config.dest)'"
    }
  }

  Set-AzKeyVaultSecret -VaultName $Config.dest `
    -Name $Secret.Name `
    -Expires $Secret.Expires `
    -NotBefore $Secret.NotBefore `
    -ContentType $Secret.ContentType `
    -Tags $Secret.Tags `
    -SecretValue $Secret.SecretValue `
    -WhatIf:$Config.dryRun

  Write-Host -ForegroundColor Green "Secret '$($Secret.Name)' synced to destination keyvault '$($Config.dest)'"
}

function Sync-Secrets {
  param($Config)

  Write-Host "Syncing secrets from $($Config.source) to $($Config.dest)"

  $Config.secrets `
    | ForEach-Object { Export-Secret -VaultName $Config.source -SecretName $_ } `
    | ForEach-Object { Sync-Secret -Config $Config -Secret $_ }
}

function Export-Cert {
  param($VaultName, $CertName)

  Write-Host "Exporting certificate '${CertName}' from source keyvault '${VaultName}'"

  $kvSecret = Export-Secret -VaultName $VaultName -SecretName $CertName `
    | Select-Object -ExpandProperty SecretValue `
    | ConvertFrom-SecureString -AsPlainText

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

  Write-Host "Importing certificate '$($Cert.Name)' to destination keyvault '$($Config.source)'"

  Import-AzKeyVaultCertificate -VaultName $Config.dest `
    -Name $Cert.Name `
    -CertificateCollection $certCollection `
    -WhatIf:$Config.dryRun

  Write-Host -ForegroundColor Green "Certificate '$($Cert.Name)' synced to destination keyvault '$($Config.dest)'"
}

function Sync-Certs {
  param($Config)

  Write-Host "Syncing certificates from $($Config.source) to $($Config.dest)"

  $Config.certs `
    | ForEach-Object { Get-AzKeyVaultCertificate -VaultName $Config.source -Name $_ } `
    | ForEach-Object { Sync-Cert -Config $Config -Cert $_ }
}

function Main {
  Write-Host "Loading config from path: ${configPath}"

  $config = Get-Content -Raw -Path $configPath | ConvertFrom-Yaml

  if (-Not (Get-AzContext)) {
    Write-Warning "You will now be asked to sign into Azure (context will be cached for future runs)"

    Connect-AzAccount
  }

  Sync-Certs -Config $config
  Sync-Secrets -Config $config
}

Main
