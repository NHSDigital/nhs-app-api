Feature: Registration
  In order to access personalised NHS services
  As an unregistered user
  I want to be able to create a new NHS account

  Scenario: Create new NHS account
    Given I am not logged in
    When I am on the home page
    Then I should a see mechanism for initiating the creation of a new account

