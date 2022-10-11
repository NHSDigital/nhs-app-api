@appointments
@wayfinder
@sharedSteps
Feature: Wayfinder Help

  Scenario: A user with access to Appointments and Referrals can follow the Missing, incorrect or not cancelled referrals or appointments help link then read the correct text on the page and return
    Given I am a user whose surgery has enabled Wayfinder
    And I have referrals and upcoming appointments
    And I am logged in
    When I navigate to the Appointment Hub page
    Then the Appointments Hub page is displayed
    When I click the 'Referrals, hospital and other appointments' link on the Appointments Hub
    Then the Referrals, hospital and other appointments screen with data is displayed
    And I see the Missing, incorrect or not cancelled referrals or appointments link
    When I click the Missing, incorrect or not cancelled referrals or appointments link
    Then I am navigated to the Wayfinder help page
    When I click the Back link
    Then the Referrals, hospital and other appointments screen with data is displayed

  Scenario: A user with access to Appointments can follow the Missing, incorrect or not cancelled appointments help link then read the correct text on the page and return
    Given I am a user whose surgery has enabled Wayfinder
    And I have referrals and upcoming appointments
    And I am logged in
    When I navigate to the Appointment Hub page
    Then the Appointments Hub page is displayed
    When I click the 'Referrals, hospital and other appointments' link on the Appointments Hub
    Then the Referrals, hospital and other appointments screen with data is displayed
    And I see the Missing, incorrect or not cancelled appointments link
    When I click the Missing, incorrect or not cancelled appointments link
    Then I am navigated to the Wayfinder help page
    When I click the Back link
    Then the Referrals, hospital and other appointments screen with data is displayed

  Scenario: A user with access to Referrals can follow the Missing or incorrect referrals in review help link then read the correct text on the page and return
    Given I am a user whose surgery has enabled Wayfinder
    And I have referrals and upcoming appointments
    And I am logged in
    When I navigate to the Appointment Hub page
    Then the Appointments Hub page is displayed
    When I click the 'Referrals, hospital and other appointments' link on the Appointments Hub
    Then the Referrals, hospital and other appointments screen with data is displayed
    And I see the Missing or incorrect referrals in review link
    When I click the Missing or incorrect referrals in review link
    Then I am navigated to the Wayfinder help page
    When I click the Back link
    Then the Referrals, hospital and other appointments screen with data is displayed

  Scenario: A user can visit the wayfinder help page then click the breadcrumb to return to secondary care summary page
    Given I am a user whose surgery has enabled Wayfinder
    And I have referrals and upcoming appointments
    And I am logged in
    When I navigate to the Appointment Hub page
    Then the Appointments Hub page is displayed
    When I click the 'Referrals, hospital and other appointments' link on the Appointments Hub
    Then the Referrals, hospital and other appointments screen with data is displayed
    And I see the Missing or incorrect referrals in review link
    When I click the Missing or incorrect referrals in review link
    Then I am navigated to the Wayfinder help page
    When I click the 'Referrals, hospital and other appointments' breadcrumb
    Then the Referrals, hospital and other appointments screen with data is displayed
