To launch: 

- Run `docker-compose up --build`
- Add `ems.cdss.stubs.local.bitraft.io` to **/etc/hosts**

To test:

- Import `cdss-wiremock/postman/NHS App FHIR Examples (Wiremock).postman_collection.json` and `cdss-wiremock/postman/NHS App Fhir Examples (Wiremock).postman_environment.json` into postman
- Ensure the `port` environment variable is set to `{{wiremock_port}}`