@my-record
@test-results
Feature: View My Medical Record Information - Test Results

#  Scenario Outline: A <Service> user can view test result section
#    Given the my record wiremocks are initialised for <Service>
#    And the GP Practice has enabled demographics functionality for <Service>
#    And I am on my record information page
#    Then I see the test result heading for <Service>
#    And I see the test result section collapsed
#
#    Examples:
#      | Service |
#      | EMIS    |
#      | TPP     |
#      | VISION  |
#
#  Scenario Outline: A <Service> user can view test result information
#    Given the my record wiremocks are initialised for <Service>
#    And the GP Practice has enabled demographics functionality for <Service>
#    And the GP Practice has six test results for <Service>
#    And I am on my record information page
#    When I click the test result section
#    Then I see test result information
#
#    Examples:
#      | Service |
#      | EMIS    |
#      | TPP     |
#      | VISION  |
#
#  Scenario Outline: A <Service> user has no access to test result section
#    Given the my record wiremocks are initialised for <Service>
#    And the GP Practice has enabled demographics functionality for <Service>
#    And I do not have access to test results for <Service>
#    And I am on my record information page
#    When I click the test result section
#    Then I see a message indicating that I have no access to view Test results on My Record
#
#    Examples:
#      | Service |
#      | EMIS    |
#      | TPP     |
#      | VISION  |
#
#  Scenario Outline: A <Service> user has no test results
#    Given the my record wiremocks are initialised for <Service>
#    And the GP Practice has enabled demographics functionality for <Service>
#    And I have no test results for <Service>
#    And I am on my record information page
#    When I click the test result section
#    Then I see a message indicating that I have no information recorded for Test results on My Record
#
#    Examples:
#      | Service |
#      | EMIS    |
#      | TPP     |
#      | VISION  |
#
#  Scenario Outline: An error occurs when trying to retrieve test result data from <Service>
#    Given the my record wiremocks are initialised for <Service>
#    And the GP Practice has enabled demographics functionality for <Service>
#    And an error occurred retrieving the test results from <Service>
#    And I am on my record information page
#    When I click the test result section
#    Then I see an error occurred message with Test results on My Record
#
#    Examples:
#      | Service |
#      | EMIS    |
#      | TPP     |
#      | VISION  |
#
#  Scenario Outline: A <Service> user has one test result with one value
#    Given the my record wiremocks are initialised for <Service>
#    And the GP Practice has enabled demographics functionality for <Service>
#    And the GP Practice has a single test result with single child values with no ranges for <Service>
#    And I am on my record information page
#    When I click the test result section
#    Then I see one test result with one value
#
#    Examples:
#      | Service |
#      | EMIS    |
#
#  Scenario Outline: A <Service> user has one test result with one value and a range
#    Given the my record wiremocks are initialised for <Service>
#    And the GP Practice has enabled demographics functionality for <Service>
#    And the GP Practice has a single test result with single child value with A range for <Service>
#    And I am on my record information page
#    When I click the test result section
#    Then I see one test result with one value and a range
#
#    Examples:
#      | Service |
#      | EMIS    |
#
#  Scenario Outline: A <Service> user has one test result with multiple child values
#    Given the my record wiremocks are initialised for <Service>
#    And the GP Practice has enabled demographics functionality for <Service>
#    And the GP Practice has a single test result with multiple child values with no ranges for <Service>
#    And I am on my record information page
#    When I click the test result section
#    Then I see one test result with multiple child values
#
#    Examples:
#      | Service |
#      | EMIS    |
#
#  Scenario Outline: A <Service> user has test results with multiple child values which have ranges
#    Given the my record wiremocks are initialised for <Service>
#    And the GP Practice has enabled demographics functionality for <Service>
#    And the GP Practice has a single test result with multiple child values with ranges for <Service>
#    And I am on my record information page
#    When I click the test result section
#    Then I see test results with multiple child values some of which have ranges
#
#    Examples:
#      | Service |
#      | EMIS    |
#
#  Scenario Outline: A <Service> user has multiple test results
#    Given the my record wiremocks are initialised for <Service>
#    And the GP Practice has enabled demographics functionality for <Service>
#    And the GP Practice has six test results for <Service>
#    And I am on my record information page
#    When I click the test result section
#    Then I see <Count> test results
#
#    Examples:
#      | Service | Count |
#      | TPP     | 6     |
#
#  Scenario Outline: An exception occurs retrieving test result detail
#    Given the my record wiremocks are initialised for <Service>
#    And the GP Practice has enabled demographics functionality for <Service>
#    And the GP Practice has six test results for <Service>
#    And an error occurs retrieving the test result detail
#    And I am on my record information page
#    When I click the test result section
#    And I click a test result
#    Then I see the appropriate error message for retrieving test result detail
#
#    Examples:
#      | Service |
#      | TPP     |

  Scenario Outline: An user navigates back to my record page from test result detail
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has six test results for <Service>
    And the GP Practice has test result details
    And I am on my record information page
    When I select a test result
    And I click the test result detail back
    Then I see the my record page scrolled to the test result section

    Examples:
      | Service |
      | TPP     |

#  @android
#  @native-smoketest
#  Scenario Outline: A <Service> user has multiple test results navigation
#    Given the my record wiremocks are initialised for <Service>
#    And the GP Practice has enabled demographics functionality for <Service>
#    And the GP Practice has six test results for <Service>
#    And the test result details are retrieved successfully
#    And I am on my record information page
#    When I click the test result section
#    And I click a test result
#
#    Examples:
#      | Service |
#      | TPP     |
#
#  Scenario: A VISION user navigates directly to the test results details page
#    Given the my record wiremocks are initialised for VISION
#    And the GP Practice has enabled demographics functionality for VISION
#    And the GP Practice has test result details
#    And I am on my record information page
#    When I enter url address for test results detail directly into the url
#    Then I am redirected to the my record page
#    And I see the my record warning page
