@authentication
@authentication-session-expiry
@authentication-session-extend
@long-running
Feature: Session Expiry and Extend for Desktop
   Notes on the terminology for the following tests:
   * A 'secure screen' is a page that is within the Desktop app, e.g. Appointments Booking Page
   * A 'non secure screen' is a page that is not within our native app, but surfaced as part of the functionality,
   e.g. Health A-Z (accessed from the symptoms page)

  Scenario: Before session expiry, on a secure screen, is prompted with the session extension dialog box, extends their session and stays signed in
    Given there are EMIS appointments available to book
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    When I am idle long enough for the session expiry dialog box to appear
    Then I am idle for a short time
    Then I see a dialog box prompting to extend the session
    When I click to extend the session
    Then the dialog box is not visible on the screen
    And I have filtered such that there is one time displayed that represents multiple slots
    When I have selected a time when multiple slots are available
    Then the Appointment Slot page is displayed
    When I enter symptoms
    And  I click the 'Confirm and book appointment' button
    Then the Appointment Booking success message is displayed

  Scenario: Before session expiry, on a secure screen, is prompted with the session extension dialog box, opts to logout
    Given there are EMIS appointments available to book
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    When I am idle long enough for the session expiry dialog box to appear
    Then I am idle for a short time
    Then I see a dialog box prompting to extend the session
    When I click to log out
    Then I see the login page
    And the dialog box is not visible on the screen

  Scenario: Before session expiry, on a secure screen, is prompted with the session extension dialog box, does nothing and is signed out
    Given there are EMIS appointments available to book
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    When I am idle long enough for the session expiry dialog box to appear
    Then I am idle for a short time
    Then I see a dialog box prompting to extend the session
    When I am idle long enough for the session to expire
    Then I see the login page with the session expiry notification
    And the dialog box is not visible on the screen

  Scenario: Before session expiry, on a secure screen, is prompted with the session extension dialog box and check the focus trap
    Given I have upcoming appointments before cutoff time for EMIS
    And I am logged in
    Then I navigate to Appointments
    When I am idle long enough for the session expiry dialog box to appear
    Then I am idle for a short time
    Then I see a dialog box prompting to extend the session
    Then I check that 'Stay logged in' is in focus
    Then I press the tab key
    Then I check that 'Log out' is in focus
    Then I press the tab key
    Then I check that 'Stay logged in' is in focus
    Then I press the tab key
    Then I check that 'Log out' is in focus