@gp_session_on_demand
Feature: GP Session On Demand prescriptions

  Scenario Outline: A <GP System> user trying to view prescriptions but whose gp system is unavailable has the option to retry and can view prescriptions when the gp system becomes available
    Given I am an <GP System> patient whose GP system is unavailable
    And I am logged in
    When I navigate to Prescriptions
    Then the Prescriptions Hub page is displayed
    When I click the View Orders link
    And I see appropriate try again error message for prescriptions when there is no GP session
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
    And I see appropriate try again error message for prescriptions when there is no GP session
    When I click the 'Try again' button
    Then I see what I can do next with a prescriptions error message and reference code '<Prefix>'
    Examples:
      | GP System | Prefix |
      | EMIS      | 3e     |
      | TPP       | 3t     |
      | VISION    | 3s     |
      | MICROTEST | 3m     |

  Scenario Outline: A <GP System> user can see the order a repeat prescription after a decoupled GpSession login
    Given I have a Decoupled GP User Session
    And I am a <GP System> patient
    And I have historic prescriptions
    And there are 4 repeatable prescriptions available
    And I am logged in
    And I navigate to prescriptions
    When I click 'Order a prescription'
    Then I SSO to NhsLogin
    And the Type of Prescriptions page is displayed
    When I select the option to order a repeat prescription
    Then I see the available repeatable prescriptions
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |
