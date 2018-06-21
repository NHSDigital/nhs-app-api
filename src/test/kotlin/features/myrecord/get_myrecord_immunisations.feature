Feature: Get Immunisations Data

  A user can get their immunisation information

  Background:
    Given wiremock is initialised
    And the my record wiremocks are initialised

  @backend
  Scenario: Requesting immunisations returns immunisations data
    Given I have logged in and have a valid session cookie
    Given the GP Practice has enabled immunisations functionality and multiple immunisation records exist
    When I get the users immunisations
    Then I receive "2" immunisations as part of the my record object

  @backend
  Scenario: Requesting immunisations returns immunisations data
    Given I have logged in and have a valid session cookie
    And no immunisation records exist for the patient
    When I get the users immunisations
    Then I receive "0" immunisations as part of the my record object

