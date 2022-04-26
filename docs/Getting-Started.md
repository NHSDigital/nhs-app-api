# nhsapp

[[_TOC_]]

## Mac Prerequisites

* [XCode](https://developer.apple.com/xcode/)

  Install from the link, an Apple Id is required to do so.

* [XCode Command Line Tools](https://developer.apple.com/library/archive/technotes/tn2339/_index.html)

  ```bash
  xcode-select --install
  ```

* [Docker Desktop Client](https://docs.docker.com/get-docker/)

  Follow the link and install the last version. Docker Desktop does require a licence but is free for the trial period or if you intend using the full Docker package, a licence will be required, [raise a support ticket for this](https://kainoshelp.atlassian.net/servicedesk/customer/portal/23/group/242/create/1027), remember to add an approvers name if required.

* [Jetbrains Intellij Idea](https://www.jetbrains.com/idea/download)

  Follow the link and install the latest version. You will require a licence for this IDE. [raise a support ticket for this](https://kainoshelp.atlassian.net/servicedesk/customer/portal/23/group/242/create/1027), remember to add an approvers name if required.

* [Jetbrains Rider](https://www.jetbrains.com/rider/download)

  Follow the link and install the latest version. You will require a licence for this IDE. [raise a support ticket for this](https://kainoshelp.atlassian.net/servicedesk/customer/portal/23/group/242/create/1027), remember to add an approvers name if required.
  
  Also, install the following plugin for Rider - [Rider Xamarin Android Support](https://plugins.jetbrains.com/plugin/12056-rider-xamarin-android-support)

* [AppLayer VPN](https://kainoshelp.atlassian.net/servicedesk/customer/portal/23/group/242/create/1027)

  Follow the link to create a new support ticket, in the license section select `other` and add the justification that you need a VPN to work on the project.

* [HomeBrew](https://brew.sh)

  ```bash
  /usr/bin/ruby -e "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/master/install)"
  ```
  **Please note** on MacOS you may require systems to give you temporary admin access to install Homebrew, you can do this by requesting temporary admin access via the following [ticket link](https://kainoshelp.atlassian.net/servicedesk/customer/portal/23/group/240/create/1006) and selecting `issues` under the my computer section.

* [Visual Studio Code](https://code.visualstudio.com)

  ```bash
  brew install visual-studio-code
  ```

* [.NET Core SDK](https://dotnet.microsoft.com/download)

  Follow the link and install version 5.0, if you have already installed dotnet 6.0, you can also install the dotnet 5.0 sdk. 

* [NPM](https://www.npmjs.com/package/npm)

  ```bash
  curl -qL https://www.npmjs.com/install.sh | sh
  ```

## Configure Git
You will need to run the SSH process on your laptop `ssh -keygen -C "your_email@hscic.gov.uk"` do not rename the file and do not add any password when requested. Copy the content of the "id_rsa.pub" file to ADO https://dev.azure.com/nhsapp/_usersSettings/keys. **NOTE**: if there is another id_rsa.pub file further down in the folder structure this may cause a clash in the keys.

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

Review the [Git Branching Strategy](https://nhsd-confluence.digital.nhs.uk/display/NO/Git+Branching+Strategy).

## Host file entries

Before running the app locally, some entries need to be added to your machine's `hosts` file (`/etc/hosts` on Mac or `C:\windows\system32\drivers\etc\hosts` on Windows) to add the following entries:

```bash
127.0.0.1 api.local.bitraft.io
127.0.0.1 auth.nhslogin.stubs.local.bitraft.io
127.0.0.1 cid.local.bitraft.io
127.0.0.1 ers.stubs.local.bitraft.io
127.0.0.1 deeplinklauncher.stubs.local.bitraft.io
127.0.0.1 log.local.bitraft.io
127.0.0.1 messages.local.bitraft.io
127.0.0.1 minimock.local.bitraft.io
127.0.0.1 mongodb.bitraft.io
127.0.0.1 paycasso.stubs.local.bitraft.io
127.0.0.1 pfs.local.bitraft.io
127.0.0.1 servicejourneyrulesapi.local.bitraft.io
127.0.0.1 settings.nhslogin.stubs.local.bitraft.io
127.0.0.1 silver.local.bitraft.io
127.0.0.1 stubs.local.bitraft.io
127.0.0.1 uaf.nhslogin.stubs.local.bitraft.io
127.0.0.1 userinfo.local.bitraft.io
127.0.0.1 users.local.bitraft.io
127.0.0.1 web.local.bitraft.io
127.0.0.1 pkb.stubs.local.bitraft.io
127.0.0.1 pkb.securestubs.local.bitraft.io
127.0.0.1 engage.stubs.local.bitraft.io
127.0.0.1 accurx.stubs.local.bitraft.io
127.0.0.1 gncr.stubs.local.bitraft.io
127.0.0.1 substrakt.stubs.local.bitraft.io
127.0.0.1 nhsd.stubs.local.bitraft.io
127.0.0.1 netcompany.stubs.local.bitraft.io
127.0.0.1 silverintegrationtestprovider.stubs.local.bitraft.io
127.0.0.1 silverintegrationtestprovider.securestubs.local.bitraft.io
127.0.0.1 wellnessandprevention.stubs.local.bitraft.io
```

If you have difficulty accessing web.local.bitraft.io:3000 and web is running in Docker. Try editing the `hosts` file to use the IPv6 addresses instead:

```bash
::1 api.local.bitraft.io
::1 auth.nhslogin.stubs.local.bitraft.io
::1 cid.local.bitraft.io
::1 ers.stubs.local.bitraft.io
::1 deeplinklauncher.stubs.local.bitraft.io
::1 log.local.bitraft.io
::1 messages.local.bitraft.io
::1 minimock.local.bitraft.io
::1 mongodb.bitraft.io
::1 paycasso.stubs.local.bitraft.io
::1 pfs.local.bitraft.io
::1 servicejourneyrulesapi.local.bitraft.io
::1 settings.nhslogin.stubs.local.bitraft.io
::1 silver.local.bitraft.io
::1 stubs.local.bitraft.io
::1 uaf.nhslogin.stubs.local.bitraft.io
::1 userinfo.local.bitraft.io
::1 users.local.bitraft.io
::1 web.local.bitraft.io
::1 pkb.stubs.local.bitraft.io
::1 pkb.securestubs.local.bitraft.io
::1 engage.stubs.local.bitraft.io
::1 accurx.stubs.local.bitraft.io
::1 gncr.stubs.local.bitraft.io
::1 substrakt.stubs.local.bitraft.io
::1 nhsd.stubs.local.bitraft.io
::1 netcompany.stubs.local.bitraft.io
::1 silverintegrationtestprovider.stubs.local.bitraft.io
::1 silverintegrationtestprovider.securestubs.local.bitraft.io
::1 wellnessandprevention.stubs.local.bitraft.io
```


## Azure DevOps Feeds

[Create a Personal Access Token](How-Tos/configure-azure-dev-ops-feeds.md#create-personal-access-token) for Azure DevOps.

Run the following command in the root of the repository:

```bash
make configure-package-feed
```

You will be prompted for parameters when needed. Alternatively you can follow the [manual instructions](How-Tos/configure-azure-dev-ops-feeds.md#manually-configure-feeds).

If you have trouble getting package feeds setup, you can [check the troubleshooting steps here](https://dev.azure.com/nhsapp/NHS%20App/_wiki/wikis/MonoRepo/649/getting-started)

## Secrets

The team use [Keybase](https://keybase.io) to share and synchronise secrets.
Sign up for an account if you don't already have one, and ask a friendly tech lead to add you to the NHSOnline team.

The secrets required for running the app locally are stored in the folder (`team/nhsonline/development_secrets`). If the keybase filesystem integration is installed and running these will be automatically copied to your home directory (`~/.nhsonline/secrets`) when the NHS App is run using make. Otherwise an error will be reported and the necessary files should be manually copied across.

You may receive `Error Code 2806` after getting access to the Keybase files,  It can take an hour or more for the automated process to apply permissions to keybase files.

## Docker

Locate the following file in Keybase: `team/nhsonline/docker-login.sh`.

Copy the file to the root of your local repository. When copying `team/nhsonline/docker-login.sh` also copy `team/nhsonline/docker.password` to the same location and then run the `team/nhsonline/docker-login.sh` script in terminal. 

This will need to be done only once at the beginning of your set up to prevent an "Authentication Required" error when building.