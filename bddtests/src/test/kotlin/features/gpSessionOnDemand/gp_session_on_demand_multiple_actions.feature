@gp_session_on_demand
Feature: GP Session On Demand multiple actions

  Scenario Outline: A <GP System> user can navigate to multiple gp connected services when the gp system becomes available
    Given I am an <GP System> patient whose GP system is unavailable
    And I am logged in
    When I navigate to Prescriptions
    Then the Prescriptions Hub page is displayed
    When I click the View Orders link
    Then I see appropriate try again shutter screen for prescriptions when there is no GP session
    And The <GP System> GP system becomes available
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And I have no booked appointments for <GP System>
    When I click the 'Try again' button
    Then I see 1 prescriptions
    When I navigate to Appointments
    Then the Appointments Hub page is displayed
    When I click the GP Appointments link
    Then the Your Appointments page is displayed
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
