# GP Integration

Collection of Postman and SOAPUI files for use with GP systems to prove integration and inform development

Install postman client - https://www.getpostman.com/

Install SoapUI - https://www.soapui.org/

Each file can be imported into the client and have been arranged in a set of folders that relate to specific interactions

Import all files and running the executions in order should result in the correct responses.

Please note these will on be able to execute from whitelisted IP addresses, these are currently

DigitalGuest
BJSS VPN
Regus
Kainos VPN/LAN

## Emis

There is one specific key that is in essence a secret. 

**PLEASE DO NOT COMMIT THE VALUE FOR X-API-ApplicationId INTO SOURCE CONTROL**

This value is empty in Gitlab, but needs adding to the Environment values within the EMIS_Test environment manually.

The value is available within keybase.io


## TPP

**Important:** ***ProviderId*** is a secret and should not be pushed into source. Find this value and patient detail fields in keybase.io.

TPP requires client certificates which can also be found in keybase.io along with the certificate passphrase. Configure postman to use these certificates for the TPP host by following the instructions [here](https://www.getpostman.com/docs/v6/postman/sending_api_requests/certificates). Also ensure that General/"SSL certificate verification" is toggled off.

```
Host: systmonline2.tpp-uk.com:60015
CRT file: <CRT FILE LOCATION>
Key file: <KEY FILE LOCATION>
Passphrase: <PASSPHRASE>
```

Ensure all values and attributes passed in are XML encoded.

## Vision

Testing the vision system requires SoapUI in order to generate security headers.

- Load the project `vision-supplied.xml` in SoapUI.

- Set the SSL keystore and password for Vision PFX test file in Preferences-->SSL Settings

- Load the `vision-supplied-environment.properties` file and set appropriate values in Preferences-->Global Properties from the secrets found in keybase.io

- Set the project certificate keystore and password, by double clicking on the project, then selecting WS-Security Configuration --> Keystores entry. (NOTE soap-ui does not allow this password to be set from an environment variable, so change it before committing into this repository.
