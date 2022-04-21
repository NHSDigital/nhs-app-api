@appointments
@wayfinder
Feature: Wayfinder Referrals

  Scenario: A user with no referrals or appointments can see the Appointments and Referrals screen
    Given I am a user who can view Wayfinder from Appointments
    And I am logged in
    When I navigate to Appointments
    Then the Appointments Hub page is displayed
    When I click the 'Referrals, hospital and other appointments' link on the Appointments Hub
    Then the Referrals, hospital and other appointments screen without data is displayed

  Scenario: A user with in review referrals can see them in the Appointments and Referrals screen
    Given I am a user who can view Wayfinder from Appointments and has referrals and upcoming appointments
    And I am logged in
    When I navigate to Appointments
    Then the Appointments Hub page is displayed
    When I click the 'Referrals, hospital and other appointments' link on the Appointments Hub
    Then the Referrals, hospital and other appointments screen with data is displayed
    And I see an in review referral

  Scenario: A user with bookable cancelled referrals can see them in the Appointments and Referrals screen
    Given I am a user who can view Wayfinder from Appointments and has referrals and upcoming appointments
    And I am logged in
    When I navigate to Appointments
    Then the Appointments Hub page is displayed
    When I click the 'Referrals, hospital and other appointments' link on the Appointments Hub
    Then the Referrals, hospital and other appointments screen with data is displayed
    And I see a bookable cancelled referral

  Scenario: A user with overdue referrals can see them in the Appointments and Referrals screen
    Given I am a user who can view Wayfinder from Appointments and has referrals and upcoming appointments
    And I am logged in
    When I navigate to Appointments
    Then the Appointments Hub page is displayed
    When I click the 'Referrals, hospital and other appointments' link on the Appointments Hub
    Then the Referrals, hospital and other appointments screen with data is displayed
    And I see an overdue referral

  Scenario: A user with referrals awaiting booking can see them in the Appointments and Referrals screen
    Given I am a user who can view Wayfinder from Appointments and has referrals and upcoming appointments
    And I am logged in
    When I navigate to Appointments
    Then the Appointments Hub page is displayed
    When I click the 'Referrals, hospital and other appointments' link on the Appointments Hub
    Then the Referrals, hospital and other appointments screen with data is displayed
    And I see a referral awaiting booking

  Scenario: A user with referrals awaiting booking can see them in the Wayfinder screen and can go back to the appointments hub
    Given I am a user who can view Wayfinder from Appointments and has referrals and upcoming appointments
    And I am logged in
    When I navigate to Appointments
    Then the Appointments Hub page is displayed
    When I click the 'Referrals, hospital and other appointments' link on the Appointments Hub
    And the Referrals, hospital and other appointments screen with data is displayed
    Then I see a referral awaiting booking
    When I click the Back link
    Then the Appointments Hub page is displayed

  Scenario: A user with an in review referral with no speciality does not see the speciality referenced
    Given I am a user who can view Wayfinder from Appointments and has referrals and upcoming appointments
    And I am logged in
    When I navigate to Appointments
    Then the Appointments Hub page is displayed
    When I click the 'Referrals, hospital and other appointments' link on the Appointments Hub
    And the Referrals, hospital and other appointments screen with data is displayed
    Then I can see the InReview referral with no speciality referenced

  Scenario: A user with a ready to rebook referral with no speciality does not see the speciality referenced
    Given I am a user who can view Wayfinder from Appointments and has referrals and upcoming appointments
    And I am logged in
    When I navigate to Appointments
    Then the Appointments Hub page is displayed
    When I click the 'Referrals, hospital and other appointments' link on the Appointments Hub
    And the Referrals, hospital and other appointments screen with data is displayed
    Then I can see the ReadyToRebook referral with no speciality referenced


