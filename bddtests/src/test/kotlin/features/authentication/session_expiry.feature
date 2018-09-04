@authentication
Feature: Session Expiry

  Background:
    Given EMIS is initialised

  @NHSO-1704
  @long-running
  Scenario: On session expiry, a user on a secure screen is automatically signed out
    Given I am logged in
    And I am on the home page
    When I am idle long enough for the session to expire
    Then I see the login page with the session expiry notification
    And the user login details are cleared from cookies

  @manual
  @NHSO-1704
  Scenario: On session expiry, a user on a non secure screen is signed out on navigating back to a secure screen
    Given I am logged in
    And I am on the symptoms page
    When I am idle long enough for the session to expire
    And I navigate back to the app
    Then I see the login page with the session expiry notification
    And the user login details are cleared from cookies

  @native
  @tech-debt @NHSO-1742
  Scenario: On session expiry, a user of the native app, on a non secure screen, is signed out on navigating back to a secure screen and sending a request
    Given I am logged in
    And I am on the symptoms page
    When I am idle long enough for the session to expire
    And I navigate back to the app
    And I click the button to go back to my appointments
    Then I see the login page
    And the user login details are cleared from cookies

  @manual
  @NHSO-1704
  Scenario: On session expiry, a user without an active internet connection is automatically signed out
    Given I am logged in
    And I am on the home page
    And I have lost internet connection
    When I am idle long enough for the session to expire
    Then I see the login page with the session expiry notification
    And the user login details are cleared from cookies
