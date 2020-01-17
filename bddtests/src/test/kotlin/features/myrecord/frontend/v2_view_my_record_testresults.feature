@my-record
@test-results
Feature: Test Results Frontend - Medical Record v2

  Scenario: A VISION user has no access to test result section - Medical Record v2
    Given I am a VISION user setup to use medical record version 2
    And I do not have access to test results
    And I am on the medical record page
    When I click the Test results link on my record - Medical Record v2
    Then I see a message indicating that I have no access to view this section on My Record - Medical Record v2

  Scenario: A VISION user has no test results - Medical Record v2
    Given I am a VISION user setup to use medical record version 2
    And I have no test results
    And I am on the medical record page
    When I click the Test results link on my record - Medical Record v2
    Then I see a message that I have no information recorded for a specific record - Medical Record v2

  Scenario: A VISION user encounters an error navigating directly to Test Results - Medical Record V2
    Given I am a VISION user setup to use medical record version 2
    And an error occurred retrieving the test results
    And I am on the medical record page
    When I retrieve the 'Gp Medical Record Test Results Detail' page directly
    Then I see a message indicating that I have no access to view this section on My Record - Medical Record v2

  Scenario: An EMIS user has one test result with one value - Medical Record v2
    Given I am a EMIS user setup to use medical record version 2
    And the GP Practice has a single test result with single child values with no ranges for EMIS
    And I am on the medical record page
    When I click the Test results link on my record - Medical Record v2
    Then I see one test result with one value - Medical Record v2

  Scenario: An EMIS user receiving a corrupt response for test results sees an error - Medical Record v2
    Given I am a EMIS user setup to use medical record version 2
    And there is a corrupted test results response returned
    And I am on the medical record page
    When I click the Test results link on my record - Medical Record v2
    Then I see an error occurred message on My Record - Medical Record v2

  Scenario: An EMIS user has one test result with one value and a range - Medical Record v2
    Given I am a EMIS user setup to use medical record version 2
    And the GP Practice has a single test result with single child value with A range for EMIS
    And I am on the medical record page
    When I click the Test results link on my record - Medical Record v2
    Then I see one test result with one value and a range - Medical Record v2

  Scenario: An EMIS user has one test result with multiple child values - Medical Record v2
    Given I am a EMIS user setup to use medical record version 2
    And the GP Practice has a single test result with multiple child values with no ranges for EMIS
    And I am on the medical record page
    When I click the Test results link on my record - Medical Record v2
    Then I see one test result with multiple child values - Medical Record v2

  Scenario: An EMIS user has test results with multiple child values which have ranges - Medical Record v2
    Given I am a EMIS user setup to use medical record version 2
    And the GP Practice has a single test result with multiple child values with ranges for EMIS
    And I am on the medical record page
    When I click the Test results link on my record - Medical Record v2
    Then I see test results with multiple child values some of which have ranges - Medical Record v2

  Scenario: An EMIS user has a test result with an unknown date - Medical Record v2
    Given I am a EMIS user setup to use medical record version 2
    And the EMIS GP Practice has two test results where the second record has no date
    And I am on the medical record page
    When I click the Test results link on my record - Medical Record v2
    Then I see 2 test results - Medical Record v2
    And The second test result record has an unknown date - Medical Record v2

  Scenario: A TPP user has multiple test results - Medical Record v2
    Given I am a TPP user setup to use medical record version 2
    And the GP Practice has six test results
    And I am on the medical record page
    When I click the Test results link on my record - Medical Record v2
    Then I see 6 test results - Medical Record v2

  Scenario: A TPP user will see a error screen when viewing an invalid individual test result - Medical Record v2
    Given I am a TPP user setup to use medical record version 2
    And the GP Practice has six test results
    And an error occurs retrieving the test result detail
    And I am on the medical record page
    When I click the Test results link on my record - Medical Record v2
    And I click a test result - Medical Record v2
    Then I see the appropriate error message for retrieving test result detail

  Scenario: A TPP user can navigate to an individual test result - Medical Record v2
    Given I am a TPP user setup to use medical record version 2
    And the GP Practice has six test results
    And the GP Practice has test result details
    And I am on the medical record page
    When I click the Test results link on my record - Medical Record v2
    And I click a test result - Medical Record v2
    And I see the test result content - Medical Record v2
