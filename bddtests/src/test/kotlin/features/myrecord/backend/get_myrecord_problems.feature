@my-record
@backend
Feature: Get patient problems data
  A user can get their patient problems information

  Scenario Outline: GP practice has enabled problems functionality
    Given the my record wiremocks are initialised for <Service>
    And I have logged into <Service> and have a valid session cookie
    And the GP Practice has enabled problems functionality for <Service>
    When I get the users my record data
    Then I receive "3" problems as part of the my record object
    And the flag informing that the patient has access to the problem data is set to "True"
    And the flag informing that there was an error retrieving the problem data is set to "False"
    And the field indicating supplier is set to <Service>

    Examples:
      |Service|
      |EMIS|
      |VISION|

  Scenario Outline: GP practice has disabled problems functionality
    Given the my record wiremocks are initialised for <Service>
    And I have logged into <Service> and have a valid session cookie
    And the GP Practice has disabled problems functionality for <Service>
    When I get the users my record data
    Then I receive "0" problems as part of the my record object
    And the flag informing that the patient has access to the problem data is set to "False"
    And the flag informing that there was an error retrieving the problem data is set to "False"
    And the field indicating supplier is set to <Service>

    Examples:
      |Service|
      |EMIS|
      |VISION|
