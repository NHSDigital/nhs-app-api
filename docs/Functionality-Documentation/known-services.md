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
    * **validateSession**: boolean, 

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
Now any pages starting with the URL https://111.nhs.uk/isolation-note will open in an `AppTab` pattern

## Making changes to the data
Each environment uses a specific `KnownServices.json` to populate the data. Care must be take when copying data between environments as some urls vary between environments.

Scratch, Staging and Production uses Helm chart magic to store environmental variants of the file.

### Develop
For local development, the `KnownServices.json` file can be in the root of the `Backend.PfsApi` project.

### Scratch 
To updated the values in scratch environments, you'll need to updated the `knownservices_config` property in `/nhsapp/values.yaml` of the `nhsapp-chart` directory.

### Staging & Production
Similar to scratch deployments, the `knownservices_config` property in the below files also need to be updated in the [nhsapp-ops-vault repo](https://dev.azure.com/nhsapp/NHS%20App/_git/nhsapp-ops-vault)

* `vars/zone/production/vars-production.yaml`
* `vars/zone/staging/vars-staging.yaml`
