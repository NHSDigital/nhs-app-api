@my-record
@backend
Feature: Get patient problems data Backend
  A user can get their patient problems information

  Scenario Outline: A <GP System> user can get problems data when GP practice has enabled problems functionality
    Given I am a <GP System> user setup to use medical record version 2
    And I have logged in and have a valid session cookie
    And the GP Practice has enabled problems functionality
    When I get the users my record data
    Then I receive "3" problems as part of the my record object
    And the flag informing that the patient has access to the problem data is set to "True"
    And the flag informing that there was an error retrieving the problem data is set to "False"
    And the field indicating supplier is set

    Examples:
      | GP System |
      | EMIS      |
      | VISION    |

  Scenario Outline: A <GP System> user cannot get problems data when GP practice has disabled problems functionality
    Given I am a <GP System> user setup to use medical record version 2
    And I have logged in and have a valid session cookie
    And the GP Practice has disabled problems functionality
    When I get the users my record data
    Then I receive "0" problems as part of the my record object
    And the flag informing that the patient has access to the problem data is set to "False"
    And the flag informing that there was an error retrieving the problem data is set to "False"
    And the field indicating supplier is set

    Examples:
      | GP System |
      | EMIS      |
      | VISION    |
