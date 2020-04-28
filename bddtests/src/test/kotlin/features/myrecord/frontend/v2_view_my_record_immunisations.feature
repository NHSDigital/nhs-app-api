@my-record
Feature: Immunisations Frontend - Medical Record v2

  Scenario Outline: A <GP System> user has multiple immunisations on their record - Medical Record v2
    Given I am a <GP System> user setup to use medical record version 2
    And the GP Practice has enabled immunisations functionality and multiple immunisation records exist
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    When I click the Immunisations link on my record - Medical Record v2
    Then I see the expected immunisations - Medical Record v2

    Examples:
      | GP System |
      | EMIS      |
      | VISION    |

  Scenario Outline: A <GP System> user has no immunisations on their record - Medical Record v2
    Given I am a <GP System> user setup to use medical record version 2
    And no immunisation records exist for the patient
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    When I click the Immunisations link on my record - Medical Record v2
    Then I see a message that I have no information recorded for a specific record - Medical Record v2

    Examples:
      | GP System |
      | EMIS      |
      | VISION    |

  Scenario Outline: A <GP System> user does not have access to their immunisations record - Medical Record v2
    Given I am a <GP System> user setup to use medical record version 2
    And the user does not have access to view immunisations
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    When I click the Immunisations link on my record - Medical Record v2
    Then I see a message indicating that I have no access to view this section on My Record - Medical Record v2

    Examples:
      | GP System |
      | EMIS      |
      | VISION    |

  Scenario: A MICROTEST user can view immunisations - Medical Record v2
    Given I am a MICROTEST user setup to use medical record version 2
    And the my record wiremocks are populated
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    When I click the Immunisations link on my record - Medical Record v2
    Then I see the expected immunisations - Medical Record v2

  Scenario: A MICROTEST user has no immunisations on their record - Medical Record v2
    Given I am a MICROTEST user setup to use medical record version 2
    And I have 0 Immunisations
    And the my record wiremocks are populated
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    When I click the Immunisations link on my record - Medical Record v2
    Then I see a message that this information isn't available through the NHS App - Medical Record v2

  Scenario: An EMIS user has a immunisation result with an unknown date - Medical Record v2
    Given I am a EMIS user setup to use medical record version 2
    And the EMIS GP Practice has two immunisation results where the first record has no date
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    When I click the Immunisations link on my record - Medical Record v2
    Then I see the expected immunisations with an unknown date for the second result - Medical Record v2

  Scenario Outline: An <GP System> user receiving a corrupted Immunisations response sees an error - Medical Record v2
    Given I am a <GP System> user setup to use medical record version 2
    And the GP practice returns a bad immunisations response
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    When I click the Immunisations link on my record - Medical Record v2
    Then I see an error occurred message on My Record - Medical Record v2
    Examples:
      | GP System |
      | EMIS      |
      | VISION    |
