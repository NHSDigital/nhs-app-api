NHS BDD Tests
==============

This repo houses the BDD-style acceptance tests for the project.
  Within the test folder you will find a number of feature files that relate to a
  subset of the app's functionality.

#Getting Started
[Download the latest chromedriver](http://chromedriver.chromium.org/) for your machine and place it
  into the root of the repo (it will not be picked up by git).

Start the app

```
cd <path to repo>
docker-compose up
```

Run the tests locally

```
gradle clean test aggregate \
-Dcucumber.options="--tags ~@bug --tags ~@pending --tags ~@manual --tags ~@native --tags ~@tech-debt" \
-Dwebdriver.base.url="http://localhost:3000"
```

View the serenity report by navigating to "<path to repo>/target/site/serenity"
  and opening the `index.html` file in a browser.

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

###Config Properties
Configuration is all defined in one file and each of the properties can be overriden by
  passing in environment variables to the runner (either via the command line or in an IDE).

Common options are:
* url - defaults to localhost:3000
* wiremockUrl - defaults to localhost:8080
* backendUrl - defaults to localhost:8082

Serenity properties **cannot** be passed as environment variables as Serenity only picks up
  system properties.

