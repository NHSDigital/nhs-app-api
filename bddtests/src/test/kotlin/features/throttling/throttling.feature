@throttling
Feature: Throttling

  @pending
  Scenario: A user searches for their practice to find out if it is participating in beta
    Given I am not logged in and I have not completed the beta throttling flow
    And I see the GP Finder Page
    When I search for a GP Practice
    Then I see the GP Search Results Page with search results

    When My GP Practice is not participating in beta
    And I select my GP Practice
    Then I see the Practice Not Participating page

    Given I am not logged in and I have not completed the beta throttling flow
    When I see the GP Finder Page
    And I search for a GP Practice

    When My GP Practice is participating in beta
    And I select my GP Practice
    Then I see the Practice Participating page

  @pending
  Scenario: A user selects the wrong GP practice and returns to the GP finder page
    Given I am not logged in and I have not completed the beta throttling flow
    And I see the GP Finder Page
    When I search for a GP Practice
    Then I see the GP Search Results Page with search results

    When My GP Practice is participating in beta
    And I select my GP Practice
    And I click the This is not my GP surgery button
    Then I see the GP Finder Page
