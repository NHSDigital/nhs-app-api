# Generating a new P12 development certificate

As apple needs these files to be associated to an email address, and they will eventually expire we should know how to create these new files

The following are the steps to generate a new p12 file:
1. Open the keychain access application
2. Under Keychain access navigate to Certificate Assistant -> Request a certificate from a certificate authority...
3. Add in your email address and common name
4. Choose save to disk
5. Click continue
6. Open in finder to locate the new file
7. Double click it to add it to your keychain
8. Select the login keychain and find the new cert
9. Right click the cert and export it as a p12 file, giving it the name `iOSDevelopmentCertificate.p12`
10. Have apple generate a secure password and keep this safe

Once this file has been generated you can now upload this to the azure secure files in the library. Locate the file and remove it before uploading your new p12 file.

You will also need to update the variable under the `iOS Development` variable group with the password your kept safe earlier. The name for this will be `ios.development.certificate.password`.

Once this has been added run a pipeline which creates an ipa and approve the added file.