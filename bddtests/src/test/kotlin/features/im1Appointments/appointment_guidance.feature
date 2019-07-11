@appointments
@guidance
Feature: Guidance prior to booking an appointment
  Users are given guidance before booking a new appointment.

  Scenario: A user with upcoming appointments is presented with guidance
    Given I have upcoming appointments before cutoff time for EMIS
    And I am logged in
    And I am on the My Appointments page
    When I select "Book new appointment" button
    Then I am given guidance as to my options before booking an appointment

  Scenario: A user with no upcoming appointments is presented with guidance
    Given I have no booked appointments for EMIS
    And I am logged in
    And I am on the My Appointments page
    When I select "Book new appointment" button
    Then I am given guidance as to my options before booking an appointment
