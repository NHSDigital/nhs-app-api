@appointments
@book
Feature: Book an available appointment slot UI with Javascript
  In order to complete a booking appointment
  As a logged in user
  I want to be able to select, confirm and book selected appointment

  Scenario Outline: Only one appointment slot time is displayed when multiple are available for <GP System>
    Given there are multiple appointment slots at the same time, provided by <GP System>
    And a booked appointment can be cancelled
    And I am logged in
    And I am on the Available Appointments page
    And I have filtered such that there is one time displayed that represents multiple slots
    When I have selected a time when multiple slots are available
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

  # Not applicable for VISION
  Scenario Outline: A <GP System> user cannot book an appointment without describing symptoms
    Given there are <GP System> appointments available to book
    And I am logged in
    And I am on the Available Appointments page
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I click the 'Confirm and book appointment' button
    Then an error is displayed that "Describe your symptoms" is mandatory
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario Outline: A <GP System> user can book an appointment describing symptoms at least 1 character
    Given there are <GP System> appointments available to book with a reason of 1 character
    And a booked appointment can be cancelled
    And I am logged in
    And I am on the Available Appointments page
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I enter symptoms of 1 characters
    And I click the 'Confirm and book appointment' button
    Then the Appointment Booking success message is displayed
    And the booked appointment before cutoff time is correctly displayed with ability to cancel
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  @smoketest
  @nativepending @NHSO-2974
  Scenario Outline: A <GP System> user can book an appointment describing symptoms no more 150 characters
    Given there are <GP System> appointments available to book with a reason of 150 character
    And a booked appointment can be cancelled
    And I am logged in
    And I am on the Available Appointments page
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I enter symptoms of 150 characters
    And I click the 'Confirm and book appointment' button
    Then the Appointment Booking success message is displayed
    And the booked appointment before cutoff time is correctly displayed with ability to cancel
    Examples:
      | GP System |
      | TPP       |
      | VISION    |

  @native-smoketest
    Examples:
      | GP System |
      | EMIS      |

  @nativepending @NHSO-2974
  Scenario Outline: A <GP System> user cannot enter symptoms with over 150 characters
    Given there are <GP System> appointments available to book with a reason of 150 characters but user attempts to enter 151 characters
    And a booked appointment can be cancelled
    And I am logged in
    And I am on the Available Appointments page
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I enter symptoms of 151 characters
    Then only the first 150 characters will be displayed
    And I click the 'Confirm and book appointment' button
    Then the Appointment Booking success message is displayed
    And the booked appointment before cutoff time is correctly displayed with ability to cancel
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  @nativepending @NHSO-2974
  Scenario Outline: A <GP System> user cannot paste symptoms with over 150 characters
    Given there are <GP System> appointments available to book with a reason of 150 characters but user attempts to enter 151 characters
    And I am logged in
    And I am on the Available Appointments page
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I paste symptoms of 151 characters
    Then only the first 150 characters will be displayed
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  @nativepending @NHSO-2974
  Scenario Outline: A <GP System> user who books successfully, but only the first 150 characters of the symptoms are sent
    Given there are <GP System> appointments available to book with a reason of 150 characters but user attempts to enter 151 characters
    And a booked appointment can be cancelled
    And I am logged in
    And I am on the Available Appointments page
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I enter symptoms of 151 characters
    And I click the 'Confirm and book appointment' button
    Then the Appointment Booking success message is displayed
    And the booked appointment before cutoff time is correctly displayed with ability to cancel
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario: A Vision user gets an alternative success message when booking and there's no ability to cancel
    Given there are VISION appointments available to book
    But a booked appointment cannot be cancelled
    And I am logged in
    And I am on the Available Appointments page
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I enter symptoms
    And I click the 'Confirm and book appointment' button
    Then the Appointment Booking success message is displayed without reference to being able to cancel
    And the booked appointment is correctly displayed without ability to cancel

  Scenario Outline: A <GP System> user cannot enter dangerous text for booking reason
    Given there are <GP System> appointments available to book and user attempts to enter booking reason <script>
    And I am logged in
    And I am on the Available Appointments page
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I enter symptoms
    And I click the 'Confirm and book appointment' button
    Then I see appropriate information message when there is an error sending data on appointment confirmation page
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  @nativepending @NHSO-2974
  Scenario Outline: A <GP System> user sees appropriate information message when there is a timeout
    Given there are <GP System> appointments available to book, but GP system doesn't respond a timely fashion when booking
    And I am logged in
    And I am on the Available Appointments page
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I enter symptoms
    And  I click the 'Confirm and book appointment' button
    Then I see appropriate information message after 10 seconds when it times-out on appointment confirmation page
    And there should be a button to go back to my appointments
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: A <GP System> user sees appropriate information message when GP system is unavailable
    Given there are <GP System> appointments available to book, but the GP system is unavailable
    And I am logged in
    And I am on the Available Appointments page
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I enter symptoms
    And  I click the 'Confirm and book appointment' button
    Then I see appropriate information message when there is an error sending data on appointment confirmation page
    And there should be a button to go back to my appointments
    Examples:
      | GP System |
      | TPP       |
      | VISION    |

  @native-smoketest
    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: A <GP System> user sees appropriate information error message when appointment has already been booked
    Given there are <GP System> appointments available to book, but the appointment slot has already been booked by somebody else
    And I am logged in
    And I am on the Available Appointments page
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I enter symptoms
    And  I click the 'Confirm and book appointment' button
    Then a message is displayed indicating that the slot has already been taken
    Examples:
      | GP System |
      | TPP       |
      | VISION    |

  @native-smoketest
    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: A <GP System> user can return directly back to their appointments after trying to book one already booked
    Given there are <GP System> appointments available to book, but the appointment slot has already been booked by somebody else
    And I am logged in
    And I am on the Available Appointments page
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    And I enter symptoms
    And  I click the 'Confirm and book appointment' button
    When I click the error page back button
    Then the Available Appointments page is displayed
    Examples:
      | GP System |
      | TPP       |
      | VISION    |

  @native-smoketest
    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: A <GP System> user reached maximum appointment booking limit
    Given there are <GP System> appointments available to book, but user reached maximum appointment booking limit
    And I am logged in
    And I am on the Available Appointments page
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    And I enter symptoms
    And  I click the 'Confirm and book appointment' button
    Then a message is displayed indicating that user has reached maximum appointment limit
    Examples:
      | GP System |
      | TPP       |
      | VISION    |

  @native-smoketest
    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: A <GP System> user is navigated back to the 'Book this appointment' screen when 'Back' button selected.
    Given there are <GP System> appointments available to book
    And I am logged in
    And I am on the Available Appointments page
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I click the 'Back' link
    Then there is a filter for the appointment types
    And there is a filter for the appointment locations
    And there is a filter for the appointment doctors/nurses
    And there is a filter for the appointment time period
    And no available slots are displayed
    Examples:
      | GP System |
      | TPP       |
      | VISION    |

  @native-smoketest
    Examples:
      | GP System |
      | EMIS      |


# EMIS Specific Scenarios (For EMIS reason necessity)
# The following scenarios covered only Optional and Not-Required reason necessity options.
# The default is mandatory and if the option is not specified in a scenario, it is set to MANDATORY by default.
  Scenario: An EMIS user can book an appointment without describing symptoms
    Given there are EMIS appointments available to book where booking reason is set optional
    And a booked appointment can be cancelled
    And I am logged in
    And I am on the Available Appointments page
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I click the 'Confirm and book appointment' button
    Then the Appointment Booking success message is displayed
    And the booked appointment before cutoff time is correctly displayed with ability to cancel

  Scenario: An EMIS user can book an appointment with describing symptoms
    Given there are EMIS appointments available to book where booking reason is set optional with a reason entered
    And a booked appointment can be cancelled
    And I am logged in
    And I am on the Available Appointments page
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I enter symptoms
    And I click the 'Confirm and book appointment' button
    Then the Appointment Booking success message is displayed
    And the booked appointment before cutoff time is correctly displayed with ability to cancel

  Scenario: An EMIS user on Old EMIS System reached maximum appointment booking limit
    Given  there are appointments available to book in old EMIS system, but user reached maximum appointment booking limit
    And I am logged in
    And I am on the Available Appointments page
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    And I enter symptoms
    And  I click the 'Confirm and book appointment' button
    Then a message is displayed indicating that user has reached maximum appointment limit


# EMIS Specific Scenarios (For EMIS telephone appointment)
# The following scenarios covered only telephone appointment options.
# The telephone number is mandatory if telephone appointment slot selected.

    # Positive submission cases

  Scenario: An EMIS user cannot enter or select a phone number for non phone appointments
    Given there are EMIS appointments available to book
    And I am logged in
    And I am on the Available Appointments page
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    And I do not see a text input to enter phone number
    And I do not see any phone numbers to select

  Scenario: An EMIS user is only given an option to enter their phone number for telephone appointments, when they don't have one saved, and can successfully book
    Given I have no telephone number(s) stored
    And there are appointments available to book which are of telephone type
    And I am logged in
    And I am on the Available Appointments page
    When I have selected a telephone appointment slot to book
    Then the Appointment Slot page is displayed
    And I do not see any phone numbers to select
    And I see a text input to enter phone number
    When I enter a phone number for the appointment
    And I enter symptoms
    And I click the 'Confirm and book appointment' button
    Then the Appointment Booking success message is displayed
    And the booked appointment before cutoff time is correctly displayed with ability to cancel

  Scenario Outline: An EMIS user can select one of their phone numbers for phone appointments, when they have <User's Telephone Numbers> phone number saved, and can successfully book
    Given I have <User's Telephone Numbers> telephone number(s) stored
    And I wish to book a telephone appointment using my <Telephone Number Type To Select> phone number
    And there are appointments available to book which are of telephone type
    And I am logged in
    And I am on the Available Appointments page
    When I have selected a telephone appointment slot to book
    Then the Appointment Slot page is displayed
    And I see radio buttons to select the user's telephone numbers
    And I see a radio button to select an alternate number
    And none of available phone numbers are selected
    And the phone number text field is not displayed
    When I select the <Telephone Number Type To Select> number from available ones
    And I enter symptoms
    And I click the 'Confirm and book appointment' button
    Then the Appointment Booking success message is displayed
    And the booked appointment before cutoff time is correctly displayed with ability to cancel
    Examples:
      | User's Telephone Numbers | Telephone Number Type To Select |
      | only a home              | home                            |
      | only a mobile            | mobile                          |
      | both home and mobile     | home                            |
      | both home and mobile     | mobile                          |

  Scenario Outline: An Emis user can enter a different phone number even when they have <User's Telephone Numbers> phone number saved
    Given I have <User's Telephone Numbers> telephone number(s) stored
    And there are appointments available to book which are of telephone type
    And I am logged in
    And I am on the Available Appointments page
    And I have selected a telephone appointment slot to book
    And the Appointment Slot page is displayed
    When I select the radio button for an alternative phone number to those stored
    Then I see a text input to enter phone number
    When I enter a phone number for the appointment
    Then the radio button for an alternate phone number remains selected
    When I enter symptoms
    And I click the 'Confirm and book appointment' button
    Then the Appointment Booking success message is displayed
    And the booked appointment before cutoff time is correctly displayed with ability to cancel
    Examples:
      | User's Telephone Numbers |
      | only a home              |
      | only a mobile            |
      | both home and mobile     |

  Scenario Outline: An Emis user can manually enter their <Telephone Number To Type> number from their stored phone numbers and still submit
    Given I have both home and mobile telephone number(s) stored
    And I wish to book a telephone appointment using my <Telephone Number To Type> phone number
    But I will manually enter this phone number
    And there are appointments available to book which are of telephone type
    And I am logged in
    And I am on the Available Appointments page
    And I have selected a telephone appointment slot to book
    And the Appointment Slot page is displayed
    When I select the radio button for an alternative phone number to those stored
    Then I see a text input to enter phone number
    When I enter a phone number for the appointment
    Then the radio button for an alternate phone number remains selected
    When I enter symptoms
    And I click the 'Confirm and book appointment' button
    Then the Appointment Booking success message is displayed
    And the booked appointment before cutoff time is correctly displayed with ability to cancel
    Examples:
      | Telephone Number To Type |
      | home                     |
      | mobile                   |

    # Scenarios for alternating between the different options

  Scenario: An EMIS user can select different phone number options and only the last one will be selected
    Given I have both home and mobile telephone number(s) stored
    And I wish to book a telephone appointment using my mobile phone number
    And there are appointments available to book which are of telephone type
    And I am logged in
    And I am on the Available Appointments page
    And I have selected a telephone appointment slot to book
    And the Appointment Slot page is displayed
    When I alternate between the different number options from available ones
      | home        |
      | home        |
      | mobile      |
      | mobile      |
      | alternative |
      | alternative |
      | home        |
      | alternative |
      | mobile      |
      | alternative |
      | mobile      |
      | home        |
      | mobile      |
      | alternative |
    And I enter a phone number for the appointment
    And I alternate between the different number options from available ones
      | home        |
      | alternative |
    And I select the mobile number from available ones
    And I enter symptoms
    And I click the 'Confirm and book appointment' button
    Then the Appointment Booking success message is displayed
    And the booked appointment before cutoff time is correctly displayed with ability to cancel

  Scenario: An Emis user alternates between selecting their phone number and selecting and entering a different one, the latter is be submitted
    Given I have only a mobile telephone number(s) stored
    And there are appointments available to book which are of telephone type
    And I am logged in
    And I am on the Available Appointments page
    And I have selected a telephone appointment slot to book
    And the Appointment Slot page is displayed
    When I select the mobile number from available ones
    And I select the radio button for an alternative phone number to those stored
    And I enter a phone number for the appointment
    And I select the mobile number from available ones
    And I select the radio button for an alternative phone number to those stored
    And I enter symptoms
    And I click the 'Confirm and book appointment' button
    Then the Appointment Booking success message is displayed
    And the booked appointment before cutoff time is correctly displayed with ability to cancel

  Scenario: An Emis user alternates between entering a different phone number and selecting their phone number, the latter is be submitted
    Given I have only a mobile telephone number(s) stored
    And I wish to book a telephone appointment using my mobile phone number
    And there are appointments available to book which are of telephone type
    And I am logged in
    And I am on the Available Appointments page
    And I have selected a telephone appointment slot to book
    And the Appointment Slot page is displayed
    When I select the radio button for an alternative phone number to those stored
    And I enter a phone number for the appointment
    And I select the mobile number from available ones
    And I select the radio button for an alternative phone number to those stored
    And I select the mobile number from available ones
    And I enter symptoms
    And I click the 'Confirm and book appointment' button
    Then the Appointment Booking success message is displayed
    And the booked appointment before cutoff time is correctly displayed with ability to cancel

    # Telephone number error scenarios

  Scenario: An EMIS user receives an error if a phone number is not provided for telephone appointments
    Given I have no telephone number(s) stored
    And there are appointments available to book which are of telephone type
    And I am logged in
    And I am on the Available Appointments page
    And I have selected a telephone appointment slot to book
    And the Appointment Slot page is displayed
    When I enter symptoms
    And I click the 'Confirm and book appointment' button
    Then a message is displayed indicating a phone number is required

  Scenario: An EMIS user receives errors if a phone number and booking reason are not provided for telephone appointments
    Given I have no telephone number(s) stored
    And there are appointments available to book which are of telephone type
    And I am logged in
    And I am on the Available Appointments page
    And I have selected a telephone appointment slot to book
    And the Appointment Slot page is displayed
    When I click the 'Confirm and book appointment' button
    Then a message is displayed indicating a phone number is required
    And an error is displayed that "Describe your symptoms" is mandatory

  Scenario: An EMIS user receives an error if a phone number is not provided for telephone appointments with optional booking reason
    Given I have no telephone number(s) stored
    And there are appointments available to book which are of telephone type with optional booking reason
    And I am logged in
    And I am on the Available Appointments page
    And I have selected a telephone appointment slot to book
    And the Appointment Slot page is displayed
    When I click the 'Confirm and book appointment' button
    Then a message is displayed indicating a phone number is required

  Scenario: An EMIS user receives an error if a phone number is not selected for telephone appointments, but symptoms have been entered
    Given I have both home and mobile telephone number(s) stored
    And there are appointments available to book which are of telephone type with optional booking reason
    And I am logged in
    And I am on the Available Appointments page
    And I have selected a telephone appointment slot to book
    And the Appointment Slot page is displayed
    When I enter symptoms
    And I click the 'Confirm and book appointment' button
    Then a message is displayed indicating a phone number is required

  Scenario: An EMIS user receives an error if a phone number is not selected for telephone appointments with optional booking reason
    Given I have both home and mobile telephone number(s) stored
    And there are appointments available to book which are of telephone type with optional booking reason
    And I am logged in
    And I am on the Available Appointments page
    And I have selected a telephone appointment slot to book
    And the Appointment Slot page is displayed
    When I click the 'Confirm and book appointment' button
    And a message is displayed indicating a phone number is required

  Scenario: An EMIS user receives an error if an alternate phone number is selected for telephone appointments without a number entered, but symptoms have been entered
    Given I have both home and mobile telephone number(s) stored
    And there are appointments available to book which are of telephone type with optional booking reason
    And I am logged in
    And I am on the Available Appointments page
    And I have selected a telephone appointment slot to book
    And the Appointment Slot page is displayed
    When I select the radio button for an alternative phone number to those stored
    And I enter symptoms
    And I click the 'Confirm and book appointment' button
    Then a message is displayed indicating a phone number is required

  Scenario: An EMIS user receives an error if an alternate phone number is selected for telephone appointments with optional booking reason, but without a number entered
    Given I have both home and mobile telephone number(s) stored
    And there are appointments available to book which are of telephone type with optional booking reason
    And I am logged in
    And I am on the Available Appointments page
    And I have selected a telephone appointment slot to book
    And the Appointment Slot page is displayed
    When I select the radio button for an alternative phone number to those stored
    And I click the 'Confirm and book appointment' button
    Then a message is displayed indicating a phone number is required

  Scenario: An EMIS user receives an error if an empty string or whitespace provided for telephone appointments
    Given I have no telephone number(s) stored
    And there are appointments available to book which are of telephone type
    And I am logged in
    And I am on the Available Appointments page
    And I have selected a telephone appointment slot to book
    Then the Appointment Slot page is displayed
    And I enter whitespace instead of a phone number for the appointment
    When I enter symptoms
    And I click the 'Confirm and book appointment' button
    Then a message is displayed indicating a phone number is required

  Scenario: An EMIS user receives an error if an empty string or whitespace provided for telephone appointments, when alternate phone number is selected
    Given I have both home and mobile telephone number(s) stored
    And there are appointments available to book which are of telephone type
    And I am logged in
    And I am on the Available Appointments page
    And I have selected a telephone appointment slot to book
    And the Appointment Slot page is displayed
    When I select the radio button for an alternative phone number to those stored
    And I enter whitespace instead of a phone number for the appointment
    And I enter symptoms
    And I click the 'Confirm and book appointment' button
    Then a message is displayed indicating a phone number is required

    # Regression Test scenarios to ensure booking reason is still validated for telephone appointments

  Scenario: An EMIS user receives an error if a phone number is entered for telephone appointments, but no reason
    Given I wish to book an appointment without specifying a reason
    And I have no telephone number(s) stored
    And there are appointments available to book which are of telephone type
    And I am logged in
    And I am on the Available Appointments page
    And I have selected a telephone appointment slot to book
    And I enter a phone number for the appointment
    When I click the 'Confirm and book appointment' button
    Then an error is displayed that "Describe your symptoms" is mandatory

  Scenario: An appointment is booked for an EMIS user if a phone number is provided for telephone appointments with optional booking reason
    Given I wish to book an appointment without specifying a reason
    And I have no telephone number(s) stored
    And there are appointments available to book which are of telephone type with optional booking reason
    And I am logged in
    And I am on the Available Appointments page
    And I have selected a telephone appointment slot to book
    And I enter a phone number for the appointment
    When I click the 'Confirm and book appointment' button
    Then the Appointment Booking success message is displayed
    And the booked appointment before cutoff time is correctly displayed with ability to cancel

  Scenario: An EMIS user receives an error if a stored phone number is selected for telephone appointments, but no reason entered
    Given I wish to book an appointment without specifying a reason
    And I have both home and mobile telephone number(s) stored
    And there are appointments available to book which are of telephone type
    And I am logged in
    And I am on the Available Appointments page
    And I have selected a telephone appointment slot to book
    When I select the mobile number from available ones
    And I click the 'Confirm and book appointment' button
    Then an error is displayed that "Describe your symptoms" is mandatory

  Scenario: An appointment is booked for an EMIS user if a stored phone number is selected for telephone appointments with optional booking reason
    Given I wish to book an appointment without specifying a reason
    And I have both home and mobile telephone number(s) stored
    And I wish to book a telephone appointment using my mobile phone number
    And there are appointments available to book which are of telephone type with optional booking reason
    And I am logged in
    And I am on the Available Appointments page
    And I have selected a telephone appointment slot to book
    When I select the mobile number from available ones
    And I click the 'Confirm and book appointment' button
    Then the Appointment Booking success message is displayed
    And the booked appointment before cutoff time is correctly displayed with ability to cancel

  Scenario: An EMIS user receives an error if an alternate phone number is selected and entered for telephone appointments, but no reason entered
    Given I wish to book an appointment without specifying a reason
    And I have both home and mobile telephone number(s) stored
    And there are appointments available to book which are of telephone type
    And I am logged in
    And I am on the Available Appointments page
    And I have selected a telephone appointment slot to book
    When I select the radio button for an alternative phone number to those stored
    And I enter a phone number for the appointment
    And I click the 'Confirm and book appointment' button
    Then an error is displayed that "Describe your symptoms" is mandatory

  Scenario: An appointment is booked for an EMIS user if an alternate phone number is selected and entered for telephone appointments with optional booking reason
    Given I wish to book an appointment without specifying a reason
    And I have both home and mobile telephone number(s) stored
    And I wish to book a telephone appointment using my mobile phone number
    And there are appointments available to book which are of telephone type with optional booking reason
    And I am logged in
    And I am on the Available Appointments page
    And I have selected a telephone appointment slot to book
    When I select the radio button for an alternative phone number to those stored
    And I enter a phone number for the appointment
    And I click the 'Confirm and book appointment' button
    Then the Appointment Booking success message is displayed
    And the booked appointment before cutoff time is correctly displayed with ability to cancel


  Scenario Outline: An <GP System> user sees no booking symptoms text box displayed if practice settings disallow booking reasons
    Given there are <GP System> appointments available to book where booking reason option is set not required
    And I am logged in
    And I am on the Available Appointments page
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    And I don't see option to type in booking reason
    Examples:
      | GP System |
      | VISION    |

  @native-smoketest
    Examples:
      | GP System |
      | EMIS      |

  @tech-debt   @NHSO-4061 # covered in Manual Regression Test pack
  Scenario: Booking a appointment when there is no internet connection
    Given I am logged in
    And I am on my appointments page
    And I lose my internet connection
    When an appointment booking is submitted
    Then I see a message indicating user may have connectivity problems
