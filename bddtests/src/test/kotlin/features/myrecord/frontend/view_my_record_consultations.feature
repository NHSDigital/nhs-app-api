Feature: View My Medical Record Information

  @smoketest
  @NHSO-1096
  Scenario Outline: An EMIS user has Consultations on their record
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has multiple consultations for <Service>
    And I am on my record information page
    And I see heading Consultations
    When I click the Consultations section
    Then I see Consultations records displayed

    Examples:
      |Service|
      |EMIS|

  @smoketest
  @NHSO-1096
  Scenario Outline: An EMIS user has no Consultations on their record
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has no consultations for <Service>
    And I am on my record information page
    And I see heading Consultations
    When I click the Consultations section
    Then I see a message indicating that I have no information recorded for Consultations

    Examples:
      |Service|
      |EMIS|

  @NHSO-1096
  Scenario Outline: An EMIS user does not have access to Consultations
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the Patient has no access to consultations for <Service>
    And I am on my record information page
    And I see heading Consultations
    When I click the Consultations section
    Then I see a message indicating that I have no access to view Consultations

    Examples:
      |Service|
      |EMIS|

  @NHSO-1096
  Scenario Outline: An Error occurs retrieving Consultations data
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And an error occurred retrieving the consultations for <Service>
    And I am on my record information page
    And I see heading Consultations
    When I click the Consultations section
    Then I see an error occured message with Consultations

    Examples:
      |Service|
      |EMIS|
