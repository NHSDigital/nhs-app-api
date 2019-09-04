@my-record
@test-results
Feature: View My Medical Record Information - Test Results Frontend

  Scenario Outline: A <Service> user has no access to test result section
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality
    And I do not have access to test results
    And I am on my record information page
    When I click the test result section
    Then I see a message indicating that I have no access to view Test results on My Record

    Examples:
      | Service |
      | EMIS    |
      | TPP     |
      | VISION  |

  Scenario Outline: A <Service> user has no test results
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality
    And I have no test results
    And I am on my record information page
    When I click the test result section
    Then I see a message indicating that I have no information recorded for Test results on My Record

    Examples:
      | Service |
      | EMIS    |
      | TPP     |
      | VISION  |

  Scenario Outline: An error occurs when trying to retrieve test result data from <Service>
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality
    And an error occurred retrieving the test results
    And I am on my record information page
    When I click the test result section
    Then I see an error occurred message with Test results on My Record

    Examples:
      | Service |
      | EMIS    |
      | TPP     |
      | VISION  |

  Scenario: An EMIS user has one test result with one value
    Given the my record wiremocks are initialised for EMIS
    And the GP Practice has enabled demographics functionality
    And the GP Practice has a single test result with single child values with no ranges for EMIS
    And I am on my record information page
    When I click the test result section
    Then I see one test result with one value

  Scenario: An EMIS user has one test result with one value and a range
    Given the my record wiremocks are initialised for EMIS
    And the GP Practice has enabled demographics functionality
    And the GP Practice has a single test result with single child value with A range for EMIS
    And I am on my record information page
    When I click the test result section
    Then I see one test result with one value and a range

  Scenario: An EMIS user has one test result with multiple child values
    Given the my record wiremocks are initialised for EMIS
    And the GP Practice has enabled demographics functionality
    And the GP Practice has a single test result with multiple child values with no ranges for EMIS
    And I am on my record information page
    When I click the test result section
    Then I see one test result with multiple child values

  Scenario: An EMIS user has test results with multiple child values which have ranges
    Given the my record wiremocks are initialised for EMIS
    And the GP Practice has enabled demographics functionality
    And the GP Practice has a single test result with multiple child values with ranges for EMIS
    And I am on my record information page
    When I click the test result section
    Then I see test results with multiple child values some of which have ranges

  Scenario: An EMIS user has a test result with an unknown date
    Given the my record wiremocks are initialised for EMIS
    And the GP Practice has enabled demographics functionality
    And the EMIS GP Practice has two test results where the second record has no date
    And I am on my record information page
    When I click the test result section
    And I click a test result
    Then I see 2 test results
    And The second test result record has an unknown date

  Scenario: A TPP user has multiple test results
    Given the my record wiremocks are initialised for TPP
    And the GP Practice has enabled demographics functionality
    And the GP Practice has six test results
    And I am on my record information page
    When I click the test result section
    Then I see 6 test results

  Scenario: An exception occurs retrieving test result detail
    Given the my record wiremocks are initialised for TPP
    And the GP Practice has enabled demographics functionality
    And the GP Practice has six test results
    And an error occurs retrieving the test result detail
    And I am on my record information page
    When I click the test result section
    And I click a test result
    Then I see the appropriate error message for retrieving test result detail
    
  Scenario: A TPP user can navigate to an individual test result
    Given the my record wiremocks are initialised for TPP
    And the GP Practice has enabled demographics functionality
    And the GP Practice has six test results
    And the GP Practice has test result details
    And I am on my record information page
    When I select a test result
    Then I see header text is My GP medical record
    And I see the test result content

  @tech-debt @bug @NHSO-6233
  Scenario: An user navigates back to my record page from test result detail
    Given the my record wiremocks are initialised for TPP
    And the GP Practice has enabled demographics functionality
    And the GP Practice has six test results
    And the GP Practice has test result details
    And I am on my record information page
    When I select a test result
    Then I see header text is My GP medical record
    And I click the test result detail back
    Then I see the my record page scrolled to the test result section

  @tech-debt @NHSO-6382
  Scenario: A TPP user has multiple test results navigation
    Given the my record wiremocks are initialised for TPP
    And the GP Practice has enabled demographics functionality
    And the GP Practice has six test results
    And the test result details are retrieved successfully
    And I am on my record information page
    When I click the test result section
    And I click a test result

  Scenario: A VISION user when navigating directly to test results is shown the My Record Warning Page
    Given the my record wiremocks are initialised for VISION
    And the GP Practice has enabled demographics functionality
    And I am on my record information page
    When I enter url address for test results detail directly into the url
    Then I am redirected to the 'My Record' page
    And I see the my medical record page
    And I see the top of my medical record page

  Scenario: A MICROTEST user can view test results section when no test results are returned
    Given I have 0 INR TestResults and 0 Path TestResults
    And the my record wiremocks are populated for MICROTEST
    And the GP Practice has enabled demographics functionality
    And I am on my record information page
    Then I see the Test results heading on My Record
    When I click the Test results section on My Record
    Then I see a message telling me to contact my GP for Test results information on My Record

  Scenario: A MICROTEST user can view test results section
    Given the my record wiremocks are populated for MICROTEST
    And the GP Practice has enabled demographics functionality
    And I am on my record information page
    Then I see the Test results heading on My Record
    And I do not see a message informing me to contact my GP for this information
    When I click the Test results section on My Record
    Then I see the expected test results displayed

  Scenario: A MICROTEST user has path results filtered from the test results section
    Given I have Path TestResults Filtered out
    And the my record wiremocks are populated for MICROTEST
    And the GP Practice has enabled demographics functionality
    And I am on my record information page
    Then I see the Test results heading on My Record
    And I do not see a message informing me to contact my GP for this information
    When I click the Test results section on My Record
    Then I see the expected test results displayed

  Scenario: A MICROTEST user has test results ordered first by result type and in reverse chronological order
    Given I have 4 INR TestResults and 5 Path TestResults
    And the TestResults have interleaved dates
    And the my record wiremocks are populated for MICROTEST
    And the GP Practice has enabled demographics functionality
    And I am on my record information page
    Then I see the Test results heading on My Record
    And I do not see a message informing me to contact my GP for this information
    When I click the Test results section on My Record
    Then I see the expected test results displayed