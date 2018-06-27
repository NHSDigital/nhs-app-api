Feature: Get Immunisations Data

  A user can get their immunisation information

  Background:
    Given wiremock is initialised
    And the my record wiremocks are initialised

  @backend
  Scenario: Requesting multiple test results returns multiple test results data
    Given I have logged in and have a valid session cookie
    And the GP Practice has multiple test results
    When I get the users test results
    Then I receive "2" test results as part of the my record object

  @backend
  Scenario: Requesting single test result with child values with no ranges
    Given I have logged in and have a valid session cookie
    And the GP Practice has a single test result with multiple child values with no ranges
    When I get the users test results
    Then I receive the test result with term set correctly to Term
    And I receive line items for each child value
    And the line item value is set correctly

  @backend
  Scenario: Requesting single test result with child values with ranges
    Given I have logged in and have a valid session cookie
    And the GP Practice has a single test result with multiple child values with ranges
    When I get the users test results
    Then I receive the test result with term set correctly to Term
    And I receive line items for each child value
    And the line item displays text value and range

  @backend
  Scenario: Requesting single test result with no child items or range
    Given I have logged in and have a valid session cookie
    And the GP Practice has test results enabled and a single test result exists with no child values or range
    When I get the users test results
    Then I receive a single test result with the term set correctly to Term TextValue NumericUnits

  @backend
  Scenario: Requesting single test result with no child items and a range
    Given I have logged in and have a valid session cookie
    And the GP Practice has test results enabled and a single test result exists with no child values and a range
    When I get the users test results
    Then I receive the term set correctly to Term TextValue NumericUnits Range

