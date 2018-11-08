@appointment
Feature: My appointments
  Users can view their upcoming and past appointments in the My Appointments screen.

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
    
  Scenario: A user sees appropriate information message when appointments are disabled on VISION
      # VISION Specific test
    Given Appointments are disabled for VISION at a GP Practice level
    And I am logged in as a VISION user
    When I am on my appointments page
    Then I see appropriate information message when appointments are disabled
    And there should not be an option to try again

  Scenario: It is made clear to a Vision user when they cannot cancel appointments they have already booked
    Given I have upcoming appointments for VISION, but without cancellation reasons
    And I am logged in as a VISION user
    When I am on my appointments page
    Then the page title is "My appointments"
    And I am given the list of upcoming appointments
    And appointments are in chronological order
    And no appointment can be cancelled
    And I can book an appointment

  @long-running
  @nativepending @NHSO-2966
  Scenario: On session expiry (when on my appointments page), a user on a secure screen is automatically signed out
    Given I have no upcoming appointments for EMIS
    And I am logged in as a EMIS user
    And I am on my appointments page
    When I am idle long enough for the session to expire
    Then I see the login page with the session expiry notification
    And the user login details are cleared from cookies

  @manual
  Scenario: Requesting list of appointments, when there is no internet connection should result with a message indicating user may have connectivity problems
    Given I have no upcoming appointments for EMIS
    And I am logged in
    And I lose my internet connection
    When I am on my appointments page
    Then I see a message indicating user may have connectivity problems