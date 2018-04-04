Feature: Login
  In order to access personalised NHS services
  As a registered user
  I want to be able to login using my NHS account

  Scenario: A user who is not signed in sees the login button
    Given I am not logged in
    When I am on the home page
    Then I should a see mechanism for initiating login

  @wip
  Scenario: A user who is not signed in sees home screen

  @wip
  Scenario: CitizenID login journey is launched when login button is clicked on home screen

  @wip
  Scenario: After completing the login journey the user is shown the welcome screen

  @wip
  Scenario: A user who is signed in is shown their name on the welcome screen

  @wip
  Scenario: A user who is signed in sees Header and Navigation Menu

  @wip
  Scenario: If CitizenID provides invalid data after successful login then user is shown an error message


