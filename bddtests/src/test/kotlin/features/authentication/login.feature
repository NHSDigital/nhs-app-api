Feature: Login

  Logging into the service is handled via the CitizenID service.  A user will be shown personalised welcome messages upon successful login.

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

  @NHSO-125
  @smoketest
  Scenario Outline: <GPSS> User sees the home page
    Given I am logged in as a <GPSS> user
    Then I see a welcome message
    And I see the header
    And I see the navigation menu

    Examples:
      | GPSS   |
      | EMIS   |
      | TPP    |
      | VISION |

  @pending
  Scenario: User has been inactive for less than 20 minutes

  Scenario: The spinner is shown while loading
    Given I am not logged in
    And sign in verification is slow
    When I log in
    Then the spinner appears
