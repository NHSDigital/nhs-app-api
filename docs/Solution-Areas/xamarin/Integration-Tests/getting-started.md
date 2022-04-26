# Getting Started Running Xamarin Integration Tests
## Basic Process
The basic process to run the Xamarin integration tests in 'vanilla' mode locally is outlined below. For different configuration options to avoid re-building components you dont need, see [Test Configurations](test-configurations.md).

### Pre-requisites
- Make sure you've done everything in the main `Getting-Started` guide.
- Get a Browserstack invite from someone in the NHS Digital organisation.

### Build & Run Tests locally
In the `nhsapp` -> `xamarin` folder:
- `make build`: this will generate the .ipa/.apk files for testing and copy them across to the 'xamarinintegrationtests' folder.

In the xamarinintegrationtests folder:
- `make build` (required if you want to build the SJR Rules - see below)
- `make run-local`: this spins up the local docker containers
    - Additional switches: 
        - `TAG=develop`: this pulls down the docker container registry images tagged with `develop` - i.e. what's merged to the develop branch in the pipeline. If you have a specific PR branch built, you can replace this with `TAG=prXXXX`.
        - `SJRCONFIG=local`: Use the local copy of the SJR rules built by running `make build` in the xamarinintegrationtests directory (see above). This is useful so we can e.g. create new ODS codes without impacting others or being impacted by others. You will want to use this if using `TAG=develop` or similar, so we're running those registry containers, but with the local SJR version.
        - Full example: `make run-local TAG=develop SJRCONFIG=local`
- When you see the API log streams you are good to go with the tests in the NHSOnline.IntegrationTests solution. You can debug locally and also log in to Browserstack to see the test runnning.

## Troubleshooting errors during the `make` processes:
- Apple certificate errors while building the iOS app on Mac
    - Solution: Open XCode and ensure you have a developer profile setup, if not sign in / create one. Then download profile and create a new certificate.
        In the event you still get certificate errors stating there are missing root or intermediate apple certificates you will need to go to https://www.apple.com/certificateauthority/ and download the relevant developer certs and install.
        This allows your developer certificate to have a chain back to the root apple cert.        
- TLS Certificate errors starting Docker containers
    - Solution: If you are Kainos and have Zscaler on your machine, get on the VPN to do any docker stuff. Zscaler messes with TLS certificate trust chain. There is a workaround published on the Kainos Systems wiki to patch local containers but its a lot of hassle.
- Error: nuget restore errors (401 Unauthorized)
    - Solution: Look at the main `Getting-Started.md` - you need to run the `make configure-package-feed` command. If this fails, see error below:
- Error running `make configure-package-feed`: 'unlink xxx/Nuget is a directory' on Mac:
    - Solution: check your `<home>\.config\Nuget` folder - this is supposed to be a Symlink to `<home>\.nuget\Nuget`. If this has got messed up, e.g. the Symilnk is actually located inside the Nuget folder, i.e. `<home>\.config\Nuget\Nuget`, just delete this folder in the .config directory and re-run the make command.
    - **NOTE**: If the above solution does not work for you, there is an alternative: First check (after running `make configure-package-feed`) that the symlink exists on the `<home>\.config\Nuget\Nuget` folder. If it does, open the folder in finder with the `columns` view, rename the `Nuget` folder in the parent folder to `Nuget1` (or similar, so long as its not named Nuget), move the symlink Nuget folder to the parent folder i.e. `<home>\.config\Nuget`. Finally, delete the `Nuget1` folder as its no longer required. This should allow the packages/symlink folder to be set up correctly. 
- Error: 'failed to solve with frontend dockerfile.v0: failed to create LLB definition: pull access denied, repository does not exist or may require authorization: server message: insufficient_scope: authorization failed'
    - Solution: you need to (re-)authenticate Docker CLI with the correct credentials. Look at the main `Getting-Started.md` - you need to (re-)run the `docker-login.sh`.
- ERROR: 'Cannot start service dns: Ports are not available: port is already allocated.'
    - Solution: **NOTE: This solution is TBC** you can:
        - comment out the entire `dns` section in the `docker/android/docker-compose.yaml` file if you dont need to be running the Android emulator locally. 
        - Or you have to play whack-a-mole with whatever is listening on port 53 - for me it is both Cisco/Zscaler, and it seems to be Zscaler that's the issue. I've opened a systems request for this. 
        
            To check what's listening on a port on a Mac: 
    `sudo lsof -i :53`
    
            get the unique PIDs from this output and run:
    `sudo kill -9 <PID>`
            
            Cisco process will go away, but Zscaler wont - it'll recreate itself with a new PID. I think i had to kill it twice before it stopped interrupting? This is weird. Waiting to hear back from Kainos Systems.

- When running the Xamarin tests they all fail at the login stage. 
    - Cause: The previously run BDD native app tests has created the SJR image and associated volume in docker which is incompatible with the Xamarin test suite.
    - Solution: To clear this, make sure you have stopped all docker containers and run
    
        `docker system prune`

        `docker volume ls`

        Find the sjr volume name, should be something like: “int_test_servicejourneyrules_data”
    
        `docker volume rm int_test_servicejourneyrules_data`

        This will removed the previously cached SJR image
        From the xamarinintegrationtests folder run `make run-local` again.
