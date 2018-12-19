@appointment
Feature: Guidance prior to booking an appointment
  Users are given guidance before booking a new appointment.

  @smoketest
  @nativebug @NHSO-2956 @NHSO-3053
  Scenario: A user is presented with guidance when booking an appointment and can proceed to check their symptoms
    Given I have upcoming appointments before cutoff time for EMIS
    And I am logged in as a EMIS user
    And I am on the Appointment Guidance page
    When I select Appointment Guidance Page Check symptoms button
    Then the Check My Symptoms page is displayed
    And the Check My Symptoms page header and navigation menu are correct

  Scenario: A user with upcoming appointments is presented with guidance
    Given I have upcoming appointments before cutoff time for EMIS
    And I am logged in as a EMIS user
    And I am on the My Appointments page
    When I select "Book new appointment" button
    Then I am given guidance as to my options before booking an appointment

  Scenario: A user with no upcoming appointments is presented with guidance
    Given I have no upcoming appointments for EMIS
    And I am logged in as a EMIS user
    And I am on the My Appointments page
    When I select "Book new appointment" button
    Then I am given guidance as to my options before booking an appointment
