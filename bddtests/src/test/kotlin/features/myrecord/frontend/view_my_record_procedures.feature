@my-record
Feature: View My Medical Record Information - Test Results

  Scenario Outline: A <Service> user can view test result section
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And I am on my record information page
    Then I see the procedures heading
    And I see the procedures section collapsed

    Examples:
      | Service |
      | VISION  |

  Scenario Outline: A <Service> user can view test result information
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has multiple procedures
    And I am on my record information page
    When I click the procedures section
    Then I see procedures information

    Examples:
      | Service |
      | VISION  |

  Scenario Outline: A <Service> user has no access to test result section
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And I do not have access to procedures
    And I am on my record information page
    When I click the procedures section
    Then I see a message indicating that I have no access to view Procedures on My Record

    Examples:
      | Service |
      | VISION  |

  Scenario Outline: A <Service> user has no test results
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And I have no procedures
    And I am on my record information page
    When I click the procedures section
    Then I see a message indicating that I have no information recorded for Procedures on My Record

    Examples:
      | Service |
      | VISION  |

  Scenario Outline: An error occurs when trying to retrieve test result data from <Service>
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And an error occurred retrieving the procedures
    And I am on my record information page
    When I click the procedures section
    Then I see an error occurred message with Procedures on My Record

    Examples:
      | Service |
      | VISION  |
