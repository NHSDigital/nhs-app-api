# Permissions

## General approach

As permissions is part of xamarin essentials, any permission requests should not be made in the platform specific projects. All requests should be made in the relevant folder inside xamarin.app. 

Before any request is presented to the user to obtain permission, we should be first checking what the status of the permission is. Once we have obtained the status, we have agreed that rather than trying to determine logic to do when permission is unknown or denied or granted, we should simplify our logic to not granted. If the permission is not granted we should make the request for permission (regardless of if the non granted status is unknown or denied).

## Platform specific differences

The iOS and Android platforms have different interpretations and ways that they implement permissions.

### iOS

The iOS platform statuses to note are:
  1. `unknown`:  Default status when the user has not agreed or disagreed. 
  2. `Denied`: User has decided not to grant permission
  3. `Granted`: User has given permission to the request

Although we have taken the approach that we will always make a request for permission if it is not granted, this will not cause iOS to make the additional request. IOS will force the user to go into their settings and update the status of that permission. 

## Sample Implementation

``` csharp
// a list of the permissions that can be used are at https://docs.microsoft.com/en-us/xamarin/essentials/permissions?tabs=android
var status = await Permissions.CheckStatusAsync<Permissions.CalendarWrite>().ConfigureAwait(true);

// we should ensure we always check up front what the current status is
var updatedPermissionStatus = status;

// this is to ensure cross platform, as Android handles the re-request differently but this will not cause issues in iOS
if (status != PermissionStatus.Granted)
{
    updatedPermissionStatus = await Permissions.RequestAsync<Permissions.CalendarWrite>().ConfigureAwait(true);
}
if (updatedPermissionStatus == PermissionStatus.Granted)
{
    // In here you would call whatever you needed the permissions for.
    await _calendar.AddEventToCalendar(calendarEvent).PreserveThreadContext();
}
```