@appointment
@noJs
Feature: Book an available appointment slot UI without Javascript
  In order to complete a booking appointment
  As a logged in user with Javascript switched off
  I want to be able to select, confirm and book selected appointment

  Background:
    Given I have disabled javascript

# EMIS Specific Scenarios (For EMIS telephone appointment)
# The following scenarios covered only telephone appointment options.
# The telephone number is mandatory if telephone appointment slot selected.

      # Positive submission cases

  Scenario: An EMIS user without Javascript cannot enter or select a phone number for non phone appointments
    Given there are EMIS appointments available to book
    And I am logged in
    And I am on the Available Appointments page
    And I have filtered such that there is one time displayed that represents multiple slots
    And I click the 'Find available appointments' button
    When I have selected a time when multiple slots are available
    Then the Appointment Slot page is displayed
    And I do not see a text input to enter phone number
    And I do not see any phone numbers to select

  Scenario Outline: An EMIS user with noJs can only enter a phone number for phone appointments, when they have <User's Telephone Numbers> phone number saved
    Given I have <User's Telephone Numbers> telephone number(s) stored
    And there are appointments available to book which are of telephone type
    And I am logged in
    And I am on the Available Appointments page
    And I have filtered such that there is one time displayed that represents multiple slots
    And I click the 'Find available appointments' button
    And I have selected a time when multiple slots are available
    Then the Appointment Slot page is displayed
    And I do not see any phone numbers to select
    And I see a text input to enter phone number
    When I enter a phone number for the appointment
    And I enter symptoms
    And I click the 'Confirm and book appointment' button
    Then the Appointment Booking success message is displayed
    And the booked appointment before cutoff time is correctly displayed with ability to cancel
    Examples:
      | User's Telephone Numbers |
      | no                       |
      | only a home              |
      | only a mobile            |
      | both home and mobile     |


      # Telephone number error scenarios

  Scenario: An EMIS user with noJs receives an error if a phone number is not provided for telephone appointments
    Given I have no telephone number(s) stored
    And there are appointments available to book which are of telephone type
    And I am logged in
    And I am on the Available Appointments page
    And I have filtered such that there is one time displayed that represents multiple slots
    And I click the 'Find available appointments' button
    And I have selected a time when multiple slots are available
    And the Appointment Slot page is displayed
    When I enter symptoms
    And I click the 'Confirm and book appointment' button
    Then the focus will go back to empty phone number input box

  Scenario: An EMIS user with noJs receives errors if a phone number and booking reason are not provided for telephone appointments
    Given I have no telephone number(s) stored
    And there are appointments available to book which are of telephone type
    And I am logged in
    And I am on the Available Appointments page
    And I have filtered such that there is one time displayed that represents multiple slots
    And I click the 'Find available appointments' button
    And I have selected a time when multiple slots are available
    And the Appointment Slot page is displayed
    When I click the 'Confirm and book appointment' button
    Then the focus will go back to empty phone number input box

  Scenario: An EMIS user with noJs receives an error if a phone number is not provided for telephone appointments with optional booking reason
    Given I have no telephone number(s) stored
    And there are appointments available to book which are of telephone type with optional booking reason
    And I am logged in
    And I am on the Available Appointments page
    And I have filtered such that there is one time displayed that represents multiple slots
    And I click the 'Find available appointments' button
    And I have selected a time when multiple slots are available
    And the Appointment Slot page is displayed
    When I click the 'Confirm and book appointment' button
    Then the focus will go back to empty phone number input box

  Scenario: An EMIS user with noJs receives an error if an empty string or whitespace provided for telephone appointments
    Given I have no telephone number(s) stored
    And there are appointments available to book which are of telephone type
    And I am logged in
    And I am on the Available Appointments page
    And I have filtered such that there is one time displayed that represents multiple slots
    And I click the 'Find available appointments' button
    And I have selected a time when multiple slots are available
    Then the Appointment Slot page is displayed
    And I see a text input to enter phone number
    When I enter whitespace instead of a phone number for the appointment
    And I enter symptoms
    And I click the 'Confirm and book appointment' button
    Then the focus will go back to empty phone number input box


    # Regression Test scenarios to ensure booking reason is still validated for telephone appointments

  Scenario: An EMIS user with noJs receives an error if a phone number is entered for telephone appointments, but no reason
    Given I wish to book an appointment without specifying a reason
    And I have no telephone number(s) stored
    And there are appointments available to book which are of telephone type
    And I am logged in
    And I am on the Available Appointments page
    And I have filtered such that there is one time displayed that represents multiple slots
    And I click the 'Find available appointments' button
    And I have selected a time when multiple slots are available
    And I enter a phone number for the appointment
    When I click the 'Confirm and book appointment' button
    Then the focus will go back to empty booking reason input box

  @bug  @NHSO-3816
  Scenario: An appointment is booked for an EMIS user with noJS if a phone number is provided for telephone appointments with optional booking reason
    Given I have no telephone number(s) stored
    And there are appointments available to book which are of telephone type with optional booking reason
    And I am logged in
    And I am on the Available Appointments page
    And I have filtered such that there is one time displayed that represents multiple slots
    And I click the 'Find available appointments' button
    And I have selected a time when multiple slots are available
    And I enter a phone number for the appointment
    When I click the 'Confirm and book appointment' button
    Then the Appointment Booking success message is displayed
    And the booked appointment before cutoff time is correctly displayed with ability to cancel


# Miscellaneous no-JS scenarios

  Scenario Outline: An <GP System> user can book appointments with javascript disabled
    Given there are multiple appointment slots at the same time, provided by <GP System>
    And a booked appointment can be cancelled
    And I am logged in
    And I am on the Available Appointments page
    And I have filtered such that there is one time displayed that represents multiple slots
    And I click the 'Find available appointments' button
    And I have selected a time when multiple slots are available
    Then the Appointment Slot page is displayed
    When I enter symptoms
    And  I click the 'Confirm and book appointment' button
    Then the Appointment Booking success message is displayed
    And the booked appointment before cutoff time is correctly displayed with ability to cancel
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |


  @bug  @NHSO-3816
  Scenario: An appointment is booked for an EMIS user with noJS with no optional booking reason
    Given I wish to book an appointment without specifying a reason
    And there are EMIS appointments available to book where booking reason is set optional
    And I am logged in
    And I am on the Available Appointments page
    And I have filtered such that there is one time displayed that represents multiple slots
    And I click the 'Find available appointments' button
    And I have selected a time when multiple slots are available
    And the Appointment Slot page is displayed
    When I click the 'Confirm and book appointment' button
    Then the Appointment Booking success message is displayed
    And the booked appointment before cutoff time is correctly displayed with ability to cancel
