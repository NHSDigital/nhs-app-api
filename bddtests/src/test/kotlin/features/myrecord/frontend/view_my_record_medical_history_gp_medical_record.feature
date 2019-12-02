@my-record
Feature: View My Medical History - Medical Record V2

  Scenario: A MICROTEST user can view medical history - GP Medical Record
    Given I am a MICROTEST user setup to use medical record version 2
    And the my record wiremocks are populated when the patient is already set for MICROTEST
    And I am logged in
    And I am on my record information page and glossary is visible - GP Medical Record
    When I click the Medical history link on my record - GP Medical Record
    Then I see the expected medical history - GP Medical Record

  Scenario: A MICROTEST user has no medical history on their record - GP Medical
    Given I am a MICROTEST user setup to use medical record version 2
    And I have 0 MedicalHistories
    And the my record wiremocks are populated when the patient is already set for MICROTEST
    And I am logged in
    And I am on my record information page and glossary is visible - GP Medical Record
    When I click the Medical history link on my record - GP Medical Record
    Then I see a message that this information isn't available through the NHS App - GP Medical Record
