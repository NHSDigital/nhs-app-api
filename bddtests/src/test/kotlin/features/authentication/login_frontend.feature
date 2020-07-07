@authentication
@authentication-login
Feature: Login frontend
  Logging into the service is handled via the CitizenID service.
  A user will be shown personalised welcome messages upon successful login.

  Scenario Outline: A <GP System> user can see the home page after logging in
    Given I am a <GP System> patient
    And I am logged in
    Then I see a welcome message
    And I see my Date of birth on the home page
    And I see my NHS number on the home page
    And I see the home page header
    And I see the navigation menu
    And I can see and follow the Check your symptoms link
    When I click the home icon
    Then I see the home page
    And I can see and follow the Book and manage appointments link
    When I click the home icon
    Then I see the home page
    And I can see and follow the Order a repeat prescription link
    When I click the home icon
    Then I see the home page
    And I can see and follow the View your GP medical record link
    When I click the home icon
    Then I see the home page
    And I can see and follow the Manage your organ donation decision link
    Examples:
      | GP System |
      | TPP       |
      | VISION    |

  @android
  Scenario: A EMIS user can see the home page after logging in
    Given I am a EMIS patient
    And I am logged in
    Then I see a welcome message
    And I see my Date of birth on the home page
    And I see my NHS number on the home page
    And I see the home page header
    And I see the navigation menu
    And I can see and follow the Check your symptoms link
    When I click the home icon
    Then I see the home page
    And I can see and follow the Book and manage appointments link
    When I click the home icon
    Then I see the home page
    And I can see and follow the Order a repeat prescription link
    When I click the home icon
    Then I see the home page
    And I navigate to the health record hub page
    And I see the health records hub page
    When I click the home icon
    Then I see the home page
    And I can see and follow the Manage your organ donation decision link

  Scenario: A user can see the native instructions
    Given I am a patient using the native app
    When I am on the login logged-out page for the first time
    And I click the 'Continue with NHS login' button
    Then the page title is 'Before you start'
    And I click the 'Continue' button
    And the page contains the header 'Integration Test Patient'

  Scenario: A user does not see the native instructions
    Given I am a patient using the native app
    When I am on the login logged-out page
    And I click the 'Continue with NHS login' button
    Then the page contains the header 'Integration Test Patient'

  Scenario: A patient with proof level 5 sees no NHS number when logging in
    Given I am a patient with proof level 5
    And I am logged in
    Then I see a welcome message
    And I see my Date of birth on the home page
    And I don't see my NHS number on the home page

  Scenario: A patient with proof level 5 sees the home page after logging in
    Given I am a patient with proof level 5
    And I am logged in
    Then I am asked to prove my identity
    And I can't see the Book and manage appointments link on the homepage
    And I can't see the Order a repeat prescription link on the homepage
    And I can't see the View your GP medical record link on the homepage
    And I can't see the Manage your organ donation decision link on the homepage
    And I can see and follow the Check your symptoms link

  Scenario: A user does not see the OLC beta banner when on not on an online consultations page
    Given I am a EMIS patient
    And I am logged in
    Then I see a welcome message
    And I do not see the yellow banner

  #Once prescriptions are completed for microtest, this test can be merged with above
  Scenario: A Microtest user sees the home page after logging in
    Given I am a MICROTEST patient
    And I am logged in
    Then I see a welcome message
    And I see my Date of birth on the home page
    And I see my NHS number on the home page
    And I see the home page header

  Scenario Outline: A <GP System> user can still login when the GP System fails
    Given I am an <GP System> patient whose GP system is unavailable
    And I am logged in
    And I see my Date of birth on the home page
    And I see my NHS number on the home page
    And I can't see the Linked profiles link on the homepage
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: A <GP System> user can still log in when the Im1 Connection Token doesn't contain a key
    Given I am logged in as a <GP System> user created before Im1 Cache Keys existed
    Then I see a welcome message
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  @pending
  @nativesmoketest
  Scenario: Any user sees the current app version on the login page
    Given I am at the login page
    And I see the login page
    Then I see the current app version

  Scenario Outline: A <GP System> user can log in, log out, and log in again
    Given I am logged in as a <GP System> user
    Then I see the home page
    When I log out
    Then I see the login page
    When I log in again
    Then I see the home page
    When I log out
    Then I see the login page
    When I log in again
    Then I see the home page
    Examples:
      | GP System |
      | TPP       |
      | EMIS      |

  Scenario: A EMIS user can log out using the header link
    Given I am logged in as a EMIS user
    Then I see the home page
    When I use the header link to log out of the website
    Then I see the login page

  Scenario: A EMIS user can cycle the header links
    Given I am logged in as a EMIS user
    And I have no booked appointments for EMIS
    And I have no repeat prescriptions
    When I see the home page
    Then I can cycle through the header links

  @nativesmoketest
  Scenario: A EMIS user on a native device can cycle the header links
    Given I am logged in as a EMIS user
    And I have no booked appointments for EMIS
    And I have no repeat prescriptions
    When I see the home page
    Then I can cycle through the native header links

  @manual
  # to enable survey link change HOTJAR_SURVEY_VISIBLE env variable value
  Scenario Outline: A <GP System> user sees a survey link on the home page if enabled
    Given I am logged in as a <GP System> user
    Then I see the home page
    And I see a collapsible link to a survey, which I can follow
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

    # covered in Manual Regression Test pack
  @manual
  Scenario Outline: <GPSS> User logs in using CitizenID
    Given I am not logged in as a <GPSS> user
    When I log in
    Then I am redirected to 'http://citizenidaddresshere'
    Examples:
      | GPSS   |
      | EMIS   |
      | TPP    |
      | VISION |

  @native
  @pending
  Scenario: Any user can click the help icon on the login page
    Given I am at the login page
    And I see the help icon on the login page
    When I click the help icon on the login page
    Then a new tab has been opened by the link

  Scenario Outline: A <GP System> user that is 13 years old can log in
    Given I attempt to log in as a <GP System> user that is 13
    Then I see the home page
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario: A patient that gets an error due to failing biometric login sees the error page
    Given I am a patient using the native app
    And I am on the login logged-out page
    When I attempt biometric login and fail
    Then I see the login biometric error page is displayed

#502
  @nativesmoketest
  Scenario: CitizenID login is successful but EMIS session cannot be established
    Given I am logged into Citizen ID but EMIS session cannot be established
    Then the Terms and Conditions page is displayed
    When I check the agree to terms and conditions checkbox
    And I click the continue button on Terms and Conditions
    Then the User Research page is displayed
    When I click the 'Yes' radio button
    And I click the 'Continue' button
    Then I see the home page
    And I can't see the Linked profiles link on the homepage
