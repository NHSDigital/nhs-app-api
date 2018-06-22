Feature: Guidance prior to booking an appointment

  Users are given guidance before booking a new appointment.

  @NHSO-420
  @pending  @NHSO-420
  Scenario: A user with upcoming appointments is presented with guidance
    Given I have upcoming appointments
    And I am on the appointments page
    When I select "Book an appointment" button
    Then I am given guidance as to my options before booking an appointment

  @NHSO-420
  @pending  @NHSO-420
  Scenario: A user with no upcoming appointments is presented with guidance
    Given I have no upcoming appointments
    And I am on the appointments page
    When I select "Book an appointment" button
    Then I am given guidance as to my options before booking an appointment

  @NHSO-420
  @pending  @NHSO-420
  Scenario: A user proceeds to check their symptoms
    Given I am on the guidance page
    When I select "Check your symptoms" button
    Then I will be presented with the "Check your symptoms" page
