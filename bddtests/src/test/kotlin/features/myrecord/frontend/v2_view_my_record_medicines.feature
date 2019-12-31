@my-record
Feature: Medicines Frontend - Medical Record v2

  Scenario Outline: A <GP System> user has multiple medicines in each section - Medical Record v2
    Given I am a <GP System> user setup to use medical record version 2
    And the GP Practice has enabled all medical records for the patient
    And I am on the medical record page
    When I click the Medicines link on my record - Medical Record v2
    And I click the Acute medicines link - Medical Record v2
    Then I see the expected acute medicines - Medical Record v2
    When I click the Back link
    And I click the Current medicines link - Medical Record v2
    Then I see the expected current medicines - Medical Record v2
    When I click the Back link
    And I click the Discontinued medicines link - Medical Record v2
    Then I see the expected discontinued medicines - Medical Record v2
    When I click the Back link
    And I click the Back link
    Then I see the medical record v2 page
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | MICROTEST |
  @smoketest
    Examples:
      | GP System |
      | VISION    |

  Scenario Outline: A <GP System> user has no medicines on their record - Medical Record v2
    Given I am a <GP System> user setup to use medical record version 2
    And I am on the medical record page
    When I click the Medicines link on my record - Medical Record v2
    And I click the Acute medicines link - Medical Record v2
    Then I see a message that I have no information recorded for a specific record - Medical Record v2
    When I click the Back link
    And I click the Current medicines link - Medical Record v2
    Then I see a message that I have no information recorded for a specific record - Medical Record v2
    When I click the Back link
    And I click the Discontinued medicines link - Medical Record v2
    Then I see a message that I have no information recorded for a specific record - Medical Record v2

    Examples:
    | GP System |
    | EMIS      |
    | VISION    |
    | TPP       |

    Scenario: A MICROTEST user has no medicines on their record - Medical Record v2
      Given I am a MICROTEST user setup to use medical record version 2
      And I have 0 Medications
      And the my record wiremocks are populated
      And I am on the medical record page
      When I click the Medicines link on my record - Medical Record v2
      And I click the Acute medicines link - Medical Record v2
      Then I see a message that I have no information recorded for a specific record - Medical Record v2
      When I click the Back link
      And I click the Current medicines link - Medical Record v2
      Then I see a message that I have no information recorded for a specific record - Medical Record v2
      When I click the Back link
      And I click the Discontinued medicines link - Medical Record v2
      Then I see a message that I have no information recorded for a specific record - Medical Record v2
