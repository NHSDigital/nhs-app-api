@appointment
Feature: My appointments
  Users can view their upcoming and past appointments in the My Appointments screen.

  Scenario Outline: A <GP System> user sees Service currently unavailable message when GP system is unavailable
    Given the <GP System> GP appointment system is unavailable
    And I am logged in
    When I am on the My Appointments page
    Then I see page header indicating there is an appointment data error
    And I see the appropriate error messages for the appointment data error
    Examples:
      | GP System |
      | TPP       |
      | VISION    |

  @native-smoketest
    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: A user has never booked an appointment
    Given I have no upcoming appointments for <GP System>
    And I am logged in
    When I am on the My Appointments page
    Then I am informed I have no booked appointments
    But I can book an appointment

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  @smoketest
  Scenario Outline: A <GP System> user can see their upcoming appointments
    Given I have upcoming appointments before cutoff time for <GP System>
    And I am logged in
    When I am on the My Appointments page
    Then the page title is "My appointments"
    And I am given the list of upcoming appointments
    And appointments are in chronological order
    And each appointment can be cancelled
    And I can book an appointment

    Examples:
      | GP System |
      | TPP       |
      | VISION    |

  @native-smoketest
    Examples:
      | GP System |
      | EMIS      |


  Scenario: A user sees appropriate information message when appointments are disabled on VISION
      # VISION Specific test
    Given Appointments are disabled for VISION at a GP Practice level
    And I am logged in
    When I am on the My Appointments page
    Then I see appropriate information message when appointments are disabled
    And there should not be an option to try again

  Scenario: Cancellation link won't be displayed for VISION appointment before cancellation cut off period without cancellation reason(s) available
    Given I have upcoming appointments before cutoff time for VISION without cancellation reasons
    And I am logged in
    When I am on the My Appointments page
    Then the page title is "My appointments"
    And I am given the list of upcoming appointments
    And appointments are in chronological order
    And no appointment can be cancelled
    And I can book an appointment

  Scenario: Cancellation link won't be displayed for VISION appointment within cancellation cut off period without cancellation reason(s) available
    Given I have upcoming appointments within cutoff time for VISION without cancellation reasons
    And I am logged in
    When I am on the My Appointments page
    Then the page title is "My appointments"
    And I am given the list of upcoming appointments
    And appointments are in chronological order
    And no appointment can be cancelled
    And I can book an appointment

  Scenario: Cancellation link won't be displayed for VISION appointment within cancellation cut off period with cancellation reason(s) available
    Given I have upcoming appointments within cutoff time for VISION with cancellation reasons
    And I am logged in
    When I am on the My Appointments page
    Then the page title is "My appointments"
    And I am given the list of upcoming appointments
    And appointments are in chronological order
    And no appointment can be cancelled
    And I can book an appointment

  Scenario: Cancellation link will be displayed for VISION appointment only before cancellation cut off period with cancellation reason(s) available
    Given I have upcoming appointments before and within cutoff time for VISION with cancellation reasons
    And I am logged in
    When I am on the My Appointments page
    Then the page title is "My appointments"
    And I am given the list of upcoming appointments
    And appointments are in chronological order
    And booked appointments before and one appointment within cutoff time are correctly displayed with relevant ability to cancel
    And I can book an appointment

  @tech-debt   @NHSO-4061 # covered in Manual Regression Test pack
  Scenario: Requesting list of appointments, when there is no internet connection should result with a message indicating user may have connectivity problems
    Given I have no upcoming appointments for EMIS
    And I am logged in
    And I lose my internet connection
    When I am on the My Appointments page
    Then I see a message indicating user may have connectivity problems
