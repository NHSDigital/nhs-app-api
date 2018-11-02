@appointment
Feature: Guidance prior to booking an appointment
  Users are given guidance before booking a new appointment.

  @smoketest
  @nativebug @NHSO-2956 @NHSO-3053
  Scenario: A user is presented with guidance when booking an appointment and can proceed to check their symptoms
    Given I have upcoming appointments for EMIS
    And I am logged in as a EMIS user
    And I am on the guidance page
    When I select Appointment Guidance Page Check symptoms button
    Then Check My symptoms page is displayed
    And Check My symptoms page header and navigation menu are correct

  Scenario: A user with upcoming appointments is presented with guidance
    Given I have upcoming appointments for EMIS
    And I am logged in as a EMIS user
    And I am on my appointments page
    When I select "Book new appointment" button
    Then I am given guidance as to my options before booking an appointment

  Scenario: A user with no upcoming appointments is presented with guidance
    Given I have no upcoming appointments for EMIS
    And I am logged in as a EMIS user
    And I am on my appointments page
    When I select "Book new appointment" button
    Then I am given guidance as to my options before booking an appointment
