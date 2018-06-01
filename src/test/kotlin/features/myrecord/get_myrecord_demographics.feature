Feature: Get demographic data

  A user can get their demographic information

  Background:
    Given wiremock is initialised

  @GetDemographicsObject
  @backend
  Scenario: Requesting demographics returns demographic data
    Given I have logged in and have a valid session cookie
    When I get the users demographic data with a valid cookie
    Then I receive the demographic object
