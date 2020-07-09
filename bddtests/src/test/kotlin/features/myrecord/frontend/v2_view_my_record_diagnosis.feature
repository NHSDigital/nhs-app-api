@my-record
@vision-diagnosis
Feature: Diagnosis Frontend - Medical Record v2

  Scenario: A VISION user has no access to diagnosis section - Medical Record v2
    Given I am a VISION user setup to use medical record version 2
    And I do not have access to diagnosis
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Diagnosis link on my record - Medical Record v2
    Then I see a message indicating that I have no access to view this section on My Record - Medical Record v2

  Scenario: An error occurs when trying to retrieve diagnosis data from VISION - Medical Record v2
    Given I am a VISION user setup to use medical record version 2
    And an error occurred retrieving the diagnosis
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Diagnosis link on my record - Medical Record v2
    Then I see an error occurred message on My Record - Medical Record v2

  Scenario: A VISION user has multiple diagnosis - Medical Record v2
    Given I am a VISION user setup to use medical record version 2
    And the GP Practice has multiple diagnosis
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Diagnosis link on my record - Medical Record v2
    Then I see the expected diagnosis - Medical Record v2

  Scenario: A VISION user encounters an error navigating directly to Diagnosis - Medical Record V2
    Given I am a VISION user setup to use medical record version 2
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And an error occurred retrieving the diagnosis
    And I retrieve the 'Gp Medical Record Diagnosis' page directly
    Then I see a message indicating that I have no access to view this section on My Record - Medical Record v2
