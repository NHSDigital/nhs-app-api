@accessibility
  Feature: Appointment hub accessibility

    Scenario: The Appointments hub page is captured

      Given I am a EMIS patient
      When I am logged in
      When I retrieve the 'Appointment Hub' page directly
      Then the Appointment_Hub page is saved to disk

