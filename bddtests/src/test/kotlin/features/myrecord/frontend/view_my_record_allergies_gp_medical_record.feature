@my-record
Feature: View My Allergies Information - Medical Record v2

  Scenario: A VISION user has a drug and non drug allergy record - GP Medical Record
    Given I am a VISION user setup to use medical record version 2
    And the GP Practice has enabled allergies functionality and has a drug and non drug allergy record for VISION - GP Medical Record
    And I am logged in
    And I am on my record information page and glossary is visible - GP Medical Record
    When I click the Allergies and adverse reactions link on my record - GP Medical Record
    Then I see a drug and non drug allergy record from VISION - GP Medical Record

  Scenario: An TPP user has no allergies on their record - GP Medical Record
    Given I am a TPP user setup to use medical record version 2
    And the GP Practice has enabled allergies functionality and the patient has "0" allergies - GP Medical Record
    And I am logged in
    And I am on my record information page and glossary is visible - GP Medical Record
    When I click the Allergies and adverse reactions link on my record - GP Medical Record
    Then I see a message that I have no information recorded for a specific record - GP Medical Record

  Scenario: A MICROTEST user can view allergies and adverse reactions section when no allergies are returned - GP Medical Record
    Given I am a MICROTEST user setup to use medical record version 2
    And I have 0 Allergies - GP Medical Record
    And I have my medical record available to view for MICROTEST - GP Medical Record
    And I am logged in
    And I am on my record information page - GP Medical Record
    When I click the Allergies and adverse reactions link on my record - GP Medical Record
    Then I see a message telling me to contact my GP for information on My Record - GP Medical Record

  Scenario: A VISION user is shown an appropriate error message when an unknown error occurs retrieving their allergies - GP Medical Record
    Given I am a VISION user setup to use medical record version 2
    And there is an unknown error getting allergies for VISION - GP Medical Record
    And I am logged in
    And I am on my record information page and glossary is visible - GP Medical Record
    When I click the Allergies and adverse reactions link on my record - GP Medical Record
    Then I see an error occurred message on My Record - GP Medical Record

  Scenario: A MICROTEST user can view allergies and adverse reactions section - GP Medical Record
    Given I am a MICROTEST user setup to use medical record version 2
    And I have my medical record available to view for MICROTEST - GP Medical Record
    And I am logged in
    And I am on my record information page and glossary is visible - GP Medical Record
    And I do not see a message informing me to contact my GP for this information - GP Medical Record
    When I click the Allergies and adverse reactions link on my record - GP Medical Record
    Then I see the expected allergies displayed - GP Medical Record

  Scenario: An EMIS user has an allergies and adverse reactions result with an unknown date - GP Medical Record
    Given I am a EMIS user setup to use medical record version 2
    And I am logged in
    And the EMIS GP Practice has two allergies results where the first record has no date - GP Medical Record
    And I am on my record information page and glossary is visible - GP Medical Record
    When I click the Allergies and adverse reactions link on my record - GP Medical Record
    Then I see the expected allergies displayed with unknown date for the first result - GP Medical Record

