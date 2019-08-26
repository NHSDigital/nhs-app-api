@appointments
@book
@noJs

Feature: Book Appointments With Javascript Disabled
  In order to complete a booking appointment
  As a logged in user with Javascript switched off
  I want to be able to select, confirm and book selected appointment

  Background:
    Given I have disabled javascript

    #This test covers navigation via buttons/links

    # EMIS/Microtest Specific Scenarios (For EMIS/Microtest telephone appointment)
    # The following scenarios covered only telephone appointment options.
    # The telephone number is mandatory if telephone appointment slot selected.
    # Positive submission cases

  Scenario Outline: A <GP System> user without Javascript cannot enter or select a phone number for non phone appointments
    Given there are <GP System> appointments available to book
    And I am logged in
    And I am on the Available Appointments page
    And I have filtered such that there is one time displayed that represents multiple slots
    And I click the 'Find available appointments' button
    When I have selected a time when multiple slots are available
    Then the Appointment Slot page is displayed
    And I do not see a text input to enter phone number
    And I do not see any phone numbers to select
    Examples:
      | GP System |
      | EMIS      |
      | MICROTEST |

# These tests navigate directly to the pages where the features are to be tested, to save time.

  Scenario Outline: A <GP System> user with noJs can only enter a phone number for phone appointments, when they have <User's Telephone Numbers> phone number saved
    Given I have <User's Telephone Numbers> telephone number(s) stored for <GP System>
    But I will manually enter this phone number
    And there are appointments available to book which are of telephone type for <GP System>
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
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
      | GP System | User's Telephone Numbers        |
      | EMIS      | no                              |
      | EMIS      | only first                      |
      | EMIS      | only second                     |
      | EMIS      | both first and second           |
      | MICROTEST | no                              |
      | MICROTEST | only first                      |
      | MICROTEST | only second                     |
      | MICROTEST | both first and second           |


      # Telephone number error scenarios

  Scenario Outline: A <GP System> user with noJs receives an error if a phone number is not provided for telephone appointments
    Given I have no telephone number(s) stored for <GP System>
    And there are appointments available to book which are of telephone type for <GP System>
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I have filtered such that there is one time displayed that represents multiple slots
    And I click the 'Find available appointments' button
    And I have selected a time when multiple slots are available
    And the Appointment Slot page is displayed
    When I enter symptoms
    And I click the 'Confirm and book appointment' button
    Then the focus will go back to empty phone number input box
    Examples:
      | GP System |
      | EMIS      |
      | MICROTEST |

  Scenario Outline: A <GP System> user with noJs receives errors if a phone number and booking reason are not provided for telephone appointments
    Given I have no telephone number(s) stored for <GP System>
    And there are appointments available to book which are of telephone type for <GP System>
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I have filtered such that there is one time displayed that represents multiple slots
    And I click the 'Find available appointments' button
    And I have selected a time when multiple slots are available
    And the Appointment Slot page is displayed
    When I click the 'Confirm and book appointment' button
    Then the focus will go back to empty phone number input box
    Examples:
      | GP System |
      | EMIS      |
      | MICROTEST |

  Scenario Outline: A <GP System> user with noJs receives an error if a phone number is not provided for telephone appointments with optional booking reason
    Given I have no telephone number(s) stored for <GP System>
    And there are appointments available to book which are of telephone type with optional booking reason for <GP System>
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I have filtered such that there is one time displayed that represents multiple slots
    And I click the 'Find available appointments' button
    And I have selected a time when multiple slots are available
    And the Appointment Slot page is displayed
    When I click the 'Confirm and book appointment' button
    Then the focus will go back to empty phone number input box
    Examples:
      | GP System |
      | EMIS      |
      | MICROTEST |

  Scenario Outline: A <GP System> user with noJs receives an error if an empty string or whitespace provided for telephone appointments
    Given I have no telephone number(s) stored for <GP System>
    And there are appointments available to book which are of telephone type for <GP System>
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I have filtered such that there is one time displayed that represents multiple slots
    And I click the 'Find available appointments' button
    And I have selected a time when multiple slots are available
    Then the Appointment Slot page is displayed
    And I see a text input to enter phone number
    When I enter whitespace instead of a phone number for the appointment
    And I enter symptoms
    And I click the 'Confirm and book appointment' button
    Then the focus will go back to empty phone number input box
    Examples:
      | GP System |
      | EMIS      |
      | MICROTEST |


    # Regression Test scenarios to ensure booking reason is still validated for telephone appointments

  Scenario Outline: A <GP System> user with noJs receives an error if a phone number is entered for telephone appointments, but no reason
    Given I wish to book an appointment without specifying a reason
    And I have no telephone number(s) stored for <GP System>
    But I will manually enter this phone number
    And there are appointments available to book which are of telephone type for <GP System>
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I have filtered such that there is one time displayed that represents multiple slots
    And I click the 'Find available appointments' button
    And I have selected a time when multiple slots are available
    And I enter a phone number for the appointment
    When I click the 'Confirm and book appointment' button
    Then the focus will go back to empty booking reason input box
    Examples:
      | GP System |
      | EMIS      |
      | MICROTEST |

  Scenario: An EMIS user with noJS can book a telephone appointment if a phone number is provided without describing symptoms when booking reason is optional
    Given I wish to book an appointment without specifying a reason
    And I have no telephone number(s) stored for EMIS
    But I will manually enter this phone number
    And there are appointments available to book which are of telephone type with optional booking reason for EMIS
    And a booked appointment can be cancelled
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I have filtered such that there is one time displayed that represents multiple slots
    And I click the 'Find available appointments' button
    And I have selected a time when multiple slots are available
    And I enter a phone number for the appointment
    When I click the 'Confirm and book appointment' button
    Then the Appointment Booking success message is displayed
    And the booked appointment before cutoff time is correctly displayed with ability to cancel


    # Miscellaneous no-JS scenarios

  Scenario Outline: A <GP System> user with noJS can book appointments
    Given there are multiple appointment slots at the same time, provided by <GP System>
    And a booked appointment can be cancelled
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
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
      | MICROTEST |

  # Not applicable for VISION
  Scenario Outline: A <GP System> user with noJS cannot book appointments without describing symptoms when booking reason is mandatory
    Given there are multiple appointment slots at the same time, provided by <GP System>
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I have filtered such that there is one time displayed that represents multiple slots
    And I click the 'Find available appointments' button
    And I have selected a time when multiple slots are available
    Then the Appointment Slot page is displayed
    And  I click the 'Confirm and book appointment' button
    Then the focus will go back to empty booking reason input box
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | MICROTEST |

  Scenario: An EMIS user with noJS can book an appointment without describing symptoms when booking reason is optional
    Given there are EMIS appointments available to book where booking reason is set optional
    And a booked appointment can be cancelled
    And I am logged in
    And I am on the Available Appointments page
    And I have filtered such that there is one time displayed that represents multiple slots
    And I click the 'Find available appointments' button
    When I have selected a time when multiple slots are available
    Then the Appointment Slot page is displayed
    When I click the 'Confirm and book appointment' button
    Then the Appointment Booking success message is displayed
    And the booked appointment before cutoff time is correctly displayed with ability to cancel

