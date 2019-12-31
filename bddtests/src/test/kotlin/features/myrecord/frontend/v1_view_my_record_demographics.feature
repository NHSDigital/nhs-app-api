@my-record
Feature: Demographics Frontend - Medical Record v1

  Scenario Outline: A <GP System> user navigates to patient information page - Medical Record v1
    Given I am a <GP System> user setup to use medical record version 1
    And I am on the Medical Record Warning page
    Then I click continue
    And I see header text is Your GP medical record
    Then I see the Your details heading on My Record - Medical Record v1
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
    And I am on the medical record page
    When I click the Your details section on My Record - Medical Record v1
    Then I do not see patient information details - Medical Record v1
