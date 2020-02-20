@appointmentsHub
Feature: Appointments Hub Frontend
  Users can see the Appointments Hub page and use thethird party silver partner links

  Scenario: An Emis can see the Appointments Hub page and navigate to GP Appointments
    Given I have no booked appointments for EMIS
    And there are EMIS appointments available to book
    And I am logged in
    When I navigate to Appointments
    Then the Appointments Hub page is displayed
    And I click the GP Appointments link
    Then the Your Appointments page is displayed
