@authentication
@authentication-login
Feature: Login
  Logging into the service is handled via the CitizenID service.
  A user will be shown personalised welcome messages upon successful login.


  @smoketest
  @nativepending @NHSO-2948
    #fails on IOS only
  Scenario Outline: A <GP System> user sees the home page after logging in
    Given I have no upcoming appointments for <GP System>
    And I have no repeat prescriptions for <GP System>
    And I am logged in as a <GP System> user
    Then I see a welcome message
    And I see the patient details of name, date of birth and NHS number
    And I see the home page header
    And I see the navigation menu
    And I see and can follow links within the home page body
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
  @pending @NHSO-2522
    Examples:
      | GP System |
      | VISION    |


  Scenario Outline: A <GP System> user sees a beta banner on the home page
    Given I am logged in as a <GP System> user
    Then I see the home page
    And I see the beta banner
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

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
      | EMIS      |
      | TPP       |


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

  Scenario: Any user can click the help icon on the login page
    Given I am at the login page
    Given I see the help icon on the login page
    When I click the help icon on the login page
    Then a new tab opens https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/help-and-support/
