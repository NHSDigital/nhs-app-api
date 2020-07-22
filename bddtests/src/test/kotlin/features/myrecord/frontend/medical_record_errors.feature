@my-record
Feature: Medical Record Errors

  Scenario Outline: A user has no GP session and sees a reference code when navigating to medical record
    Given I have valid OAuth details and <GP System> fails to respond in 31 seconds
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    Then I see the error reference code with prefix '<Prefix>'
    And I click the error 'Report a problem' link with a url of 'https://www.nhs.uk/contact-us/nhs-app-contact-us'
    And a new tab has been opened by the link
    Examples:
      | GP System | Prefix |
      | EMIS      | ze     |
      | TPP       | zt     |
      | VISION    | zs     |
      | MICROTEST | zm     |
