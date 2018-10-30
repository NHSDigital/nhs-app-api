@my-record
Feature: View My Medical Record Information - Medications

  Scenario Outline: A <Service> user has no acute medications
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has enabled medication functionality and the patient has no medications for <Service>
    And I am on my record information page
    When I click the Acute (short-term) medications section on My Record
    Then I see a message indicating that I have no information recorded for Acute (short-term) medications on My Record

    When I click the Repeat medications: current section on My Record
    Then I see a message indicating that I have no information recorded for Repeat medications: current on My Record

    When I click the Repeat medications: discontinued section on My Record
    Then I see a message indicating that I have no information recorded for Repeat medications: discontinued on My Record

    Examples:
      | Service |
      | EMIS    |
      | TPP     |
      | VISION  |


  Scenario Outline: A <Service> user views medications
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has enabled medications functionality for <Service>
    And I am on my record information page
    Then I see the Acute (short-term) medications heading on My Record
    When I click the Acute (short-term) medications section on My Record
    Then I see acute medication information

    When I click the Repeat medications: current section on My Record
    Then I see current repeat medication information

    When I click the Repeat medications: discontinued section on My Record
    Then I see discontinued repeat medication information

    Examples:
      | Service |
      | EMIS    |
      | TPP     |
      | VISION  |


  Scenario Outline: A <Service> user has no access to view medications
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has disabled summary care record functionality for <Service>
    And I am on my record information page
    Then I see a message indicating that I have no access to view my summary care record

    Examples:
      | Service |
      | EMIS    |
      | TPP     |
      | VISION  |