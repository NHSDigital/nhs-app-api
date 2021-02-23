@pushNotifications
Feature: Push Notifications
#The following tests are gp system agnostic

  Scenario: A user can enable push notifications for their device for the first time
    Given I am using the native app user agent
    And I am a user wishing to enable push notifications for the first time, with my initial state undetermined
    And I am logged in
    When I navigate to the More page
    Then the More Settings links are available
    And I click the Notifications link on the More page
    And the Notifications Settings page is displayed
    And the notifications toggle is displayed as off
    When I change the notifications toggle to on
    Then the notifications toggle is displayed as on
    And the push registration has been added to the repository

  Scenario: A user can enable push notifications during login
    Given I am using the native app user agent
    And I am a user wishing to enable push notifications for the first time, with my initial state undetermined
    And I log in to the app expecting to see the notifications prompt
    Then I see the notifications prompt
    When I accept notifications and continue
    Then I see the home page

  Scenario: A user who already has the notifications cookie will not see the prompt during login
    Given I am using the native app user agent
    And I am a user wishing to enable push notifications for the first time, with my initial state undetermined
    And I am logged in
    Then I see the home page

  Scenario: A user can log in without accepting notifications
    Given I am using the native app user agent
    And I am a user wishing to enable push notifications for the first time, with my initial state undetermined
    And I log in to the app expecting to see the notifications prompt
    Then I see the notifications prompt
    When I do not accept notifications and continue
    Then I see the home page

  Scenario: A user viewing notifications prompt when notifications is denied sees an error
    Given I am using the native app user agent
    And I am a user wishing to enable push notifications for the first time, with my initial state undetermined
    And I log in to the app expecting to see the notifications prompt
    Then I see the notifications prompt
    When I accept notifications but I am denied
    Then I see the notification failure
    When I continue from the notification failure
    Then I see the home page

  Scenario: A user can enable push notifications for their device
    Given I am using the native app user agent
    And I am a user wishing to enable push notifications
    And I am logged in
    When I navigate to the More page
    Then the More Settings links are available
    And I click the Notifications link on the More page
    And the Notifications Settings page is displayed
    And the notifications toggle is displayed as off
    When I change the notifications toggle to on
    Then the notifications toggle is displayed as on
    And the push registration has been added to the repository

  Scenario: A user can navigate to the push notifications Privacy policy page
    Given I am using the native app user agent
    And I am a user wishing to enable push notifications for the first time, with my initial state undetermined
    And I am logged in
    When I navigate to the More page
    Then the More Settings links are available
    And I click the Notifications link on the More page
    And the Notifications Settings page is displayed
    When I click the link called 'NHS App privacy policy' with a url of 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/'
    Then a new tab has been opened by the link

  @smoketest
  Scenario: A user can disable push notifications for their device
    Given I am using the native app user agent
    And I am a user wishing to disable push notifications
    And I am logged in skipping the notifications prompt
    When I navigate to the More page
    And I click the Notifications link on the More page
    Then the Notifications Settings page is displayed
    And the notifications toggle is displayed as on
    When I change the notifications toggle to off
    Then the notifications toggle is displayed as off
    And the push registration has been removed from the repository

  Scenario: A desktop user cannot register for push notifications
    Given I am a user wishing to enable push notifications
    And I am logged in
    When I navigate to the More page
    Then the More page is displayed
    And there is no notification link available
    When I browse to the pages at the following urls I see the home page
      | /more/notifications |

  Scenario: A user with notifications disabled in service journey rules cannot register for push notifications
    Given I am using the native app user agent
    And I am a EMIS user where the journey configurations are:
      | Journey       | Value    |
      | notifications | disabled |
    And I am logged in
    When I navigate to the More page
    Then there is no notification link available
    When I browse to the pages at the following urls I see the home page
      | /more/notifications                |
      | /more/notifications?source=ios     |
      | /more/notifications?source=android |

  Scenario: A user navigating back from the push notifications settings page is directed to the account page
    Given I am using the native app user agent
    And I am a user wishing to enable push notifications
    And I am logged in
    When I navigate to the More page
    And I click the Notifications link on the More page
    Then the Notifications Settings page is displayed
    When I click the 'Back' breadcrumb
    Then the More page for mobile devices is displayed

  Scenario: A user viewing notifications settings when an internal server error occurs sees an error
    Given I am using the native app user agent
    And I am a user wishing to enable push notifications with existing incorrect user device
    And I am logged in with notifications enabled, but with an existing incorrect user device
    When I navigate to the More page
    And I click the Notifications link on the More page
    Then an error is displayed indicating that the notifications service is not available
    When I click the 'Back to settings' button
    Then the More page for mobile devices is displayed

  Scenario: A user attempting to disable a non-existing registration is shown an error message
    Given I am using the native app user agent
    And I am a user wishing to disable push notifications
    And I am logged in skipping the notifications prompt
    When I navigate to the More page
    And I click the Notifications link on the More page
    Then the Notifications Settings page is displayed
    And the notifications toggle is displayed as on
    When the push notification registration record can no longer be found in the repository
    And I change the notifications toggle to off
    Then an error is displayed indicating that the notifications service is not available
    When I click the 'Back to settings' button
    Then the More page for mobile devices is displayed

  Scenario: A user viewing notifications settings when their device's notifications are disabled sees an error
    Given I am using the native app user agent
    And I am a user wishing to enable push notifications with disabled device's notifications
    And I am logged in with notifications denied
    When I navigate to the More page
    And I click the Notifications link on the More page
    Then an error is displayed indicating that the device's notifications are disabled
    When I click the 'Try again' button
    # verify that the error is re-displayed if no action is taken to rectify the issue
    Then an error is displayed indicating that the device's notifications are disabled
    When I enable notifications in the device's settings
    And I click the 'Try again' button
    Then the Notifications Settings page is displayed

  Scenario: A user attempting to disable push notifications when device's notifications are disabled sees an error
    Given I am using the native app user agent
    And I am a user wishing to disable push notifications
    And I am logged in skipping the notifications prompt
    When I navigate to the More page
    And I click the Notifications link on the More page
    Then the Notifications Settings page is displayed
    And the notifications toggle is displayed as on
    When I disable notifications in the device's settings
    And I change the notifications toggle to off
    Then an error is displayed indicating that it could not save because the device's notifications are disabled
    When I click the 'Try again' button
    # verify that the error is re-displayed if no action is taken to rectify the issue
    Then an error is displayed indicating that it could not save because the device's notifications are disabled
    When I enable notifications in the device's settings
    And I click the 'Try again' button
    Then the notifications toggle is displayed as off
    And the push registration has been removed from the repository

  Scenario: A user attempting to enable push notifications when device's notifications are disabled sees an error
    Given I am using the native app user agent
    And I am a user wishing to enable push notifications for the first time, with my initial state undetermined
    And I am logged in
    When I navigate to the More page
    And I click the Notifications link on the More page
    Then the Notifications Settings page is displayed
    And the notifications toggle is displayed as off
    When I disable notifications in the device's settings
    And I change the notifications toggle to on
    Then an error is displayed indicating that it could not save because the device's notifications are disabled
    When I click the 'Try again' button
    # verify that the error is re-displayed if no action is taken to rectify the issue
    Then an error is displayed indicating that it could not save because the device's notifications are disabled
    When I enable notifications in the device's settings
    And I click the 'Try again' button
    Then the notifications toggle is displayed as on
    And the push registration has been added to the repository
