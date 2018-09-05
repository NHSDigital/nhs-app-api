Feature: Analytics

  A user's analytics tracking information is available

  @smoketest
  Scenario: The analytics data object is available
    Given a patient from EMIS is defined
    When I am logged in
    Then the analytics data object is available
