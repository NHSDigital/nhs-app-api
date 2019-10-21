@my-record
Feature: View My Medical Record Information - Health conditions Frontend

  Scenario Outline: A <GP System> user has no Problems on their record
    Given the my record wiremocks are initialised for <GP System>
    And the GP Practice has enabled demographics functionality
    And no Problems records exist for the patient
    And I am on my record information page
    Then I see the Health conditions heading on My Record
    When I click the Health conditions section on My Record
    Then I see a message indicating that I have no information recorded for Health conditions on My Record

    Examples:
      | GP System |
      | EMIS      |
      | VISION    |

  Scenario Outline: A <GP System> user does not have access to Problems
    Given the my record wiremocks are initialised for <GP System>
    And the GP Practice has enabled demographics functionality
    And the GP Practice has disabled problems functionality
    And I am on my record information page
    Then I see the Health conditions heading on My Record
    When I click the Health conditions section on My Record
    Then I see a message indicating that I have no access to view Health conditions on My Record

    Examples:
      | GP System |
      | EMIS      |
      | VISION    |

  Scenario Outline: An Error occurs retrieving Problems data for <GP System>
    Given the my record wiremocks are initialised for <GP System>
    And the GP Practice has enabled demographics functionality
    And there is an error retrieving Problems data
    And I am on my record information page
    Then I see the Health conditions heading on My Record
    When I click the Health conditions section on My Record
    Then I see an error occurred message with Health conditions on My Record

    Examples:
      | GP System |
      | EMIS      |
      | VISION    |

  Scenario: A MICROTEST user can view problems
    Given the my record wiremocks are populated for MICROTEST
    And the GP Practice has enabled demographics functionality
    And I am on my record information page
    Then I see the Health conditions heading on My Record
    When I click the Health conditions section on My Record
    Then I see the expected health conditions displayed

  Scenario: A MICROTEST user can view problems section when no problems are returned
    Given I have 0 Problems
    And the my record wiremocks are populated for MICROTEST
    And the GP Practice has enabled demographics functionality
    And I am on my record information page
    Then I see the Health conditions heading on My Record
    When I click the Health conditions section on My Record
    Then I see a message telling me to contact my GP for Health conditions information on My Record