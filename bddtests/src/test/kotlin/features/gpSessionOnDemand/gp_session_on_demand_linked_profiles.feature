@gp_session_on_demand
Feature: GP Session On Demand linked profiles

  Scenario Outline: A <GP System> user trying to view linked profiles but whose gp system is unavailable has the option to retry and can view linked profiles when the gp system becomes available
    Given I am an <GP System> patient whose GP system is unavailable
    And I am logged in
    When I can see and follow the Linked profiles link
    And I see appropriate linked profiles try again error message when there is no GP session
    And The <GP System> GP system becomes available
    And I click the 'Try again' button
    And the linked profiles page is displayed
    And linked profiles are displayed
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario Outline: A <GP System> user trying to view linked profiles but whose gp system is unavailable has the option to retry and an advice screen when the gp session does not recover
    Given I am an <GP System> patient whose GP system is unavailable
    And I am logged in
    When I can see and follow the Linked profiles link
    And I see appropriate linked profiles try again error message when there is no GP session
    And I click the 'Try again' button
    Then I see what I can do next with a linked accounts error message and reference code '<Code>'
    Examples:
      | GP System | Code   |
      | EMIS      | 3e     |
      | TPP       | 3t     |
