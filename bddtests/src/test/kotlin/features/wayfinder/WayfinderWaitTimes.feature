@appointments
@wayfinder
@sharedSteps
Feature: Wayfinder Wait Times

  Scenario: A user with access to secondary care summary screen and wait times enabled can follow the wait times link to open the wait times page
    Given I am a user whose surgery has enabled Wayfinder
    And I have referrals and upcoming appointments
    And I have wait times available
    And I am logged in
    When I navigate to the Appointment Hub page
    Then the Appointments Hub page is displayed
    When I click the 'Referrals, hospital and other appointments' link on the Appointments Hub
    Then the Referrals, hospital and other appointments screen with data is displayed
    And  I see the wait times link
    When I click the wait times jump off link
    Then I am navigated to the Wayfinder wait times page
