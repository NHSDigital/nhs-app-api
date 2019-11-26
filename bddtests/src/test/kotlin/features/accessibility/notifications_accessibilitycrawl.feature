@accessibility
@notifications-accessibility
  Feature: Notifications accessibility

    Scenario: The notifications page is captured
      Given I am a user wishing to enable push notifications
      And I am logged in
      When I navigate to the Account page for mobile devices
      Then the Account Settings are available
      And I click the Notifications link on the Account page
      Then the Notifications Settings page is displayed
      When I change the notifications toggle to on
      Then the notifications toggle is displayed as on
      Then the Notifications page is saved to disk