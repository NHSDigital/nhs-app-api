@pushNotifications
Feature: Push Notifications
#The following tests are gp system agnostic

  @smoketest
  Scenario: A user can enable push notifications for their device
    Given I am a user wishing to enable push notifications
    And I am logged in
    When I navigate to the Account page for mobile devices
    Then the Account Settings are available
    And I click the Notifications link on the Account page
    Then the Notifications Settings page is displayed
    When I change the notifications toggle to on
    Then the notifications toggle is displayed as on

  Scenario: A user can disable push notifications for their device
    Given I am a user wishing to disable push notifications
    And I am logged in
    When I navigate to the Account page for mobile devices
    And I click the Notifications link on the Account page
    Then the Notifications Settings page is displayed
    When I change the notifications toggle to off
    Then the notifications toggle is displayed as off
    
  Scenario: A desktop user cannot register for push notifications
    Given I am a user wishing to enable push notifications
    And I am logged in
    When I navigate to the Account page for desktop
    Then the Account page is displayed
    Then there are no Account Settings available
    When I browse to the pages at the following urls I see the home page
      | /account/notifications  |

  Scenario: A user with notifications disabled in service journey rules cannot register for push notifications
    Given I am a EMIS user where the journey configurations are:
      | Journey            | Value    |
      | notifications      | disabled |
    And I am logged in
    When I navigate to the Account page for mobile devices
    Then there are no Account Settings available
    When I browse to the pages at the following urls I see the home page
      | /account/notifications  |
      | /account/notifications?source=ios  |
      | /account/notifications?source=android  |

  Scenario: A user navigating back from the push notifications settings page is directed to the account page
    Given I am a user wishing to enable push notifications
    And I am logged in
    When I navigate to the Account page for mobile devices
    And I click the Notifications link on the Account page
    Then the Notifications Settings page is displayed
    When I click the 'Back' button
    Then the Account page for mobile devices is displayed