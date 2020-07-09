@my-record
Feature: Demographics Frontend - Medical Record v1

  Scenario Outline: A <GP System> user navigates to patient information page - Medical Record v1
    Given I am a <GP System> user setup to use medical record version 1
    And I am logged in
    When I retrieve the 'my record' page directly
    Then I click the 'Continue' button
    And I see header text is Your GP medical record
    And I see the Your details heading on My Record - Medical Record v1
    And I see the patient information details - Medical Record v1
    And I see my record button on the nav bar is highlighted

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario: A EMIS user collapses the patient details section - Medical Record v1
    Given I am a EMIS user setup to use medical record version 1
    And I am logged in
    When I retrieve the 'my record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Your details section on My Record - Medical Record v1
    Then I do not see patient information details - Medical Record v1
