# Silver Integration Jump-Off Creation

## Introduction

Silver Integration Jump-Offs allow users of the NHS App to seamlessly access functionality provided by NHS services and third parties commissioned by their CCG, such as e-RS Manage Your Referral and Patients Know Best.

The technical implementation of this functionality builds upon a number of enabling technologies, including Service Journey Rules, [Known Services](known-services.md), and NHS Login Asserted Login Identity.

These notes are intended to explain the code changes required to add a single new Silver Integration Jump-Off to the NHS App.

## Silver Integration Jump-Off Creation Steps

### Service Journey Rules

Silver Integration Service Journey Rules are defined in the backend C# code in the SilverIntegrations.cs class within NHSOnline.Backend.ServiceJourneyRulesApi.Models namespace.

Each silver integration rule describes a feature (appointments, medicines, health trackers etc) corresponding to a jump-off location within the NHS App. The values taken by each of these rules is a list of enum values corresponding to third-party providers of each feature. As such, when adding a new silver integration jump off point for a new provider, it may not be necessary to create a new rule in SilverIntegrations - if the relevant feature rule already exists, your work here may be limited to adding a new enum value for the new provider and updating the configuration_schema.json.

If an entirely new silver integration rule is required, for a new feature that does not yet exist in the Silver integrations, the following areas of the code will need to be extended:

* `backendworker/configurations/journeys/all_defaults.yaml` - to set default value for the new rule (likely an empty array).
* `backendworker/configurations/journeys/silver_integrations/(files)` - to set appropriate values for the new rule for test practices.
* `backendworker/ServiceJourneyRulesApi.Models/SilverIntegrations`
* `backendworker/ServiceJourneyRulesApi.Models/(add new Provider class)`
* `backendworker/ServiceJourneyRulesApi/RuleConfiguration/Utils/Steps/ValidateOdsJourneys` - to validate presence of the new rule for each ODS code
* `backendworker/ServiceJourneyRulesApi/Schemas/Journeys/configuration_schema.json`
* `backendworker/ServiceJourneyRulesApi.Models.UnitTests/SilverIntegrationsBuilder`
* `backendworker/ServiceJourneyRulesApi.UnitTests/MergeOdsJourneysTests`
* `backendworker/ServiceJourneyRulesApi.UnitTests/ValidateOdsJourneysTests`
* `web/src/store/modules/sjr/mutation_types.js`

Conversely, if a suitable silver integration rule already exists, and a new enum value simply requires adding for the new provider, the changes are limited to:

* `backendworker/configurations/journeys/silver_integrations/(files)` - to set appropriate values for the new provider for test practices.
* `backendworker/ServiceJourneyRulesApi.Models/(provider class for feature)` - to add the new provider enum to the relevant feature.
* `backendworker/ServiceJourneyRulesApi/Schemas/Journeys/configuration_schema.json`

Production service journey rules should be updated at the appropriate time in the nhsapp-journeyrules repository - https://dev.azure.com/nhsapp/NHS%20App/_git/nhsapp-journeyrules

### Known Services

See additional [Known Services documentation](known-services.md)

Known Services configures the behaviour of third party URLs surfaced within the NHS App. This is generally configured at a domain-level, i.e. per third-party provider. _If the relevant third-party provider already exists, you may not require any changes here._

If an entirely new third-party provider is being introduced, add a new entry to the KnownServices JSON. Some notes on the field values to use:

* **id** should be a short unique ID for the third-party, in lowercase without any whitespace. This will end up in splunk analytics logs to be used for reporting purposes.
* **url** should be the fully-qualified domain of the third party, no trailing slash.
* **javaScriptInteractionMode** - should probably be **SilverThirdParty**. This new mode introduced by NHSO-8473 PR1084 gives the third-party access to JavaScript API via which they can invoke function in the native NHS App.
* **viewMode** should probably be **WebView**, so that the third party website appears within the native webview (as opposed to the site opening in a new AppTab).
* **showThirdPartyWarning** is used to indicate whether the user is presented with a warning page after clicking on the Jump Off point, containing a Continue button which they must click to proceed to the third party site. Generally this is **false** for other NHS services (such as e-RS MYR), but **true** for services provided by third parties such as Patients Know Best. If this is set to **true**, content for the warning message should be defined in the third-party-providers.js locale file (see below).
* **requiresAssertedLoginIdentity** is used to indicate whether access to the third party service requires use of NHS Login asserted identity flow (SSO). If so, the redirector page will invoke a backend API method to generate an asserted login identity token, and append it to the URL of the page redirected to, in an assertedLoginIdentity querystring.
* **validateSession** is used to indicate whether the user has an active session before rendering the third-party webview. Generally this should be **true** for silver integrations.

### Web Changes

Changes to the web project to add a new silver integration jump-off point should be limited to editing the following four files:

* **web / src / lib / third-party-providers / jump-off-configuration.js** - edit the JSON to add a new jump off point, within a section for a new provider if this is the first jump-off for a particular provider. Note that in the redirectPath field, any querystring parameters should be url encoded.
If adding a new child brand for the same primary provider (e.g Care Information Exchange as a child brand of Patients Know Best), add these as new jump-offs within the same parent thirdPartyProvider, disambiguated with suffixes - e.g. appointmentsCie for the CIE-branded version of PKB appointments.
* **web / src / locale / en / third-party-providers.js** - again, edit the JSON to add a new jump off point, within a section for a new provider if this is the first jump off for a particular provider. Child brands for the same primary provider should be added as new jump-offs within the same provider. 
    * **serviceId** - this should correspond to the ID of the associated Known Service.
    * **provider Name** - this text will appear on the third-party warning screen (if showThirdPartyWarning set to true in Known Services), and also within splunk logging and audit records.
    * **jumpOffs / id** - should correspond to the entry added in jump-off-configuration.js
    * **jumpOffs / path** - the path on the third party site to be linked to. Note that here, any querystring parameters should not be url encoded, unlike in jump-off-configuration.js. For child brands, this path should be suffixed with a dummy `brand` querysting parameter, to allow disambiguation of content between multiple brands.
    * **jumpOffs / jumpOffContent** - headerText and descriptionText containing the content to be shown on the jump-off button.
    * **jumpOffs / thirdPartyWarning** - additional content to be shown on the third-party warning screen. This can be omitted if showThirdPartyWarning is set to false in Known Services (e.e. ers manageYourReferral). Note that the `brandName` property is optional - if provided this will be used instead of the `providerName` in the warning page content.
* **The vue page to which the jump-off button should be added.**
    * Add an instance of the vue component ThirdPartyJumpOffButton, specifying the `provider-configuration` by referencing the relevant object from jump-off-configuration.js.
    * Use the sjrIf library to add a computed property to determine whether the jump-off component should be shown. It may be necessary to combine this with additional checks, e.g. that the user is not proxying, or has a p9 proof level.
* **The unit test fixture for the above page.**

### Integration Tests

In general there are only two areas of the integration test suite that will require enhancements when a new silver integration jump off is added:

* **Add a new feature file within the test / kotlin / features / silverIntegration folder for the specific jump-off point.**

    This can be expedited by copying-and-pasting one of the existing PKB silver integration feature files, such as patients_know_best_sharedLinks.feature and using this as a stating point, changing the scenarios and step titles as necessary, then implementing any new step definitions as required. Note carefully the construction of the urls in the "When I navigate to the redirector page with a url of" step - some aspects of the URLs may need to be doubly-url-encoded for the test to match correctly.
    Also, for tests that assert that the link is not present on a page, be sure to have also included a step to check that the user has actually reached the expected page. If the test has failed to reach the page, the link will not be present, and the test will pass incorrectly.
* **If required, edit the feature file for any 'Hub' or other pages which have behaviour affected by the presence or absence of the new jump off point.**

    For example, the Health Records Hub page currently only shows if the user has visibility of either the PKB Care Plans or PKB Health Trackers. If the user does not, the page will not appear and the patient will be SJR-redirected to their medical record page.
    Similarly, if a patient does not have either ERS secondary appointments nor PKB secondary appointments, then the Appointments Hub will not include a link to the Hospital Appointments page, and any attempts to directly access the Hospital Appointments page will be unsuccessful. These scenarios should be covered with integration tests on the appropriate page's feature file.

## Further Resources

* An example pull request that added a new silver integration jump off point (Patients Know Best Shared Links) - https://dev.azure.com/nhsapp/NHS%20App/_git/nhsapp/pullrequest/770
* The [Knowledge Share](https://confluence.service.nhs.uk/display/NO/Knowledge+share) session on 01 May 2020 includes a piece by Ian Nelson demonstrating how to add a new silver integration jump off point.