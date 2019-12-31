@my-record
Feature: Combined Frontend - Medical Record v2

  Scenario Outline: A <GP System> user can view medicines, consultations, and test results - Medical Record v2
    Given I am a <GP System> user setup to use medical record version 2
    And the GP Practice has enabled all medical records for the patient
    When I am on the Medical Record Warning page
    When I click continue
    Then I see the medical record v2 page
    When I click the Medicines link on my record - Medical Record v2
    And I click the Acute medicines link - Medical Record v2
    Then I see the expected acute medicines - Medical Record v2
    When I click the Back link
    When I click the Back link
    And I click the Consultations and events link on my record - Medical Record v2
    Then I see the expected consultations and events - Medical Record v2
    When I click the Back link
    And I click the Test results link on my record - Medical Record v2
    Then I see the correct number of test results for current the supplier - Medical Record v2
    Examples:
      | GP System |
      | EMIS      |
  @smoketest
    Examples:
      | GP System |
      | TPP       |

  Scenario Outline: A <GP System> user can view immunisations and problems - Medical Record v2
    Given I am a <GP System> user setup to use medical record version 2
    And the GP Practice has enabled all medical records for the patient
    And I am on the medical record page
    When I click the Immunisations link on my record - Medical Record v2
    Then I see the expected immunisations - Medical Record v2
    When I click the Back link
    When I click the Health conditions link on my record - Medical Record v2
    Then I see the expected health conditions - Medical Record v2
    Examples:
      | GP System |
      | EMIS      |
  @smoketest
    Examples:
      | GP System |
      | VISION    |