Feature: Get Immunisations Data

  A user can get their immunisation information

  Background:
    Given wiremock is initialised

  @backend
  Scenario: Requesting immunisations returns immunisations data
    Given I have logged in and have a valid session cookie
    Given the GP Practice has enabled immunisations functionality
    When I get the users immunisations
    Then I receive "2" immunisations as part of the my record object

