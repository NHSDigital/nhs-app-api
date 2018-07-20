Feature: Guidance prior to booking an appointment

  Users are given guidance before booking a new appointment.

  Background:
    Given wiremock is initialised

  @NHSO-420
  @appointment
  Scenario: A user with upcoming appointments is presented with guidance
    Given I have upcoming appointments for EMIS
    And I am on my appointments page
    When I select "Book an appointment" button
    Then I am given guidance as to my options before booking an appointment

  @NHSO-420
  @appointment
  Scenario: A user with no upcoming appointments is presented with guidance
    Given I have upcoming appointments for EMIS
    And I am on my appointments page
    When I select "Book an appointment" button
    Then I am given guidance as to my options before booking an appointment

  @NHSO-420
  @appointment
  Scenario: A user proceeds to check their symptoms
    Given I am on the guidance page
    When I select Appointment Guidance Page Check your symptoms button
    Then a new tab opens https://111.nhs.uk/
