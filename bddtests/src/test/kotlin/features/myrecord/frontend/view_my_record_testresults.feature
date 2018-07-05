Feature: View My Medical Record Information

  @NHSO-686
  Scenario Outline: An EMIS user can view test result section
    Given the my record wiremocks are initialised for <Service>
    And I am logged in
    And the GP Practice has enabled demographics functionality for <Service>
    And I am on my record information page
    Then I see the test result heading
    And I see the test result section collapsed

    Examples:
      |Service|
      |EMIS|

  @smoketest
  @NHSO-686
  Scenario Outline: An EMIS user can view test result information
    Given the my record wiremocks are initialised for <Service>
    And I am logged in
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has multiple test results for <Service>
    And I am on my record information page
    When I click the test result section
    Then I see test result information

    Examples:
      |Service|
      |EMIS|

  @NHSO-686
  Scenario Outline: An EMIS user has no access to test result section
    Given the my record wiremocks are initialised for <Service>
    And I am logged in
    And the GP Practice has enabled demographics functionality for <Service>
    And I do not have access to test results for <Service>
    And I am on my record information page
    When I click the test result section
    Then I see a message indicating that I have no access to view test result

    Examples:
      |Service|
      |EMIS|

  @NHSO-686
  Scenario Outline: An EMIS user has no test results
    Given the my record wiremocks are initialised for <Service>
    And I am logged in
    And the GP Practice has enabled demographics functionality for <Service>
    And I have no test results for <Service>
    And I am on my record information page
    When I click the test result section
    Then I see a message indicating that I have no information recorded for this section

    Examples:
      |Service|
      |EMIS|

  @NHSO-686
  Scenario Outline: An error occurs when trying to retrieve test result data from EMIS
    Given the my record wiremocks are initialised for <Service>
    And I am logged in
    And the GP Practice has enabled demographics functionality for <Service>
    And an error occurred retrieving the test results from <Service>
    And I am on my record information page
    When I click the test result section
    Then I see an error occured message

    Examples:
      |Service|
      |EMIS|

  @NHSO-686
  Scenario Outline: An EMIS user has one test result with one value
    Given the my record wiremocks are initialised for <Service>
    And I am logged in
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has a single test result with single child values with no ranges for <Service>
    And I am on my record information page
    When I click the test result section
    Then I see one test result with one value

    Examples:
      |Service|
      |EMIS|

  @NHSO-686
  Scenario Outline: An EMIS user has one test result with one value and a range
    Given the my record wiremocks are initialised for <Service>
    And I am logged in
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has a single test result with single child value with A range for <Service>
    And I am on my record information page
    When I click the test result section
    Then I see one test result with one value and a range

    Examples:
      |Service|
      |EMIS|

  @NHSO-686
  Scenario Outline: An EMIS user has one test result with multiple child values
    Given the my record wiremocks are initialised for <Service>
    And I am logged in
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has a single test result with multiple child values with no ranges for <Service>
    And I am on my record information page
    When I click the test result section
    Then I see one test result with multiple child values

    Examples:
      |Service|
      |EMIS|

  @NHSO-686
  Scenario Outline: An EMIS user has test results with multiple child values which have ranges
    Given the my record wiremocks are initialised for <Service>
    And I am logged in
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has a single test result with multiple child values with ranges for <Service>
    And I am on my record information page
    When I click the test result section
    Then I see test results with multiple child values some of which have ranges

    Examples:
    |Service|
    |EMIS|