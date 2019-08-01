@my-record
@backend
Feature: Get medications data Backend
  A user can get their medication information

  Scenario Outline: A <GP System> user can get medications data when GP Practice has enabled medications functionality
    Given the my record wiremocks are initialised for <GP System>
    And I have logged in and have a valid session cookie
    And the GP Practice has enabled medications functionality
    When I get the users my record data
    Then I receive "1" acute medications as part of the my record object
    And I receive "3" current repeat medications as part of the my record object
    And I receive "2" discontinued repeat medications as part of the my record object
    And the field indicating supplier is set

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario Outline: A <GP System> user cannot get medications data when GP Practice has disabled medications functionality
    Given the my record wiremocks are initialised for <GP System>
    And I have logged in and have a valid session cookie
    But the GP Practice has disabled medications functionality
    When I get the users my record data
    Then the flag informing that the patient has access to the medications data is set to "False"
    And the flag informing that there was an error retrieving the medications data is set to "False"
    And the field indicating supplier is set

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
