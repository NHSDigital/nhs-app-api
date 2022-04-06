@accessibility
@notifications-errors-accessibility
Feature: notifications errors accessibility

  Scenario: Native Failure when user toggles is captured
    Given I am using the native app user agent
    And I am a user wishing to enable push notifications for the first time, with my initial state undetermined
    And I am logged in
    When I navigate to the More page
    And I click the Account and settings link on the More page
    And I click the Manage notifications link on the account and settings page
    Then the Notifications Settings page is displayed
    And the notifications toggle is displayed as off
    When I disable notifications in the device's settings
    And I change the notifications toggle to on
    Then an error is displayed indicating that the device's notifications are disabled
    And the Errors_ENO1_NativeFailureWhenUserToggles page is saved to disk

  Scenario: Notification api error is captured
    Given I am using the native app user agent
    And I am a user wishing to enable push notifications with existing incorrect user device
    And I am logged in with notifications enabled, but with an existing incorrect user device
    When I navigate to the More page
    And I click the Account and settings link on the More page
    And I click the Manage notifications link on the account and settings page
    Then an error is displayed indicating that the notifications service is not available
    And the Errors_ENO2_NotificationApiFailure page is saved to disk

  Scenario: Native failure when user tries to access Notifications page is captured
    Given I am using the native app user agent
    And I am a user wishing to enable push notifications with disabled device's notifications
    And I am logged in with notifications denied
    When I navigate to the More page
    And I click the Account and settings link on the More page
    And I click the Manage notifications link on the account and settings page
    Then an error is displayed indicating that the device's notifications are disabled
    And the Errors_ENO4_NativeFailureWhenUserAccessesNotificationsPage page is saved to disk

  Scenario: Native notifications failure on user registration journey is captured
    Given I am using the native app user agent
    And I am a user wishing to enable push notifications for the first time, with my initial state undetermined
    And I log in to the app expecting to see the notifications prompt
    Then I see the notifications prompt
    When I click the 'Yes, turn on notifications on this device' radio button
    And I click the 'Continue' button
    And I deny notifications
    Then I see the notification failure
    And the Errors_ENO5_NativeNotificationFailureOnRegistration page is saved to disk
