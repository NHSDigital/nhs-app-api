Feature: Login
  Logging into the service is handled via the CitizenID service.
  A user will be shown personalised welcome messages upon successful login.


  @NHSO-125
  @smoketest
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


  @NHSO-125
  Scenario Outline: A <GP System> user sees a beta banner and a survey link on the home page
    Given I am logged in as a <GP System> user
    Then I see the home page
    And I see the beta banner
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
