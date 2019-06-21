@accessibility
@appointment-accessibility
  Feature: Appointment accessibility

    Scenario: The MyAppointments page is captured with no appointments
      Given I have no booked appointments for EMIS
      And there are EMIS appointments available to book
      And I am logged in
      When I retrieve the 'My Appointments' page directly
      Then the MyAppointments_NoAppointments page is saved to disk
      When I retrieve the 'Appointment Booking' page directly
      And I have selected an appointment slot to book
      Then the Appointment Slot page is displayed
      Then the AppointmentSlot_NonPhone page is saved to disk

    Scenario: The appointment cancellation page is captured
      Given EMIS is available to cancel a previously booked appointment before cutoff time because No longer required
      And I am logged in
      When I retrieve the 'My Appointments' page directly
      Then the MyAppointments_WithAppointments page is saved to disk
      And I select a "Cancel this appointment" link
      Then I will be on the "Cancellation reason" screen
      Then the CancelAppointments page is saved to disk
      And I select a cancellation reason of No longer required
      When I select "Cancel appointment" button
      Then the My Appointments page is displayed
      Then the CancelConfirmation page is saved to disk

    Scenario: The appointment booking pages are captured
      Given I have only first telephone number(s) stored for EMIS
      And I wish to book a telephone appointment using my first phone number
      And there are appointments available to book which are of telephone type for EMIS
      And I am logged in
      When I retrieve the 'Appointment Booking' page directly
      Then the AppointmentBooking page is saved to disk
      When I have selected a telephone appointment slot to book
      Then the Appointment Slot page is displayed
      Then the AppointmentSlot_Telephone page is saved to disk
      When I select the first number from available ones
      And I enter symptoms
      And I click the 'Confirm and book appointment' button
      Then the Appointment Booking success message is displayed
      Then the BookingConfirmation page is saved to disk