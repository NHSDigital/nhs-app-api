# Fake GP Supplier

[[_TOC_]]

To assist with testing and supplier onboarding we have created a 5th Fake GP Supplier.

This implementation is designed to return dummy responses for a limited subset of PFS API endpoints, let users gain access to the NHS App as a P9 user and simulate 3rd party API faults.

Currently this functionality only works when the target NHS App environment is configured to use the `NHS Login Sandpit` environment.

All code is maintained in the `NHSOnline.Backend.GpSystems.Suppliers.Fake` namespace in `backendworker/NHSOnline.Backend.GpSystems/Suppliers/Fake`.

## User configuration

The registered users for the Fake GP supplier are maintained in the `backendworker/FakeGpSupplierConfig.yml` file.

### Adding/Updating Users

Simply add/update a YAML entry under one of the user groups (`pkb`, `substrakt` etc.) or create a new user group as required:

```yaml
---
defaultUsers:
  # …other entries…
  substrakt:    # User Group Name
    1234567890: # NHS Number
      userId: a43cd1fe-7d42-4d5f-a095-24a9ae5b9fd
      emailAddress: onboarding.nhsapp+substrakt.user999@gmail.com
      odsCode: F00003
      givenName: Linda
      familyName: Gardner
      dateOfBirth: "1995-07-06T00:00:00"
      sex: Female
      linkedAccountsNhsNumbers: # Optional
        - "0987654321"
  # …other entries…
```

**Note: new users must be setup in the NHS Login sandbox environment or else this will not work!**

### User Behaviours 

Each user can be given a specific set of behaviours, which will determine what the PFS API returns to the front end of each application area. This allows you to create some users with appointment test data, others which throw an error when trying to login etc.

#### Configuration

All behaviours are set to `default` if none is specified for a user. If you wish to override a behaviour, set the appropriate key in the `behaviours` object:

```yaml
  substrakt:
    1234567890:
      userId: a43cd1fe-7d42-4d5f-a095-24a9ae5b9fd
      # …other fields…
      sex: Female
      behaviours:
        appointments: Default
        appointmentSlots: Default
        session: BadGateway
        # etc…
```

### Changing Users at Runtime

The Fake GP supplier has the ability to load users config from a Mongo collection instead of the YAML configuration file.

**Note: only users in the `dynamic` user group in the YAML file can be dynamically changed at runtime**

The supplier follows the below logic:

1. The user is loaded from the YAML file
1. If the user is in the `dynamic` user group, it will write a record to Mongo
1. From that point on the API will load config for that `dynamic` user from Mongo
1. If the user is removed from the Mongo collection, the API will simply start from step one again

Once you have logged in at least once with a user, access the `fake_gp_users` collection for your current environment to edit users in the same way as you would in the YAML file.

*Note: the Mongo collection is configured to set the user's NHS Number as the main key*

## Adding New Functionality

To allow more areas of the app to work with the fake supplier, you need to implement both areas and their associated behaviours.

See `backendworker/NHSOnline.Backend.GpSystems/Suppliers/Fake/Users/AreaBehaviours.cs` for a full list of area names.

*For the majority of areas, only the `default` behaviour is available - which just returns an error response.*

### Adding an Area

1. Add a new directory to `backendworker/NHSOnline.Backend.GpSystems/Suppliers/Fake`
1. Create a new interface which describes the actions that behaviours must implement:

    ```csharp
    namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.SomeNewArea
    {
        [FakeGpArea("SomeNewArea")]
        public interface ISomeNewAreaAreaBehaviour
        {
        // …describe the interface…
    ```

    *The `FakeGpArea` registers this interface as a Fake GP area at runtime automatically*

1. Create a default behavior implementation (*see the next section for guidance*)
1. Add a new area property to the class `backendworker/NHSOnline.Backend.GpSystems/Suppliers/Fake/Users/AreaBehaviours.cs`
1. Add a new getter for your area behaviour interface in the class `backendworker/NHSOnline.Backend.GpSystems/Suppliers/Fake/Users/FakeUser.cs`:

    ```csharp
    [YamlIgnore]
    [BsonIgnore]
    public ISomeNewAreaAreaBehaviour SomeNewAreaAreaBehaviour =>
        ServiceProvider?.ResolveAreaBehaviour<ISomeNewAreaAreaBehaviour>(
            Behaviours?.SomeNewAreaArea ?? Behaviour.Default
        );
    ```

1. Create a new `Fake` service for your area which implements the related `GpSystem` interface used by the API
1. Create a `ServiceCollectionExtensions` class in your new namespace that adds the new service for both `PFS` and/or `CID`, depending on where it is used
1. Call your new extensions for `PFS` / `CID` in the class `backendworker/NHSOnline.Backend.GpSystems/Suppliers/Fake/ServiceCollectionExtensions.cs`
1. Add a getter for your new area to the class `backendworker/NHSOnline.Backend.GpSystems/Suppliers/Fake/FakeGpSystem.cs`

### Adding a Behaviour

1. If adding a new behaviour type, make sure to add it to `backendworker/NHSOnline.Backend.GpSystems/Suppliers/Fake/Behaviour.cs` 
1. Identify the area you wish to add a new behaviour to
1. Identify the behaviour interface for that area. For example in the session area it is `ISessionAreaBehaviour`. *Check the default behaviour class if you are not sure*
1. In the area directory, inside the fake GP implementation, add a new class. As an example, here is a session area behaviour:

    ```csharp
    namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Session
    {
        [FakeGpAreaBehaviour(Behaviour.Magic)]
        public class SomeMagicalSessionAreaBehaviour : ISessionAreaBehaviour
        {
        // …implement the interface…
    ```

    *The `FakeGpAreaBehaviour` attribute will automatically register this behaviour at run time*

1. You can now use the new behaviour in configuration, referencing the `Behaviour` enum key you used
