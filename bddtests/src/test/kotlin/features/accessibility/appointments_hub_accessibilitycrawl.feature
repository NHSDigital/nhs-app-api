@accessibility
@appointments-hub-accessibility
Feature: Appointments Hub accessibility

  Scenario: The 'Appointments hub' and 'Manage your referrals' Page are captured
    Given I am a user who can manage their hospital appointments
    And I am logged in
    When I retrieve the 'Appointment Hub' page directly
    Then the Appointments Hub page is displayed
    And the Appointments_Hub page is saved to disk
    When I click the 'Hospital and other services' link on the Appointments Hub
    Then the Hospital Appointments page is displayed
    And the Appointments_HospitalAndOtherAppointments page is saved to disk

  Scenario: The Appointments - 'No Upcoming Appointments' and 'Confirmation' pages are captured
    Given I have no booked appointments for EMIS
    And there are EMIS appointments available to book
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    And the page title is 'Your GP appointments'
    Then the Appointments_NoUpcomingAppointments page is saved to disk
    When I retrieve the 'Appointment Booking' page directly
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    And the Appointments_ConfirmAppointmentNonPhone page is saved to disk

  Scenario: The appointments cancellation journey and confirmation pages are captured
    Given EMIS is available to cancel a previously booked appointment before cutoff time because No longer required
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    And the page title is 'Your GP appointments'
    Then the Appointments_WithAtleastOneUpcomingAppointment page is saved to disk
    And I select a "Cancel this appointment" link
    And I will be on the "Cancellation reason" screen
    And the Appointments_CancelAppointment page is saved to disk
    And I select a cancellation reason of No longer required
    When I select "Cancel appointment" button
    Then the Appointment Cancel success page is displayed
    And the Appointments_CancelledConfirmation page is saved to disk

  Scenario: The appointments booking journey and confirmation pages are captured
    Given I wish to book a EMIS telephone appointment
      | number of stored telephone numbers | 1         |
      | the reason on the appointment is   | mandatory |
      | selecting telephone number         | select    |
      | symptoms to enter                  | yes       |
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    Then the page title is 'Book a GP appointment'
    And the Appointments_BookAGPAppointment page is saved to disk
    When I have selected a telephone appointment slot to book
    Then the Appointment Slot page is displayed
    And the Appointments_ConfirmTelephoneAppointment page is saved to disk
    When I select a telephone number to book an appointment
    And I enter symptoms
    And I click the 'Confirm and book appointment' button
    Then the Appointment Booking success page is displayed
    And the Appointments_BookingConfirmation page is saved to disk

  Scenario: The 'Appointments proxy shutter' page is captured
    Given I am logged in as a TPP user with linked profiles but no access to core services and appointments provider IM1
    And the scenario is submit prescription
    Then I see the home page
    When I have switched to a linked profile
    And prescriptions is disabled for the proxy account at a GP Practice level
    And the GP Practice has disabled proxy access to summary care record functionality
    And the GP Practice has disabled proxy access to dcr events functionality for TPP
    And TPP user is not allowed to view appointments
    Then I see the proxy home page
    When I navigate to Appointments
    Then the Appointments Hub page is displayed
    And I click the GP Appointments link
    And the appointments shutter page is displayed
    And the Appointments_ProxyShutter page is saved to disk

  Scenario: The 'Appointments hub - prove your identity shutter' page for a 'P5 user' is captured
    Given I am a patient with proof level 5
    And I am logged in
    When I retrieve the 'Appointment Hub' page directly
    Then the page title is 'Appointments'
    And the Appointments_ProveYourIdentityShutter page is saved to disk

  Scenario: The 'No appointments available matching filter' page is captured
    Given there are available appointment slots with different criteria for TPP
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I select options from the filters that don't yield any results
    Then a message is displayed indicating there are no slots for selected criteria
    And the Appointments_NoAppointmentsAvailableMatchingFilter page is saved to disk

  Scenario: The 'Zero appointments available matching filter' page is captured
    Given there are no available appointment slots for TPP
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    Then I see a message informing me that the GP has no online appointments available and what to do next
    And the Appointments_ZeroAppointmentsAvailableToBookOnline page is saved to disk

  Scenario: The 'Appointment limit reached' page is captured
    Given  there are appointments available to book in old EMIS system, but user reached maximum appointment booking limit
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    And I enter symptoms
    And  I click the 'Confirm and book appointment' button
    And a message is displayed indicating that user has reached maximum appointment limit
    And the Appointments_LimitReached page is saved to disk

  Scenario: The 'Appointments GP session error - temporary problem' page is captured
    Given I am an EMIS user with no booked appointments
    And there are EMIS appointments available to book
    And I am logged in
    And EMIS GP appointments returns unauthorized
    When I navigate to Appointments
    And I click the GP Appointments link
    Then I see appropriate warning message when there is no GP session
    And the Appointments_GPSessionError page is saved to disk

  Scenario: The 'Appointments GP session error - what you can do next' page is captured
    Given TPP user is not allowed to view appointments
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    Then I see appropriate error message when appointments are disabled
    And the Appointments_GPSessionErrorWhatYouCanDoNext page is saved to disk

  Scenario: The 'Main user has cancelled another person's appointment in proxy mode' page is captured
    Given I am logged in as a EMIS user with linked profiles and appointments provider IM1
    Then I see the home page
    When I can see and follow the Linked profiles link
    Then the linked profiles page is displayed
    And linked profiles are displayed
    When I select a linked profile
    Then details for the selected linked profile are displayed
    When I click the Switch to this profile button for the proxy user
    Then I see the proxy home page
    And EMIS is available to cancel a previously booked appointment before cutoff time because No longer required
    And I navigate to Appointments
    And the Appointments Hub page is displayed
    And I click the GP Appointments link
    And the page title is "Your GP appointments"
    And I select a "Cancel this appointment" link
    And I select a cancellation reason of No longer required
    When I select "Cancel appointment" button
    Then The appointment cancellation success page is shown
    And the Appointments_CancelAppointmentInProxyMode page is saved to disk

  Scenario: The 'Main user has booked an appointment for another person in proxy mode' page is captured
    Given I am logged in as a TPP user with linked profiles and appointments provider IM1
    Then I see the home page
    When I can see and follow the Linked profiles link
    Then the linked profiles page is displayed
    And linked profiles are displayed
    When I select a linked profile
    Then details for the selected linked profile are displayed
    When I click the Switch to this profile button for the proxy user
    Then I see the proxy home page
    And there are TPP appointments available to book with a reason
    And I have no booked appointments for TPP
    And I navigate to Appointments
    And the Appointments Hub page is displayed
    And I click the GP Appointments link
    And the page title is "Your GP appointments"
    When I select "Book an appointment" button
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I enter symptoms
    And I click the 'Confirm and book appointment' button
    Then The appointment booking success page is shown
    And the Appointments_BookingConfirmationInProxyMode page is saved to disk
