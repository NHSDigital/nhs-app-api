@my-record
Feature: Allergies Frontend - Medical Record v1

  Scenario: A VISION user has a drug and non drug allergy record - Medical Record v1
    Given I am a VISION user setup to use medical record version 1
    And the GP Practice has enabled allergies functionality and has a drug and non drug allergy record for VISION
    And I am on the medical record page
    When I click the Allergies and adverse reactions section on My Record - Medical Record v1
    Then I see a drug and non drug allergy record from VISION - Medical Record v1

  Scenario: A TPP user without Summary Care Record access cannot view allergies section - Medical Record v1
    Given I am a TPP user setup to use medical record version 1
    But the GP Practice has disabled allergies functionality
    And I am on the medical record page
    Then I do not see the Allergies and adverse reactions heading on My Record - Medical Record v1
    But I see the Test results heading on My Record - Medical Record v1

  Scenario: An EMIS user has no allergies on their record - Medical Record v1
    Given I am a EMIS user setup to use medical record version 1
    And the GP Practice has enabled allergies functionality and the patient has "0" allergies
    And I am on the medical record page
    When I click the Allergies and adverse reactions section on My Record - Medical Record v1
    Then I see a message indicating that I have no information recorded for Allergies and adverse reactions on My Record - Medical Record v1

  Scenario: A MICROTEST user can view allergies and adverse reactions section when no allergies are returned - Medical Record v1
    Given I am a MICROTEST user setup to use medical record version 1
    Given I have 0 Allergies
    And the my record wiremocks are populated
    And I am on the medical record page
    Then I see the Allergies and adverse reactions heading on My Record - Medical Record v1
    When I click the Allergies and adverse reactions section on My Record - Medical Record v1
    Then I see a message telling me to contact my GP for Allergies and adverse reactions information on My Record - Medical Record v1
    
  Scenario: An EMIS user receiving a bad allergies response sees an error
    Given I am a EMIS user setup to use medical record version 1
    And the GP practice returns a bad allergies response
    And I am on the medical record page
    When I click the Allergies and adverse reactions section on My Record - Medical Record v1
    Then I see an error occurred message with Allergies and adverse reactions on My Record - Medical Record v1

  Scenario: An TPP user receiving a bad allergies response sees an error
    Given I am a TPP user setup to use medical record version 1
    And the GP practice returns a bad allergies response
    And I am on the medical record page
    When I click the Allergies and adverse reactions section on My Record - Medical Record v1
    Then I see an error occurred message with Allergies and adverse reactions on My Record - Medical Record v1

  Scenario: An EMIS user has multiple allergies with different date display formats - Medical Record v1
    Given I am a EMIS user setup to use medical record version 1
    And the GP Practice has enabled allergies functionality and has 5 different allergies with different date formats
    And I am on the medical record page
    When I click the Allergies and adverse reactions section on My Record - Medical Record v1
    And I see 5 allergies with different date formats - Medical Record v1

  Scenario: A VISION user is shown an appropriate error message when an unknown error occurs retrieving their allergies - Medical Record v1
    Given I am a VISION user setup to use medical record version 1
    And there is an unknown error getting allergies for VISION
    And I am on the medical record page
    When I click the Allergies and adverse reactions section on My Record - Medical Record v1
    Then I see an error occurred message with Allergies and adverse reactions on My Record - Medical Record v1

  Scenario: A MICROTEST user can view allergies and adverse reactions section - Medical Record v1
    Given I am a MICROTEST user setup to use medical record version 1
    And the my record wiremocks are populated
    And I am on the medical record page
    Then I do not see a message informing me to contact my GP for this information - Medical Record v1
    When I click the Allergies and adverse reactions section on My Record - Medical Record v1
    Then I see the expected allergies displayed - Medical Record v1

  Scenario: An EMIS user has an allergies and adverse reactions result with an unknown date - Medical Record v1
    Given I am a EMIS user setup to use medical record version 1
    And the EMIS GP Practice has two allergies results where the first record has no date
    And I am on the medical record page
    When I click the Allergies and adverse reactions section on My Record - Medical Record v1
    Then I see the expected allergies displayed with unknown date for the first result - Medical Record v1
