@accessibility
  Feature: Accessibility

    Scenario: The logged-out login page is captured
      When I am on the login logged-out page
      Then the login page is saved to disk

    Scenario: The welcome page is captured
      Given I am a EMIS patient
      When I am logged in
      Then the welcome page is saved to disk