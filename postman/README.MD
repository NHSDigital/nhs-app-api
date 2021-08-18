# How to use the postman collection
## Generate a key pair
In order to use the postman collection with signed jwt authentication you will require a private key pair. These can be gnerated using the following command (Do not set a passphrase when prompted)

```
ssh-keygen -t rsa -b 4096 -m PEM -f jwtRS512.key
openssl rsa -in jwtRS512.key -pubout -outform PEM -out jwtRS512.key.pub
```

## Register the public key
The public key **jwtRS512.key.pub** in the example needs to be registered with the Api Management team along with a unique **kid** value of your choice. See [this](https://digital.nhs.uk/developer/guides-and-documentation/security-and-authorisation/application-restricted-restful-apis-signed-jwt-authentication) document for further details.

## Execute requests
1. Copy the contents of the **jwtRS512.key** into the **RSAKEY** environment variable in postman.
2. Set the **kid** postman environment variable to be the kid value your public key was registered with
3. Set the **apiKey** environment variable in postman to be the key for the app you are using to authenticate (This is the same value used as an api key for the legacy endpoints).
4. Execute the **Load Crypto Library for RS512** request to load in the required crypto library for generating the jwt token
5. Execute the **Get Access Token** request. This will send the jwt token created in step 4 and retrieve a **access token** which is stored in an environment variable
6. Execute the required request. This will use the **access token** previously generated to authticate the request.

N.B The **access token** is valid for 10 minutes and step 5 will have to be repeated before making further calls after this time.