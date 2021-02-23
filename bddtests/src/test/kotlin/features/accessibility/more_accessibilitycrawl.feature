@accessibility
@notifications-accessibility
  Feature: More section accessibility

    Scenario: The more page is captured
      Given I am a EMIS patient
      When I am logged in
      And I click the more icon
      Then the more page is saved to disk

    Scenario: The manage cookies page is captured
      Given I am an EMIS patient
      And I am logged in
      When I navigate to the Manage cookies page
      Then the Cookies page is displayed
      And the Cookies page is saved to disk

    Scenario: The notifications page is captured
      Given I am using the native app user agent
      And I am a user wishing to enable push notifications
      And I am logged in
      When I navigate to the More page
      Then the More Settings links are available
      And I click the Notifications link on the More page
      And the Notifications Settings page is displayed
      When I change the notifications toggle to on
      Then the notifications toggle is displayed as on
      And the Notifications page is saved to disk
