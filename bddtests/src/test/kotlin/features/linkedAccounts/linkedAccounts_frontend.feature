@linked-accounts
Feature: Login with proxy access

  Scenario: An EMIS user with proxy accounts can see the linked profiles link on the home page after logging in
    Given I am logged in as a EMIS user with linked profiles
    Then I see the home page
    And I see the linked profiles link


