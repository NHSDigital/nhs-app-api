@authentication
@authentication-login
Feature: Login
  Logging into the service is handled via the CitizenID service.
  A user will be shown personalised welcome messages upon successful login.

  Scenario Outline: A <GP System> user sees the home page after logging in
    Given I have no booked appointments for <GP System>
    And I have no repeat prescriptions
    And I am logged in
    Then I see a welcome message
    And I see the patient details of name, date of birth and NHS number
    And I see the home page header
    And I see the navigation menu
    And I see and can follow links within the home page body
    Examples:
      | GP System |
      | TPP       |
      | VISION    |
  @android
  @smoketest
    Examples:
      | GP System |
      | EMIS      |

  #Once prescriptions are completed for microtest, this test can be merged with above
  Scenario Outline: A <GP System> user sees the home page after logging in
    Given I am a <GP System> patient
    And I am logged in
    Then I see a welcome message
    And I see the patient details of name, date of birth and NHS number
    And I see the home page header
    Examples:
      | GP System |
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


  @manual # covered in Manual Regression Test pack
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
    Given I see the help icon on the login page
    When I click the help icon on the login page
    Then a new tab has been opened by the link

  Scenario Outline: A <GP System> user that is 13 years old can log in
    Given I attempt to log in as a <GP System> user that is 13
    Then I see the home page
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
