# Add Local Development Secrets

1. Add a file containing the secret to the `development_secrets` directory in keybase
2. Add a line to [buildscripts/validate_local_secrets.sh](https://dev.azure.com/nhsapp/NHS%20App/_git/nhsapp?path=/buildscripts/validate_local_secrets.sh&version=GBdevelop) containing the name of the secret. e.g.

    ```bash
    validate_secret my_new_secret
    ```

3. Add the secret to the top of the root `docker-compose.yml` file and include it in the `secrets` section of any services that require it. e.g.

    ```bash
    my_new_secret:
        file: "~/.nhsonline/secrets/my_new_secret"

    services:
        my.service:
            secrets:
                - my_new_secret
    ```

4. Update the appropriate `.env` file in the `docker` directory to set the envrionment variable expected to contain the secret. e.g.

    ```bash
    MY_NEW_SECRET=/run/secrets/my_new_secret
    ```

5. If the envrionment variable is expected to contain the secret and not a path to the secret update the entrypoint for the appropriate services to set the variable from the contents of the file. e.g.

    ```bash
    entrypoint:
      - /bin/bash
      - -c
      - |
          export MY_NEW_SECRET=$$(<$$MY_NEW_SECRET)
          exec $$*
      - "--"
    ```

6. Configure stubbed environment

   If the secret is a certificate or key update [docker/stubbed/docker-compose.yml](https://dev.azure.com/nhsapp/NHS%20App/_git/nhsapp?path=/docker/stubbed/docker-compose.yml&version=GBdevelop) to reference the appropriate files in [docker/stubbed/certs](https://dev.azure.com/nhsapp/NHS%20App/_git/nhsapp?path=/docker/stubbed/certs&version=GBdevelop)

      ```bash
      my_new_secret:
          file: "./docker/stubbed/certs/TestCert.pfx"
      ```

7. Configure Integration Tests

    If a specific value (different to the default) is required for integration tests create a suitable dummy secret file in [docker/bddtests/secrets](https://dev.azure.com/nhsapp/NHS%20App/_git/nhsapp?path=/docker/bddtests/secrets&version=GBdevelop). This must *not* contain a real secret but a dummy value for the secret expected by the tests.

    Update [docker/bddtests/docker-compose.yml](https://dev.azure.com/nhsapp/NHS%20App/_git/nhsapp?path=/docker/bddtests/docker-compose.yml&version=GBdevelop) to reference the dummy secret.

      ```bash
      my_new_secret:
          file: "./docker/bddtests/secrets/my_new_secret"
      ```
