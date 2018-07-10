Feature: View My Medical Record Information - Immunisations

  @smoketest
  @NHSO-685
  Scenario Outline: A user has immunisations on their record
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has enabled immunisations functionality and multiple immunisation records exist for <Service>
    And I am on my record information page
    And I see heading Immunisations
    When I click the Immunisations section
    Then I see immunisation records displayed

  Examples:
  |Service|
  |EMIS|

  @NHSO-685
  Scenario Outline: A user has no immunisations on their record
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And no immunisation records exist for the patient for <Service>
    And I am on my record information page
    And I see heading Immunisations
    When I click the Immunisations section
    Then I see message No information recorded for this section

  Examples:
  |Service|
  |EMIS|

  @NHSO-685
  Scenario Outline: A user does not have access to immunisations
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the user does not have access to view immunisations for <Service>
    And I am on my record information page
    And I see heading Immunisations
    When I click the Immunisations section
    Then I see message You do not have access to this section

  Examples:
  |Service|
  |EMIS|

  @NHSO-685
  Scenario Outline: An Error occurs retrieving immunisations data
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And there is an error retrieving immunisations data for <Service>
    And I am on my record information page
    And I see heading Immunisations
    When I click the Immunisations section
    Then I see message An error has occurred trying to retrieve this data

  Examples:
  |Service|
  |EMIS|