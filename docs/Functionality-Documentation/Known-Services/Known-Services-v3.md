# Known Services

> **Note:** Legacy Known Services is still being used in the Legacy (non-Xamarin) native apps. [Find out more about the Legacy Known Services here](Legacy-Known-Services.md)


[[_TOC_]]

## What is KnownServices V3?

Known Services is used in the web to allow us to manage how we ThirdParty Web Integrations should be handled in the web and in in Xamarin native app. 

The knownServices can be accessed on the v3 KnownServices Endpoint in the Backend.PfsApi project (/v3/configuration)

For third-party integrations, 

* If the site requires asserted Login Identity
* Should show the third party warning page to the user

## V3? What were the other two versions
* v1 - Known Services was hard-coded lists in the legacy Android and iOS native apps.
* v2 - Introduced in early 2020, Known Services was added to the v2 configuration endpoint to be consumed by the legacy native apps and also the web to aid the ThirdParty web integrations.
* v3 - This version.

## KnownService properties
* **id**: Unique id for the service.
* **url**: Unique URL for the service.
* **showThirdPartyWarning**: boolean, flag for third-party web integrations to show the warning message before progressing to their site.
* **requiresAssertedLoginIdentity**: boolean, if enabled the redirector will append an token to the url that can be used to verify sso
* **domains**: A list of domains related to this service. 

``` json
{
    "id": "111",
    "url": "https://111.nhs.uk",
    "requiresAssertedLoginIdentity": false,
    "validateSession": false,
    "domains": ["https://subdomain1.111.nhs.uk", "https://subdomains2.111.nhs.uk"]
}
```

## Making changes to the data
In the `backendworker/NHSOnline.Backend.PfsApi` directory there is a base `KnownServicesv3.json` file which contains the `production` known services configuration values. 
Any URLs which can change between environments have been excluded from this base file.

Scratch, Staging and Production uses Helm chart magic to store environmental variants of the file.

### Develop
For local development, a number of `env` files are used to overwrite any values defined in the base file mentioned above.  Currently, known service configuration can be found in the following files, all of which live in the `docker` directory:
* `knownservices.env`
* `nhslogin_connectivity_dev.env` (cid urls for dev)
* `nhslogin_connectivity_ext.env` (cid urls for ext)
* `nhslogin_connectivity_sandpit.env` (cid urls for sandpit)

The double underscore notation `"__"` is used to represent the structured data we see in the base `KnownServices.json` file.

Care must be taken <span style="color:red">*not to include any dashes*</span> when adding/modifying keys as they will be ignored.

### Scratch 
#### Generic
To update the values in scratch environments, you'll need to update the `knownServicesOverrides` environment variables in `/nhsapp/vars/zone/dev/vars-dev.yaml` of the `nhsapp-chart` directory. Any data defined here will override the data from the  base `KnownServices.json` file.

As with the env files in the local environment, the double underscore notation `"__"` is used to represent the structured json data.

#### Specific
Additionally, if an individual scratch environment (eg, Scratch19) needs a specific knownServices value, the environment specific file in `nhsapp-chart/vars/zone/dev/namespace` can define its own values.

This would now give us two levels of "inheritance": 
* `vars-dev.yaml` overriding the values from `KnownServices.json`
* `vars-dev-scratch19.yaml` (for example) overriding values from `vars-dev.yaml`

#### CID - URLs (and other properties)
As with the local develpment environment, there iis no longer a need to have individual known service objects for CID `ext, dev` or `sandpit` urls.  Using `cid_auth` as an exampe, there used to be different json definitions for these three objects so that the url could be set accordingly:
* `cid_auth`
* `cid_dev_auth`
* `cid_sandpit_auth`

But now, a single definition of the cid property (i.e. `cid_auth`) is all that is needed.  The URL (and any other CID property) can be set in the following files:
* `nhsapp-chart/vars/zone/dev/environment_overrides/vars-nhslogin-aos.yaml`
* `nhsapp-chart/vars/zone/dev/environment_overrides/vars-nhslogin-dev18.yaml`
* `nhsapp-chart/vars/zone/dev/environment_overrides/vars-nhslogin-sandpit.yaml`

Then, when deploying from Team City, selecting the correct value from the `NHS login environment` dropdown will deploy the app with the desired CID configuration.

### Staging & Production
Similar to scratch deployments, environment variables in the `knownServicesOverrides` section in the below files need to be updated accordingly
* `vars/zone/production/vars-production.yaml`
* `vars/zone/staging/vars-staging.yaml`
