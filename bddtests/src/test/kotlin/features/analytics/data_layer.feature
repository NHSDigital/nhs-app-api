@analytics
Feature: Analytics

  A user's analytics tracking information is available

  @smoketest
  Scenario: The analytics data object is available
    Given I am a EMIS patient
    When I am logged in
    Then the analytics data object is available
