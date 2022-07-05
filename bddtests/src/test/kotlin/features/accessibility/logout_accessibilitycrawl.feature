@accessibility
@logout-accessibility
  Feature: Logout section accessibility

    Scenario: The more page is captured
      Given I am a EMIS patient
      When I am logged in
      And I click the logout link
      Then the logout page is saved to disk
