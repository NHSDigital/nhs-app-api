@help
Feature: Help pages

  Scenario: A user can get help about appointments
    Given I am an EMIS patient
    And I am logged in
    And I navigate to appointments
    And I click help and support
    Then I see the appointments help page

  Scenario: A user can get help about prescriptions
    Given I am an EMIS patient
    And I am logged in
    And I navigate to prescriptions
    And I click help and support
    Then I see the prescriptions help page

  Scenario: A user can get help about messaging
    Given I am an EMIS patient
    And I am logged in
    And I navigate to messages
    And I click help and support
    Then I see the messaging help page

  Scenario: A user can get help about your health
    Given I am an EMIS patient
    And I am logged in
    And I navigate to your_health
    And I click help and support
    Then I see the your health help page

  Scenario: A user can get help with referrals and upcoming appointments
    Given I am a user whose surgery has enabled Wayfinder
    And I have referrals and upcoming appointments
    And I am logged in
    When I navigate to Appointments
    Then the Appointments Hub page is displayed
    When I click the 'Referrals, hospital and other appointments' link on the Appointments Hub
    And I click help and support
    Then I see the referrals and upcoming appointments help page

