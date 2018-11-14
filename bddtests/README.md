NHS BDD Tests
==============

This repo houses the BDD-style acceptance tests for the project.
  Within the test folder you will find a number of feature files that relate to a
  subset of the app's functionality.

#Getting Started
[Download the latest chromedriver](http://chromedriver.chromium.org/) for your machine and place it
  into the root of the repo (it will not be picked up by git).

Copy docker-compose.override.yml (sets VISION_CERT_PASSPHRASE env variable) from keybase root folder into:
   - backendworker folder
   - web folder

Start the app

```
cd <path to repo>
docker-compose up
```

Run the tests locally

```
gradle clean prepare test aggregate \
-Dcucumber.options="--tags 'not (@pending or @bug or @native or @manual or @tech-debt)'" \
-Dwebdriver.base.url="http://web.local.bitraft.io:3000"
```

View the serenity report by navigating to "<path to repo>/target/site/serenity"
  and opening the `index.html` file in a browser.
  
To run headless append -Dwebdriver.provided.type=chromeheadless onto the command

#Understanding the Structure
Docs in progress.

#Configuring the Framework

##Filtering Tests
###As a gradle command
As shown above, the default test command filters out the tests that are currently
  bugs or pending.  To add other filters e.g. `backend` simply add another tag to
  the cucumber options

###In an IDE
Each of the feature themes (e.g. appointments, prescriptions) has its own runner
  that points to the features in its package.  This allows you to only run a subset
  of the tests in the area that you're working on easily by running the runner class.

On windows, you might see an error that says that the classpath is too long.
  This can be overcome in IDEA by editing the 'shorten command line' to be set to the
  classpath.

##Changing Device/Browser
###Web Drivers
Web drivers are the way that selenium interacts with browsers.

The `serenity.properties` file points to a certain type of driver using the
  `webdriver.provided.type` property.  This points to one of the lines below
  that defines a type of driver.  Changing the driver currently must be done manually.

To add another type of driver, add a class in the same package as the other web drivers
  , add the type onto the properties file list and point `webdriver.provided.type` to the
  new type.

To run in firefox, download geckodriver, place it in the bdd folder, and use the parameter
  ' -Dwebdriver.provided.type=firefox'.

###Config Properties
Configuration is all defined in one file and each of the properties can be overriden by
  passing in environment variables to the runner (either via the command line or in an IDE).

Common options are:
* url - defaults to web.local.bitraft.io:3000
* wiremockUrl - defaults to stubs.local.bitraft.io:8080
* backendUrl - defaults to api.local.bitraft.io:8082

Serenity properties **cannot** be passed as environment variables as Serenity only picks up
  system properties.

###Mock Environment setup
To populate wiremock in your local environment, gradle task 'mock' with following arguments

'nft' - to populate wiremock with nft stubs (for nft stubs, you will also need to provide 'number of patients' as a gradle argument)
'mockenvironment' - to populate wiremock with stubs setup for generic scenarios including error scenarios

Command format:
 gradle mock -DmockArgs="['mockenvironment']
  gradle mock -DmockArgs="['nft',15]


### Semi-stubbed Environment
It is possible to run the application against a real GP environement, with a mocked CID instance.

Run the application, pointing towards the CID mocks.

```
docker-compose -f docker-compose.semi-stubbed.yml  up
```

Populate the CID mocks, with the default file (This should be copied from the shared keybase folder - `<NHS APP TEAM>/gp\ practice\ details/performance/EmisUsers.csv` to `src/main/kotlin/mocking/defaults/dataPopulation/nft`):

```
gradle mock -DmockArgs="['semistubbed']"
```

Or to run it with a custom csv in the format: `firstName,surname,dob,odsCode,connectionToken,nhsNumber`, including a header row:

```
gradle mock -DmockArgs="['semistubbed', FILE_LOCATION]"
```

Note that all DOBs whould be in the format: `yyyy-MM-dd`

