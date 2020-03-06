@accessibility
  Feature: Appointment hub accessibility

    Scenario: The Hospital Appointments Page is captured
      Given I am a user who can manage their hospital appointments
      And I am logged in
      When I retrieve the 'Appointment Hub' page directly
      Then the Appointments Hub page is displayed
      Then the Appointment_Hub page is saved to disk
      When I click the 'Hospital and other services' link on the Appointments Hub
      Then the Hospital Appointments page is displayed
      Then the Manage_Your_Referrals page is saved to disk