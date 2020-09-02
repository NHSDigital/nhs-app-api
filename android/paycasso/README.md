# Paycasso Maven Publishing

Paycasso is a 3rd party library used for document capture and ID verification. We embed this into our Android app and surface an API to NHS Login.

The Paycasso SDK is private and as such needs to be downloaded from a password protected SFTP site and added to our DevOps feeds as an artifact.

### Publishing

Follow the below steps to publish a new Paycasso version to the Azure DevOps `nhsapp` maven feed.

- Ensure you have your feeds token configured (see [here](https://dev.azure.com/nhsapp/NHS%20App/_wiki/wikis/MonoRepo/203/configure-azure-dev-ops-feeds))   
    - Note: this token **must** have write access to feeds and you **must** have permission to publish packages
- Get the latest Paycasso `aar` packages from their SFTP server:
    - Create a `lib` directory beside this README
    - Create two files by copying from the SFTP site:
        - `paycasso.aar` - main sdk, will be named `libPaycasso-xx` on the SFTP site
        - `paycass-view.aar` - NHS view library, will be named `libPaycassoView-NHS` on the SFTP site
- Set the correct version in `paycasso-version.txt` (no newlines or spaces)
- Run the `publish.sh` script
- Profit

### Versioning format

- Packages are versioned as `YYYY.M.D-<paycasso version>` - for example, `v12` packaged on the 1st of Oct 2020 would be `2020.10.1-12`
  - To republish a paycasso version on the same day you can add a postfix in the `paycasso-version.txt` file of `-1`, `-2` etc. (becoming `2020.10.1-12-1`)

### Changing dependency versions

The `template` directory contains the `xml` used for the two paycasso libraries, changes can be made there.
