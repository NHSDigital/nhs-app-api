@appointments
@my
Feature: Your Appointments Frontend
  Users can view their upcoming and past appointments in the Your Appointments screen.

  #This test covers navigation via buttons/links

  #502
  Scenario Outline: A <GP System> user sees Service currently unavailable message when GP system is unavailable
    Given the <GP System> GP appointment system is unavailable
    And I am logged in
    When I am on the Your Appointments error page
    Then I see page header indicating there is an appointment data error
    And I see the appropriate error messages for the appointment data error
    Examples:
      | GP System |
      | TPP       |
      | VISION    |
      | MICROTEST |

  @nativesmoketest
    Examples:
      | GP System |
      | EMIS      |

  # These tests navigate directly to the pages where the features are to be tested, to save time.

  Scenario Outline: A <GP System> user sees appropriate messages when they have no upcoming or historical appointments
    Given I have no booked appointments for <GP System>
    And I am logged in
    When I retrieve the 'Your Appointments' page directly
    Then the page title is "Your appointments"
    And I am informed I have no upcoming appointments
    And I am informed I have no historical appointments
    And I can book an appointment
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | MICROTEST |

  Scenario Outline: A <GP System> user sees appropriate messages when they have no upcoming appointments
    Given I have no booked appointments for <GP System>
    And I am logged in
    When I retrieve the 'Your Appointments' page directly
    Then the page title is "Your appointments"
    And I am informed I have no upcoming appointments
    And I am not informed I have no historical appointments
    And I can book an appointment
    Examples:
      | GP System |
      | VISION    |

  Scenario Outline: A <GP System> user can see their upcoming appointments and a message if there are no historical
  appointments
    Given I have upcoming appointments before cutoff time for <GP System>
    And I am logged in
    When I retrieve the 'Your Appointments' page directly
    Then the page title is "Your appointments"
    And I am given the list of upcoming appointments
    And each appointment can be cancelled
    And I am informed I have no historical appointments
    And I can book an appointment
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | MICROTEST |

  Scenario Outline: A <GP System> user sees their historical appointments and a message if they have no upcoming
  appointments
    Given I have historical appointments for <GP System>
    And I am logged in
    When I retrieve the 'Your Appointments' page directly
    Then the page title is "Your appointments"
    And I am informed I have no upcoming appointments
    And I am given the list of historical appointments
    And I can book an appointment
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | MICROTEST |

  Scenario Outline: A <GP System> user sees both their upcoming and historical appointments
    Given I have historical and upcoming appointments for <GP System>
    And I am logged in
    When I retrieve the 'Your Appointments' page directly
    Then the page title is "Your appointments"
    And I am given the list of upcoming appointments
    And each appointment can be cancelled
    And I am given the list of historical appointments
    And I can book an appointment
    @nativesmoketest
    @smoketest
    Examples:
      | GP System |
      | EMIS      |
    Examples:
      | GP System |
      | TPP       |
      | MICROTEST |

  @smoketest
  Scenario: A VISION user can see their upcoming appointments
    Given I have upcoming appointments before cutoff time for VISION
    And I am logged in
    When I retrieve the 'Your Appointments' page directly
    Then the page title is "Your appointments"
    And I am given the list of upcoming appointments
    And each appointment can be cancelled
    And I am not informed I have no historical appointments
    And I can book an appointment

  Scenario Outline: A <GP System> user can see the telephone number they will be phoned on for an upcoming telephone appointment
    Given I have upcoming telephone appointments before cutoff time for <GP System>
    And I am logged in
    When I retrieve the 'Your Appointments' page directly
    Then the page title is "Your appointments"
    And I can see the list of upcoming telephone appointments
    Examples:
      | GP System |
      | EMIS      |
      | MICROTEST |

  Scenario Outline: A <GP System> user can see the telephone number they have been phoned on for a past telephone appointment
    Given I have historical telephone appointments for <GP System>
    And I am logged in
    When I retrieve the 'Your Appointments' page directly
    Then the page title is "Your appointments"
    And I can see the list of past telephone appointments
    Examples:
      | GP System |
      | EMIS      |
      | MICROTEST |

    #403
  Scenario: A user sees appropriate information message when appointments are disabled on VISION
      # VISION Specific test
    Given Appointments are disabled for VISION at a GP Practice level
    And I am logged in
    When I retrieve the 'Your Appointments' page directly
    Then I see appropriate information message when appointments are disabled
    And there should not be an option to try again

  Scenario: Cancellation link won't be displayed for VISION appointment before cancellation cut off period without cancellation reason(s) available
    Given I have upcoming appointments before cutoff time for VISION without cancellation reasons
    And I am logged in
    When I retrieve the 'Your Appointments' page directly
    Then the page title is "Your appointments"
    And I am given the list of upcoming appointments
    And no appointment can be cancelled
    And I can book an appointment

  Scenario: Cancellation link won't be displayed for VISION appointment within cancellation cut off period without cancellation reason(s) available
    Given I have upcoming appointments within cutoff time for VISION without cancellation reasons
    And I am logged in
    When I retrieve the 'Your Appointments' page directly
    Then the page title is "Your appointments"
    And I am given the list of upcoming appointments
    And no appointment can be cancelled
    And I can book an appointment

  Scenario: Cancellation link won't be displayed for VISION appointment within cancellation cut off period with cancellation reason(s) available
    Given I have upcoming appointments within cutoff time for VISION with cancellation reasons
    And I am logged in
    When I retrieve the 'Your Appointments' page directly
    Then the page title is "Your appointments"
    And I am given the list of upcoming appointments
    And no appointment can be cancelled
    And I can book an appointment

  Scenario: Cancellation link will be displayed for VISION appointment only before cancellation cut off period with cancellation reason(s) available
    Given I have upcoming appointments before and within cutoff time for VISION with cancellation reasons
    And I am logged in
    When I retrieve the 'Your Appointments' page directly
    Then the page title is "Your appointments"
    And I am given the list of upcoming appointments
    And booked appointments before and one appointment within cutoff time are correctly displayed with relevant ability to cancel
    And I can book an appointment

     # covered in Manual Regression Test pack
  @tech-debt   @NHSO-4061
  Scenario: Requesting list of appointments, when there is no internet connection should result with a message indicating user may have connectivity problems
    Given I have no booked appointments for EMIS
    And I am logged in
    And I lose my internet connection
    When I am on the Your Appointments page
    Then I see a message indicating user may have connectivity problems