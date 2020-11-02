@authentication
@authentication-session-expiry
@authentication-session-extend
@native
Feature: Session Expiry and Extend Frontend
  Notes on the terminology for the following tests:
  * A 'secure screen' is a page that is within our native app, e.g. Appointments Booking Page
  * A 'non secure screen' is a page that is not within our native app, but surfaced as part of the functionality,
  e.g. Health A-Z (accessed from the symptoms page)

  @long-running
  @tech-debt @NHSO-1742
  Scenario: On session expiry, a user on a secure screen is automatically signed out
    Given I am a EMIS patient
    And I am logged in
    And I am on the home page
    When I am idle long enough for the session to expire
    Then I see the login page with the session expiry notification
    And the user login details are cleared from cookies

  @pending @pending-web
  Scenario: On session expiry, a user on a non secure screen is signed out on navigating back to a secure screen
    Given I am a EMIS patient
    And I am logged in
    And I navigate to Advice
    When I am idle long enough for the session to expire
    And I navigate to Appointments
    Then I see the login page with the session expiry notification
    And the user login details are cleared from cookies

  @tech-debt @NHSO-1742
  Scenario: On session expiry, a user of the native app, on a non secure screen, is signed out on navigating back to a secure screen and sending a request
    Given I am a EMIS patient
    And I am logged in
    And I navigate to Advice
    When I am idle long enough for the session to expire
    And I navigate to Appointments
    Then I see the login page with the session expiry notification
    And the user login details are cleared from cookies

  # covered in Manual Regression Test pack
  @tech-debt   @NHSO-4064
  Scenario: On session expiry, a user without an active internet connection is automatically signed out
    Given I am a EMIS patient
    And I am logged in
    And I am on the home page
    And I have lost internet connection
    When I am idle long enough for the session to expire
    Then I see the login page with the session expiry notification
    And the user login details are cleared from cookies

  @manual
  Scenario: Before session expiry, a user without an active internet connection is prompted with the session dialog box, extends their session and sees the internet connection error screen.
    Given I am a EMIS patient
    And I am logged in
    And I am on the home page
    And I have lost internet connection
    When I am idle long enough for the session expiry dialog box to appear
    Then I navigate to Appointments
    And I see a dialog box prompting to extend the session
    When I click to extend the session
    Then I see the connection error screen
    And I am idle for a short time
    When I navigate to Prescriptions
    Then I am still on the connection error screen

  @manual
  Scenario: Before session expiry, a user without an active internet connection is prompted with the session dialog box, opts to logout.
    Given I am a EMIS patient
    And I am logged in
    And I am on the home page
    And I have lost internet connection
    When I am idle long enough for the session expiry dialog box to appear
    Then I navigate to Appointments
    And I see a dialog box prompting to extend the session
    When I click to log out
    Then I see the login page
    And the dialog box is not visible on the screen
    And the user login details are cleared from cookies

  @long-running
  Scenario: Before session expiry, a user of the native app, on a secure screen, is prompted with the session extension dialog box, extends their session and stays signed in
    Given I have upcoming appointments before cutoff time for EMIS
    And I am using the native app user agent
    And I log in to the app expecting to see the notifications prompt
    Then I see the notifications prompt
    When I do not accept notifications and continue
    Then I see the home page
    And I navigate to Appointments
    When I am idle long enough for the session expiry dialog box to appear
    Then I am idle for a short time
    And I see a dialog box prompting to extend the session
    When I click to extend the session
    Then the dialog box is not visible on the screen
    When I click the GP Appointments link
    Then the Your Appointments page is displayed

  @long-running
  Scenario: Before session expiry, a user of the native app, on a secure screen, is prompted with the session extension dialog box, opts to logout
    Given I have upcoming appointments before cutoff time for EMIS
    And I am using the native app user agent
    And I log in to the app expecting to see the notifications prompt
    Then I see the notifications prompt
    When I do not accept notifications and continue
    Then I see the home page
    And I navigate to Appointments
    When I am idle long enough for the session expiry dialog box to appear
    Then I am idle for a short time
    And I see a dialog box prompting to extend the session
    When I click to log out
    Then I see the login page
    And the dialog box is not visible on the screen

  @long-running
  Scenario: Before session expiry, a user of the native app, on a secure screen, is prompted with the session extension dialog box, does nothing and is signed out
    Given I have upcoming appointments before cutoff time for EMIS
    And I am using the native app user agent
    And I log in to the app expecting to see the notifications prompt
    Then I see the notifications prompt
    When I do not accept notifications and continue
    Then I see the home page
    And I navigate to Appointments
    When I am idle long enough for the session expiry dialog box to appear
    Then I am idle for a short time
    And I see a dialog box prompting to extend the session
    When I am idle long enough for the session to expire
    Then I see the login page with the session expiry notification
    And the dialog box is not visible on the screen

  @long-running
  @bug @NHSO-8713
  Scenario: Before session expiry, a user of the native app, on a non secure screen, when navigating back to a secure screen is prompted with the session extension dialog box, extends their session and stays signed in
    Given I have upcoming appointments before cutoff time for EMIS
    And I am using the native app user agent
    And I log in to the app expecting to see the notifications prompt
    Then I see the notifications prompt
    When I do not accept notifications and continue
    Then I see the home page
    When I navigate to Advice
    And I click Use NHS 111 online
    And I am idle long enough for the session expiry dialog box to appear
    And I navigate to Appointments
    Then the dialog box is not visible on the screen
    When I am idle long enough on a secure page for the session expiry dialog box to appear
    And I click to extend the session
    Then the dialog box is not visible on the screen
    When I click the GP Appointments link
    Then the Your Appointments page is displayed

  @long-running
  @android
  Scenario: Before session expiry, a user of the native app, on a non secure screen, when navigating back to a secure screen is prompted with the session extension dialog box, opts to logout
    Given I have upcoming appointments before cutoff time for EMIS
    And I am using the native app user agent
    And I log in to the app expecting to see the notifications prompt
    Then I see the notifications prompt
    When I do not accept notifications and continue
    Then I see the home page
    When I navigate to Advice
    And I click Use NHS 111 online
    And I am idle long enough for the session expiry dialog box to appear
    And I am idle for a short time
    And I navigate to Appointments
    Then the dialog box is not visible on the screen
    When I am idle long enough for the session expiry dialog box to appear
    Then I see a dialog box prompting to extend the session
    When I click to log out
    Then I see the login page
    And the dialog box is not visible on the screen

  @long-running
  @android
  Scenario: Before session expiry, a user of the native app, on a non secure screen, when navigating back to a secure screen is prompted with the session extension dialog box, does nothing and is signed out
    Given I have upcoming appointments before cutoff time for EMIS
    And I am using the native app user agent
    And I log in to the app expecting to see the notifications prompt
    Then I see the notifications prompt
    When I do not accept notifications and continue
    Then I see the home page
    When I navigate to Advice
    And I click Use NHS 111 online
    And I am idle long enough for the session expiry dialog box to appear
    And I am idle for a short time
    And I navigate to Appointments
    Then the dialog box is not visible on the screen
    When I am idle long enough for the session expiry dialog box to appear
    Then I see a dialog box prompting to extend the session
    When I am idle long enough for the session to expire after the dialog
    Then I see the login page with the session expiry notification
    And the dialog box is not visible on the screen

  @long-running
  @android
  Scenario: The session expires, a user of the native app, on a non secure screen, when navigating back to a secure screen is signed out
    Given I have upcoming appointments before cutoff time for EMIS
    And I am using the native app user agent
    And I log in to the app expecting to see the notifications prompt
    Then I see the notifications prompt
    When I do not accept notifications and continue
    Then I see the home page
    When I navigate to Advice
    And I click Use NHS 111 online
    And I am idle long enough for the session to expire
    And I am idle for a short time
    And I navigate to Appointments
    Then I see the login page

  @long-running
  @bug @NHSO-8713
  Scenario: The native app, on a secure screen is in the background. Prior to session expiry the user brings the app to the foreground and the session extension dialog box is displayed, extends their session and stays signed in
    Given I have upcoming appointments before cutoff time for EMIS
    And I am using the native app user agent
    And I log in to the app expecting to see the notifications prompt
    Then I see the notifications prompt
    When I do not accept notifications and continue
    Then I see the home page
    When I navigate to Appointments
    And I background the app long enough for the session warning dialog to appear and bring it back to foreground
    Then I see a dialog box prompting to extend the session
    When I click to extend the session
    Then the dialog box is not visible on the screen
    When I click the GP Appointments link
    Then the Your Appointments page is displayed

  @long-running
  @bug @NHSO-8713
  Scenario: The native app, on a secure screen is in the background. Prior to session expiry the user brings the app to the foreground and the session extension dialog box is displayed, opts to logout
    Given I have upcoming appointments before cutoff time for EMIS
    And I am using the native app user agent
    And I log in to the app expecting to see the notifications prompt
    Then I see the notifications prompt
    When I do not accept notifications and continue
    Then I see the home page
    When I navigate to Appointments
    And I background the app long enough for the session warning dialog to appear and bring it back to foreground
    Then I see a dialog box prompting to extend the session
    When I click to log out
    Then I see the login page
    And the dialog box is not visible on the screen

  @long-running
  Scenario: The native app, on a secure screen is in the background. Prior to session expiry the user brings the app to the foreground and the session extension dialog box is displayed, does nothing and is signed out
    Given I have upcoming appointments before cutoff time for EMIS
    And I am using the native app user agent
    And I log in to the app expecting to see the notifications prompt
    Then I see the notifications prompt
    When I do not accept notifications and continue
    Then I see the home page
    When I navigate to Appointments
    Then the Appointments Hub page is displayed
    And I click the GP Appointments link
    And the Your Appointments page is displayed
    And I background the app long enough for the session warning dialog to appear and bring it back to foreground
    And I see a dialog box prompting to extend the session
    When I am idle long enough for the session to expire after the dialog
    Then I see the login page with the session expiry notification
    And the dialog box is not visible on the screen

  @long-running
  Scenario: The native app, on a secure screen is in the background. The session expires, the user brings the app to the foreground and is signed out
    Given I have upcoming appointments before cutoff time for EMIS
    And I am using the native app user agent
    And I log in to the app expecting to see the notifications prompt
    Then I see the notifications prompt
    When I do not accept notifications and continue
    Then I see the home page
    When I navigate to Appointments
    And I background the app long enough for the session expiry and bring it back to foreground
    Then I see the login page

  @manual
  Scenario: The user has locked the device with the native app on a secure screen. Prior to session expiry the user unlocks the device and the session extension dialog box is displayed, extends their session and stays signed in
    Given I have upcoming appointments before cutoff time for EMIS
    And I am logged in
    Then I navigate to Appointments
    And I lock the device
    When I am idle long enough for the session expiry dialog box to appear
    Then I unlock the device
    And I see a dialog box prompting to extend the session
    When I click to extend the session
    Then the dialog box is not visible on the screen
    And I see the header
    When I click the GP Appointments link
    Then the Your Appointments page is displayed

  @manual
  Scenario: The user has locked the device with the native app on a secure screen. Prior to session expiry the user unlocks the device and the session extension dialog box is displayed, opts to logout
    Given I have upcoming appointments before cutoff time for EMIS
    And I am logged in
    Then I navigate to Appointments
    And I lock the device
    When I am idle long enough for the session expiry dialog box to appear
    Then I am idle for a short time
    When I unlock the device
    And I am idle for a short time
    Then I see a dialog box prompting to extend the session
    When I click to log out
    Then I see the login page
    And the dialog box is not visible on the screen

  @manual
  Scenario: The user has locked the device with the native app on a secure screen. Prior to session expiry the user unlocks the device and the session extension dialog box is displayed, does nothing and is signed out
    Given I have upcoming appointments before cutoff time for EMIS
    And I am logged in
    Then I lock the device
    When I am idle long enough for the session expiry dialog box to appear
    Then I unlock the device
    And I am idle for a short time
    And I see a dialog box prompting to extend the session
    When I am idle long enough for the session to expire
    Then the dialog box is not visible on the screen
    And I see the login page with the session expiry notification
    And the dialog box is not visible on the screen

  @manual
  Scenario: The user has locked the device with the native app on a secure screen. The session expires, the user unlocks the device and is signed out
    Given I have upcoming appointments before cutoff time for EMIS
    And I am logged in
    Then I navigate to Appointments
    And I lock the device
    When I am idle long enough for the session to expire
    Then I unlock the device
    And I see the login page with the session expiry notification

  Scenario Outline: The <GP System> GP practice session has expired and user selects the prescriptions button
    Given I am patient using the <GP System> GP System
    And I am using the native app user agent
    And I log in to the app expecting to see the notifications prompt
    Then I see the notifications prompt
    When I do not accept notifications and continue
    Then I see the home page
    And the GP System session has expired when viewing prescriptions
    When I navigate to prescriptions
    And I click the Order a repeat prescription button
    Then I see appropriate try again error message for prescriptions when there is no GP session
    Examples:
      | GP System |
      | TPP       |
    @bug @NHSO-7780
    Examples:
      | GP System |
      | EMIS      |
