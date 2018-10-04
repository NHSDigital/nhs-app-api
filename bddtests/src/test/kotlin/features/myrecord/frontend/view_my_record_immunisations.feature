@my-record
Feature: View My Medical Record Information - Immunisations

 Scenario Outline: A <Service> user has immunisations on their record
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has enabled immunisations functionality and multiple immunisation records exist for <Service>
    And I am on my record information page
    Then I see the Immunisations heading on My Record
    When I click the Immunisations section on My Record
    Then I see immunisation records displayed

  Examples:
  |Service|
  |EMIS|

  Scenario Outline: A <Service> user has no immunisations on their record
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And no immunisation records exist for the patient for <Service>
    And I am on my record information page
    Then I see the Immunisations heading on My Record
    When I click the Immunisations section on My Record
    Then I see a message indicating that I have no information recorded for Immunisations on My Record

  Examples:
  |Service|
  |EMIS|

  Scenario Outline: A <Service> user does not have access to immunisations
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the user does not have access to view immunisations for <Service>
    And I am on my record information page
    Then I see the Immunisations heading on My Record
    When I click the Immunisations section on My Record
    Then I see a message indicating that I have no access to view Immunisations on My Record

  Examples:
  |Service|
  |EMIS|

  Scenario Outline: An Error occurs retrieving immunisations data for <Service>
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And there is an error retrieving immunisations data for <Service>
    And I am on my record information page
    Then I see the Immunisations heading on My Record
    When I click the Immunisations section on My Record
    Then I see an error occurred message with Immunisations on My Record

  Examples:
  |Service|
  |EMIS|