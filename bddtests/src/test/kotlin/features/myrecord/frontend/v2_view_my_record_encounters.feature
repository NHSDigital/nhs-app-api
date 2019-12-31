@my-record
Feature: Encounters Frontend - Medical Record v2

  Scenario: A MICROTEST user can view immunisations - Medical Record v2
    Given I am a MICROTEST user setup to use medical record version 2
    And the my record wiremocks are populated
    And I am on the medical record page
    When I click the Encounters link on my record - Medical Record v2
    Then I see the expected encounters - Medical Record v2

  Scenario: A MICROTEST user has no immunisations on their record - Medical Record v2
    Given I am a MICROTEST user setup to use medical record version 2
    And I have 0 Encounters
    And the my record wiremocks are populated
    And I am on the medical record page
    When I click the Encounters link on my record - Medical Record v2
    Then I see a message that this information isn't available through the NHS App - Medical Record v2
