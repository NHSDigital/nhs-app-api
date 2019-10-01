@my-record
Feature: View My Medical Record Information - Allergies Frontend

  Scenario: A VISION user has a drug and non drug allergy record
    Given the my record wiremocks are initialised for VISION
    And the GP Practice has enabled demographics functionality
    And the GP Practice has enabled allergies functionality and has a drug and non drug allergy record for VISION
    And I am on my record information page
    When I click the Allergies and adverse reactions section on My Record
    Then I see a drug and non drug allergy record from VISION

  Scenario: A TPP user without Summary Care Record access cannot view allergies section
    Given the my record wiremocks are initialised for TPP
    And the GP Practice has enabled demographics functionality
    But the GP Practice has disabled allergies functionality
    And I am on my record information page
    Then I do not see the Allergies and adverse reactions heading on My Record
    But I see the test result heading

  Scenario: An EMIS user has no allergies on their record
    Given the my record wiremocks are initialised for EMIS
    And the GP Practice has enabled demographics functionality
    And the GP Practice has enabled allergies functionality and the patient has "0" allergies
    And I am on my record information page
    When I click the Allergies and adverse reactions section on My Record
    Then I see a message indicating that I have no information recorded for Allergies and adverse reactions on My Record

  Scenario: A MICROTEST user can view allergies and adverse reactions section when no allergies are returned
    Given I have 0 Allergies
    And the my record wiremocks are populated for MICROTEST
    And the GP Practice has enabled demographics functionality
    And I am on my record information page
    Then I see the Allergies and adverse reactions heading on My Record
    When I click the Allergies and adverse reactions section on My Record
    Then I see a message telling me to contact my GP for Allergies and adverse reactions information on My Record

  Scenario: An EMIS user has multiple allergies with different date display formats
    Given the my record wiremocks are initialised for EMIS
    And the GP Practice has enabled demographics functionality
    And the GP Practice has enabled allergies functionality and has 5 different allergies with different date formats
    And I am on my record information page
    When I click the Allergies and adverse reactions section on My Record
    And I see 5 allergies with different date formats

  Scenario: A VISION user is shown an appropriate error message when an unknown error occurs retrieving their allergies
    Given the my record wiremocks are initialised for VISION
    And the GP Practice has enabled demographics functionality
    And there is an unknown error getting allergies for VISION
    And I am on my record information page
    When I click the Allergies and adverse reactions section on My Record
    Then I see an error occurred message with Allergies and adverse reactions on My Record

  Scenario: A MICROTEST user can view allergies and adverse reactions section
    Given the my record wiremocks are populated for MICROTEST
    And the GP Practice has enabled demographics functionality
    And I am on my record information page
    Then I see the Allergies and adverse reactions heading on My Record
    And I do not see a message informing me to contact my GP for this information
    When I click the Allergies and adverse reactions section on My Record
    Then I see the expected allergies displayed

  Scenario: An EMIS user has an allergies and adverse reactions result with an unknown date
    Given the my record wiremocks are initialised for EMIS
    And the GP Practice has enabled demographics functionality
    And the EMIS GP Practice has two allergies results where the first record has no date
    And I am on my record information page
    Then I see the Allergies and adverse reactions heading on My Record
    When I click the Allergies and adverse reactions section on My Record
    Then I see the expected allergies displayed with unknown date for the first result
