@my-record
Feature: Medical Record Errors

  Scenario Outline: A <GP System> user has no GP session and sees a reference code when navigating to medical record
    Given I have valid OAuth details and <GP System> fails to respond in 31 seconds
    And 'NHS UK' responds to requests for '/nhs-app-contact-us'
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I see appropriate try again error message for gp medical record when there is no GP session
    And I click the 'Try again' button
    Then I see the error reference code with prefix '<Prefix>'
    When I click the 'Report a problem' link
    And a new tab has been opened by the link
    Examples:
      | GP System | Prefix |
      | EMIS      | ze     |
      | TPP       | zt     |
      | VISION    | zs     |
