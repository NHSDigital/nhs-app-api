@appointments
@wayfinder
Feature: Wayfinder Appointments

  Scenario: A user with appointments to confirm can see them in the Appointments and Referrals screen
    Given I am a user whose surgery has enabled Wayfinder
    And I have referrals and upcoming appointments
    And I am logged in
    When I navigate to Appointments
    Then the Appointments Hub page is displayed
    When I click the 'Referrals, hospital and other appointments' link on the Appointments Hub
    Then the Referrals, hospital and other appointments screen with data is displayed
    And I see an appointment ready to confirm

  @smoketest
  Scenario: A user with booked appointments can see them in the Appointments and Referrals screen
    Given I am a user whose surgery has enabled Wayfinder
    And I have referrals and upcoming appointments
    And I am logged in
    When I navigate to Appointments
    Then the Appointments Hub page is displayed
    When I click the 'Referrals, hospital and other appointments' link on the Appointments Hub
    Then the Referrals, hospital and other appointments screen with data is displayed
    And I see a booked appointment

  Scenario: A user with no confirmed appointments can see this on the wayfinder screen
    Given I am a user whose surgery has enabled Wayfinder
    And I have referrals but no upcoming appointments
    And I am logged in
    When I navigate to Appointments
    Then the Appointment Hub page is displayed
    When I click the 'Referrals, hospital and other appointments' link on the Appointments Hub
    Then the Referrals, hospital and other appointments screen with data is displayed
    And I can see that I have no confirmed appointments

  Scenario: A user with cancelled appointments can see this on the wayfinder screen
    Given I am a user whose surgery has enabled Wayfinder
    And I have referrals and upcoming appointments
    And I am logged in
    When I navigate to Appointments
    Then the Appointment Hub page is displayed
    When I click the 'Referrals, hospital and other appointments' link on the Appointments Hub
    Then the Referrals, hospital and other appointments screen with data is displayed
    And I see a cancelled appointment
