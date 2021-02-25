@prescriptionsHub
Feature: Prescriptions Hub

  Scenario: A user with no access to PKB cannot see the Medicines link on the Prescriptions Hub
    Given I am a user who cannot view Medicines from Patients Know Best
    And I am logged in
    Then I see the home page
    Given I navigate to the your prescriptions page
    Then the Prescriptions Hub page is displayed
    And the PKB View Medicines link is not available on the Prescriptions Hub

  Scenario: A user with no access to CIE cannot see the Medicines link on the Prescriptions Hub
    Given I am a user who cannot view Medicines from Care Information Exchange
    And I am logged in
    Then I see the home page
    Given I navigate to the your prescriptions page
    Then the Prescriptions Hub page is displayed
    And the CIE View Medicines link is not available on the Prescriptions Hub

  Scenario: A user with no access to PKB Secondary Care cannot see the Medicines link on the Prescriptions Hub
    Given I am a user who cannot view Medicines from PKB Secondary Care
    And I am logged in
    Then I see the home page
    Given I navigate to the your prescriptions page
    Then the Prescriptions Hub page is displayed
    And the PKB Secondary Care View Medicines link is not available on the Prescriptions Hub

  Scenario: A user with no access to PKB My Care View cannot see the Medicines link on the Prescriptions Hub
    Given I am a user who cannot view Medicines from PKB My Care View
    And I am logged in
    Then I see the home page
    Given I navigate to the your prescriptions page
    Then the Prescriptions Hub page is displayed
    And the PKB My Care View Medicines link is not available on the Prescriptions Hub
