@gp_session_on_demand
Feature: GP Session On Demand prescriptions

  Scenario Outline: A <GP System> user trying to view prescriptions but whose gp system is unavailable has the option to retry and can view prescriptions when the gp system becomes available
    Given I am an <GP System> patient whose GP system is unavailable
    And I am logged in
    When I navigate to Prescriptions
    Then the Prescriptions Hub page is displayed
    When I click the View Orders link
    And I see appropriate try again shutter screen for prescriptions when there is no GP session
    And The <GP System> GP system becomes available
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And I click the 'Try again' button
    Then I see 1 prescriptions
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario Outline: A <GP System> user trying to view prescriptions but whose gp system is unavailable has the option to retry and an advice screen when the gp session does not recover
    Given I am an <GP System> patient whose GP system is unavailable
    And I am logged in
    When I navigate to Prescriptions
    Then the Prescriptions Hub page is displayed
    And I click the View Orders link
    And I see appropriate try again shutter screen for prescriptions when there is no GP session
    When I click the 'Try again' button
    Then I see what I can do next with a repeat prescriptions error message and reference code '<Prefix>'
    Examples:
      | GP System | Prefix |
      | EMIS      | 3e     |
      | TPP       | 3t     |
      | VISION    | 3s     |

