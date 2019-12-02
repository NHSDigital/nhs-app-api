@my-record
Feature: View My Encounters - Medical Record v2

  Scenario: A MICROTEST user can view immunisations - GP Medical Record
    Given I am a MICROTEST user setup to use medical record version 2
    And the my record wiremocks are populated when the patient is already set for MICROTEST
    And I am logged in
    And I am on my record information page and glossary is visible - GP Medical Record
    When I click the Encounters link on my record - GP Medical Record
    Then I see the expected encounters - GP Medical Record

  Scenario: A MICROTEST user has no immunisations on their record - GP Medical
    Given I am a MICROTEST user setup to use medical record version 2
    And I have 0 Encounters
    And the my record wiremocks are populated when the patient is already set for MICROTEST
    And I am logged in
    And I am on my record information page and glossary is visible - GP Medical Record
    When I click the Encounters link on my record - GP Medical Record
    Then I see a message that this information isn't available through the NHS App - GP Medical Record
