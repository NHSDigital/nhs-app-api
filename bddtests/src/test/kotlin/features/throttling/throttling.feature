@throttling
Feature: Throttling

  @pending
  Scenario: A user can search for their GP Practice
    Given I am not logged in and I have not selected previously my GP Practice
    And the NHS Azure Search Service has GP Practices
    Then I see the GP Finder Page
    When I search for a GP Practice
    Then I see the GP Search Results Page with a search results
