Serenity Build Reporting Tools
=====================

The scripts in this directory are intended to be used in conjunction with the builds on TeamCity.  They offer a way of gaining some insight into trend data provided by the Serenity test outputs.

# Getting Started
## Pre-requisites
- Install Python 3.X
- Install [Pipenv](https://docs.pipenv.org/#install-pipenv-today)

## Installing dependencies
Open a terminal and navigate to this directory

```
pipenv install
```

Ensure you can access the pipenv environment by running
```
pipenv --where
```
You should see the location of this directory.

Ensure that you can access the dependencies by doing a quick check.  Open a shell and try to use one of the dependencies.
```
pipenv shell

python

>>> import requests
>>> r = requests.get("http://google.com")
>>> r.status_code
# 200
```

# build_results
The build_results.py script requires some environment variables to be set up in order to create a session with TeamCity.

Create a new file called ".env" and add the following
```
TC_USERNAME=<your username>
TC_PASS=<your password>
```

NOTE: Make sure you never commit this file.  It is in the .gitignore of this directory.

Running the script from this directory will make the env vars available to you.  You can run it as you would any other python script.