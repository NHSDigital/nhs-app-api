@appointments
@appointments-book
@book
Feature: Book Telephone Appointments Frontend

  Scenario Outline: An <GP System> user can book a telephone appointment
    Given I wish to book a <GP System> telephone appointment
      | number of stored telephone numbers  | 0         |
      | the reason on the appointment is    | mandatory |
      | selecting telephone number          | manual    |
      | symptoms to enter                   | yes       |
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    When I have selected a telephone appointment slot to book
    Then the Appointment Slot page is displayed
    And I do not see any phone numbers to select
    And I see a text input to enter phone number
    When I enter a phone number for the appointment
    And I enter symptoms
    And I click the 'Confirm and book appointment' button
    Then the Appointment Booking success page is displayed
    And I select the back to home link on the appointments page
    # NHSO-8593: Changes below linked to this bug that caused an infinite loop on the back link in native
    # Will need removing/altering when either a fix is complete or story to add appointment details to the
    # success page is done.
    Then the Appointment Hub page is displayed
    # And I can see the booked telephone appointment and it has a cancel link
    Examples:
      | GP System |
      | EMIS      |
      | MICROTEST |

  Scenario Outline: An <GP System> user receives an error if a phone number is not provided for telephone appointments
    Given I wish to book a <GP System> telephone appointment
      | number of stored telephone numbers  | 0         |
      | the reason on the appointment is    | mandatory |
      | selecting telephone number          | manual    |
      | symptoms to enter                   | yes       |
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I have selected a telephone appointment slot to book
    And the Appointment Slot page is displayed
    When I enter symptoms
    And I click the 'Confirm and book appointment' button
    Then a message is displayed indicating a phone number is required
    Examples:
      | GP System |
      | EMIS      |
      | MICROTEST |

  Scenario Outline: An <GP System> user receives errors if a phone number and booking reason are not provided for telephone appointments
    Given I wish to book a <GP System> telephone appointment
      | number of stored telephone numbers  | 0         |
      | the reason on the appointment is    | mandatory |
      | selecting telephone number          | manual    |
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I have selected a telephone appointment slot to book
    And the Appointment Slot page is displayed
    When I click the 'Confirm and book appointment' button
    Then a message is displayed indicating a phone number is required
    And an error is displayed that "Describe your symptoms" is mandatory
    Examples:
      | GP System |
      | EMIS      |
      | MICROTEST |

  Scenario Outline: An <GP System> user receives an error if a phone number is not provided for telephone appointments with optional booking reason
    Given I wish to book a <GP System> telephone appointment
      | number of stored telephone numbers  | 0         |
      | the reason on the appointment is    | optional  |
      | selecting telephone number          | manual    |
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I have selected a telephone appointment slot to book
    And the Appointment Slot page is displayed
    When I click the 'Confirm and book appointment' button
    Then a message is displayed indicating a phone number is required
    Examples:
      | GP System |
      | EMIS      |
      | MICROTEST |


  Scenario Outline: An <GP System> user receives an error if a phone number is not selected for telephone appointments, but symptoms have been entered
    Given I wish to book a <GP System> telephone appointment
      | number of stored telephone numbers  | 2         |
      | the reason on the appointment is    | mandatory |
      | selecting telephone number          | manual    |
      | symptoms to enter                   | yes       |
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I have selected a telephone appointment slot to book
    And the Appointment Slot page is displayed
    When I enter symptoms
    And I click the 'Confirm and book appointment' button
    Then a message is displayed indicating a phone number is required
    Examples:
      | GP System |
      | EMIS      |
      | MICROTEST |

  Scenario Outline: An <GP System> user receives an error if a phone number is not selected for telephone appointments with optional booking reason
    Given I wish to book a <GP System> telephone appointment
      | number of stored telephone numbers  | 2         |
      | the reason on the appointment is    | optional  |
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I have selected a telephone appointment slot to book
    And the Appointment Slot page is displayed
    When I click the 'Confirm and book appointment' button
    And a message is displayed indicating a phone number is required
    Examples:
      | GP System |
      | EMIS      |
      | MICROTEST |

  Scenario Outline: An <GP System> user receives an error if an alternate phone number is selected for telephone appointments without a number entered, but symptoms have been entered
    Given I wish to book a <GP System> telephone appointment
      | number of stored telephone numbers  | 2         |
      | the reason on the appointment is    | mandatory |
      | selecting telephone number          | manual    |
      | symptoms to enter                   | yes       |
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I have selected a telephone appointment slot to book
    And the Appointment Slot page is displayed
    When I select the radio button for an alternative phone number to those stored
    And I enter symptoms
    And I click the 'Confirm and book appointment' button
    Then a message is displayed indicating a phone number is required
    Examples:
      | GP System |
      | EMIS      |
      | MICROTEST |

  Scenario Outline: An <GP System> user receives an error if an alternate phone number is selected for telephone appointments with optional booking reason, but without a number entered
    Given I wish to book a <GP System> telephone appointment
      | number of stored telephone numbers  | 2         |
      | the reason on the appointment is    | optional  |
      | selecting telephone number          | manual    |
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I have selected a telephone appointment slot to book
    And the Appointment Slot page is displayed
    When I select the radio button for an alternative phone number to those stored
    And I click the 'Confirm and book appointment' button
    Then a message is displayed indicating a phone number is required
    Examples:
      | GP System |
      | EMIS      |
      | MICROTEST |

  Scenario Outline: An <GP System> user receives an error if an empty string or whitespace provided for telephone appointments
    Given I wish to book a <GP System> telephone appointment
      | number of stored telephone numbers  | 0         |
      | the reason on the appointment is    | mandatory |
      | selecting telephone number          | manual    |
      | symptoms to enter                   | yes       |
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I have selected a telephone appointment slot to book
    Then the Appointment Slot page is displayed
    And I enter whitespace instead of a phone number for the appointment
    When I enter symptoms
    And I click the 'Confirm and book appointment' button
    Then a message is displayed indicating a phone number is required
    Examples:
      | GP System |
      | EMIS      |
      | MICROTEST |

  Scenario Outline: An <GP System> user receives an error if an empty string or whitespace provided for telephone appointments, when alternate phone number is selected
    Given I wish to book a <GP System> telephone appointment
      | number of stored telephone numbers  | 2         |
      | the reason on the appointment is    | mandatory |
      | selecting telephone number          | manual    |
      | symptoms to enter                   | yes       |
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I have selected a telephone appointment slot to book
    And the Appointment Slot page is displayed
    When I select the radio button for an alternative phone number to those stored
    And I enter whitespace instead of a phone number for the appointment
    And I enter symptoms
    And I click the 'Confirm and book appointment' button
    Then a message is displayed indicating a phone number is required
    Examples:
      | GP System |
      | EMIS      |
      | MICROTEST |

    # Regression Test scenarios to ensure booking reason is still validated for telephone appointments

  Scenario Outline: An <GP System> user receives an error if a phone number is entered for telephone appointments, but no reason
    Given I wish to book a <GP System> telephone appointment
      | number of stored telephone numbers  | 0         |
      | the reason on the appointment is    | mandatory |
      | selecting telephone number          | manual    |
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I have selected a telephone appointment slot to book
    And I enter a phone number for the appointment
    When I click the 'Confirm and book appointment' button
    Then an error is displayed that "Describe your symptoms" is mandatory
    Examples:
      | GP System |
      | EMIS      |
      | MICROTEST |

  Scenario Outline: An appointment is booked for an <GP System> user if a phone number is provided for telephone appointments with optional booking reason
    Given I wish to book a <GP System> telephone appointment
      | number of stored telephone numbers  | 0         |
      | the reason on the appointment is    | optional  |
      | selecting telephone number          | manual    |
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I have selected a telephone appointment slot to book
    And I enter a phone number for the appointment
    When I click the 'Confirm and book appointment' button
    Then the Appointment Booking success page is displayed
    And I select the back to home link on the appointments page
    # NHSO-8593: Changes below linked to this bug that caused an infinite loop on the back link in native
    # Will need removing/altering when either a fix is complete or story to add appointment details to the
    # success page is done.
    Then the Appointment Hub page is displayed
    # And I can see the booked telephone appointment and it has a cancel link
    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: A <GP System> user receives an error when booking a telephone appointments with no reason entered, but a reason is required
    Given I wish to book a <GP System> telephone appointment
      | number of stored telephone numbers  | 1         |
      | the reason on the appointment is    | mandatory |
      | selecting telephone number          | select    |
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I have selected a telephone appointment slot to book
    When I select a telephone number to book an appointment
    And I click the 'Confirm and book appointment' button
    Then an error is displayed that "Describe your symptoms" is mandatory
    Examples:
      | GP System |
      | EMIS      |
      | MICROTEST |

  Scenario: An appointment can be booked for an EMIS with a stored phone number no reason when booking reasons are optional
    Given I wish to book a EMIS telephone appointment
      | number of stored telephone numbers  | 1         |
      | the reason on the appointment is    | optional  |
      | selecting telephone number          | select    |
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I have selected a telephone appointment slot to book
    When I select a telephone number to book an appointment
    And I click the 'Confirm and book appointment' button
    Then the Appointment Booking success page is displayed
    And I select the back to home link on the appointments page
    # NHSO-8593: Changes below linked to this bug that caused an infinite loop on the back link in native
    # Will need removing/altering when either a fix is complete or story to add appointment details to the
    # success page is done.
    Then the Appointment Hub page is displayed
    # And the booked appointment before cutoff time is correctly displayed with ability to cancel

  Scenario Outline: A <GP System> user receives an error if a telephone number is manually entered, but no reason entered, but a reason is required
    Given I wish to book a <GP System> telephone appointment
      | number of stored telephone numbers  | 1         |
      | the reason on the appointment is    | mandatory |
      | selecting telephone number          | manual    |
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I have selected a telephone appointment slot to book
    When I select the radio button for an alternative phone number to those stored
    And I enter a phone number for the appointment
    And I click the 'Confirm and book appointment' button
    Then an error is displayed that "Describe your symptoms" is mandatory
    Examples:
      | GP System |
      | EMIS      |
      | MICROTEST |

  Scenario: An appointment is booked for an EMIS user if an phone number is manually entered for telephone appointments with optional booking reason
    Given I wish to book a EMIS telephone appointment
      | number of stored telephone numbers  | 1         |
      | the reason on the appointment is    | optional  |
      | selecting telephone number          | manual    |
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I have selected a telephone appointment slot to book
    When I select the radio button for an alternative phone number to those stored
    And I enter a phone number for the appointment
    And I click the 'Confirm and book appointment' button
    Then the Appointment Booking success page is displayed
    And I select the back to home link on the appointments page
    # NHSO-8593: Changes below linked to this bug that caused an infinite loop on the back link in native
    # Will need removing/altering when either a fix is complete or story to add appointment details to the
    # success page is done.
    Then the Appointment Hub page is displayed
    # And the booked appointment before cutoff time is correctly displayed with ability to cancel
