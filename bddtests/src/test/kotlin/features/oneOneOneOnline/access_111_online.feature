Feature: Access 111 Online

  A user can navigate to the 111 service either before or after logging in.

  Background:
    Given EMIS is initialised

  @smoketest
  Scenario: The user is not logged in
    Given I am not logged in
    When I Check My Symptoms
    And Check My symptoms page is displayed Logged Out

  @smoketest
  Scenario: The user is logged in
    Given I am logged in
    When I navigate to Symptoms
    And Check My symptoms page is displayed Logged In