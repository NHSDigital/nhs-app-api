@gp_session_on_demand
Feature: GP Session On Demand appointments

  Scenario Outline: A <GP System> user trying to view appointments but whose gp system is unavailable has the option to retry and can view appointments when the gp system becomes available
    Given I am an <GP System> patient whose GP system is unavailable
    And I am logged in
    When I navigate to Appointments
    Then the Appointments Hub page is displayed
    And I click the GP Appointments link
    And I see appropriate warning message when there is no GP session
    And The <GP System> GP system becomes available
    And I have no booked appointments for <GP System>
    When I click the 'Try again' button
    Then the Your Appointments page is displayed
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario Outline: A <GP System> user trying to view appointments but whose gp system is unavailable has the option to retry and an advice screen when the gp session does not recover
    Given I am an <GP System> patient whose GP system is unavailable
    And I am logged in
    When I navigate to Appointments
    Then the Appointments Hub page is displayed
    And I click the GP Appointments link
    And I see appropriate warning message when there is no GP session
    When I click the 'Try again' button
    Then I see what I can do next with an error message and reference code '<Prefix>'
    Examples:
      | GP System | Prefix |
      | EMIS      | 3e     |
      | TPP       | 3t     |
      | VISION    | 3s     |

  Scenario: A user with Accurx access trying to view appointments when the Gp System is unavailable is shown alternative options after retrying
    Given I am a patient with access to all Accurx services
    And GP session is unavailable
    And I am logged in
    When I navigate to Appointments
    Then the Appointments Hub page is displayed
    And I click the GP Appointments link
    And I see appropriate warning message when there is no GP session
    When I click the 'Try again' button
    Then I am shown a message and a list of actions I can perform as an Accurx user

  Scenario: A user with Patchs access trying to view appointments when the Gp System is unavailable is shown alternative options after retrying
    Given I am a patient with access to all Patchs services
    And GP session is unavailable
    And I am logged in
    When I navigate to Appointments
    Then the Appointments Hub page is displayed
    And I click the GP Appointments link
    And I see appropriate warning message when there is no GP session
    When I click the 'Try again' button
    Then I am shown a message and a list of actions I can perform as a Patchs user
