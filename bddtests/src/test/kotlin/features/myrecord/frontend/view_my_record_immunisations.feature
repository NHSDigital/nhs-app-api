@my-record
Feature: View My Medical Record Information - Immunisations Frontend

  Scenario Outline: A <Service> user has no immunisations on their record
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality
    And no immunisation records exist for the patient
    And I am on my record information page
    Then I see the Immunisations heading on My Record
    When I click the Immunisations section on My Record
    Then I see a message indicating that I have no information recorded for Immunisations on My Record

  Examples:
  |Service|
  |EMIS   |
  |VISION |

  Scenario: An EMIS user does not have access to immunisations
    Given the my record wiremocks are initialised for EMIS
    And the GP Practice has enabled demographics functionality
    And the user does not have access to view immunisations
    And I am on my record information page
    Then I see the Immunisations heading on My Record
    When I click the Immunisations section on My Record
    Then I see a message indicating that I have no access to view Immunisations on My Record

  Scenario Outline: An Error occurs retrieving immunisations data for <Service>
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality
    And there is an error retrieving immunisations data
    And I am on my record information page
    Then I see the Immunisations heading on My Record
    When I click the Immunisations section on My Record
    Then I see an error occurred message with Immunisations on My Record

  Examples:
  |Service|
  |EMIS|
  |VISION|

  Scenario: A MICROTEST user can view immunisations
    Given the my record wiremocks are populated for MICROTEST
    And the GP Practice has enabled demographics functionality
    And I am on my record information page
    Then I see the Immunisations heading on My Record
    When I click the Immunisations section on My Record
    Then I see the expected immunisations displayed
