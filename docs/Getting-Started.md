# nhsapp

[[_TOC_]]

## Configure Git

```bash
git config --global user.name "Your name"
git config --global user.email "Your email address"
git config --global pull.rebase true
git config --global push.default current
git config --global core.autocrlf false
git config --global fetch.prune true
git config --global fetch.pruneTags true
```

## Get code

Clone the repo: https://nhsapp@dev.azure.com/nhsapp/NHS%20App/_git/nhsapp

```bash
git clone https://nhsapp@dev.azure.com/nhsapp/NHS%20App/_git/nhsapp
```

## Commit code

Review the [Git Branching Strategy](https://confluence.service.nhs.uk/display/NO/Git+Branching+Strategy).

## Host file entries

Before running the app locally, some entries need to be added to your machine's `hosts` file (`/etc/hosts` on Mac or `C:\windows\system32\drivers\etc\hosts` on Windows) to add the following entries:

```bash
127.0.0.1 api.local.bitraft.io
127.0.0.1 cid.local.bitraft.io
127.0.0.1 minimock.local.bitraft.io
127.0.0.1 mongodb.bitraft.io
127.0.0.1 servicejourneyrulesapi.local.bitraft.io
127.0.0.1 silver.local.bitraft.io
127.0.0.1 web.local.bitraft.io
127.0.0.1 stubs.local.bitraft.io
127.0.0.1 auth.nhslogin.stubs.local.bitraft.io
127.0.0.1 uaf.nhslogin.stubs.local.bitraft.io
127.0.0.1 ers.stubs.local.bitraft.io
```

## Azure DevOps Feeds

[Create a Personal Access Token](How-Tos/configure-azure-dev-ops-feeds.md#create-personal-access-token) for Azure DevOps.

Run the following command in the root of the repository:

```bash
make configure-package-feed
```

You will be prompted for parameters when needed. Alternatively you can follow the [manual instructions](How-Tos/configure-azure-dev-ops-feeds.md#manually-configure-feeds).

## Secrets

Secrets required for running the app locally are stored in Keybase (`team/nhsonline/development_secrets`). These are automatically copied to your home directory (`~/.nhsonline/secrets`) when the app is run using make if the keybase filesystem integration is working. If it is not an error will be reported and the files should be manually copied across.
