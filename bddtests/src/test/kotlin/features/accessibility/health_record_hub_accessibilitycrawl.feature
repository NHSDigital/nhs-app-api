@accessibility
Feature: Health record hub page accessibility
  Scenario: Health record hub page is captured
    Given I am an EMIS patient and I have access to Patients Know Best Care Plans
    And I am logged in
    When I navigate to the health record hub page
    Then I see the health records hub page
    And the HEALTH_RECORD_HUB page is saved to disk
