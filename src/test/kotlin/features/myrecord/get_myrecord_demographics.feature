Feature: Get demographic data

  A user can get their demographic information

  Background:
    Given wiremock is initialised

  @backend
  Scenario: Requesting demographics returns demographic data
    Given I have logged in and have a valid session cookie
    Given the GP Practice has enabled demographics functionality
    When I get the users demographic data
    Then I receive the demographic object

  @backend
  Scenario: GP practice has disabled demographics functionality
    Given I have logged in and have a valid session cookie
    But the GP Practice has disabled demographics functionality
    When I get the users demographic data
    Then I receive a "Forbidden" error

  @pending
  @backend
  Scenario: GP System Unavailable
    Given I have logged in and have a valid session cookie
    But the GP System is unavailable
    When I communicate with EMIS
    Then I get a "Bad gateway" error

  @pending
  @backend
  Scenario: GP System Times Out
    Given I have logged in and have a valid session cookie
    But the GP System times out
    When I communicate with EMIS
    Then I get a "Gateway timeout" error