@my-record
Feature: View My Medical Record Information - Consultations Frontend

  Scenario Outline: An <Service> user has no Consultations on their record
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality
    And the GP Practice has no consultations
    And I am on my record information page
    Then I see the Consultations heading on My Record
    When I click the Consultations section on My Record
    Then I see a message indicating that I have no information recorded for Consultations on My Record

    Examples:
      |Service|
      |EMIS|
      |TPP|

  Scenario: An EMIS user does not have access to Consultations
    Given the my record wiremocks are initialised for EMIS
    And the GP Practice has enabled demographics functionality
    And the Patient has no access to consultations
    And I am on my record information page
    Then I see the Consultations heading on My Record
    When I click the Consultations section on My Record
    Then I see a message indicating that I have no access to view Consultations on My Record

  Scenario Outline: An Error occurs retrieving Consultations data <Service>
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality
    And an error occurred retrieving the consultations
    And I am on my record information page
    Then I see the Consultations heading on My Record
    When I click the Consultations section on My Record
    Then I see an error occurred message with Consultations on My Record

    Examples:
      |Service|
      |EMIS|
      |TPP|
