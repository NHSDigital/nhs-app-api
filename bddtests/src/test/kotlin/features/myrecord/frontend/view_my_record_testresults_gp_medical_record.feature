@my-record
@test-results

Feature: View My GP Medical Record Information - Test Results Frontend GP Medical Record

  Scenario: A VISION user has no access to test result section
    Given I am a VISION user setup to use medical record version 2
    And I do not have access to test results - GP Medical Record
    And I am logged in
    And I am on my record information page - GP Medical Record
    When I click the test result link on my record - GP Medical Record
    Then I see a message indicating that I have no access to view this section on My Record - GP Medical Record

  Scenario: A VISION user has no test results
    Given I am a VISION user setup to use medical record version 2
    And I have no test results - GP Medical Record
    And I am logged in
    And I am on my record information page - GP Medical Record
    When I click the test result link on my record - GP Medical Record
    Then I see a message that I have no information recorded for a specific record - GP Medical Record

  Scenario: An EMIS user has one test result with one value
    Given I am a EMIS user setup to use medical record version 2
    And the GP Practice has a single test result with single child values with no ranges for EMIS - GP Medical Record
    And I am logged in
    And I am on my record information page - GP Medical Record
    When I click the test result link on my record - GP Medical Record
    Then I see one test result with one value - GP Medical Record

  Scenario: An EMIS user has one test result with one value and a range
    Given I am a EMIS user setup to use medical record version 2
    And the GP Practice has a single test result with single child value with A range for EMIS - GP Medical Record
    And I am logged in
    And I am on my record information page - GP Medical Record
    When I click the test result link on my record - GP Medical Record
    Then I see one test result with one value and a range - GP Medical Record

  Scenario: An EMIS user has one test result with multiple child values
    Given I am a EMIS user setup to use medical record version 2
    And the GP Practice has a single test result with multiple child values with no ranges for EMIS - GP Medical Record
    And I am logged in
    And I am on my record information page - GP Medical Record
    When I click the test result link on my record - GP Medical Record
    Then I see one test result with multiple child values - GP Medical Record

  Scenario: An EMIS user has test results with multiple child values which have ranges
    Given I am a EMIS user setup to use medical record version 2
    And the GP Practice has a single test result with multiple child values with ranges for EMIS - GP Medical Record
    And I am logged in
    And I am on my record information page - GP Medical Record
    When I click the test result link on my record - GP Medical Record
    Then I see test results with multiple child values some of which have ranges - GP Medical Record

  Scenario: An EMIS user has a test result with an unknown date
    Given I am a EMIS user setup to use medical record version 2
    And the EMIS GP Practice has two test results where the second record has no date - GP Medical Record
    And I am logged in
    And I am on my record information page - GP Medical Record
    When I click the test result link on my record - GP Medical Record
    Then I see 2 test results - GP Medical Record
    And The second test result record has an unknown date - GP Medical Record

  Scenario: A TPP user has multiple test results
    Given I am a TPP user setup to use medical record version 2
    And the GP Practice has multiple test results - GP Medical Record
    And I am logged in
    And I am on my record information page - GP Medical Record
    When I click the test result link on my record - GP Medical Record
    Then I see 6 test results - GP Medical Record

  Scenario: A TPP user will see a error screen when viewing an invalid individual test result
    Given I am a TPP user setup to use medical record version 2
    And the GP Practice has multiple test results - GP Medical Record
    And an error occurs retrieving the test result detail - GP Medical Record
    And I am logged in
    And I am on my record information page - GP Medical Record
    When I click the test result link on my record - GP Medical Record
    And I click a test result - GP Medical Record
    Then I see the appropriate error message for retrieving test result detail

  Scenario: A TPP user can navigate to an individual test result
    Given I am a TPP user setup to use medical record version 2
    And the GP Practice has multiple test results - GP Medical Record
    And the GP Practice has test result details - GP Medical Record
    And I am logged in
    And I am on my record information page - GP Medical Record
    When I click the test result link on my record - GP Medical Record
    And I click a test result - GP Medical Record
    And I see the test result content - GP Medical Record
