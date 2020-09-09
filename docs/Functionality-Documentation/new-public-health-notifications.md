# Adding New Public Health Notifications

Public health notifications are a mechanism by which, with just an update to SJR, we can display important persisting notifications within the app (an entirely separate concept to push notifications or app messaging).

## **Notification Structure**

- **id**: an identifier unique to this config
- **type**: the type of component to use to render the notification in the app. One of:
  - **callout**
- **urgency**: the particular style of component of this type. One of:
  - **warning**
- **title**: the notification title
- **body**: html markup to be rendered within the body of the component

The `type` and `urgency` properties togther will determine the component and its style rendered in the app. A combination of `callout` and `warning` for example will result in a [warning callout](https://nhsuk.github.io/nhsuk-frontend/components/warning-callout).

> This config file has no `$schema` defined as it's included in the journey configuration files, and is validated all together.

## **Creating a new type of Public Health Notification on a new page**

### **Service Journey Rules**

> Public health notifications are defined slightly different to other rules. They are defined as separate config files, each representing a distinct notification, found in the [PublicHealthNotifications](../../backendworker/configurations/Globals/PublicHealthNotifications) directory, and are then included in the main journey configurations.

- [configurations / Globals / PublicHealthNotifications](../../backendworker/configurations/Globals/PublicHealthNotifications) - add a new config file with the structure outlined in [Notification Structure](#notification-structure) and give it the same name as its `id` (see [coronavirus_covid19](../../backendworker/configurations/Globals/PublicHealthNotifications/coronavirus_covid19.yaml) as an example)
- [configurations / Journeys / public_health_notifications](../../backendworker/configurations/Journeys/public_health_notifications) - create a new/update an existing journey configuration file adding the new screen and notification config (see [odscode_A00002_A10006](../../backendworker/configurations/Journeys/public_health_notifications/odscode_A00002_A10006.yaml) as an example)
- [configuration_schema.json](../../backendworker/NHSOnline.Backend.ServiceJourneyRulesApi/Schemas/Journeys/configuration_schema.json) - add definitions for the new screen and notification
  - notifications with mutually exclusive `type`/`urgency` combinations should have separate definitions here as an extra level of validation
  - add the new screen as a property of `journeys` (see the `homeScreen` definition as an example)
- [contracts / servicejourneyrules.yaml](../../backendworker/contracts/servicejourneyrules.yaml) - add/update the component schemas based on the changes made above to the configuration schema
- [Service Journey Rules Models](../../backendworker/NHSOnline.Backend.ServiceJourneyRulesApi.Models)
  - add a new model for the new screen (see [HomeScreen.cs](../../backendworker/NHSOnline.Backend.ServiceJourneyRulesApi.Models/HomeScreen.cs) as an example)
  - add the new screen to [Journeys.cs](../../backendworker/NHSOnline.Backend.ServiceJourneyRulesApi.Models/Journeys.cs)
  - add any new types to [NotificationType.cs](../../backendworker/NHSOnline.Backend.ServiceJourneyRulesApi.Models/NotificationType.cs)
  - add any new urgencies to [NotificationUrgency.cs](../../backendworker/NHSOnline.Backend.ServiceJourneyRulesApi.Models/NotificationUrgency.cs)
- [Service Journey Rules Configuration Steps](../../backendworker/NHSOnline.Backend.ServiceJourneyRulesApi/RuleConfiguration/Utils/Steps) - these files will need extended to support the new screen
  - [MergeOdsJourneys.cs](../../backendworker/NHSOnline.Backend.ServiceJourneyRulesApi/RuleConfiguration/Utils/Steps/MergeOdsJourneys.cs)
  - [ValidateOdsJourneys.cs](../../backendworker/NHSOnline.Backend.ServiceJourneyRulesApi/RuleConfiguration/Utils/Steps/ValidateOdsJourneys.cs)
  - [SanitizeOdsJourneys.cs](../../backendworker/NHSOnline.Backend.ServiceJourneyRulesApi/RuleConfiguration/Utils/Steps/SanitizeOdsJourneys.cs)

### **Web**

- If adding a new screen, import the [PublicHealthNotification](../../web/src/components/widgets/PublicHealthNotification.vue) to display the notifications on the page
- If there are any new `type`/`urgency` values
  - Add a new/update existing components (e.g. [WarningCallout](../../web/src/components/widgets/WarningCallout.vue)) to support them
  - Update the PublicHealthNotification component to validate them and render the appropriate component in the template