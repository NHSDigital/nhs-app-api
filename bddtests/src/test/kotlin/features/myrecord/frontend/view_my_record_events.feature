@my-record
Feature: View My Medical Record Information - Events

  @smoketest
  @NHSO-1504
  Scenario Outline: A TPP user has Events on their record
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has multiple dcr events for <Service>
    And I am on my record information page
    And I see heading Events
    When I click the Events section
    Then I see Events records displayed

    Examples:
      |Service|
      |TPP|

  @NHSO-1504
  Scenario Outline: A TPP user has no Events on their record
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And I have no dcr events for <Service>
    And I am on my record information page
    And I see heading Events
    When I click the Events section
    Then I see a message indicating that I have no information recorded for Events

    Examples:
      |Service|
      |TPP|

  @NHSO-1504
  @pending
  Scenario: A TPP user does not have access to Events
    Given the my record wiremocks are initialised for TPP
    And the GP Practice has enabled demographics functionality for TPP
    And the GP Practice has disabled dcr events functionality for TPP
    And I am on my record information page
    And I see heading Events
    When I click the Events section
    Then I see a message indicating that I have no access to view Events

  @NHSO-1504
  Scenario Outline: An Error occurs retrieving Events data for <Service>
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And an error occurred retrieving the dcr events from <Service>
    And I am on my record information page
    And I see heading Events
    When I click the Events section
    Then I see an error occured message with Events

    Examples:
      |Service|
      |TPP|
