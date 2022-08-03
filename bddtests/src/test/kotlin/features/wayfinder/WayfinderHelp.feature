@appointments
@wayfinder
@sharedSteps
Feature: Wayfinder Help

  Scenario: A user with access to Appointments can follow the Missing, incorrect or cancelled referrals or appointments help link then read the correct text on the page and return
    Given I am a user whose surgery has enabled Wayfinder
    And I have referrals and upcoming appointments
    And I am logged in
    When I navigate to the Appointment Hub page
    Then the Appointments Hub page is displayed
    When I click the 'Referrals, hospital and other appointments' link on the Appointments Hub
    Then the Referrals, hospital and other appointments screen with data is displayed
    And I see the Missing, incorrect or cancelled referrals or appointments link
    When I click the Missing, incorrect or cancelled referrals or appointments link
    Then I am navigated to the help page for Missing, incorrect or cancelled referrals or appointments
    When I click the Back link
    Then the Referrals, hospital and other appointments screen with data is displayed

  Scenario: A user with access to Appointments can follow the Missing, incorrect or cancelled confirmed appointments help link then read the correct text on the page and return
    Given I am a user whose surgery has enabled Wayfinder
    And I have referrals and upcoming appointments
    And I am logged in
    When I navigate to the Appointment Hub page
    Then the Appointments Hub page is displayed
    When I click the 'Referrals, hospital and other appointments' link on the Appointments Hub
    Then the Referrals, hospital and other appointments screen with data is displayed
    And I see the Missing, incorrect or cancelled confirmed appointments link
    When I click the Missing, incorrect or cancelled confirmed appointments link
    Then I am navigated to the help page for Missing, incorrect or cancelled confirmed appointments
    When I click the Back link
    Then the Referrals, hospital and other appointments screen with data is displayed

  Scenario: A user with access to Appointments can follow the Missing, incorrect or cancelled referrals in review help link then read the correct text on the page and return
    Given I am a user whose surgery has enabled Wayfinder
    And I have referrals and upcoming appointments
    And I am logged in
    When I navigate to the Appointment Hub page
    Then the Appointments Hub page is displayed
    When I click the 'Referrals, hospital and other appointments' link on the Appointments Hub
    Then the Referrals, hospital and other appointments screen with data is displayed
    And I see the Missing, incorrect or cancelled referrals in review link
    When I click the Missing, incorrect or cancelled referrals in review link
    Then I am navigated to the help page for Missing, incorrect or cancelled referrals in review
    When I click the Back link
    Then the Referrals, hospital and other appointments screen with data is displayed
