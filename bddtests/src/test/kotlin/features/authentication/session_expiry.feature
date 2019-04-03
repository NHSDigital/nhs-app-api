@authentication
@authentication-session-expiry
@authentication-session-extend
@native
@native-smoketest
Feature: Session Expiry and Extend
   Notes on the terminology for the following tests:
   * A 'secure screen' is a page that is within our native app, e.g. Appointments Booking Page
   * A 'non secure screen' is a page that is not within our native app, but surfaced as part of the functionality,
   e.g. Health A-Z (accessed from the symptoms page)

  @long-running
  @nativepending @NHSO-2974
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
    And I navigate to Symptoms
    When I am idle long enough for the session to expire
    And I navigate to Appointments
    Then I see the login page with the session expiry notification
    And the user login details are cleared from cookies

  @tech-debt @NHSO-1742
  Scenario: On session expiry, a user of the native app, on a non secure screen,
  is signed out on navigating back to a secure screen and sending a request
    Given I am a EMIS patient
    And I am logged in
    And I navigate to Symptoms
    When I am idle long enough for the session to expire
    And I navigate to Appointments
    Then I see the login page with the session expiry notification
    And the user login details are cleared from cookies

  @tech-debt   @NHSO-4064 # covered in Manual Regression Test pack
  Scenario: On session expiry, a user without an active internet connection is automatically signed out
    Given I am a EMIS patient
    And I am logged in
    And I am on the home page
    And I have lost internet connection
    When I am idle long enough for the session to expire
    Then I see the login page with the session expiry notification
    And the user login details are cleared from cookies

  @manual
  Scenario: Before session expiry, a user without an active internet connection is prompted with the session
  dialog box, extends their session and sees the internet connection error screen.
    Given I am a EMIS patient
    And I am logged in
    And I am on the home page
    And I have lost internet connection
    When I am idle long enough for the session expiry dialog box to appear
    Then I navigate to Appointments
    Then I see a dialog box prompting to extend the session
    When I click to extend the session
    Then I see the connection error screen
    Then I am idle for a short time
    When I navigate to Prescriptions
    Then I am still on the connection error screen

  @manual
  Scenario: Before session expiry, a user without an active internet connection is prompted with the session
  dialog box, opts to logout.
    Given I am a EMIS patient
    And I am logged in
    And I am on the home page
    And I have lost internet connection
    When I am idle long enough for the session expiry dialog box to appear
    Then I navigate to Appointments
    Then I see a dialog box prompting to extend the session
    When I click to log out
    Then I see the login page
    And the dialog box is not visible on the screen
    And the user login details are cleared from cookies

  @android
  Scenario: Before session expiry, a user of the native app, on a secure screen, is prompted with the session extension
  dialog box, extends their session and stays signed in
    Given I have upcoming appointments before cutoff time for EMIS
    And I am logged in
    Then I navigate to Appointments
    When I am idle long enough for the session expiry dialog box to appear
    Then I am idle for a short time
    Then I see a dialog box prompting to extend the session
    When I click to extend the session
    Then the dialog box is not visible on the screen
    When I select "Book new appointment" button
    Then I am given guidance as to my options before booking an appointment

  @android
  Scenario: Before session expiry, a user of the native app, on a secure screen, is prompted with the session extension
  dialog box, opts to logout
    Given I have upcoming appointments before cutoff time for EMIS
    And I am logged in
    Then I navigate to Appointments
    When I am idle long enough for the session expiry dialog box to appear
    Then I am idle for a short time
    Then I see a dialog box prompting to extend the session
    When I click to log out
    Then I see the login page
    And the dialog box is not visible on the screen

  @android
  Scenario: Before session expiry, a user of the native app, on a secure screen, is prompted with the session extension
  dialog box, does nothing and is signed out
    Given I have upcoming appointments before cutoff time for EMIS
    And I am logged in
    Then I navigate to Appointments
    When I am idle long enough for the session expiry dialog box to appear
    Then I am idle for a short time
    Then I see a dialog box prompting to extend the session
    When I am idle long enough for the session to expire
    Then I see the login page with the session expiry notification
    And the dialog box is not visible on the screen

  @android
  Scenario: Before session expiry, a user of the native app, on a non secure screen, when navigating back to a secure
  screen is prompted with the session extension dialog box, extends their session and stays signed in
    Given I have upcoming appointments before cutoff time for EMIS
    And I am logged in
    And I navigate to more
    When I choose to set my organ donation preferences
    When I am idle long enough for the session expiry dialog box to appear
    Then I navigate to Appointments
    Then the dialog box is not visible on the screen
    When I am idle long enough on a secure page for the session expiry dialog box to appear
    When I click to extend the session
    Then the dialog box is not visible on the screen
    When I select "Book new appointment" button
    Then I am given guidance as to my options before booking an appointment

  @android
  Scenario: Before session expiry, a user of the native app, on a non secure screen, when navigating back to a secure
  screen is prompted with the session extension dialog box, opts to logout
    Given I have upcoming appointments before cutoff time for EMIS
    And I am logged in
    And I navigate to more
    When I choose to set my organ donation preferences
    When I am idle long enough for the session expiry dialog box to appear
    Then I am idle for a short time
    Then I navigate to Appointments
    Then the dialog box is not visible on the screen
    When I am idle long enough for the session expiry dialog box to appear
    Then I see a dialog box prompting to extend the session
    When I click to log out
    Then I see the login page
    And the dialog box is not visible on the screen

  @android
  Scenario: Before session expiry, a user of the native app, on a non secure screen, when navigating back to a secure
  screen is prompted with the session extension dialog box, does nothing and is signed out
    Given I have upcoming appointments before cutoff time for EMIS
    And I am logged in
    And I navigate to more
    When I choose to set my organ donation preferences
    Then I scroll the device
    When I am idle long enough for the session expiry dialog box to appear
    Then I am idle for a short time
    Then I navigate to Appointments
    Then the dialog box is not visible on the screen
    When I am idle long enough for the session expiry dialog box to appear
    Then I see a dialog box prompting to extend the session
    When I am idle long enough for the session to expire after the dialog
    Then I see the login page with the session expiry notification
    And the dialog box is not visible on the screen

  @android
  Scenario: The session expires, a user of the native app, on a non secure screen, when navigating back to a secure
  screen is signed out
    Given I have upcoming appointments before cutoff time for EMIS
    And I am logged in
    And I navigate to more
    When I choose to set my organ donation preferences
    When I am idle long enough for the session to expire
    Then I am idle for a short time
    Then I navigate to Appointments
    Then I see the login page

  @android
  Scenario: The native app, on a secure screen is in the background. Prior to session expiry the user brings the app
  to the foreground and the session extension dialog box is displayed, extends their session and stays signed in
    Given I have upcoming appointments before cutoff time for EMIS
    And I am logged in
    Then I navigate to Appointments
    Then I background the app long enough for the session warning dialog to appear and bring it back to foreground
    Then I see a dialog box prompting to extend the session
    When I click to extend the session
    Then the dialog box is not visible on the screen
    When I select "Book new appointment" button
    Then I am given guidance as to my options before booking an appointment

  @android
  Scenario: The native app, on a secure screen is in the background. Prior to session expiry the user brings the app
  to the foreground and the session extension dialog box is displayed, opts to logout
    Given I have upcoming appointments before cutoff time for EMIS
    And I am logged in
    Then I navigate to Appointments
    Then I background the app long enough for the session warning dialog to appear and bring it back to foreground
    Then I see a dialog box prompting to extend the session
    When I click to log out
    Then I see the login page
    And the dialog box is not visible on the screen

  @android
  Scenario: The native app, on a secure screen is in the background. Prior to session expiry the user brings the app
  to the foreground and the session extension dialog box is displayed, does nothing and is signed out
    Given I have upcoming appointments before cutoff time for EMIS
    And I am logged in
    Then I navigate to Appointments
    Then I background the app long enough for the session warning dialog to appear and bring it back to foreground
    Then I see a dialog box prompting to extend the session
    When I am idle long enough for the session to expire after the dialog
    Then I see the login page with the session expiry notification
    And the dialog box is not visible on the screen

  @android
  Scenario: The native app, on a secure screen is in the background. The session expires, the user brings the app
  to the foreground and is signed out
    Given I have upcoming appointments before cutoff time for EMIS
    And I am logged in
    Then I navigate to Appointments
    Then I background the app long enough for the session expiry and bring it back to foreground
    Then I see the login page

  @manual
  Scenario: The user has locked the device with the native app on a secure screen. Prior to session expiry the user
  unlocks the device and the session extension dialog box is displayed, extends their session and stays signed in
    Given I have upcoming appointments before cutoff time for EMIS
    And I am logged in
    Then I navigate to Appointments
    Then I lock the device
    When I am idle long enough for the session expiry dialog box to appear
    Then I unlock the device
    Then I see a dialog box prompting to extend the session
    When I click to extend the session
    Then the dialog box is not visible on the screen
    And I see the header
    When I select "Book new appointment" button
    Then I am given guidance as to my options before booking an appointment

  @manual
  Scenario: The user has locked the device with the native app on a secure screen. Prior to session expiry the user
  unlocks the device and the session extension dialog box is displayed, opts to logout
    Given I have upcoming appointments before cutoff time for EMIS
    And I am logged in
    Then I navigate to Appointments
    Then I lock the device
    When I am idle long enough for the session expiry dialog box to appear
    Then I am idle for a short time
    Then I scroll the device
    Then I unlock the device
    Then I scroll the device
    Then I am idle for a short time
    Then I see a dialog box prompting to extend the session
    When I click to log out
    Then I see the login page
    And the dialog box is not visible on the screen

  @manual
  Scenario: The user has locked the device with the native app on a secure screen. Prior to session expiry the user
  unlocks the device and the session extension dialog box is displayed, does nothing and is signed out
    Given I have upcoming appointments before cutoff time for EMIS
    And I am logged in
    Then I lock the device
    When I am idle long enough for the session expiry dialog box to appear
    Then I unlock the device
    Then I am idle for a short time
    Then I see a dialog box prompting to extend the session
    When I am idle long enough for the session to expire
    Then the dialog box is not visible on the screen
    Then I see the login page with the session expiry notification
    And the dialog box is not visible on the screen

  @manual
  Scenario: The user has locked the device with the native app on a secure screen. The session expires, the user
  unlocks the device and is signed out
    Given I have upcoming appointments before cutoff time for EMIS
    And I am logged in
    Then I navigate to Appointments
    Then I lock the device
    When I am idle long enough for the session to expire
    Then I unlock the device
    Then I see the login page with the session expiry notification

  @backend
  Scenario Outline: <GP System> GP practice session has expired
    Given I have logged into <GP System> and have a valid session cookie
    And the GP System session has expired when viewing prescriptions
    When I request prescriptions for the last 6 months
    Then I receive a "Unauthorized" error

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario Outline: The <GP System> GP practice session has expired and user selects the prescriptions button
    Given I am patient using the <GP System> GP System
    And I am logged in
    Given the GP System session has expired when viewing prescriptions
    When I navigate to prescriptions
    Then I see the login page with the session expiry notification

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
