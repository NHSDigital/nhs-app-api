@my-record
@vision-procedures
Feature: View My Medical Record Information - Procedures Frontend

  Scenario: A VISION user has no access to procedures section
    Given the my record wiremocks are initialised for VISION
    And the GP Practice has enabled demographics functionality
    And I do not have access to procedures
    And I am on my record information page
    When I click the procedures section
    Then I see a message indicating that I have no access to view Procedures on My Record

  Scenario: An error occurs when trying to retrieve procedures data from VISION
    Given the my record wiremocks are initialised for VISION
    And the GP Practice has enabled demographics functionality
    And an error occurred retrieving the procedures
    And I am on my record information page
    When I click the procedures section
    Then I see an error occurred message with Procedures on My Record
