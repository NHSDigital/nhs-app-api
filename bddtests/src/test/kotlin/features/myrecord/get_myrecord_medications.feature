Feature: Get medications data

  A user can get their medication information

  Background:
    Given wiremock is initialised
    And the my record wiremocks are initialised

  @NHSO-689
  @backend
  Scenario: Requesting medications returns medications data
    Given I have logged in and have a valid session cookie
    And the GP Practice has enabled medications functionality
    When I get the users my record data
    Then I receive "1" acute medications as part of the my record object
    And I receive "3" current repeat medications as part of the my record object
    And I receive "2" discontinued repeat medications as part of the my record object

  @NHSO-689
  @backend
  Scenario: GP practice has disabled medications functionality
    Given I have logged in and have a valid session cookie
    But the GP Practice has disabled medications functionality
    When I get the users my record data
    Then the flag informing that the patient has access to the medications data is set to "False"
    And the flag informing that there was an error retrieving the medications data is set to "False"