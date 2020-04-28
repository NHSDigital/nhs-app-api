@my-record
Feature: Medical History Frontend - Medical Record V2

  Scenario: A MICROTEST user can view medical history - Medical Record v2
    Given I am a MICROTEST user setup to use medical record version 2
    And the my record wiremocks are populated
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    When I click the Medical history link on my record - Medical Record v2
    Then I see the expected medical history - Medical Record v2

  Scenario: A MICROTEST user has no medical history on their record - Medical Record v2
    Given I am a MICROTEST user setup to use medical record version 2
    And I have 0 MedicalHistories
    And the my record wiremocks are populated
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    When I click the Medical history link on my record - Medical Record v2
    Then I see a message that this information isn't available through the NHS App - Medical Record v2
