@my-record
Feature: Get Immunisations Data

  A user can get their immunisation information

  @backend
  Scenario Outline: Requesting immunisations returns immunisations data
    Given the my record wiremocks are initialised for <Service>
    And I have logged into <Service> and have a valid session cookie
    And the GP Practice has enabled immunisations functionality and multiple immunisation records exist for <Service>
    When I get the users immunisations
    Then I receive "2" immunisations as part of the my record object
    And the field indicating supplier is set to <Service>

  Examples:
  |Service|
  |EMIS|

  @backend
  Scenario Outline: Requesting immunisations returns immunisations data
    Given the my record wiremocks are initialised for <Service>
    And I have logged into <Service> and have a valid session cookie
    And no immunisation records exist for the patient for <Service>
    When I get the users immunisations
    Then I receive "0" immunisations as part of the my record object
    And the field indicating supplier is set to <Service>

  Examples:
  |Service|
  |EMIS|

