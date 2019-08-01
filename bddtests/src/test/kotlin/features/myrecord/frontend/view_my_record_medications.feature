@my-record
Feature: View My Medical Record Information - Medications Frontend

  Scenario Outline: A <Service> user has no acute medications
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality
    And the GP Practice has enabled medication functionality and the patient has no medications
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

  Scenario Outline: A <Service> user cannot view medications when they cannot access their Summary Care Record
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality
    And the GP Practice has disabled summary care record functionality
    And I am on my record information page
    Then I do not see the Acute (short-term) medications heading on My Record
    And I do not see the Repeat medications: current heading on My Record
    And I do not see the Repeat medications: discontinued heading on My Record
    But I see the test result heading

    Examples:
      | Service |
      | EMIS    |
      | TPP     |
      | VISION  |


  Scenario: A MICROTEST user can view medications section
    Given the my record wiremocks are populated for MICROTEST
    And the GP Practice has enabled demographics functionality
    And I am on my record information page
    Then I see the Acute (short-term) medications heading on My Record
    When I click the Acute (short-term) medications section on My Record
    Then I see the expected acute medications displayed
    When I click the Repeat medications: current section on My Record
    Then I see the expected current repeat medications displayed
    When I click the Repeat medications: discontinued section on My Record
    Then I see the expected discontinued repeat medications displayed
