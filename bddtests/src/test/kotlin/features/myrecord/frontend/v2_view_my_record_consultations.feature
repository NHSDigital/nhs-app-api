@my-record
Feature: Consultations Frontend - Medical Record v2

  Scenario Outline: A <GP System> user has multiple consultations on their record - Medical Record v2
    Given I am a <GP System> user setup to use medical record version 2
    And the GP Practice has multiple consultations
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    And I click the Consultations and events link on my record - Medical Record v2
    Then I see the expected consultations and events - Medical Record v2

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario Outline: A <GP System> user has no consultations on their record - Medical Record v2
    Given I am a <GP System> user setup to use medical record version 2
    And the GP Practice has no consultations
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    And I click the Consultations and events link on my record - Medical Record v2
    Then I see a message that I have no information recorded for a specific record - Medical Record v2

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario: An EMIS user does not have access to their consultations record - Medical Record v2
    Given I am a EMIS user setup to use medical record version 2
    And the Patient has no access to consultations
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    And I click the Consultations and events link on my record - Medical Record v2
    Then I see a message indicating that I have no access to view this section on My Record - Medical Record v2

  Scenario: An EMIS user receiving bad consultations response sees and error
    Given I am a EMIS user setup to use medical record version 2
    And the GP practice returns bad consultations data
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    And I click the Consultations and events link on my record - Medical Record v2
    Then I see an error occurred message on My Record - Medical Record v2

  Scenario: An EMIS user has a consultation record with an unknown date - GP Medical Record
    Given I am a EMIS user setup to use medical record version 2
    And the EMIS GP Practice has two consultations where the first record has no date
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    And I click the Consultations and events link on my record - Medical Record v2
    Then I see the second consultation record have unknown date - Medical Record v2
