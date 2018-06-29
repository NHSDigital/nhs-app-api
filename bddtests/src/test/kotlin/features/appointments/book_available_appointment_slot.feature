Feature: Book an available appointment slot
  In order to complete a booking appointment
  As a logged in user
  I want to be able to select, confirm and book selected appointment

  Background:
    Given there are available appointment slots

  @NHSO-72
  Scenario: A user tries to book an appointment without describing symptoms
    Given I am on the appointments booking page
    And I have selected an appointment slot to book
    When I click the 'Confirm and book appointment' button
    Then an error is displayed that "Describe your symptoms" is mandatory

  @NHSO-72
  @smoketest
  Scenario: A user tries to book an appointment describing symptoms at least 1 character
    Given I am on the appointments booking page
    And I have selected an appointment slot to book
    And I enter symptoms of 1 characters
    When I click the 'Confirm and book appointment' button
    Then Appointment Booking confirmation screen is displayed
    And booking request is successfully made with valid details

  @NHSO-72
  Scenario: A user tries to book an appointment describing symptoms no more 150 characters
    Given I am on the appointments booking page
    And I have selected an appointment slot to book
    And I enter symptoms of 150 characters
    When I click the 'Confirm and book appointment' button
    Then Appointment Booking confirmation screen is displayed
    And booking request is successfully made with valid details

  @NHSO-72
  Scenario: A user tries to enter symptoms with over 150 characters
    Given I am on the appointments booking page
    And I have selected an appointment slot to book
    When I enter symptoms of 151 characters
    Then only the first 150 characters will be displayed

  @NHSO-72
  @smoketest
  Scenario: A user tries to paste symptoms with over 150 characters
    Given I am on the appointments booking page
    And I have selected an appointment slot to book
    When I paste symptoms of 151 characters
    Then only the first 150 characters will be displayed

  @NHSO-72
  Scenario: A user who books successfully, but only the first 150 characters of the symptoms are sent
    Given I am on the appointments booking page
    And I have selected an appointment slot to book
    And I enter symptoms of 151 characters
    When I click the 'Confirm and book appointment' button
    Then Appointment Booking confirmation screen is displayed
    And booking request is successfully made with valid details

  @NHSO-517
  Scenario: A user sees appropriate information message when there is a timeout
    Given GP system doesn't respond a timely fashion when booking an appointment
    And I am on the appointments booking page
    And I have selected an appointment slot to book
    When I enter symptoms
    And  I click the 'Confirm and book appointment' button
    Then I see appropriate information message after 10 seconds when it times-out on appointment confirmation page
    And there should be a button to go back to my appointments

  @NHSO-517
  Scenario: A user sees appropriate information message when GP system is unavailable
    Given GP system is unavailable when booking an appointment
    And I am on the appointments booking page
    And I have selected an appointment slot to book
    When I enter symptoms
    And  I click the 'Confirm and book appointment' button
    Then I see appropriate information message when there is an error sending data on appointment confirmation page
    And there should be a button to go back to my appointments

  @NHSO-517
  Scenario: A user sees appropriate information message when appointment has already been booked
    Given the appointment slot has already been booked by somebody else
    And I am on the appointments booking page
    And I have selected an appointment slot to book
    When I enter symptoms
    And  I click the 'Confirm and book appointment' button
    Then I see appropriate information message when appointment has already been booked
    And there should be a button to go back to my appointments

  @NHSO-517
  Scenario: A user can return directly back to their appointments after trying to book one already booked
    Given the appointment slot has already been booked by somebody else
    And I am on the appointments booking page
    And I have selected an appointment slot to book
    And I enter symptoms
    And  I click the 'Confirm and book appointment' button
    When I click the button to go back to my appointments
    Then I will be on the My appointments screen

  @NHSO-528
  @pending  @NHSO-71
  @smoketest
  Scenario: A user is navigated back to the 'Book this appointment' screen when 'Change this appointment' button selected.
    Given I am logged in
    And there are available appointment slots
    When I click appointments button in menu
    And I select appointment slot and click Book this appointment button
    Then I see Book this appointment screen when Change this appointment button is clicked