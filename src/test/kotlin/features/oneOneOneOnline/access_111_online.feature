Feature: Access 111 Online

  A user can navigate to the 111 service either before or after logging in.

  Background:
    Given wiremock is initialised

  Scenario: The user is not logged in
    Given I am not logged in
    When I Check My Symptoms
    Then I am redirected to https://111.nhs.uk/


  Scenario: The user is logged in
    Given I am logged in
    When I navigate to Symptoms
    Then a new tab opens https://111.nhs.uk/
    And Symptoms is unselected