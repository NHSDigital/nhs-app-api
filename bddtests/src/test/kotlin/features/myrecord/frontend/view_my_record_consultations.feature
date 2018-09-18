@my-record
Feature: View My Medical Record Information

  @NHSO-1096
  Scenario Outline: An <Service> user has Consultations on their record
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has multiple consultations for <Service>
    And I am on my record information page
    And I see heading Consultations <Service>
    When I click the Consultations section <Service>
    Then I see Consultations records displayed <Service>

    Examples:
      |Service|
      |EMIS|
      |TPP|

  @NHSO-1096
  Scenario Outline: An <Service> user has no Consultations on their record
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has no consultations for <Service>
    And I am on my record information page
    And I see heading Consultations <Service>
    When I click the Consultations section <Service>
    Then I see a message indicating that I have no information recorded for Consultations <Service>

    Examples:
      |Service|
      |EMIS|
      |TPP|

  @NHSO-1096
  Scenario Outline: An <Service> user does not have access to Consultations
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the Patient has no access to consultations for <Service>
    And I am on my record information page
    And I see heading Consultations <Service>
    When I click the Consultations section <Service>
    Then I see a message indicating that I have no access to view Consultations <Service>

    Examples:
      |Service|
      |EMIS|



  @NHSO-1096
  Scenario Outline: An Error occurs retrieving Consultations data <Service>
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And an error occurred retrieving the consultations for <Service>
    And I am on my record information page
    And I see heading Consultations <Service>
    When I click the Consultations section <Service>
    Then I see an error occurred message with Consultations <Service>

    Examples:
      |Service|
      |EMIS|
      |TPP|
