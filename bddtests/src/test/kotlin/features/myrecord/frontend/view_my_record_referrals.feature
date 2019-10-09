@my-record
Feature: View My Medical Record Information - Referrals Frontend

  Scenario: A MICROTEST user can view referrals
    Given the my record wiremocks are populated for MICROTEST
    And the GP Practice has enabled demographics functionality
    And I am on my record information page
    Then I see the Referrals heading on My Record
    When I click the Referrals section on My Record
    Then I see the expected referrals displayed

  Scenario: A MICROTEST user can view referrals section when no referrals are returned
    Given I have 0 Referrals
    And the my record wiremocks are populated for MICROTEST
    And the GP Practice has enabled demographics functionality
    And I am on my record information page
    Then I see the Referrals heading on My Record
    When I click the Referrals section on My Record
    Then I see a message telling me to contact my GP for Referrals information on My Record