@my-record
@backend
Feature: Get Immunisations Data Backend
  A user can get their immunisation information

  Scenario Outline: A <GP System> user can get immunisations data when GP Practice has enabled immunisations functionality
    Given the my record wiremocks are initialised for <GP System>
    And I have logged in and have a valid session cookie
    And the GP Practice has enabled immunisations functionality and multiple immunisation records exist
    When I get the users immunisations
    Then I receive "2" immunisations as part of the my record object
    And the field indicating supplier is set

  Examples:
    | GP System |
    | EMIS      |
    | VISION    |


  Scenario Outline: A <GP System> user cannot get immunisations data when no immunisation records exist for the patient
    Given the my record wiremocks are initialised for <GP System>
    And I have logged in and have a valid session cookie
    And no immunisation records exist for the patient
    When I get the users immunisations
    Then I receive "0" immunisations as part of the my record object
    And the field indicating supplier is set

  Examples:
  | GP System |
  | EMIS      |
  | VISION    |
