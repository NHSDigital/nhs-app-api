@appointments
@my
@noJs
Feature: My appointments UI without Javascript
  Users can view their upcoming and past appointments in the My Appointments screen.

  Background:
    Given I have disabled javascript

  #HAPPY PATH JOURNIES

  Scenario: An EMIS user sees appropriate messages when they have no upcoming or historical appointments
    Given I have no booked appointments for EMIS
    And I am logged in
    When I am on the My Appointments page
    Then the page title is "My appointments"
    And I am informed I have no upcoming appointments
    And I am informed I have no historical appointments
    And I can book an appointment

   #FEATURE JOURNIES

  #    Only testing each Scenario for 1 GP System, for no JS

  @bug  @NHSO-4129
  Scenario: An EMIS user sees Service currently unavailable message when GP system is unavailable
    Given the EMIS GP appointment system is unavailable
    And I am logged in
    When I retrieve the 'My Appointments' page directly
    Then I see page header indicating there is an appointment data error
    And I see the appropriate error messages for the appointment data error


#  This Scenario can be removed once implemented for ALL GP Systems
  Scenario: A VISION user sees appropriate messages when they have no upcoming or historical appointments
    Given I have no booked appointments for VISION
    And I am logged in
    When I retrieve the 'My Appointments' page directly
    Then the page title is "My appointments"
    And I am informed I have no upcoming appointments
    And I am not informed I have no historical appointments
    And I can book an appointment


  Scenario: An EMIS user can see their upcoming appointments and a message if there are no historical appointments
    Given I have upcoming appointments before cutoff time for EMIS
    And I am logged in
    When I retrieve the 'My Appointments' page directly
    Then the page title is "My appointments"
    And I am given the list of upcoming appointments
    And each appointment can be cancelled
    And I am informed I have no historical appointments
    And I can book an appointment

  Scenario: An EMIS user sees their historical appointments and a message if they have no upcoming appointments
    Given I have historical appointments for EMIS
    And I am logged in
    When I retrieve the 'My Appointments' page directly
    Then the page title is "My appointments"
    And I am informed I have no upcoming appointments
    And I am given the list of historical appointments
    And I can book an appointment

  Scenario: An EMIS user sees both their upcoming and historical appointments
    Given I have historical and upcoming appointments for EMIS
    And I am logged in
    When I retrieve the 'My Appointments' page directly
    Then the page title is "My appointments"
    And I am given the list of upcoming appointments
    And each appointment can be cancelled
    And I am given the list of historical appointments
    And I can book an appointment


#  This Scenario can be removed once implemented for ALL GP Systems
  Scenario: A VISION user can see their upcoming appointments
    Given I have upcoming appointments before cutoff time for VISION
    And I am logged in
    When I retrieve the 'My Appointments' page directly
    Then the page title is "My appointments"
    And I am given the list of upcoming appointments
    And each appointment can be cancelled
    And I am not informed I have no historical appointments
    And I can book an appointment

# VISION Specific tests

  Scenario: A user sees appropriate information message when appointments are disabled on VISION
    Given Appointments are disabled for VISION at a GP Practice level
    And I am logged in
    When I retrieve the 'My Appointments' page directly
    Then I see appropriate information message when appointments are disabled
    And there should not be an option to try again

  Scenario: Cancellation link won't be displayed for VISION appointment before cancellation cut off period without cancellation reason(s) available
    Given I have upcoming appointments before cutoff time for VISION without cancellation reasons
    And I am logged in
    When I retrieve the 'My Appointments' page directly
    Then the page title is "My appointments"
    And I am given the list of upcoming appointments
    And no appointment can be cancelled
    And I can book an appointment

  Scenario: Cancellation link won't be displayed for VISION appointment within cancellation cut off period without cancellation reason(s) available
    Given I have upcoming appointments within cutoff time for VISION without cancellation reasons
    And I am logged in
    When I retrieve the 'My Appointments' page directly
    Then the page title is "My appointments"
    And I am given the list of upcoming appointments
    And no appointment can be cancelled
    And I can book an appointment

  Scenario: Cancellation link won't be displayed for VISION appointment within cancellation cut off period with cancellation reason(s) available
    Given I have upcoming appointments within cutoff time for VISION with cancellation reasons
    And I am logged in
    When I retrieve the 'My Appointments' page directly
    Then the page title is "My appointments"
    And I am given the list of upcoming appointments
    And no appointment can be cancelled
    And I can book an appointment

  Scenario: Cancellation link will be displayed for VISION appointment only before cancellation cut off period with cancellation reason(s) available
    Given I have upcoming appointments before and within cutoff time for VISION with cancellation reasons
    And I am logged in
    When I retrieve the 'My Appointments' page directly
    Then the page title is "My appointments"
    And I am given the list of upcoming appointments
    And booked appointments before and one appointment within cutoff time are correctly displayed with relevant ability to cancel
    And I can book an appointment
