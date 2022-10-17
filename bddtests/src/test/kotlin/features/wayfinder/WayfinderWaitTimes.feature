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
    Then the Wayfinder wait times page is displayed

  Scenario: A user with access to wait times page can click the help link to visit help page then click back to return to wait times
    Given I am a user whose surgery has enabled Wayfinder
    And I have referrals and upcoming appointments
    And I have wait times available
    And I am logged in
    When I navigate to the Wayfinder Wait Times page
    Then the Wayfinder wait times page is displayed
    And I see the help link on the wait times page
    When I click the help link from wait times page
    Then I am navigated to the Wayfinder help page
    When I click the Back link
    Then the Wayfinder wait times page is displayed

  Scenario: A user with access to wait times page can click the help link to visit help page then click the breadcrumb to return to wait times
    Given I am a user whose surgery has enabled Wayfinder
    And I have referrals and upcoming appointments
    And I have wait times available
    And I am logged in
    When I navigate to the Wayfinder Wait Times page
    Then the Wayfinder wait times page is displayed
    And I see the help link on the wait times page
    When I click the help link from wait times page
    Then I am navigated to the Wayfinder help page
    When I click the 'Waiting Lists' breadcrumb
    Then the Wayfinder wait times page is displayed

  Scenario: A user with access to wait times page page can follow the wait times link and see the error page
    Given I am a user whose surgery has enabled Wayfinder
    And I have referrals and upcoming appointments
    And I get a wait times error
    And I am logged in
    When I navigate to the Appointment Hub page
    Then the Appointments Hub page is displayed
    When I click the 'Referrals, hospital and other appointments' link on the Appointments Hub
    Then the Referrals, hospital and other appointments screen with data is displayed
    And  I see the wait times link
    When I click the wait times jump off link
    Then I am navigated to the Wayfinder wait times error page
    And I see the correct error page elements
    When I click the Back link
    Then the Referrals, hospital and other appointments screen with data is displayed
