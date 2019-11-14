@my-record
Feature: View My Medical Record Information - Medical History Frontend

  Scenario: A MICROTEST user can view their medical history
    Given the my record wiremocks are populated for MICROTEST
    And the GP Practice has enabled demographics functionality
    And I am on my record information page
    Then I see the Medical history heading on My Record
    When I click the Medical history section on My Record
    Then I see the expected medical histories displayed

  Scenario: A MICROTEST user can view medical history section when no medical history records are returned
    Given I have 0 MedicalHistories
    And the my record wiremocks are populated for MICROTEST
    And the GP Practice has enabled demographics functionality
    And I am on my record information page
    Then I see the Medical history heading on My Record
    When I click the Medical history section on My Record
    Then I see a message telling me to contact my GP for Medical history information on My Record
