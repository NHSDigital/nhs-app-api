@my-record
@vision-diagnosis
Feature: Diagnosis Frontend - Medical Record v2

  Scenario: A VISION user has no access to diagnosis section - Medical Record v2
    Given I am a VISION user setup to use medical record version 2
    And I do not have access to diagnosis
    And I am on the medical record page
    When I click the Diagnosis link on my record - Medical Record v2
    Then I see a message indicating that I have no access to view this section on My Record - Medical Record v2

  Scenario: An error occurs when trying to retrieve diagnosis data from VISION - Medical Record v2
    Given I am a VISION user setup to use medical record version 2
    And an error occurred retrieving the diagnosis
    And I am on the medical record page
    When I click the Diagnosis link on my record - Medical Record v2
    Then I see an error occurred message on My Record - Medical Record v2

  Scenario: A VISION user has multiple diagnosis - Medical Record v2
    Given I am a VISION user setup to use medical record version 2
    And the GP Practice has multiple diagnosis
    And I am on the medical record page
    When I click the Diagnosis link on my record - Medical Record v2
    Then I see the expected diagnosis - Medical Record v2

  Scenario: A VISION user encounters an error navigating directly to Diagnosis - Medical Record V2
    Given I am a VISION user setup to use medical record version 2
    And I am on the medical record page
    And an error occurred retrieving the diagnosis
    When I retrieve the 'Gp Medical Record Diagnosis' page directly
    Then I see a message indicating that I have no access to view this section on My Record - Medical Record v2