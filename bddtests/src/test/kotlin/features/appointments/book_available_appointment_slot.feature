@appointment
Feature: Book an available appointment slot
  In order to complete a booking appointment
  As a logged in user
  I want to be able to select, confirm and book selected appointment

  Scenario Outline: Only one appointment slot time is displayed when multiple are available for <GP System>
    Given there are multiple appointment slots at the same time, provided by <GP System>
    And a booked appointment can be cancelled
    And I am logged in
    And I am on the available appointments page
    And I have filtered such that there is one time displayed that represents multiple slots
    And I have selected a time when multiple slots are available
    Then the Appointment Slot page is displayed
    When I enter symptoms
    And  I click the 'Confirm and book appointment' button
    Then the Appointment Booking success message is displayed
    And the booked appointment is correctly displayed with ability to cancel
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
  @pending @NHSO-3145
    Examples:
      | GP System |
      | VISION    |

    # Not applicable for VISION
  Scenario Outline: A <GP System> user cannot book an appointment without describing symptoms
    Given there are <GP System> appointments available to book
    And I am logged in
    And I am on the available appointments page
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
    And I am on the available appointments page
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I enter symptoms of 1 characters
    And I click the 'Confirm and book appointment' button
    Then the Appointment Booking success message is displayed
    And the booked appointment is correctly displayed with ability to cancel
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
  @pending @NHSO-3145
    Examples:
      | GP System |
      | VISION    |

  @smoketest
  Scenario Outline: A <GP System> user can book an appointment describing symptoms no more 150 characters
    Given there are <GP System> appointments available to book with a reason of 150 character
    And a booked appointment can be cancelled
    And I am logged in
    And I am on the available appointments page
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I enter symptoms of 150 characters
    And I click the 'Confirm and book appointment' button
    Then the Appointment Booking success message is displayed
    And the booked appointment is correctly displayed with ability to cancel
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
  @pending @NHSO-3145
    Examples:
      | GP System |
      | VISION    |

  @nativepending @NHSO-2956
    #native issue - length mismatch - false failures
  Scenario Outline: A <GP System> user cannot enter symptoms with over 150 characters
    Given there are <GP System> appointments available to book with a reason of 150 characters but user attempts to enter 151 characters
    And a booked appointment can be cancelled
    And I am logged in
    And I am on the available appointments page
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I enter symptoms of 151 characters
    Then only the first 150 characters will be displayed
    And I click the 'Confirm and book appointment' button
    Then the Appointment Booking success message is displayed
    And the booked appointment is correctly displayed with ability to cancel
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
  @pending @NHSO-3145
    Examples:
      | GP System |
      | VISION    |

  @nativepending @NHSO-2956
    #native issue - length mismatch - false failures
  Scenario Outline: A <GP System> user cannot paste symptoms with over 150 characters
    Given there are <GP System> appointments available to book with a reason of 150 characters but user attempts to enter 151 characters
    And I am logged in
    And I am on the available appointments page
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I paste symptoms of 151 characters
    Then only the first 150 characters will be displayed
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  @nativepending @NHSO-2956
    #native issue - length mismatch
  Scenario Outline: A <GP System> user who books successfully, but only the first 150 characters of the symptoms are sent
    Given there are <GP System> appointments available to book with a reason of 150 characters but user attempts to enter 151 characters
    And a booked appointment can be cancelled
    And I am logged in
    And I am on the available appointments page
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I enter symptoms of 151 characters
    And I click the 'Confirm and book appointment' button
    Then the Appointment Booking success message is displayed
    And the booked appointment is correctly displayed with ability to cancel
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
  @pending @NHSO-3145
    Examples:
      | GP System |
      | VISION    |

  Scenario: A Vision user gets an alternative success message when booking and there's no ability to cancel
    Given there are VISION appointments available to book
    But a booked appointment cannot be cancelled
    And I am logged in
    And I am on the available appointments page
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I enter symptoms
    And I click the 'Confirm and book appointment' button
    Then the Appointment Booking success message is displayed without reference to being able to cancel
    And the booked appointment is correctly displayed without ability to cancel

  Scenario Outline: A <GP System> user cannot enter dangerous text for booking reason
    Given there are <GP System> appointments available to book and user attempts to enter booking reason <script>
    And I am logged in
    And I am on the available appointments page
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

  Scenario Outline: A <GP System> user sees appropriate information message when there is a timeout
    Given there are <GP System> appointments available to book, but GP system doesn't respond a timely fashion when booking
    And I am logged in
    And I am on the available appointments page
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
    And I am on the available appointments page
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I enter symptoms
    And  I click the 'Confirm and book appointment' button
    Then I see appropriate information message when there is an error sending data on appointment confirmation page
    And there should be a button to go back to my appointments
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: A <GP System> user sees appropriate information error message when appointment has already been booked
    Given there are <GP System> appointments available to book, but the appointment slot has already been booked by somebody else
    And I am logged in
    And I am on the available appointments page
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I enter symptoms
    And  I click the 'Confirm and book appointment' button
    Then a message is displayed indicating that the slot has already been taken
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: A <GP System> user can return directly back to their appointments after trying to book one already booked
    Given there are <GP System> appointments available to book, but the appointment slot has already been booked by somebody else
    And I am logged in
    And I am on the available appointments page
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    And I enter symptoms
    And  I click the 'Confirm and book appointment' button
    When I click the error page back button
    Then I am taken to the available appointment slots screen
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: A <GP System> user reached maximum appointment booking limit
    Given there are <GP System> appointments available to book, but user reached maximum appointment booking limit
    And I am logged in
    And I am on the available appointments page
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    And I enter symptoms
    And  I click the 'Confirm and book appointment' button
    Then a message is displayed indicating that user has reached maximum appointment limit
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: A <GP System> user is navigated back to the 'Book this appointment' screen when 'Change this appointment' button selected.
    Given there are <GP System> appointments available to book
    And I am logged in
    And I am on the available appointments page
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I choose to change the appointment slot
    Then there is a filter for the appointment types
    And there is a filter for the appointment locations
    And there is a filter for the appointment doctors/nurses
    And there is a filter for the appointment time period
    And no available slots are displayed
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

# EMIS Specific Scenarios (For EMIS reason necessity)
# The following scenarios covered only Optional and Not-Required reason necessity options.
# The default is mandatory and if the option is not specified in a scenario, it is set to MANDATORY by default.
  Scenario: An EMIS user can book an appointment without describing symptoms
    Given there are EMIS appointments available to book where booking reason is set optional
    And a booked appointment can be cancelled
    And I am logged in
    And I am on the available appointments page
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I click the 'Confirm and book appointment' button
    Then the Appointment Booking success message is displayed
    And the booked appointment is correctly displayed with ability to cancel

  Scenario: An EMIS user can book an appointment with describing symptoms
    Given there are EMIS appointments available to book where booking reason is set optional with a reason entered
    And a booked appointment can be cancelled
    And I am logged in
    And I am on the available appointments page
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I enter symptoms
    And I click the 'Confirm and book appointment' button
    Then the Appointment Booking success message is displayed
    And the booked appointment is correctly displayed with ability to cancel

  Scenario: An EMIS user on Old EMIS System reached maximum appointment booking limit
    Given  there are appointments available to book in old EMIS system, but user reached maximum appointment booking limit
    And I am logged in
    And I am on the available appointments page
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    And I enter symptoms
    And  I click the 'Confirm and book appointment' button
    Then a message is displayed indicating that user has reached maximum appointment limit

  Scenario Outline: An <GP System> user sees no booking symptoms text box displayed if practice settings disallow booking reasons
    Given there are <GP System> appointments available to book where booking reason option is set not required
    And I am logged in
    And I am on the available appointments page
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    And I don't see option to type in booking reason
    Examples:
      | GP System |
      | EMIS      |
      | VISION    |
