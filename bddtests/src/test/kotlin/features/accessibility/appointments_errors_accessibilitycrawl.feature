@accessibility
@appointment-errors-accessibility
Feature: appointment errors accessibility

  Scenario Outline: Timeout getting appointment slots is captured
    Given the first request to EMIS for available appointment slots times out but later requests succeed
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    Then I see appropriate warning message for loading time-outs with '<Error Code>'
    And the Errors_AB04C_TimeoutGettingAppointmentSlots page is saved to disk
    Examples:
      | Error Code |
      | ze      |

  Scenario Outline: Appointment slot conflict is captured
    Given there are <GP System> appointments available to book, but the appointment slot has already been booked by somebody else
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I enter symptoms
    And  I click the 'Confirm and book appointment' button
    Then a warning message is displayed indicating that the slot has already been taken
    And the Errors_AB05A_AppointmentSlotsConflict page is saved to disk
    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: Reason for appointment missing is captured
    Given there are <GP System> appointments available to book
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I click the 'Confirm and book appointment' button
    Then an error is displayed that "Describe your symptoms" is mandatory
    And the Errors_AB05B_ReasonForAppointmentMissing page is saved to disk
    Examples:
      | GP System |
      | EMIS      |

  Scenario: Too late to cancel appointment is captured
    Given TPP prevents cancellation of previously booked appointment because it is too late
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    And I select a "Cancel this appointment" link
    And I select "Cancel appointment" button
    Then I see an appropriate error message when it is too late to cancel
    And the Errors_AB07G_TooLateToCancel page is saved to disk

  Scenario Outline: Cannot cancel appointment is captured
    Given <GP System> prevents cancellation of previously booked appointment with '<Reason>' because it is already cancelled
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    And I select a "Cancel this appointment" link
    And I select a cancellation reason of <Reason>
    And I select "Cancel appointment" button
    Then I see an appropriate error message when it is already cancelled
    And the Errors_AB07H_CannotCancelAppointment page is saved to disk
    Examples:
      | Reason           | GP System |
      | Unable to attend | EMIS      |

  Scenario: Timeout getting appointment history is captured
    Given the first request to EMIS for available appointment slots times out but later requests succeed
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    Then I see appropriate warning message for loading time-outs with 'ze'
    And the Errors_AGP09_TimeoutGettingAppointmentHistory page is saved to disk

  Scenario Outline: Appointment booking is unavailable is captured
    Given <GP System> user is not allowed to cancel appointments with '<Reason>'
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    And I select a "Cancel this appointment" link
    And I select a cancellation reason of <Reason>
    And I select "Cancel appointment" button
    Then I see appropriate error message when appointments are disabled
    And the Errors_AGP11_AppointmentBookingUnavaliable page is saved to disk
    Examples:
      | Reason           | GP System |
      | Unable to attend | EMIS      |

  Scenario: Bad request is captured
    Given there are EMIS appointments available to book and user attempts to enter a dangerous booking reason
    And '111' responds to requests for '/home'
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I enter symptoms
    And I click the 'Confirm and book appointment' button
    Then I see appropriate information message when there is an error sending data on appointment confirmation page
    And the Errors_AGP13_AppointmentBookingUnavailable page is saved to disk

  Scenario Outline: GP14 error is captured
    Given there are <GP System> appointments available to book, but GP system doesn't respond a timely fashion when booking
    And 'NHS UK' responds to requests for '/nhs-app-contact-us'
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I enter symptoms
    And  I click the 'Confirm and book appointment' button
    Then I see appropriate submit error message when there is an error with '<Prefix>'
    And the Errors_AGP14 page is saved to disk
    Examples:
      | Prefix | GP System |
      | ze     | EMIS      |

  Scenario: GP15A error is captured
    Given EMIS returns corrupted response once when trying to retrieve my appointments
    And I am logged in
    When I retrieve the 'appointment hub' page directly
    Then the Appointments Hub page is displayed
    When I click the GP Appointments link
    And I see appropriate try again warning message when there is an error with 'xx'
    And the Errors_AGP15A_UnexpectedErrorBookingAppointment page is saved to disk

  Scenario: GP15b error is captured
    Given EMIS is unavailable for available appointment slots
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    Then I see appropriate warning message when there is a loading error with '4e'
    And the Errors_AGP15B_UnexpectedErrorRetrievingAppointmentSlots page is saved to disk
