# Router

We use the [VueRouter](https://router.vuejs.org/) to manage the rendering of components based on the app's URL.

_For those familiar with the app when built with Nuxt, routes were generated based on the folder structure in the **src / pages** directory. We've stuck with that concept, and now build the router configuration ourselves._

## Route configuration

All routes are configured within the **src / router / routes** directory. Configurations have been grouped based on the area/feature of the app in which they exist, to make them a bit more manageable.

_The configuration here is very similar to that defined in the old `lib/routes.js`_

Route configurations are defined as follows:
- **path**: in the url for which this route should match (to be defined in **src / router / paths.js**)
- **name**: by which to identify this route. Format is the same as the `path`, replacing / with - (with none trailing)(to be defined in **src / router / names.js**)
- **component**: _page_ component that will be rendered for this route
- **redirect**: the `path` of another route configuration to redirect to (see [Legacy redirect routes](#legacy-redirect-routes))
- **meta**: a custom object used to define meta info about a route (like the old `middleware/meta.js`)
  - **headerKey**: a locale reference or callback function used to update the header for this route
  - **captionKey**: same as `headerKey`, but instead is used for the caption displayed above it
  - **captionSize**: override the default caption size (one of **src / lib / caption-size.js**)
  - **shouldShowContentHeader**: whether the ContentHeader component should be shown on this page (defaulted in the component to `true`)
    - see [OnUpdateHeaderMixin docs](onupdateheadermixin.md#the-payload) for more information on how these properties are used. The payload in this instance is the entire route `meta`
  - **proofLevel**: proof level required to access this route (one of **src / lib / proofLevel.js**)
  - **upliftRoute**: if `proofLevel` is P9, the configuration for a route to which the app will redirect if the user's proof level does not match
  - **crumb**: breadcrumb configuration for the given route. If no crumb is required, set to `{}` (see [Breadcrumbs](breadcrumbs.md))
  - **helpUrl**: to link to from the help icon (to be defined in **src / router / externalLinks.js**)
  - **nativeNavigation**: the native menu tab to select when on this route (to be defined in **src / middleware / nativeNavigation.js**)
  - **redirectRules**: an array of rules where if the `rule.condition` (store getter) matches the `rule.value`, the app will redirect to `rule.route`
  - **sjrRedirectRules**: an array of rules where if the `rule.journey` or `rule.journey_disabled` is enabled or disabled respectively, the app will redirect to the the page at `rule.url`
  - **isAnonymous**: if this page can be viewed without a session
  - **middleware**: an array of [router guards](https://router.vuejs.org/guide/advanced/navigation-guards.html#global-before-guards) to be executed on navigation to this page but before it's rendered (see [Middleware pipeline](#middleware-pipeline))

## Nested routes and the path

We can leverage the nested route functionality of the vue-router to apply base configuration to a number of different paths, defined once. (Essentially `layout` functionality for those familiar with Nuxt)

These routes are defined as follows:
- **path**: same as above
- **name**: same as above
- **component**: a _layout_ component in this case, as opposed to a _page_. This is a wrapping component in which to render all the `children` components, and requires a `<router-view>` to be included in the template
- **children**: an array of route configurations as defined either above to render a page, or here, indicating another set of nested routes
  - **path**: on children, the path must not begin with a `/`, otherwise it will be resolved as an absolute url, rather than relative to the base route path

### Patient ID route prefix

Currently the only nested routes we have are those that need to know the patient context - i.e. are they in proxy mode or not, which allows us to maintain proxy state across full page refreshes.

There is a base route with a path defined as `/patient/:patientId(<guid-regex>)?` (regex omitted for brevity). The `(<guid-regex>)?` here denotes the `patientId` is an optional value that matches the regular expression (see [vue-router docs](https://router.vuejs.org/guide/essentials/dynamic-matching.html#advanced-matching-patterns) for other parameter patterns). If this GUID is not provided, it is assumed the user is not in proxy mode.

Use the `redirectTo` and `redirectByName` methods in **src / lib / utils.js**, which automatically add the `/patient/:patientId` prefix.

Some routes require the NhsukLayout as a base template, but not the `/patient/:patientId` prefix in the url, e.g. **src / pages / get-health-advice.vue**. For these routes, simply use the layout directly in the template. The layout has both a `<slot>` and `<router-view>` to allow this behaviour.

_Consider parent/nested route parameters when defining paths, ensuring there are no conflicting parameter names_

## Middleware pipeline

There are two distinct groups of middleware that run on route navigation:
- **global middleware**: these are executed regardless of where we're navigating to
- **route specific middleware**: these are executed only when the route on which they're defined is matched

It's the responsibility of the src/router/middlewarePipeline.js) to orchestrate the execution of these pieces of middleware. It runs as part of a global `beforeEach` router guard, concats the matched route specific middleware with the global middleware, and executes them in sequence. If the custom `next` callback passed to each middleware is invoked with a an argument, it calls the vue-router `next` passing through the argument, triggering a new navigation event, otherwise it invokes the next piece of middleware and runs until all have been executed.

## Not found route

This route is a fallback route if no other matches were found for the current path. This should always be defined as the last matched route in `allRoutes` in **src / router / index.js**. If a path is defined later in `allRoutes` or `children` routes, it will never match as its path is `*` to catch everything.

## Legacy redirect routes

When updating/removing routes, especially those once referenced in the native code, for example [when we moved symptoms -> advice](https://dev.azure.com/nhsapp/NHS%20App/_git/nhsapp/pullrequest/2392), for backwards compatibility we will need to redirect from the old route to the new route for some time, until the older versions of the app become unsupported. Simply remove all but the `name` and `path` from the configuration, and add a `redirect` containing the path from the route configuration it should redirect to.