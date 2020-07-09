@my-record
Feature: Referrals Frontend - Medical Record v2

  Scenario: A MICROTEST user can view referrals - Medical Record v2
    Given I am a MICROTEST user setup to use medical record version 2
    And the my record wiremocks are populated
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Referrals link on my record - Medical Record v2
    Then I see the expected referrals - Medical Record v2

  Scenario: A MICROTEST user has no referrals on their record - Medical Record v2
    Given I am a MICROTEST user setup to use medical record version 2
    And I have 0 Referrals
    And the my record wiremocks are populated
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Referrals link on my record - Medical Record v2
    Then I see a message that this information isn't available through the NHS App - Medical Record v2
