Feature: Access 111 Online when not logged in
  In order to get medical help or advice
  As a not logged user
  I want to view NHS 111 online page

  Scenario: Selecting the "Check your symptoms" button on the home page when not logged in opens 111 Online
    Given I am not logged in
    And I am on the home page
    When I invoke symptom checker
    Then I should be on the symptom checker
