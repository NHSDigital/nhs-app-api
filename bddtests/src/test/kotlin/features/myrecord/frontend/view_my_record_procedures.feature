@my-record
@vision-procedures
Feature: View My Medical Record Information - Procedures

  Scenario Outline: A <Service> user can view procedures information
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has multiple procedures
    And I am on my record information page
    When I click the procedures section
    Then I see procedures information

    Examples:
      | Service |
      | VISION  |

  Scenario Outline: A <Service> user has no access to procedures section
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And I do not have access to procedures
    And I am on my record information page
    When I click the procedures section
    Then I see a message indicating that I have no access to view Procedures on My Record

    Examples:
      | Service |
      | VISION  |

  Scenario Outline: An error occurs when trying to retrieve procedures data from <Service>
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And an error occurred retrieving the procedures
    And I am on my record information page
    When I click the procedures section
    Then I see an error occurred message with Procedures on My Record

    Examples:
      | Service |
      | VISION  |