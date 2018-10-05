Feature: Access 111 Online

  A user can navigate to the 111 service either before or after logging in.


  @smoketest
  Scenario: The user is not logged in
    Given I am not logged in
    When I Check My Symptoms
    Then Check My symptoms page is displayed

  @smoketest
  Scenario: The user is logged in
    Given a patient from EMIS is defined
    And I am logged in
    When I navigate to Symptoms
    Then Check My symptoms page is displayed
