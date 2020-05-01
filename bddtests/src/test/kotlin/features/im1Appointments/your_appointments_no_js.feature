@appointments
@my
@noJs
Feature: Your Appointments With Javascript Disabled
  Users can view their upcoming and past appointments in the Your Appointments screen.

  #This test covers navigation via buttons/links

  Scenario: An EMIS user sees appropriate messages when they have no upcoming or historical appointments
    Given I have disabled javascript
    And I have no booked appointments for EMIS
    And I am logged in
    When I am on the Your Appointments page
    Then the page title is "Your GP appointments"
    And I am informed I have no upcoming appointments
    And I am informed I have no historical appointments
    And I can book an appointment

 # These tests navigate directly to the pages where the features are to be tested, to save time.

  #    Only testing each Scenario for 1 GP System, for no JS

  @bug  @NHSO-4129
  Scenario Outline: An EMIS user sees Service currently unavailable message when GP system is unavailable
    Given I have disabled javascript
    And the EMIS GP appointment system is unavailable
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    Then I see appropriate try again error message when there is an error with '<Prefix>'
    Examples:
      | Prefix |
      | 4e     |

#  This Scenario can be removed once implemented for ALL GP Systems
  Scenario: A VISION user sees appropriate messages when they have no upcoming or historical appointments
    Given I have disabled javascript
    And I have no booked appointments for VISION
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    Then the page title is "Your GP appointments"
    And I am informed I have no upcoming appointments
    And I am not informed I have no historical appointments
    And I can book an appointment

  Scenario: An EMIS user can see their upcoming appointments and a message if there are no historical appointments
    Given I have disabled javascript
    And I have upcoming appointments before cutoff time for EMIS
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    Then the page title is "Your GP appointments"
    And I am given the list of upcoming appointments
    And each appointment can be cancelled
    And I am informed I have no historical appointments
    And I can book an appointment

  Scenario: An EMIS user sees their historical appointments and a message if they have no upcoming appointments
    Given I have disabled javascript
    And I have historical appointments for EMIS
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    Then the page title is "Your GP appointments"
    And I am informed I have no upcoming appointments
    And I am given the list of historical appointments
    And I can book an appointment

  Scenario: An EMIS user sees both their upcoming and historical appointments
    Given I have disabled javascript
    And I have historical and upcoming appointments for EMIS
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    Then the page title is "Your GP appointments"
    And I am given the list of upcoming appointments
    And each appointment can be cancelled
    And I am given the list of historical appointments
    And I can book an appointment

  Scenario: An EMIS user can see the telephone number they will be phoned on for an upcoming telephone appointment
    Given I have disabled javascript
    And I have upcoming telephone appointments before cutoff time for EMIS
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    Then the page title is "Your GP appointments"
    And I can see the list of upcoming telephone appointments

  Scenario: An EMIS user can see the telephone number they have been phoned on for a past telephone appointment
    Given I have historical telephone appointments for EMIS
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    Then the page title is "Your GP appointments"
    And I can see the list of past telephone appointments

#  This Scenario can be removed once implemented for ALL GP Systems
  Scenario: A VISION user can see their upcoming appointments
    Given I have disabled javascript
    And I have upcoming appointments before cutoff time for VISION
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    Then the page title is "Your GP appointments"
    And I am given the list of upcoming appointments
    And each appointment can be cancelled
    And I am not informed I have no historical appointments
    And I can book an appointment

# VISION Specific tests

  Scenario: Cancellation link won't be displayed for VISION appointment before cancellation cut off period without cancellation reason(s) available
    Given I have disabled javascript
    And I have upcoming appointments before cutoff time for VISION without cancellation reasons
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    Then the page title is "Your GP appointments"
    And I am given the list of upcoming appointments
    And no appointment can be cancelled
    And I can book an appointment

  Scenario: Cancellation link won't be displayed for VISION appointment within cancellation cut off period without cancellation reason(s) available
    Given I have disabled javascript
    And I have upcoming appointments within cutoff time for VISION without cancellation reasons
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    Then the page title is "Your GP appointments"
    And I am given the list of upcoming appointments
    And no appointment can be cancelled
    And I can book an appointment

  Scenario: Cancellation link won't be displayed for VISION appointment within cancellation cut off period with cancellation reason(s) available
    Given I have disabled javascript
    And I have upcoming appointments within cutoff time for VISION with cancellation reasons
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    Then the page title is "Your GP appointments"
    And I am given the list of upcoming appointments
    And no appointment can be cancelled
    And I can book an appointment

  Scenario: Cancellation link will be displayed for VISION appointment only before cancellation cut off period with cancellation reason(s) available
    Given I have disabled javascript
    And I have upcoming appointments before and within cutoff time for VISION with cancellation reasons
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    Then the page title is "Your GP appointments"
    And I am given the list of upcoming appointments
    And booked appointments before and one appointment within cutoff time are correctly displayed with relevant ability to cancel
    And I can book an appointment
