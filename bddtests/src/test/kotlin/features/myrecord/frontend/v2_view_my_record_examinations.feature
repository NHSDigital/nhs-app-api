@my-record
@vision-examinations
Feature: Examinations Frontend - Medical Record v2

  Scenario: A VISION user has no access to examinations section - Medical Record v2
    Given I am a VISION user setup to use medical record version 2
    And I do not have access to examinations
    And I am on the medical record page
    When I click the Examinations link on my record - Medical Record v2
    Then I see a message indicating that I have no access to view this section on My Record - Medical Record v2

  Scenario: An error occurs when trying to retrieve examinations data from VISION - Medical Record v2
    Given I am a VISION user setup to use medical record version 2
    And an error occurred retrieving the examinations
    And I am on the medical record page
    When I click the Examinations link on my record - Medical Record v2
    Then I see an error occurred message on My Record - Medical Record v2

  Scenario: A VISION user has multiple examinations - Medical Record v2
    Given I am a VISION user setup to use medical record version 2
    And the GP Practice has multiple examinations
    And I am on the medical record page
    When I click the Examinations link on my record - Medical Record v2
    Then I see the expected examinations - Medical Record v2

  Scenario: A VISION user encounters an error navigating directly to Examinations - Medical Record V2
    Given I am a VISION user setup to use medical record version 2
    And an error occurred retrieving the examinations
    And I am on the medical record page
    When I retrieve the 'Gp Medical Record Examinations' page directly
    Then I see a message indicating that I have no access to view this section on My Record - Medical Record v2
