# Breadcrumbs

## **Breadcrumb Configuration**

All breadcrumbs are configured within the [web/src/breadcrumbs](../../../../web/src/breadcrumbs) directory. Configurations have been grouped based on the area/feature of the app in which they exist, and pretty much map directly to the associated routes configuration files.

The properties of the breadcrumb can be thought of in two distinct groups:

- When referenced in another breadcrumb configuration's `defaultCrumb` (or alternative crumb) properties
  - **name**: the [name](../../../../web/src/router/names.js) of the route to navigate to when this crumb is clicked in the breadcrumb trail
  - **i18nKey**: the locale key to use for the crumb label

- When used as a `crumb` value on a [route configuration](router.md)
  - **defaultCrumb**: an array of one or more breadcrumb configurations. It is from this array the actual breadcrumb trail is built.
  - **customCrumb**: to be used when the breadcrumb configuration depends on the navigation context - e.g. `appMessagesOnlyCrumb` for when we want to override the default messages hub crumb when navigating to app messaging from the Home page
  - **nativeDisabled**: only needs defined if the breadcrumb should be hidden on native (currently defaulted to false)

> Note: If a route has no breadcrumb either on desktop or native, set its configuration `crumb` to be `{}`

### **Examples**

```js
const HEALTH_INFORMATION_UPDATES_MESSAGES_CRUMB = {
  defaultCrumb: [INDEX_CRUMB, MESSAGES_CRUMB, HEALTH_INFORMATION_UPDATES_CRUMB],
  appMessagesOnlyCrumb: [INDEX_CRUMB, HEALTH_INFORMATION_UPDATES_CRUMB],
};

const HEALTH_INFORMATION_UPDATES_CRUMB = {
  defaultCrumb: [INDEX_CRUMB, MESSAGES_CRUMB],
  appMessagesOnlyCrumb: [INDEX_CRUMB],
  i18nKey: 'healthAndInformationUpdates',
  name: HEALTH_INFORMATION_UPDATES_NAME,
  nativeDisable: true,
};
```