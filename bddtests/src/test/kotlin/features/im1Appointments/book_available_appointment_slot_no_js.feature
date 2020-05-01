@appointments
@appointments-book
@book
@noJs
Feature: Book Appointments With Javascript Disabled
  In order to complete a booking appointment
  As a logged in user with Javascript switched off
  I want to be able to select, confirm and book selected appointment

    #This test covers navigation via buttons/links

    # EMIS/Microtest Specific Scenarios (For EMIS/Microtest telephone appointment)
    # The following scenarios covered only telephone appointment options.
    # The telephone number is mandatory if telephone appointment slot selected.
    # Positive submission cases

  Scenario Outline: A <GP System> user without Javascript cannot enter or select a phone number for non phone appointments
    Given I have disabled javascript
    And there are <GP System> appointments available to book
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

  @tech-debt
  Scenario Outline: A <GP System> user with noJs can only enter a phone number for phone appointments, when they have phone numbers saved
    Given I have disabled javascript
    And I wish to book a <GP System> telephone appointment
      | number of stored telephone numbers  | 2         |
      | the reason on the appointment is    | optional  |
      | selecting telephone number          | manual    |
      | symptoms to enter                   | yes       |
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
      | GP System |
      | EMIS      |
      | MICROTEST |


      # Telephone number error scenarios

  Scenario Outline: A <GP System> user with noJs receives an error if a phone number is not provided for telephone appointments
    Given I have disabled javascript
    And I wish to book a <GP System> telephone appointment
      | number of stored telephone numbers  | 0         |
      | the reason on the appointment is    | optional  |
      | selecting telephone number          | manual    |
      | symptoms to enter                   | yes       |
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
    Given I have disabled javascript
    And I wish to book a <GP System> telephone appointment
      | number of stored telephone numbers  | 0         |
      | the reason on the appointment is    | mandatory |
      | selecting telephone number          | manual    |
      | symptoms to enter                   | yes       |
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
    Given I have disabled javascript
    And I wish to book a <GP System> telephone appointment
      | number of stored telephone numbers  | 0         |
      | the reason on the appointment is    | optional  |
      | selecting telephone number          | manual    |
      | symptoms to enter                   | yes       |
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
    Given I have disabled javascript
    And I wish to book a <GP System> telephone appointment
      | number of stored telephone numbers  | 0         |
      | the reason on the appointment is    | optional  |
      | selecting telephone number          | manual    |
      | symptoms to enter                   | yes       |
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
    Given I have disabled javascript
    And I wish to book a <GP System> telephone appointment
      | number of stored telephone numbers  | 0         |
      | the reason on the appointment is    | mandatory |
      | selecting telephone number          | manual    |
      | symptoms to enter                   | yes       |
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

  @tech-debt
  Scenario: An EMIS user with noJS can book a telephone appointment if a phone number is provided without describing symptoms when booking reason is optional
    Given I have disabled javascript
    And I wish to book a EMIS telephone appointment
      | number of stored telephone numbers  | 0         |
      | the reason on the appointment is    | optional  |
      | selecting telephone number          | manual    |
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
  @tech-debt
  Scenario Outline: A <GP System> user with noJS can book appointments
    Given I have disabled javascript
    And there are multiple appointment slots at the same time, provided by <GP System>
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
    Given I have disabled javascript
    And there are multiple appointment slots at the same time, provided by <GP System>
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

  @tech-debt
  Scenario: An EMIS user with noJS can book an appointment without describing symptoms when booking reason is optional
    Given I have disabled javascript
    And there are EMIS appointments available to book where booking reason is set optional
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

