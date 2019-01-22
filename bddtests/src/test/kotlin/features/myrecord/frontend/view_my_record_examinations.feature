@my-record
Feature: View My Medical Record Information - Examinations

  Scenario Outline: A <Service> user can view examinations section
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And I am on my record information page
    Then I see the examinations heading
    And I see the examinations section collapsed

    Examples:
      | Service |
      | VISION  |

  Scenario Outline: A <Service> user can view examinations information
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has multiple examinations
    And I am on my record information page
    When I click the examinations section
    Then I see examinations information

    Examples:
      | Service |
      | VISION  |

  Scenario Outline: A <Service> user has no access to examinations section
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And I do not have access to examinations
    And I am on my record information page
    When I click the examinations section
    Then I see a message indicating that I have no access to view Examinations on My Record

    Examples:
      | Service |
      | VISION  |

  Scenario Outline: A <Service> user has no examinations
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And I have no examinations
    And I am on my record information page
    When I click the examinations section
    Then I see a message indicating that I have no information recorded for Examinations on My Record

    Examples:
      | Service |
      | VISION  |

  Scenario Outline: An error occurs when trying to retrieve examination data from <Service>
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And an error occurred retrieving the examinations
    And I am on my record information page
    When I click the examinations section
    Then I see an error occurred message with Examinations on My Record

    Examples:
      | Service |
      | VISION  |