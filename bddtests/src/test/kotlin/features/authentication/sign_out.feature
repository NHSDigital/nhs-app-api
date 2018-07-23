Feature: Sign out of mobile web
  In order to ensure privacy with personalised NHS services
  As a registered and signed in user
  I want to be able to sign out from my NHS account so personal data cannot be seen

  Background:
    Given a patient from EMIS is defined

  @NHSO-2038
  @NHSO-186
  @NHSO-985
  @smoketest
  Scenario Outline: A <GP Supplier> user is shown the onboarding sign in screen after clicking the "Sign out" button
    Given I am logged in as a <GP Supplier> user
    When I sign out
    Then I see the login page
    Examples:
    | GP Supplier |
    | EMIS        |
    | TPP         |
  # | VISION      | - Blocked until Vision login implemented

  @NHSO-186
  @NHSO-985
  @manual
    Scenario Outline: A spinner is shown if there is a delay in the action of the "Sign out" button for a <GP Supplier> user
    # Cannot slow the sign-out down enough to detect the spinner icon.
  Examples:
  | GP Supplier |
  | EMIS        |
  | TPP         |
  | VISION      |

  @NHSO-186
  @NHSO-985
  @manual
  Scenario: The nsho cookie should be clear of session and user information if server side sign out fails
    # Cannot enduce session signout failure.
    Given I am logged in
    When I sign out
    And session fails to clear
    Then the user login details are cleared from cookies

  @NHSO-2038
  @NHSO-186
  @NHSO-985
  Scenario Outline: A signed out <GP Supplier> user should not see the navigation bar or header on the onboarding sign in screen
    Given I am logged in as a <GP Supplier> user
    When I sign out
    Then I see the login page
    And I do not see the menu bar
  Examples:
  | GP Supplier |
  | EMIS        |
  | TPP         |
  # | VISION      | - Blocked until Vision login implemented

  @NHSO-2038
  @NHSO-186
  @NHSO-985
  Scenario Outline: The nsho cookie should be clear of session and user information if the <GP Supplier> user is not signed in
    Given I am logged in as a <GP Supplier> user
    When I sign out
    Then I see the login page
    And the user login details are cleared from cookies
  Examples:
  | GP Supplier |
  | EMIS        |
  | TPP         |
  # | VISION      | - Blocked until Vision login implemented

  @NHSO-985
  Scenario Outline: The nsho cookie should be clear of <GP Supplier> session and user information if server side sign out fails
    # Only TPP has a sign out endpoint which can fail. Other suppliers tested manually
    Given I am logged in as a <GP Supplier> user where the session will fail to clear on signout
    When I sign out
    Then I see the login page
    And the user login details are cleared from cookies
    Examples:
      | GP Supplier |
      | TPP         |
