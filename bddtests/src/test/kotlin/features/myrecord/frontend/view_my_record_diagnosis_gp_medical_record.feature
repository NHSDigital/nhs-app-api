@my-record
@vision-diagnosis
@pending @NHSO-7509
Feature: View My Diagnosis - Medical Record v2

  Scenario: A VISION user has no access to diagnosis section - GP Medical Record
    Given I am a VISION user setup to use medical record version 2
    And I do not have access to diagnosis
    And I am logged in
    And I am on my record information page and glossary is visible - GP Medical Record
    When I click the Diagnosis link on my record - GP Medical Record
    Then I see a message indicating that I have no access to view this section on My Record - GP Medical Record

  Scenario: An error occurs when trying to retrieve diagnosis data from VISION - GP Medical Record
    Given I am a VISION user setup to use medical record version 2
    And an error occurred retrieving the diagnosis
    And I am logged in
    And I am on my record information page and glossary is visible - GP Medical Record
    When I click the Diagnosis link on my record - GP Medical Record
    Then I see an error occurred message on My Record - GP Medical Record

  Scenario: A VISION user has multiple diagnosis - GP Medical Record
    Given I am a VISION user setup to use medical record version 2
    And the GP Practice has multiple diagnosis
    And I am logged in
    And I am on my record information page and glossary is visible - GP Medical Record
    When I click the Diagnosis link on my record - GP Medical Record
    Then I see the expected diagnosis - GP Medical Record