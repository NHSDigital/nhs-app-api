@appointments
@appointments-book
@book
Feature: Book Appointments Frontend
  In order to complete a booking appointment
  As a logged in user
  I want to be able to select, confirm and book selected appointment

  #This test covers navigation via buttons/links

  Scenario Outline: A <GP System> user can navigate to the available appointments page
    Given there are multiple appointment slots at the same time, provided by <GP System>
    And a booked appointment can be cancelled
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    Then the Available Appointments page is displayed
    Examples:
      | GP System |
      | EMIS      |

  # These tests navigate directly to the pages where the features are to be tested, to save time.

  Scenario Outline: Only one appointment slot time is displayed when multiple are available for <GP System>
    Given there are multiple appointment slots at the same time, provided by <GP System>
    And a booked appointment can be cancelled
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I have filtered such that there is one time displayed that represents multiple slots
    And I have selected a time when multiple slots are available
    Then the Appointment Slot page is displayed
    When I enter symptoms
    And  I click the 'Confirm and book appointment' button
    Then the Appointment Booking success page is displayed
    And I select the back to home link on the appointments page
    # NHSO-8593: Changes below linked to this bug that caused an infinite loop on the back link in native
    # Will need removing/altering when either a fix is complete or story to add appointment details to the
    # success page is done.
    And the Appointment Hub page is displayed
    # And the booked appointment before cutoff time is correctly displayed with ability to cancel
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  # Not applicable for VISION
  Scenario Outline: A <GP System> user cannot book an appointment without describing symptoms
    Given there are <GP System> appointments available to book
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I click the 'Confirm and book appointment' button
    Then an error is displayed that "Describe your symptoms" is mandatory
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | MICROTEST |

  Scenario Outline: A <GP System> user can book an appointment describing symptoms
    Given there are <GP System> appointments available to book with a reason
    And a booked appointment can be cancelled
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I enter symptoms
    And I click the 'Confirm and book appointment' button
    Then the Appointment Booking success page is displayed
    And I select the back to home link on the appointments page
    # NHSO-8593: Changes below linked to this bug that caused an infinite loop on the back link in native
    # Will need removing/altering when either a fix is complete or story to add appointment details to the
    # success page is done.
    And the Appointment Hub page is displayed
    # And the booked appointment before cutoff time is correctly displayed with ability to cancel
    Examples:
      | GP System |
      | EMIS      |
      | VISION    |
      | MICROTEST |
    @smoketest
    Examples:
      | GP System |
      | TPP       |

  Scenario: A Vision user gets an alternative success message when booking and there's no ability to cancel
    Given there are VISION appointments available to book
    But a booked appointment cannot be cancelled
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And the Available Appointments page is displayed
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I enter symptoms
    And I click the 'Confirm and book appointment' button
    Then the Appointment Booking success page is displayed without reference to being able to cancel
    And I select the back to home link on the appointments page
    # NHSO-8593: Changes below linked to this bug that caused an infinite loop on the back link in native
    # Will need removing/altering when either a fix is complete or story to add appointment details to the
    # success page is done.
    And the Appointment Hub page is displayed
    # And the booked appointment is correctly displayed without ability to cancel

  #400
  Scenario Outline: A <GP System> user cannot enter dangerous text for booking reason
    Given there are <GP System> appointments available to book and user attempts to enter a dangerous booking reason
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I enter symptoms
    And I click the 'Confirm and book appointment' button
    Then I see appropriate information message when there is an error sending data on appointment confirmation page
    When I click the error '111.nhs.uk' link with a url of 'https://111.nhs.uk'
    Then a new tab has been opened by the link
    Examples:
      | GP System |
      | EMIS      |

  #409
  @nativesmoketest
  Scenario Outline: A <GP System> user sees appropriate information error message when appointment has already been booked
    Given there are <GP System> appointments available to book, but the appointment slot has already been booked by somebody else
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I enter symptoms
    And  I click the 'Confirm and book appointment' button
    Then a message is displayed indicating that the slot has already been taken
    When I click the error 'Back' link
    Then the Your Appointments page is displayed
    Examples:
      | GP System |
      | EMIS      |

  #460
  @nativesmoketest
  Scenario Outline: A <GP System> user reached maximum appointment booking limit
    Given there are <GP System> appointments available to book, but user reached maximum appointment booking limit
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    And I enter symptoms
    And  I click the 'Confirm and book appointment' button
    And a message is displayed indicating that user has reached maximum appointment limit
    When I click the error 'Back' link
    Then the Your Appointments page is displayed
    Examples:
      | GP System |
      | TPP       |

  #460
  Scenario: An EMIS user on Old EMIS System reached maximum appointment booking limit
    Given  there are appointments available to book in old EMIS system, but user reached maximum appointment booking limit
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    And I enter symptoms
    And  I click the 'Confirm and book appointment' button
    And a message is displayed indicating that user has reached maximum appointment limit

  #500
  Scenario Outline: <GP System> user sees appropriate error message when it returns corrupt data
    Given <GP System> returns corrupt data when booking appointment
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I enter symptoms
    And  I click the 'Confirm and book appointment' button
    Then I see appropriate submit error message when there is an error with '<Prefix>'
    When I click the error 'Back' link
    Then the Your Appointments page is displayed
    Examples:
      | Prefix | GP System |
      | xx     | TPP       |

  #502
  @nativesmoketest
  Scenario Outline: A <GP System> user sees appropriate information message when GP system is unavailable
    Given there are <GP System> appointments available to book, but the GP system is unavailable
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I enter symptoms
    And  I click the 'Confirm and book appointment' button
    Then I see appropriate submit error message when there is an error with '<Prefix>'
    When I click the error 'Contact us' link with a url of 'https://www.nhs.uk/contact-us/nhs-app-contact-us'
    Then a new tab has been opened by the link
    Examples:
      | Prefix | GP System |
      | 4s     | VISION    |


  #504
  Scenario Outline: A <GP System> user sees appropriate information message when there is a timeout
    Given there are <GP System> appointments available to book, but GP system doesn't respond a timely fashion when booking
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I enter symptoms
    And  I click the 'Confirm and book appointment' button
    Then I see appropriate submit error message when there is an error with '<Prefix>'
    When I click the error 'Contact us' link with a url of 'https://www.nhs.uk/contact-us/nhs-app-contact-us'
    Then a new tab has been opened by the link
    Examples:
      | Prefix | GP System |
      | ze     | EMIS      |

  Scenario Outline: A <GP System> user is navigated back to the 'Book this appointment' screen when Back button selected.
    Given there are <GP System> appointments available to book
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I click the Back link
    Then there is a filter for the appointment types
    And there is a filter for the appointment locations
    And there is a filter for the appointment doctors/nurses
    And there is a filter for the appointment time period
    And no available slots are displayed
    Examples:
      | GP System |
      | TPP       |
      | VISION    |
      | MICROTEST |
      | EMIS      |


# EMIS Specific Scenarios (For EMIS reason necessity)
# The following scenarios covered only Optional and Not-Required reason necessity options.
# The default is mandatory and if the option is not specified in a scenario, it is set to MANDATORY by default.
  Scenario: An EMIS user can book an appointment without describing symptoms
    Given there are EMIS appointments available to book where booking reason is set optional
    And a booked appointment can be cancelled
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I click the 'Confirm and book appointment' button
    Then the Appointment Booking success page is displayed
    And I select the back to home link on the appointments page
    # NHSO-8593: Changes below linked to this bug that caused an infinite loop on the back link in native
    # Will need removing/altering when either a fix is complete or story to add appointment details to the
    # success page is done.
    And the Appointment Hub page is displayed
    # And the booked appointment before cutoff time is correctly displayed with ability to cancel

  Scenario: An EMIS user can book an appointment with describing symptoms
    Given there are EMIS appointments available to book where booking reason is set optional with a reason entered
    And a booked appointment can be cancelled
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I enter symptoms
    And I click the 'Confirm and book appointment' button
    Then the Appointment Booking success page is displayed
    And I select the back to home link on the appointments page
    # NHSO-8593: Changes below linked to this bug that caused an infinite loop on the back link in native
    # Will need removing/altering when either a fix is complete or story to add appointment details to the
    # success page is done.
    And the Appointment Hub page is displayed
    # And the booked appointment before cutoff time is correctly displayed with ability to cancel

    # Positive submission cases
  Scenario: An EMIS user cannot enter or select a phone number for non phone appointments
    Given there are EMIS appointments available to book
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    And I do not see a text input to enter phone number
    And I do not see any phone numbers to select

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
    @nativesmoketest
    Examples:
      | GP System |
      | EMIS      |


  @pending
  @native
  Scenario: An EMIS user can book an appointment and add it to the native calendar
    Given there are EMIS appointments available to book where booking reason is set optional
    And a booked appointment can be cancelled
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I click the 'Confirm and book appointment' button
    Then the Appointment Booking success page is displayed
    And I click on the Add to calendar link
    And the Add to calendar interrupt page is displayed
    When I click on the Add to calendar button
    # TODO - figure out how to get browerstack and the native calendar working together ??
    #Then the native calendar is shown
    #And I save the appointment to the calendar
    #And the Appointment Hub page is displayed

