# Known Services

## What is KnownServices?

Known Services is used in both native devices and also the web to allow us to manage how we handle pages. The knownServices data in included in the v2 Configuration Endpoint in the Backend.PfsApi project (/v2/configuration)

We can also use to control other behaviours in the native apps, for instance:

* Whether a page should open in WebView (i.e with NHS App header and footer) or in the AppTab pattern (simply a close button at the top to return to the app).
* Setting the the footer MenuTab.
* If a site can interact with the native apps via javascript.
* If a site requires a user to be logged in.

and for third-party integrations, 

* If the site requires asserted Login Identity
* Should show the third party warning page to the user

### Knowledge share videos: 
* [video1](https://hscic365.sharepoint.com/:v:/r/sites/NHS%20Online/Shared%20Documents/Knowledge%20Sharing/Recordings/KnowledgeShare_20200214.mp4?csf=1&e=WgwsDE) (25:28:30 - Paul Sharpe - Known Services Intro)
* [video2](https://hscic365.sharepoint.com/:v:/r/sites/NHS%20Online/Shared%20Documents/Knowledge%20Sharing/Recordings/KnowledgeShare_20200228.mp4?csf=1&e=GHcWhS) (15:02:00 - Andrew Robinson - Known Services Configuration)

## RootServices & SubServices and inheritance
KnownServices is a list of RootServices (domains) which can be optionally broken down into SubServices (paths).

* RootService properties
    * **id**: Unique id for the service
    * **url**: Unique URL for the service
    * **subServices**: list of sub services if required
* SubServices properties
    * **path**: of subService (optional, but either path or a queryString is required)
    * **queryString**: of subService (optional, but either path or a queryString is required)
* Shared properties
    * **javascriptInteractionMode**: enum - allows access to control native apps via javascript interfaces)
    * **menuTab**: enum, highlights tab in menu bar, 'none' makes no changes to currently highlighted tab 
    * **viewMode**: enum, webView or appTab
    * **showThirdPartyWarning**: boolean, flag for silver third-party integrations to show the warning message before progressing to their site
    * **requiresAssertedLoginIdentity**: boolean, 
    * **validateSession**: boolean, ensures that the blank screen that is hidden when the app is foregrounded does so when the session is validated first. Thusly preventing showing personal information from momentarily flashing up. 
    * **integrationLevel**: enum, controls header and footer strategy for the native apps.

## How it works

Known services code in native is designed to to work on partial matches. Starting with Host, then paths (and/or querystrings if further rules are required). 

If you want to ensure that `viewMode` is set to `webView` for the whole of 111.nhs.uk, just setting this on a RootService is suitable. e.g.
``` json
{
    "id": "111",
    "url": "https://111.nhs.uk",
    "javaScriptInteractionMode": "None",
    "menuTab": "None",
    "viewMode": "WebView",
    "showThirdPartyWarning": false,
    "requiresAssertedLoginIdentity": false,
    "validateSession": false,
    "subServices": null,
}
```

If you find pages that shouldn't be opening in `webView`, these can be added as subServices. e.g.
``` json
{
    "id": "111",
    "url": "https://111.nhs.uk",
    "javaScriptInteractionMode": "None",
    "menuTab": "None",
    "viewMode": "WebView",
    "showThirdPartyWarning": false,
    "requiresAssertedLoginIdentity": false,
    "validateSession": false,
    "subServices": [
        {
            "path": "/isolation-note",
            "queryString": null,
            "javaScriptInteractionMode": "None",
            "menuTab": "None",
            "viewMode": "AppTab",
            "showThirdPartyWarning": false,
            "requiresAssertedLoginIdentity": false,
            "validateSession": false
        }
    ]
}
```
Now any pages starting with the URL https://111.nhs.uk/isolation-note will open in an `AppTab` pattern, whilst the rest of https://111.nhs.uk opens in the `WebView`.

## Making changes to the data
In the `backendworker/NHSOnline.Backend.PfsApi` directory there is a base `KnownServices.json` file which contains the `production` known services configuration values. 
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
* `nhsapp-chart/vars/zone/dev/environement_overrides/vars-nhslogin-aos.yaml`
* `nhsapp-chart/vars/zone/dev/environement_overrides/vars-nhslogin-dev18.yaml`
* `nhsapp-chart/vars/zone/dev/environement_overrides/vars-nhslogin-sandpit.yaml`

Then, when deploying from Team City, selecting the correct value from the `NHS login environment` dropdown will deploy the app with the desired CID configuration.

### Staging & Production
Similar to scratch deployments, environment variables in the `knownServicesOverrides` section in the below files need to be updated accordingly
* `vars/zone/production/vars-production.yaml`
* `vars/zone/staging/vars-staging.yaml`
