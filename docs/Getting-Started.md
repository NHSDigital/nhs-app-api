# nhsapp

[[_TOC_]]

## Mac Software installation guide

* [XCode](https://developer.apple.com/xcode/)

  Install from the link, an Apple Id is required to do so. Check with your team first what version they are on - its not good to be too far ahead / behind.

* [XCode Command Line Tools](https://developer.apple.com/library/archive/technotes/tn2339/_index.html)

  ```bash
  xcode-select --install
  ```

**Kainos staff - the following install section requires Admin rights to be granted to your Mac - ask your TL to raise/approve a Kainos systems request to do this.**
* [HomeBrew](https://brew.sh)

  ```bash
  /usr/bin/ruby -e "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/master/install)"
  ```

* Selected software to install once you get temporary Admin rights granted - confirm list with your TL:

  ```bash
  # Rider IDE for .NET Dev on Mac - needs a licence (ask TL to raise this systems request)
  brew install --cask rider
  # IntelliJ if doing anything in Java / Scala (such as performance tests) - needs a licence (ask TL)
  brew install --cask intellij-idea
  # Will need docker running locally
  brew install --cask docker
  # VS Code is a good WebDev IDE. Other IDEs are available!
  brew install --cask visual-studio-code
  # Slack App (browser version is great too if you dont want this)
  brew install --cask slack
  # You will need this for access to dev secrets, secure file share, etc
  brew install --cask keybase
  # good to have in case you need to update anything android specific
  brew install --cask android-studio
  # Used for testing APIs manually
  brew install --cask postman
  # Need this for accessing Azure resources via CLI
  brew install azure-cli
  # Need this for checking K8s resources via CLI
  brew install kubernetes-cli
  # K9s is much much easier to use than e.g. kubectl
  brew install k9s
  ```

* [.NET Core SDK](https://dotnet.microsoft.com/download)

  Follow the link and install version 5.0, if you have already installed dotnet 6.0, you can also install the dotnet 5.0 sdk. 

* [NPM](https://www.npmjs.com/package/npm)

  ```bash
  curl -qL https://www.npmjs.com/install.sh | sh
  ```
  
* [Docker Desktop Client](https://docs.docker.com/get-docker/)

  Follow the link and install the last version. Docker Desktop does require a licence but is free for the trial period or if you intend using the full Docker package, a licence will be required. Ask your TL to add this to the Systems request seeking licences which has Cost Centre approval.

* Once Rider is up and running, install the following plugin - [Rider Xamarin Android Support](https://plugins.jetbrains.com/plugin/12056-rider-xamarin-android-support)

### ZScaler Certificates required to install for selected software (Rider, IntelliJ, etc)
Some of the software you just installed will not play nicely with ZScaler unless you install the ZScaler certificate. 
Go here: https://kainossoftwareltd.sharepoint.com/systems/SitePages/Zscaler-Quick-Fixes.aspx and set this certificate up with everything you need.
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
127.0.0.1 accurx.stubs.local.bitraft.io
127.0.0.1 api.local.bitraft.io
127.0.0.1 auth.nhslogin.stubs.local.bitraft.io
127.0.0.1 cid.local.bitraft.io
127.0.0.1 deeplinklauncher.stubs.local.bitraft.io
127.0.0.1 drdoctor.stubs.local.bitraft.io
127.0.0.1 engage.stubs.local.bitraft.io
127.0.0.1 ers.stubs.local.bitraft.io
127.0.0.1 gncr.stubs.local.bitraft.io
127.0.0.1 log.local.bitraft.io
127.0.0.1 messages.local.bitraft.io
127.0.0.1 minimock.local.bitraft.io
127.0.0.1 mongodb.bitraft.io
127.0.0.1 netcall.stubs.local.bitraft.io
127.0.0.1 netcompany.stubs.local.bitraft.io
127.0.0.1 nhsd.stubs.local.bitraft.io
127.0.0.1 paycasso.stubs.local.bitraft.io
127.0.0.1 pfs.local.bitraft.io
127.0.0.1 pkb.securestubs.local.bitraft.io
127.0.0.1 pkb.stubs.local.bitraft.io
127.0.0.1 servicejourneyrulesapi.local.bitraft.io
127.0.0.1 settings.nhslogin.stubs.local.bitraft.io
127.0.0.1 silver.local.bitraft.io
127.0.0.1 silverintegrationtestprovider.stubs.local.bitraft.io
127.0.0.1 silverintegrationtestprovider.securestubs.local.bitraft.io
127.0.0.1 substrakt.stubs.local.bitraft.io
127.0.0.1 stubs.local.bitraft.io
127.0.0.1 uaf.nhslogin.stubs.local.bitraft.io
127.0.0.1 userinfo.local.bitraft.io
127.0.0.1 users.local.bitraft.io
127.0.0.1 web.local.bitraft.io
127.0.0.1 wellnessandprevention.stubs.local.bitraft.io
127.0.0.1 zesty.stubs.local.bitraft.io
```

If you have difficulty accessing web.local.bitraft.io:3000 and web is running in Docker. Try editing the `hosts` file to use the IPv6 addresses instead:

```bash
::1 accurx.stubs.local.bitraft.io
::1 api.local.bitraft.io
::1 auth.nhslogin.stubs.local.bitraft.io
::1 cid.local.bitraft.io
::1 deeplinklauncher.stubs.local.bitraft.io
::1 drdoctor.stubs.local.bitraft.io
::1 engage.stubs.local.bitraft.io
::1 ers.stubs.local.bitraft.io
::1 gncr.stubs.local.bitraft.io
::1 log.local.bitraft.io
::1 messages.local.bitraft.io
::1 minimock.local.bitraft.io
::1 mongodb.bitraft.io
::1 netcall.stubs.local.bitraft.io
::1 netcompany.stubs.local.bitraft.io
::1 nhsd.stubs.local.bitraft.io
::1 paycasso.stubs.local.bitraft.io
::1 pfs.local.bitraft.io
::1 pkb.securestubs.local.bitraft.io
::1 pkb.stubs.local.bitraft.io
::1 servicejourneyrulesapi.local.bitraft.io
::1 settings.nhslogin.stubs.local.bitraft.io
::1 silver.local.bitraft.io
::1 silverintegrationtestprovider.stubs.local.bitraft.io
::1 silverintegrationtestprovider.securestubs.local.bitraft.io
::1 substrakt.stubs.local.bitraft.io
::1 stubs.local.bitraft.io
::1 uaf.nhslogin.stubs.local.bitraft.io
::1 userinfo.local.bitraft.io
::1 users.local.bitraft.io
::1 web.local.bitraft.io
::1 wellnessandprevention.stubs.local.bitraft.io
::1 zesty.stubs.local.bitraft.io
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