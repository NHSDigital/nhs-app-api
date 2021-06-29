# Xamarin Tests

## Running tests without building everything.
Sometimes when working on the integration tests it would be useful to run the integration tests without rebuilding everything if the app code is in a stable position.
To do this you will need 1 run of the pipeline that has built the containers you need and the version of the app you want to test.
This process can also be used to run the tests locally if you were to ignore the pipeline changes.

### No backend and web build
In order to not need the pipeline to build the web and backend you can grab the docker tag from the full build you ran and set that to be the `DOCKER_TAG` env var in the [set_env file](https://dev.azure.com/nhsapp/NHS%20App/_git/nhsapp?path=%2Fxamarinintegrationtests%2Fbuildscripts%2Flib%2Fset_env.sh&version=GBdevelop&line=12&lineEnd=12&lineStartColumn=1&lineEndColumn=40&lineStyle=plain&_a=contents) replacing the existing value.
e.g. `export DOCKER_TAG="9b19478327cdd2bed4c6952ae993c68e506d7d1c"`

Once this is set you can deselect the backend and web stages of the build when running the pipeline.

### No Xamarin app build and upload
To not need the Xamarin app to build you will need the browserstack identifier for the version of the app that has been previously uploaded. This can be found by looking for entries in the build log from your full build. You need the values that are set to the env vars
[iOS__App](https://dev.azure.com/nhsapp/NHS%20App/_build/results?buildId=49040&view=logs&j=4ee9f41d-98c2-5f23-7bba-42ad239b76a7&t=305dd881-43ad-54c3-35b7-fafd32b44cbd&l=41) and [Android_App](https://dev.azure.com/nhsapp/NHS%20App/_build/results?buildId=49040&view=logs&j=4ee9f41d-98c2-5f23-7bba-42ad239b76a7&t=305dd881-43ad-54c3-35b7-fafd32b44cbd&l=23).

Once you have these they should be added to the [set_env](https://dev.azure.com/nhsapp/NHS%20App/_git/nhsapp?path=%2Fxamarinintegrationtests%2Fbuildscripts%2Flib%2Fset_env.sh) file like this
```
export iOS__App="bs://45a33202ae70330798f1ef2fb4680e0501765c89"
export Android__App="bs://5001ed59a8ea72e6f7b1ecc8ee399b2da879083c"
```
 At this stage you can use the `make run-local` commands and the tests will use the specified version of the app rather than uploading again. In order to make this work in the pipeline you will need to disable the upload stages of the Xamarin test build. You can do this by commenting out those section of the [Xamarin test template file](https://dev.azure.com/nhsapp/NHS%20App/_git/nhsapp?path=%2F.azuredevops%2Ftemplates%2Fintegration-tests-xamarin-job.yml&version=GBdevelop&line=10&lineEnd=24&lineStartColumn=6&lineEndColumn=63&lineStyle=plain&_a=contents)
 ```
    # - task: DownloadBuildArtifacts@0
    #   displayName: Download Android App
    #   inputs:
    #     downloadType: single
    #     artifactName: Xamarin Android Clients
    #     itemPattern: |
    #       Xamarin Android Clients/com.nhs.online.nhsonline.browserstack.apk
    # - task: DownloadBuildArtifacts@0
    #   displayName: Download iOS
    #   inputs:
    #     downloadType: single
    #     artifactName: Xamarin iOS Clients
    #     itemPattern: |
    #       Xamarin iOS Clients/NHSOnline.App.BrowserStack.ipa
```
