Feature: Login
  In order to get medical help or advice
  As a not logged user
  I want to view NHS 111 online page

  Scenario: Login with your NHS account
    Given I am not logged in
    And I am on the home page
    When I invoke symptom checker
    Then I should be on the symptom checker
