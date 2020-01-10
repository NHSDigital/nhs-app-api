@my-record
@test-results
Feature: Test Results Frontend - Medical Record v1

  Scenario: An EMIS user has test results with multiple child values which have ranges - Medical Record v1
    Given I am a EMIS user setup to use medical record version 1
    And the GP Practice has a single test result with multiple child values with ranges for EMIS
    And I am on the medical record page
    When I click the Test results section on My Record - Medical Record v1
    Then I see test results with multiple child values some of which have ranges - Medical Record v1

  Scenario: An EMIS user has a test result with an unknown date - Medical Record v1
    Given I am a EMIS user setup to use medical record version 1
    And the EMIS GP Practice has two test results where the second record has no date
    And I am on the medical record page
    When I click the Test results section on My Record - Medical Record v1
    And I click a test result - Medical Record v1
    Then I see 2 test results - Medical Record v1
    And The second test result record has an unknown date - Medical Record v1

  Scenario: A TPP user has multiple test results - Medical Record v1
    Given I am a TPP user setup to use medical record version 1
    And the GP Practice has six test results
    And I am on the medical record page
    When I click the Test results section on My Record - Medical Record v1
    Then I see 6 test results - Medical Record v1

  Scenario: An exception occurs retrieving test result detail - Medical Record v1
    Given I am a TPP user setup to use medical record version 1
    And the GP Practice has six test results
    And an error occurs retrieving the test result detail
    And I am on the medical record page
    When I click the Test results section on My Record - Medical Record v1
    And I click a test result - Medical Record v1
    Then I see the appropriate error message for retrieving test result detail

  Scenario Outline: An exception occurs retrieving test results - Medical Record v1
    Given I am a <GP System> user setup to use medical record version 1
    And the GP Practice sends a bad test results response
    And I am on the medical record page
    When I click the Test results section on My Record - Medical Record v1
    Then I see an error occurred message with Test results on My Record - Medical Record v1
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario: A TPP user can navigate to an individual test result - Medical Record v1
    Given I am a TPP user setup to use medical record version 1
    And the GP Practice has six test results
    And the GP Practice has test result details
    And I am on the medical record page
    When I select a test result - Medical Record v1
    Then I see header text is Your GP medical record
    And I see the test result content - Medical Record v1

  Scenario: A TPP user can navigate to a test result containing HTML entities - Medical Record v1
    Given I am a TPP user setup to use medical record version 1
    And the GP Practice has six test results
    And the GP Practice has test result details with HTML entities
    And I am on the medical record page
    When I select a test result - Medical Record v1
    Then I see header text is Your GP medical record
    And there are no wrongly displayed HTML entities - Medical Record v1

  Scenario: A MICROTEST user has test results ordered first by result type and in reverse chronological order - Medical Record v1
    Given I am a MICROTEST user setup to use medical record version 1
    And I have 4 INR TestResults and 5 Path TestResults
    And the TestResults have interleaved dates
    And the my record wiremocks are populated
    And I am on the medical record page
    Then I see the Test results heading on My Record - Medical Record v1
    And I do not see a message informing me to contact my GP for this information - Medical Record v1
    When I click the Test results section on My Record - Medical Record v1
    Then I see the expected test results displayed - Medical Record v1
