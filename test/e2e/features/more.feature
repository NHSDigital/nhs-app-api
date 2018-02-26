Feature: More
  In order to access functionality that is not available on the standard app menu bar
  As a user
  I want to be able access more available functionality

  Scenario: Invoking Organ Donation
    Given I am not logged in
    And I am on the more page
    When I invoke organ donation
    Then I should be on the organ donation register