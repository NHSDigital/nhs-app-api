# Recreate iOS Distribution Certificate

The iOS distribution certificate expires after a year. The follow steps describe how to generate a new certficate

1. Get the `NHS App.p12` file from the team leads keybase.
2. Manually generate a certificate signing request. Note currently this cannot be done using the "Keychain Access" tool on a mac. These instructions are based on [this Stack Overflow answer](https://stackoverflow.com/a/18589759).

    1. Extract the key from the p12 file

        ```bash
        openssl pkcs12 -in NHS\ App.p12 -out NHS\ App.pem -nodes
        ```

    2. Generate Certificate Signing Request

        ```bash
        openssl req -out NHS\ App.csr -key NHS\ App.pem -new
        ```

        * Use "NHS App" for the `Command Name (CN)`
        * Use "nhsapp.ops@nhs.net" for the `Email Address`
        * Use blank (".") for the rest

3. Create a new certificate.

    1. Navigate to the [Certificates section of the Apple Developer portal](https://developer.apple.com/account/resources/certificates/list)
    2. Click the plus icon
    3. Select "iOS Distribution (App Store and Ad Hoc)"
    4. Click "Continue"
    5. Choose the "NHS App.csr" file generated above and click Continue
    6. Download the new certificate

4. Import the certificate and private key (from "NHS App.p12", if not already imported) into your `login` Keychain in the Keychain Access tool.
5. Export the certificate in "Personal Information Exchange (.p12)" format.
6. Add the new certificate to the team leads keybase.
7. Delete the `iOSProductionCertificate.p12` [Secure file](https://dev.azure.com/nhsapp/NHS%20App/_library?itemType=SecureFiles) from Azure DevOps.
8. Upload the new certificate as a [Secure file](https://dev.azure.com/nhsapp/NHS%20App/_library?itemType=SecureFiles) to Azure DevOps with the name `iOSProductionCertificate.p12`.
9. Recreate the provisioning profiles for Demo (`iOSDemo.mobileprovision`) and Production (`iOSProduction.mobileprovision`)

    1. Navigate to the [Profiles section of the Apple Developer portal](https://developer.apple.com/account/resources/profiles/list)
    2. Select the appropriate Profile ("NHS App Demo Distribution" for Demo; "NHS App Distribution" for Production)
    3. Click "Edit"
    4. Select your new certificate in the "Certificates" section at the bottom, it should be the one with a date exactly one year from the date you generated the certificate
    5. Click "Save"
    6. Download the new profile
    7. Delete the existing [Secure file](https://dev.azure.com/nhsapp/NHS%20App/_library?itemType=SecureFiles) from Azure DevOps
    8. Upload the new [Secure file](https://dev.azure.com/nhsapp/NHS%20App/_library?itemType=SecureFiles) to Azure DevOps

10. Delete all local files used in this process.

The first run of builds which use the newly uploaded secure files will need to be granted permission to access them. They will pause asking for the appropriate button to be clicked.
