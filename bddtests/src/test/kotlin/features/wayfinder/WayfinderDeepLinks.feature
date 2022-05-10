@appointments
@wayfinder
Feature: Wayfinder DeepLinks

  Scenario: A user with an eRS referral can follow the deep link without seeing the third party warning screen
    Given I am a user who can view Wayfinder from Appointments and has eRS referrals and upcoming appointments
    And I am logged in
    When I navigate to Appointments
    Then the Appointments Hub page is displayed
    When I click the 'Referrals, hospital and other appointments' link on the Appointments Hub
    Then the Referrals, hospital and other appointments screen with data is displayed
    And I see an appointment to confirm
    When I click the 'Manage this referral' button
    Then I am navigated to a third party site

  Scenario: A user with a PKB referral can follow the deep link without seeing the third party warning screen
    Given I am using the native app user agent
    And I am an eRS user who can view Wayfinder from Appointments and has PKB referrals and upcoming appointments
    And I am logged in
    When I navigate to the Appointment Hub page
    Then the Appointments Hub page is displayed
    When I click the 'Referrals, hospital and other appointments' link on the Appointments Hub
    Then the Referrals, hospital and other appointments screen with data is displayed
    And I see an appointment to confirm
    When I click the 'Manage this referral' button
    Then I am redirected to the redirector page with the header 'View appointments'
