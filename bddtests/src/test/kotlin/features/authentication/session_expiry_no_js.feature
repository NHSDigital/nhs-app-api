@authentication
@authentication-session-expiry
@noJs
Feature: NoJS Session Expiry

  @long-running
  Scenario: On session expiry, a user on a secure screen is automatically signed out
    Given I have disabled javascript
    And I am a EMIS patient
    And I am logged in
    When I am idle long enough for the desktop session to expire
    Then I see the login page
    And the user login details are cleared from cookies
