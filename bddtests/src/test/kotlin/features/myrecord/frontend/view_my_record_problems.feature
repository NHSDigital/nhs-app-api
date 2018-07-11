Feature: View My Medical Record Information - Problems

  @smoketest
  @NHSO-1095
  Scenario Outline: A user has Problems on their record
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has enabled problems functionality for <Service>
    And I am on my record information page
    And I see heading Problems
    When I click the Problems section
    Then I see Problems records displayed

    Examples:
      |Service|
      |EMIS|

  @smoketest
  @NHSO-1095
  Scenario Outline: A user has no Problems on their record
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And no Problems records exist for the patient for <Service>
    And I am on my record information page
    And I see heading Problems
    When I click the Problems section
    Then I see a message indicating that I have no information recorded for problems

    Examples:
      |Service|
      |EMIS|

  @NHSO-1095
  Scenario Outline: A user does not have access to Problems
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has disabled problems functionality for <Service>
    And I am on my record information page
    And I see heading Problems
    When I click the Problems section
    Then I see a message indicating that I have no access to view problems

    Examples:
      |Service|
      |EMIS|

  @NHSO-1095
  Scenario Outline: An Error occurs retrieving Problems data
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And there is an error retrieving Problems data for <Service>
    And I am on my record information page
    And I see heading Problems
    When I click the Problems section
    Then I see an error occured message with problems

    Examples:
      |Service|
      |EMIS|
