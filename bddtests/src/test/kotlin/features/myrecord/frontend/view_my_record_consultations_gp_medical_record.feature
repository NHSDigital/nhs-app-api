@my-record
Feature: View My Consultations Information

  Scenario Outline: A <GP System> user has multiple consultations on their record - GP Medical Record
    Given I am a <GP System> user setup to use medical record version 2
    And the GP Practice has multiple consultations - GP Medical Record
    And I am logged in
    And I am on my record information page and glossary is visible - GP Medical Record
    When I click the Consultations and events link on my record - GP Medical Record
    Then I see the expected consultations and events - GP Medical Record

  Examples:
    | GP System |
    | EMIS      |
    | TPP    |

  Scenario Outline: A <GP System> user has no consultations on their record - GP Medical Record
    Given I am a <GP System> user setup to use medical record version 2
    And the GP Practice has no consultations
    And I am logged in
    And I am on my record information page and glossary is visible - GP Medical Record
    When I click the Consultations and events link on my record - GP Medical Record
    Then I see a message that I have no information recorded for a specific record - GP Medical Record

  Examples:
    | GP System |
    | EMIS      |
    | TPP    |

  Scenario: An EMIS user does not have access to their consultations record - GP Medical Record
   Given I am a EMIS user setup to use medical record version 2
   And the Patient has no access to consultations
   And I am logged in
   And I am on my record information page and glossary is visible - GP Medical Record
   When I click the Consultations and events link on my record - GP Medical Record
   Then I see a message indicating that I have no access to view this section on My Record - GP Medical Record