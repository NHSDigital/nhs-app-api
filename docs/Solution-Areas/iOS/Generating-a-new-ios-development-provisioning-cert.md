# Generating a new ios development provisioning certificate

As apple needs these files to be associated to an email address, and they will eventually expire we should know how to create these new files

The following are the steps to generate a new cert file:
1. Open the keychain access application
2. Under Keychain access navigate to Certificate Assistant -> Request a certificate from a certificate authority...
3. Add in your email address and common name
4. Choose save to disk
5. Click continue
6. Open in finder to locate the new file

When this file has been created log in to [apple developer](https://developer.apple.com) and navigate to Certificates, Identifiers & Profiles. Open the Certificates window and click the plus button beside certificates. From here you can upload the file you created.

With the cert uploaded you can now click the profiles tab and go to the environments provisioning profile you need to update. Click the row and then click the edit button on the next screen. There will be a section labeled certificates that you should be able to find the cert you just added in. Check the box beside this and click save. Once saved click download on the next window. You should also see the status of the profile as active.

Before uploading the new provisioning profile you need to remove the old file which can be found in the secure files in the library. Ensure that the name of the file you are about to upload matches that of the one you are removing as the pipeline uses these names. The format should be `iOSDevelopment<env_name>` like `iOSDevelopmentScratch20`. Once this has been uploaded run a pipeline to generate an ipa for that environment and approve the new file.