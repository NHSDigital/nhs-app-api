@111Online
@nativesmoketest
Feature: Access 111 Online
        A user can navigate to the 111 service either before or after logging in.

  Scenario: The user is not logged in
    Given I am not logged in
    When I Check My Symptoms
    Then the Check My Symptoms page is displayed

  Scenario: The user is logged in
    Given I am a EMIS patient
    And I am logged in
    When I navigate to Symptoms
    Then the Check My Symptoms page is displayed