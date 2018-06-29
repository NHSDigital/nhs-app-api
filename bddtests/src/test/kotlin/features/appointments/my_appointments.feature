Feature: My appointments

  Users can view their upcoming and past appointments in the My Appointments screen.

  Background:
    Given wiremock is initialised


  @NHSO-524
  @backend
  Scenario: API appropriately filters for upcoming appointments
    Given I have upcoming appointments
    And I have logged in and have a valid session cookie
    When the API retrieves upcoming appointments from "Emis"
    Then I will only receive upcoming appointments
    And a list of cancellation reasons

  @NHSO-524
  Scenario: A user has never booked an appointment
    Given I have no upcoming appointments
    When I am on the appointments page
    Then I am informed I have no booked appointments
    But I can book an appointment

  @NHSO-196
  @NHSO-663
  @bug  @NHSO-1591
  @smoketest
  Scenario: A user has upcoming appointments
    Given I have upcoming appointments
    When I am on the appointments page
    Then the page title is "My appointments"
    And I am given the list of upcoming appointments
    And appointments are in chronological order
    And each appointment can be cancelled
    And I can book an appointment
