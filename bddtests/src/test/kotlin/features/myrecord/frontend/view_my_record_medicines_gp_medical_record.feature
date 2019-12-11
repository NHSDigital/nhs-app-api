@my-record
Feature: View My Medicines - Medical Record v2

  Scenario Outline: A <GP System> user has multiple medicines in each section - GP Medical Record
    Given I am a <GP System> user setup to use medical record version 2
    And the my record wiremocks have data when the patient is already set for <GP System>
    And I am logged in
    And I am on my record information page and glossary is visible - GP Medical Record
    When I click the Medicines link on my record - GP Medical Record
    And I click the Acute medicines link - GP Medical Record
    Then I see the expected acute medicines - GP Medical Record
    When I click the Back link
    And I click the Current medicines link - GP Medical Record
    Then I see the expected current medicines - GP Medical Record
    When I click the Back link
    And I click the Discontinued medicines link - GP Medical Record
    Then I see the expected discontinued medicines - GP Medical Record
    When I click the Back link
    And I click the Back link
    Then I am on the medical record v2 page
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | MICROTEST |
  @smoketest
    Examples:
      | GP System |
      | VISION    |

  Scenario Outline: A <GP System> user has no medicines on their record - GP Medical Record
    Given I am a <GP System> user setup to use medical record version 2
    And the my record wiremocks are initialised when the patient is already set for <GP System>
    And I am logged in
    And I am on my record information page and glossary is visible - GP Medical Record
    When I click the Medicines link on my record - GP Medical Record
    And I click the Acute medicines link - GP Medical Record
    Then I see a message that I have no information recorded for a specific record - GP Medical Record
    When I click the Back link
    And I click the Current medicines link - GP Medical Record
    Then I see a message that I have no information recorded for a specific record - GP Medical Record
    When I click the Back link
    And I click the Discontinued medicines link - GP Medical Record
    Then I see a message that I have no information recorded for a specific record - GP Medical Record

    Examples:
    | GP System |
    | EMIS      |
    | VISION    |
    | TPP       |

    Scenario: A MICROTEST user has no medicines on their record - GP Medical Record
      Given I am a MICROTEST user setup to use medical record version 2
      And I have 0 Medications
      And the my record wiremocks are populated when the patient is already set for MICROTEST
      And I am logged in
      And I am on my record information page and glossary is visible - GP Medical Record
      When I click the Medicines link on my record - GP Medical Record
      And I click the Acute medicines link - GP Medical Record
      Then I see a message that I have no information recorded for a specific record - GP Medical Record
      When I click the Back link
      And I click the Current medicines link - GP Medical Record
      Then I see a message that I have no information recorded for a specific record - GP Medical Record
      When I click the Back link
      And I click the Discontinued medicines link - GP Medical Record
      Then I see a message that I have no information recorded for a specific record - GP Medical Record
