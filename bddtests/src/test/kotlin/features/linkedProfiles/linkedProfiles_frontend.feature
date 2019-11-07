@linked-profiles
Feature: Login with proxy access

  Scenario: An EMIS user with proxy accounts can see their linked profiles
    Given I am logged in as a EMIS user with linked profiles
    Then I see the home page
    And I see the linked profiles link
    When I select the linked profiles link from the home page
    Then the linked profiles page is displayed
    And linked profiles are displayed
    When I select a linked profile
    Then details for the selected linked profile are displayed
    When I click the Switch to this profile button for the proxy user
    Then I see the home page
    And I see the yellow banner
    And the yellow banner contains details for the user I am acting on behalf of
    And I do not see the home page links