@authentication
@authentication-logout
Feature: Sign out of mobile web
  In order to ensure privacy with personalised NHS services
  As a registered and signed in user
  I want to be able to sign out from my NHS account so personal data cannot be seen

  Scenario Outline: A <GP Supplier> user is shown the onboarding sign in screen after clicking the "Sign out" button
    Given I am logged in as a <GP Supplier> user
    When I sign out
    Then I see the login page
    Examples:
      | GP Supplier |
      | EMIS        |
      | TPP         |
  @smoketest
    Examples:
      | GP Supplier |
      | VISION      |

  @manual
  Scenario: A spinner is shown if there is a delay in the action of the "Sign out" button for a user
    # Cannot slow the sign-out down enough to detect the spinner icon.

    # covered in Manual Regression Test pack
  @manual
  Scenario: The nsho cookie should be clear of session and user information if server side sign out fails
    # Cannot induce session sign-out failure.
    Given I am logged in
    When I sign out
    And session fails to clear
    Then the user login details are cleared from cookies

  Scenario Outline: A signed out <GP Supplier> user should not see the navigation bar or header on the onboarding sign in screen
    Given I am logged in as a <GP Supplier> user
    When I sign out
    Then I see the login page
    And I do not see the menu bar
    Examples:
      | GP Supplier |
      | EMIS        |
      | TPP         |
      | VISION      |

  Scenario Outline: The nsho cookie should be clear of session and user information if the <GP Supplier> user is not signed in
    Given I am logged in as a <GP Supplier> user
    When I sign out
    Then I see the login page
    And the user login details are cleared from cookies
    Examples:
      | GP Supplier |
      | EMIS        |
      | TPP         |
      | VISION      |

  Scenario: The nsho cookie should be clear of TPP session and user information if server side sign out fails
    # Only TPP has a sign out endpoint which can fail. Other suppliers tested manually
    Given I am logged in as a TPP user where the session will fail to clear on signout
    When I sign out
    Then I see the login page
    And the user login details are cleared from cookies