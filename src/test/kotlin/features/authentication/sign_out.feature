Feature: Sign out of mobile web
  In order to ensure privacy with personalised NHS services
  As a registered and signed in user
  I want to be able to sign out from my NHS account so personal data cannot be seen

  Background:
    Given wiremock is initialised

  @NHSO-186
  Scenario: A user is shown the onboarding sign in screen after clicking the "Sign out" button
    Given I am logged in
    When I sign out
    Then I see the login page

  @NHSO-186
  @manual
    Scenario: A spinner is shown if there is a delay in the action of the "Sign out" button
    # Cannot slow the sign-out down enough to detect the spinner icon.

  @NHSO-186
  @manual
  Scenario: If session fails to clear when a signed in user clicks the sign out button, the local and session storage is cleared and the user is shown the onboarding sign in screen
    # Cannot enduce session signout failure.

  @NHSO-415
  @pending
  Scenario: When a signed in user clicks the sign out button and loses connection a "Service is unavailable" message and "Try again" button is displayed

  @NHSO-415
  @pending
  Scenario: When a signed in user lost connection while attempting to reach the my account page and clicks the "Try again" button the user should be shown the onboarding sign in screen

  @NHSO-186
  Scenario: A signed out user should not see the navigation bar or header on the onboarding sign in screen
    Given I am logged in
    When I sign out
    Then I do not see the menu bar

  @NHSO-186
  Scenario: The nsho cookie should be clear of session and user information if the user is not signed in
    Given I am logged in
    When I sign out
    Then the user login details are cleared from cookies