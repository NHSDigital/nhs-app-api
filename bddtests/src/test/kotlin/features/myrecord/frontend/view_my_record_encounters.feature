@my-record
  Feature: View My Medical Record Information - Encounters Frontend

  Scenario: A MICROTEST user can view encounters
    Given the my record wiremocks are populated for MICROTEST
    And the GP Practice has enabled demographics functionality
    And I am on my record information page
    Then I see the Encounters heading on My Record
    When I click the Encounters section on My Record
    Then I see the expected encounters displayed

  Scenario: A MICROTEST user can view encounters section when no encounters are returned
    Given I have 0 Encounters
    And the my record wiremocks are populated for MICROTEST
    And the GP Practice has enabled demographics functionality
    And I am on my record information page
    Then I see the Encounters heading on My Record
    When I click the Encounters section on My Record
    Then I see a message telling me to contact my GP for Encounters information on My Record