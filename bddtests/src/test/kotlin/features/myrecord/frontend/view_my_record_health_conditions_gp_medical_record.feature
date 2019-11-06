@my-record
Feature: View My Health Conditions - Medical Record V2

  Scenario Outline: A <GP System> user has multiple health conditions on their record - GP Medical Record
    Given I am a <GP System> user setup to use medical record version 2
    And the GP Practice has enabled problems functionality
    And I am logged in
    And I am on my record information page and glossary is visible - GP Medical Record
    When I click the Health conditions link on my record - GP Medical Record
    Then I see the expected health conditions - GP Medical Record

  Examples:
    | GP System |
    | EMIS      |
    | VISION    |

  Scenario Outline: A <GP System> user has no health conditions on their record - GP Medical Record
    Given I am a <GP System> user setup to use medical record version 2
    And no Problems records exist for the patient
    And I am logged in
    And I am on my record information page and glossary is visible - GP Medical Record
    When I click the Health conditions link on my record - GP Medical Record
    Then I see a message that I have no information recorded for a specific record - GP Medical Record

  Examples:
    | GP System |
    | EMIS      |
    | VISION    |

  Scenario Outline: A <GP System> user does not have access to their health conditions record - GP Medical Record
   Given I am a <GP System> user setup to use medical record version 2
   And the GP Practice has disabled problems functionality
   And I am logged in
   And I am on my record information page and glossary is visible - GP Medical Record
   When I click the Health conditions link on my record - GP Medical Record
   Then I see a message indicating that I have no access to view this section on My Record - GP Medical Record

   Examples:
     | GP System |
     | EMIS      |
     | VISION    |

  Scenario Outline: A <GP System> user has an error accessing thier health conditions - GP Medical Record
    Given I am a <GP System> user setup to use medical record version 2
    And there is an error retrieving Problems data
    And I am logged in
    And I am on my record information page and glossary is visible - GP Medical Record
    When I click the Health conditions link on my record - GP Medical Record
    Then I see an error occurred message on My Record - GP Medical Record

    Examples:
      | GP System |
      | EMIS      |
      | VISION    |

  Scenario: A MICROTEST user can view health conditions - GP Medical Record
    Given I am a MICROTEST user setup to use medical record version 2
    And the my record wiremocks are populated without setting the patient for MICROTEST
    And I am logged in
    And I am on my record information page and glossary is visible - GP Medical Record
    When I click the Health conditions link on my record - GP Medical Record
    Then I see the expected health conditions - GP Medical Record

  Scenario: A MICROTEST user has no health conditions on their record - GP Medical
    Given I am a MICROTEST user setup to use medical record version 2
    And I have 0 Problems
    And the my record wiremocks are populated without setting the patient for MICROTEST
    And I am logged in
    And I am on my record information page and glossary is visible - GP Medical Record
    When I click the Health conditions link on my record - GP Medical Record
    Then I see a message that this information isn't available through the NHS App - GP Medical Record