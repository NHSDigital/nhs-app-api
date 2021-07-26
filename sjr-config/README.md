# Journey Rules

Collection of rules for production SJR config.

## Data Creation

The data creation process is documented [here](docs/ConfigurationUpdates.md)

## Migrating from GitLab

1. Switch the origin URL, either:

    * HTTP

        ```bash
        git remote set-url origin https://nhsapp@dev.azure.com/nhsapp/NHS%20App/_git/nhsapp-journeyrules
        ```

    * SSH

        ```bash
        git remote set-url origin git@ssh.dev.azure.com:v3/nhsapp/NHS%20App/nhsapp-journeyrules
        ```

2. Prune old remote branches (only develop has been migrated)

    ```bash
    git fetch --prune
    ```