@accessibility
@notifications-accessibility
  Feature: Settings section accessibility

    Scenario: The my account page is captured
      Given I am a EMIS patient
      When I am logged in
      And I click the settings icon
      Then the myAccount page is saved to disk

    Scenario: The manage cookies page is captured
      Given I am an EMIS patient
      And I am logged in
      When I navigate to the Manage cookies page
      Then the Cookies page is displayed
      Then the Cookies page is saved to disk

    Scenario: The notifications page is captured
      Given I am using the native app user agent
      And I am a user wishing to enable push notifications
      And I have the instructions cookie
      And I am logged in
      When I navigate to the Account page
      Then the Account Settings are available
      And I click the Notifications link on the Account page
      Then the Notifications Settings page is displayed
      When I change the notifications toggle to on
      Then the notifications toggle is displayed as on
      Then the Notifications page is saved to disk
