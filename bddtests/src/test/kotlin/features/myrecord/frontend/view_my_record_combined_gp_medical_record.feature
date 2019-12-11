@my-record
Feature: Combined Frontend - Medical Record v2

  Scenario Outline: A <GP System> user can view medicines, consultations, and test results - GP Medical Record
    Given I am a <GP System> user setup to use medical record version 2
    And the my record wiremocks have data when the patient is already set for <GP System>
    When I am on the record warning page
    When I click continue
    Then I am on the medical record v2 page
    When I click the Medicines link on my record - GP Medical Record
    And I click the Acute medicines link - GP Medical Record
    Then I see the expected acute medicines - GP Medical Record
    When I click the Back link
    When I click the Back link
    And I click the Consultations and events link on my record - GP Medical Record
    Then I see the expected consultations and events - GP Medical Record
    When I click the Back link
    And I click the Test results link on my record - GP Medical Record
    Then I see the correct number of test results for current the supplier - GP Medical Record
    Examples:
      | GP System |
      | EMIS      |
  @smoketest
    Examples:
      | GP System |
      | TPP       |

  Scenario Outline: A <GP System> user can view immunisations and problems - GP Medical Record
    Given I am a <GP System> user setup to use medical record version 2
    And the my record wiremocks have data when the patient is already set for <GP System>
    And I am on my record information page
    When I click the Immunisations link on my record - GP Medical Record
    Then I see the expected immunisations - GP Medical Record
    When I click the Back link
    When I click the Health conditions link on my record - GP Medical Record
    Then I see the expected health conditions - GP Medical Record
    Examples:
      | GP System |
      | EMIS      |
  @smoketest
    Examples:
      | GP System |
      | VISION    |