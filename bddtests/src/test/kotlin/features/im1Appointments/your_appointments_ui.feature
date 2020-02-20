@appointments
@my
Feature: Your Appointments Frontend
  Users can view their upcoming and past appointments in the Your Appointments screen.

  # These tests navigate directly to the pages where the features are to be tested, to save time.

  Scenario Outline: A <GP System> user sees appropriate messages when they have no upcoming or historical appointments
    Given I have no booked appointments for <GP System>
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    Then the page title is "Your GP appointments"
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
    When I retrieve the 'Your GP Appointments' page directly
    Then the page title is "Your GP appointments"
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
    When I retrieve the 'Your GP Appointments' page directly
    Then the page title is "Your GP appointments"
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
    When I retrieve the 'Your GP Appointments' page directly
    Then the page title is "Your GP appointments"
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
    When I retrieve the 'Your GP Appointments' page directly
    Then the page title is "Your GP appointments"
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
    When I retrieve the 'Your GP Appointments' page directly
    Then the page title is "Your GP appointments"
    And I am given the list of upcoming appointments
    And each appointment can be cancelled
    And I am not informed I have no historical appointments
    And I can book an appointment

  Scenario Outline: A <GP System> user can see the telephone number they will be phoned on for an upcoming telephone appointment
    Given I have upcoming telephone appointments before cutoff time for <GP System>
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    Then the page title is "Your GP appointments"
    And I can see the list of upcoming telephone appointments
    Examples:
      | GP System |
      | EMIS      |
      | MICROTEST |

  Scenario Outline: A <GP System> user can see the telephone number they have been phoned on for a past telephone appointment
    Given I have historical telephone appointments for <GP System>
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    Then the page title is "Your GP appointments"
    And I can see the list of past telephone appointments
    Examples:
      | GP System |
      | EMIS      |
      | MICROTEST |

  #403
  Scenario: VISION user sees appropriate error message when appointments are disabled
    Given VISION user is not allowed to view appointments
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    Then I see appropriate error message when appointments are disabled

  #500
  Scenario: TPP user sees appropriate error message when it returns corrupt data
    Given TPP returns corrupted response for my appointments
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    Then I see appropriate try again error message when there is an error with 'xx'
    When I click the error 'Contact us' link with a url of 'https://www.nhs.uk/contact-us/nhs-app-contact-us'
    Then a new tab has been opened by the link

  Scenario: EMIS user retries to view my appointments after it returns corrupt data
    Given EMIS returns corrupted response once when trying to retrieve my appointments
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    Then I see appropriate try again error message when there is an error with 'xx'
    When I click the 'Try again' button
    Then the page title is "Your GP appointments"
    And I am informed I have no historical appointments

  #502
  @nativesmoketest
  Scenario: MICROTEST user sees appropriate error message when it returns unknown exception viewing appointments
    Given an unknown exception occurs when I want to view my MICROTEST appointments
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    Then I see appropriate try again error message when there is an error with '4m'
    When I click the error 'Back' link
    Then I see the home page

  #504
  Scenario: VISION user opens up contact us after a timeout
    Given VISION will time out when trying to retrieve my appointments
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    Then I see appropriate try again book/cancel error message when there is an error with 'zs'
    When I click the error 'Contact us' link with a url of 'https://www.nhs.uk/contact-us/nhs-app-contact-us'
    Then a new tab has been opened by the link

  Scenario: Cancellation link won't be displayed for VISION appointment before cancellation cut off period without cancellation reason(s) available
    Given I have upcoming appointments before cutoff time for VISION without cancellation reasons
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    Then the page title is "Your GP appointments"
    And I am given the list of upcoming appointments
    And no appointment can be cancelled
    And I can book an appointment

  Scenario: Cancellation link won't be displayed for VISION appointment within cancellation cut off period without cancellation reason(s) available
    Given I have upcoming appointments within cutoff time for VISION without cancellation reasons
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    Then the page title is "Your GP appointments"
    And I am given the list of upcoming appointments
    And no appointment can be cancelled
    And I can book an appointment

  Scenario: Cancellation link won't be displayed for VISION appointment within cancellation cut off period with cancellation reason(s) available
    Given I have upcoming appointments within cutoff time for VISION with cancellation reasons
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    Then the page title is "Your GP appointments"
    And I am given the list of upcoming appointments
    And no appointment can be cancelled
    And I can book an appointment

  Scenario: Cancellation link will be displayed for VISION appointment only before cancellation cut off period with cancellation reason(s) available
    Given I have upcoming appointments before and within cutoff time for VISION with cancellation reasons
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    Then the page title is "Your GP appointments"
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
