@pending  @NHSO-524
Feature: View upcoming appointments

  Users can view their upcoming appointments in the My Appointments screen.

  @NHSO-524
  @backend
  @pending  @NHSO-524
  Scenario: API appropriately filters for upcoming appointments
    Given I have upcoming appointments
    When the API retrieves upcoming appointments from "Emis"
    Then I will only receive upcoming appointments
    And a list of cancellation reasons

  @NHSO-524
  @pending  @NHSO-524
  Scenario: A user has never booked an appointment
    Given I have no upcoming appointments
    When I am on the appointments page
    Then I am informed I have no booked appointments
    But I can book an appointment

  @NHSO-524
  @pending  @NHSO-524
  Scenario: A user proceeds to book an appointment
    Given I have no upcoming appointments
    And I am on the appointments page
    When I select "Book new appointment"
    Then I am taken to the available appointment slots screen

  @NHSO-196
  @pending  @NHSO-196
  Scenario: A user has upcoming appointments
    Given I have upcoming appointments
    When I am on the appointments page
    Then I am given the list of upcoming appointments in chronological order
    And I can book an appointment