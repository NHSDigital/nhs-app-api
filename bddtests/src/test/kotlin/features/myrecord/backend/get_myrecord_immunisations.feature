@my-record
@backend
Feature: Get Immunisations Data
  A user can get their immunisation information

  Scenario Outline: Requesting immunisations returns immunisations data
    Given the my record wiremocks are initialised for <Service>
    And I have logged in and have a valid session cookie
    And the GP Practice has enabled immunisations functionality and multiple immunisation records exist
    When I get the users immunisations
    Then I receive "2" immunisations as part of the my record object
    And the field indicating supplier is set

  Examples:
  |Service|
  |EMIS|
  |VISION|


  Scenario Outline: Requesting immunisations returns immunisations data
    Given the my record wiremocks are initialised for <Service>
    And I have logged in and have a valid session cookie
    And no immunisation records exist for the patient
    When I get the users immunisations
    Then I receive "0" immunisations as part of the my record object
    And the field indicating supplier is set

  Examples:
  |Service|
  |EMIS|
  |VISION|
