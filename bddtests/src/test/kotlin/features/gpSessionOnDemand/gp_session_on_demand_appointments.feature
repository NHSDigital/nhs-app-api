@gp_session_on_demand
Feature: GP Session On Demand appointments

  Scenario Outline: A <GP System> user can see the Gp Appointments page after a decoupled GpSession login
    Given I have a Decoupled GP User Session
    And I have upcoming appointments before cutoff time for <GP System>
    And I am logged in
    When I retrieve the 'appointment hub' page directly
    Then the Appointments Hub page is displayed
    When I click the GP Appointments link
    Then I SSO to NhsLogin
    And the page title is "Your GP appointments"
    And I am given the list of upcoming appointments
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: A <GP System> user trying to view appointments but whose gp system is unavailable has the option to retry and can view appointments when the gp system becomes available
    Given I have a Decoupled GP User Session
    And I am an <GP System> patient whose GP system is unavailable
    And I am logged in
    When I navigate to Appointments
    Then the Appointments Hub page is displayed
    And I click the GP Appointments link
    And I SSO to NhsLogin
    And I see appropriate try again error message when there is no GP session
    And The <GP System> GP system becomes available
    And I have no booked appointments for <GP System>
    When I click the 'Try again' button
    Then the Your Appointments page is displayed
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario Outline: A <GP System> user trying to view appointments but whose gp system is unavailable has the option to retry and an advice screen when the gp session does not recover
    Given I have a Decoupled GP User Session
    And I am an <GP System> patient whose GP system is unavailable
    And I am logged in
    When I navigate to Appointments
    Then the Appointments Hub page is displayed
    And I click the GP Appointments link
    And I SSO to NhsLogin
    And I see appropriate try again error message when there is no GP session
    When I click the 'Try again' button
    Then I see what I can do next with an error message and reference code '<Prefix>'
    Examples:
      | GP System | Prefix |
      | EMIS      | 3e     |
      | TPP       | 3t     |
      | VISION    | 3s     |
      | MICROTEST | 3m     |
