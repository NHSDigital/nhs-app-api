# Setting up and Debugging iOS Entitlements / Claimed Urls / Associated Domains
What are iOS Entitlements / Claimed Urls / Associated Domains? [This article has a good summary.](https://developer.apple.com/documentation/Xcode/supporting-associated-domains)

When debugging entitlements there are a few different places to look to check the configuration. [This](https://developer.apple.com/documentation/bundleresources/entitlements/diagnosing_issues_with_entitlements) article from Apple provides good detail on the steps needed to check each stage of the process.

## Tips
- After updating capabilities, you will need to download the latest version of the associated provisioning profile
- Installed provisioning profiles can be found here on your Mac `~/Library/MobileDevice/Provisioning\ Profiles/`
- During the build process the log will have entries that include the location of the `.app` file needed to check the entitlements in the build e.g., `/Users/joe.bloggs/Src/nhsapp/xamarin/NHSOnline.App.iOS/bin/iPhone/Debug/device-builds/iphone12.1-14.5.1/NHSOnline.App.iOS.app`

## Associated Domains
- Each environment for iOS (e.g. ScratchX, Preview, Staging, etc) has a BuildConfiguration file (e.g. BuildConfiguration.env.Preview.props) which sets up the claimed urls for that environment.
- As part of the Docker setup for web, it will build the correct `apple-app-site-association` file at the expected location to add the other side of the chain - ensuring what is list as the entitlements in the given native App is also trusted by the website.
- The apple CDN collects the website's listed entitlement urls from the `apple-app-site-association` file. We host this at a known location e.g. `/.well-known/assetlinks.json`.
- We currently provide associated domains for both the main NHS App and ERS. These files will be hosted at:
  - Production NHS App Deeplink Configuration for iOS = https://www.nhsapp.service.nhs.uk/.well-known/apple-app-site-association
  - Production eRS Deeplink configuration for iOS = https://refer.nhs.uk/.well-known/apple-app-site-association

### Associated domains in pre-production environments
- Our development environments block access from unknown IP addresses preventing the apple CDN from collecting this information.
In order to test universal links in iOS against our development environments you will need to ensure that the entry in the entitlements for the applink has the `?mode=developer` query parameter add to the value. This makes the device query for the `apple-app-site-association` details from your local connection rather than the CDN.
- You will also need to enable the developer option on the device to allow debugging of associated domains. Which can be found in Settings --> Developer --> Associated Domains Development.