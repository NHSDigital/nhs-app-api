@my-record
Feature: Allergies Frontend - Medical Record v2

  @smoketest
  Scenario: A VISION user has a drug and non drug allergy record - Medical Record v2
    Given I am a VISION user setup to use medical record version 2
    And the GP Practice has enabled allergies functionality and has a drug and non drug allergy record for VISION
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    And I click the Allergies and adverse reactions link on my record - Medical Record v2
    Then I see a drug and non drug allergy record from VISION - Medical Record v2

  Scenario: An TPP user has no allergies on their record - Medical Record v2
    Given I am a TPP user setup to use medical record version 2
    And the GP Practice has enabled allergies functionality and the patient has "0" allergies
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    And I click the Allergies and adverse reactions link on my record - Medical Record v2
    Then I see a message that I have no information recorded for a specific record - Medical Record v2

  Scenario: A MICROTEST user can view allergies and adverse reactions section when no allergies are returned - Medical Record v2
    Given I am a MICROTEST user setup to use medical record version 2
    And I have 0 Allergies
    And the my record wiremocks are populated
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    And I click the Allergies and adverse reactions link on my record - Medical Record v2
    Then I see a message telling me to contact my GP for information on My Record - Medical Record v2

  Scenario: A VISION user is shown an appropriate error message when an unknown error occurs retrieving their allergies - Medical Record v2
    Given I am a VISION user setup to use medical record version 2
    And there is an unknown error getting allergies for VISION
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    And I click the Allergies and adverse reactions link on my record - Medical Record v2
    Then I see an error occurred message on My Record - Medical Record v2

  Scenario: A MICROTEST user can view allergies and adverse reactions section - Medical Record v2
    Given I am a MICROTEST user setup to use medical record version 2
    And the my record wiremocks are populated
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    And I click the Allergies and adverse reactions link on my record - Medical Record v2
    Then I see the expected allergies displayed - Medical Record v2

  Scenario: An EMIS user has an allergies and adverse reactions result with an unknown date - Medical Record v2
    Given I am a EMIS user setup to use medical record version 2
    And the EMIS GP Practice has two allergies results where the first record has no date
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    And I click the Allergies and adverse reactions link on my record - Medical Record v2
    Then I see the expected allergies displayed with unknown date for the second result - Medical Record v2

  Scenario: An EMIS user receiving a bad allergies response sees an error - Medical Record v2
    Given I am a EMIS user setup to use medical record version 2
    And the GP practice returns a bad allergies response
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    And I click the Allergies and adverse reactions link on my record - Medical Record v2
    Then I see an error occurred message on My Record - Medical Record v2

