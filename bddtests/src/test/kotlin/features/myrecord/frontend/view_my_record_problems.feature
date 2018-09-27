@my-record
Feature: View My Medical Record Information - Problems

  @NHSO-1095
  Scenario Outline: A <Service> user has Problems on their record
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has enabled problems functionality for <Service>
    And I am on my record information page
    Then I see the Problems heading on My Record
    When I click the Problems section on My Record
    Then I see Problems records displayed

    Examples:
      |Service|
      |EMIS|


  @NHSO-1095
  Scenario Outline: A <Service> user has no Problems on their record
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And no Problems records exist for the patient for <Service>
    And I am on my record information page
    Then I see the Problems heading on My Record
    When I click the Problems section on My Record
    Then I see a message indicating that I have no information recorded for Problems on My Record

    Examples:
      |Service|
      |EMIS|

  @NHSO-1095
  Scenario Outline: A <Service> user does not have access to Problems
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has disabled problems functionality for <Service>
    And I am on my record information page
    Then I see the Problems heading on My Record
    When I click the Problems section on My Record
    Then I see a message indicating that I have no access to view Problems on My Record

    Examples:
      |Service|
      |EMIS|

  @NHSO-1095
  Scenario Outline: An Error occurs retrieving Problems data for <Service>
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And there is an error retrieving Problems data for <Service>
    And I am on my record information page
    Then I see the Problems heading on My Record
    When I click the Problems section on My Record
    Then I see an error occurred message with Problems on My Record

    Examples:
      |Service|
      |EMIS|
