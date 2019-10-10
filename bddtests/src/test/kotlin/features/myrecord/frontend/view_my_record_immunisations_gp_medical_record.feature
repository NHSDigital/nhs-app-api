@my-record
@pending @NHSO-7509
Feature: View My Immunisation Information

  Scenario Outline: A <GP System> user has multiple immunisations on their record - GP Medical Record
    Given I am a <GP System> user setup to use medical record version 2
    And the GP Practice has enabled immunisations functionality and multiple immunisation records exist - GP Medical Record
    And I am logged in
    And I am on my record information page and glossary is visible - GP Medical Record
    When I click the Immunisations link on my record - GP Medical Record
    Then I see the expected immunisations - GP Medical Record

  Examples:
    | GP System |
    | EMIS      |
    | VISION    |

  Scenario Outline: A <GP System> user has no immunisations on their record - GP Medical Record
    Given I am a <GP System> user setup to use medical record version 2
    And the GP Practice has enabled immunisations functionality and no immunisation records exist - GP Medical Record
    And I am logged in
    And I am on my record information page and glossary is visible - GP Medical Record
    When I click the Immunisations link on my record - GP Medical Record
    Then I see a message that I have no information recorded for a specific record - GP Medical Record

  Examples:
    | GP System |
    | EMIS      |
    | VISION    |

  Scenario Outline: A <GP System> user does not have access to their immunisations record - GP Medical Record
   Given I am a <GP System> user setup to use medical record version 2
   And the user does not have access to view immunisations - GP Medical Record
   And I am logged in
   And I am on my record information page and glossary is visible - GP Medical Record
   When I click the Immunisations link on my record - GP Medical Record
   Then I see a message indicating that I have no access to view this section on My Record - GP Medical Record

   Examples:
     | GP System |
     | EMIS      |
     | VISION    |

  Scenario: A MICROTEST user can view immunisations - GP Medical Record
    Given I am a MICROTEST user setup to use medical record version 2
    And the my record wiremocks are populated when the patient is already set for MICROTEST
    And I am logged in
    And I am on my record information page and glossary is visible - GP Medical Record
    When I click the Immunisations link on my record - GP Medical Record
    Then I see the expected immunisations - GP Medical Record

  Scenario: A MICROTEST user has no immunisations on their record - GP Medical
    Given I am a MICROTEST user setup to use medical record version 2
    And I have 0 Immunisations
    And the my record wiremocks are populated when the patient is already set for MICROTEST
    And I am logged in
    And I am on my record information page and glossary is visible - GP Medical Record
    When I click the Immunisations link on my record - GP Medical Record
    Then I see a message that this information isn't available through the NHS App - GP Medical Record

  Scenario: An EMIS user has a immunisation result with an unknown date - GP Medical Record
    Given I am a EMIS user setup to use medical record version 2
    And the EMIS GP Practice has two immunisation results where the first record has no date - GP Medical Record
    And I am logged in
    And I am on my record information page and glossary is visible - GP Medical Record
    When I click the Immunisations link on my record - GP Medical Record
    Then I see the expected immunisations with an unknown date for the first result - GP Medical Record
