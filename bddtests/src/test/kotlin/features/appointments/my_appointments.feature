@appointment
Feature: My appointments
  Users can view their upcoming and past appointments in the My Appointments screen.

  @NHSO-797
  @backend
  Scenario Outline: API call for upcoming appointments will only request upcoming appointments from <GP System>
    Given I have upcoming appointments for <GP System>
    And I have logged into <GP System> and have a valid session cookie
    When the upcoming appointments are requested
    Then I will only receive upcoming appointments
    And a list of cancellation reasons if the GP Service provides the list
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  @backend
  Scenario Outline: API call for upcoming appointments won't filter out past appointments from <GP System>
    Given I have upcoming appointments for <GP System>, with one in the past
    And I have logged into <GP System> and have a valid session cookie
    When the upcoming appointments are requested
    Then I will receive upcoming appointments with appointments in the past
    Examples:
      | GP System |
      | EMIS      |
      | VISION    |

  @backend
  Scenario: API call for upcoming appointments will filter out past appointments from TPP
    Given I have upcoming appointments for TPP, with one in the past
    And I have logged into TPP and have a valid session cookie
    When the upcoming appointments are requested
    Then I will only receive upcoming appointments

  @backend
  Scenario Outline: Online appointment booking is not available to a particular patient
    Given I have logged into <GP System> and have a valid session cookie
    But <GP System> does not offer online booking to my patient
    When the upcoming appointments are requested
    Then I receive a "Forbidden" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  @backend
  @NHSO-802
  Scenario Outline: Appropriate error response, when <GP System> returns corrupted data
    Given I have logged into <GP System> and have a valid session cookie
    But <GP System> returns corrupted response for my appointments
    When the upcoming appointments are requested
    Then I receive a "Internal Server Error" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  @backend
  @NHSO-802
  Scenario Outline: Requesting my appointments the <GP System> times out and returns "Gateway Timeout" error
    Given I have logged into <GP System> and have a valid session cookie
    But <GP System> will time out when trying to retrieve my appointments
    When the upcoming appointments are requested
    Then I receive a "Gateway Timeout" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  @NHSO-2910
  Scenario Outline: A <GP System> user sees Service currently unavailable message when GP system is unavailable
    Given the <GP System> GP appointment system is unavailable
    And I am logged in as a <GP System> user
    When I am on my appointments page
    Then I see page header indicating there is an appointment data error
    And I see the appropriate error messages for the appointment data error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  @NHSO-797
  Scenario Outline: A user has never booked an appointment
    Given I have no upcoming appointments for <GP System>
    And I am logged in as a <GP System> user
    When I am on my appointments page
    Then I am informed I have no booked appointments
    But I can book an appointment

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  @smoketest
  @NHSO-797
  Scenario Outline: A <GP System> user can see their upcoming appointments
    Given I have upcoming appointments for <GP System>
    And I am logged in as a <GP System> user
    When I am on my appointments page
    Then the page title is "My appointments"
    And I am given the list of upcoming appointments
    And appointments are in chronological order
    And each appointment can be cancelled
    And I can book an appointment

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  @long-running
  @NHSO-802
  Scenario: On session expiry (when on my appointments page), a user on a secure screen is automatically signed out
    Given I have no upcoming appointments for EMIS
    And I am logged in as a EMIS user
    And I am on my appointments page
    When I am idle long enough for the session to expire
    Then I see the login page with the session expiry notification
    And the user login details are cleared from cookies

  @manual
  @NHSO-802
  Scenario: Requesting list of appointments, when there is no internet connection should result with a message indicating user may have connectivity problems
    Given I have no upcoming appointments for EMIS
    And I am logged in
    And I lose my internet connection
    When I am on my appointments page
    Then I see a message indicating user may have connectivity problems