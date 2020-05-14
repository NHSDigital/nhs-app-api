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

    [video1](https://hscic365.sharepoint.com/:v:/r/sites/NHS%20Online/Shared%20Documents/Knowledge%20Sharing/Recordings/KnowledgeShare_20200214.mp4?csf=1&e=WgwsDE) (25:28:30 - Paul Sharpe - Known Services Intro)
    [video2](https://hscic365.sharepoint.com/:v:/r/sites/NHS%20Online/Shared%20Documents/Knowledge%20Sharing/Recordings/KnowledgeShare_20200228.mp4?csf=1&e=GHcWhS) (15:02:00 - Andrew Robinson - Known Services Configuration)

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


## Making changes to the data

Each environment uses a specific KnownServices.json to populate the data. Care must be take when copying data between environments as some urls vary between environments.

### Develop

KnownServices.json file in the root of the Backend.PfsApi project.
Scratch

`knownservices_config` property in `/nhsapp/values.yaml` of the `nhsapp-chart` directory.

### Staging

### Production


