@switch-profiles
Feature: Login with proxy access and switch back to main profile

  Scenario: An EMIS user with proxy accounts can see proxy details and switch back to their own account
    Given I am logged in as a EMIS user with linked profiles
    When I select the linked profiles link from the home page
    And I select a linked profile
    And I click the Switch to this profile button for the proxy user
    And I click the yellow banner
    Then the switch profiles page is displayed
    And the correct proxy user details are displayed
    Then I click the Switch to my profile button for the main user
    And I see the home page
    And I do not see the yellow banner

