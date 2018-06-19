Feature: View My Medical Record Information

  Background:
    Given wiremock is initialised

  @NHSO-686
  Scenario: An EMIS user can view test result section
    Given I am logged in
    And the GP Practice has enabled demographics functionality
    And I am on my record information page
    Then I see the test result heading
    And I see the test result section collapsed

  @NHSO-686
  Scenario: An EMIS user can view test result information
    Given I am logged in
    And the GP Practice has enabled demographics functionality
    And the GP Practice has multiple test results
    And I am on my record information page
    When I click the test result section
    Then I see test result information

  @NHSO-686
  Scenario: An EMIS user has no access to test result section
    Given I am logged in
    And the GP Practice has enabled demographics functionality
    And I do not have access to test results
    And I am on my record information page
    When I click the test result section
    Then I see a message indicating that I have no access to view test result

  @NHSO-686
  Scenario: An EMIS user has no test results
    Given I am logged in
    And the GP Practice has enabled demographics functionality
    And I have no test results
    And I am on my record information page
    When I click the test result section
    Then I see a message indicating that I have no information recorded for this section

  @NHSO-686
  Scenario: An error occurs when trying to retrieve test result data from EMIS
    Given I am logged in
    And the GP Practice has enabled demographics functionality
    And an error occurred retrieving the test results from EMIS
    And I am on my record information page
    When I click the test result section
    Then I see an error occured message

  @NHSO-686
  Scenario: An EMIS user has one test result with one value
    Given I am logged in
    And the GP Practice has enabled demographics functionality
    And the GP Practice has a single test result with single child values with no ranges
    And I am on my record information page
    When I click the test result section
    Then I see one test result with one value

  @NHSO-686
  Scenario: An EMIS user has one test result with one value and a range
    Given I am logged in
    And the GP Practice has enabled demographics functionality
    And the GP Practice has a single test result with single child value with A range
    And I am on my record information page
    When I click the test result section
    Then I see one test result with one value and a range

  @NHSO-686
  Scenario: An EMIS user has one test result with multiple child values
    Given I am logged in
    And the GP Practice has enabled demographics functionality
    And the GP Practice has a single test result with multiple child values with no ranges
    And I am on my record information page
    When I click the test result section
    Then I see one test result with multiple child values

  @NHSO-686
  Scenario: An EMIS user has test results with multiple child values which have ranges
    Given I am logged in
    And the GP Practice has enabled demographics functionality
    And the GP Practice has a single test result with multiple child values with ranges
    And I am on my record information page
    When I click the test result section
    Then I see test results with multiple child values some of which have ranges