@my-record
Feature: View My Medical Record Information - Recalls Frontend

  Scenario: A MICROTEST user can view recalls
    Given the my record wiremocks are populated for MICROTEST
    And the GP Practice has enabled demographics functionality
    And I am on my record information page
    Then I see the Recalls heading on My Record
    When I click the Recalls section on My Record
    Then I see the expected recalls displayed

  Scenario: A MICROTEST user can view recalls section when no recalls are returned
    Given I have 0 Recalls
    And the my record wiremocks are populated for MICROTEST
    And the GP Practice has enabled demographics functionality
    And I am on my record information page
    Then I see the Recalls heading on My Record
    When I click the Recalls section on My Record
    Then I see a message telling me to contact my GP for Recalls information on My Record
