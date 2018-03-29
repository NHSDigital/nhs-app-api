Feature: Login
  In order to access personalised NHS services
  As a registered user
  I want to be able to login using my NHS account

  Scenario: Login with your NHS account
    Given I am not logged in
    When I am on the home page
    Then I should a see mechanism for initiating login
