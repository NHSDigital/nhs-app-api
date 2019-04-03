@my-record
Feature: View My Medical Record Information - Allergies

  Scenario Outline: A <Service> user can view allergies and adverse reactions section
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality
    And I am on my record information page
    Then I see the Allergies and adverse reactions heading on My Record
    And I see the Allergies and adverse reactions section collapsed on My Record

    Examples:
      |Service|
      |EMIS|
      |TPP|
      |VISION|


  Scenario Outline: A <Service> user does not have Summary Care Record access who is disabled at practice-level or patient level
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality
    But the GP Practice has disabled allergies functionality
    And I am on my record information page
    Then I see Service not offered by GP or to specific user or access revoked warning message

    Examples:
      |Service|
      |EMIS|
      |TPP|
      |VISION|

  Scenario Outline: A <Service> user has no allergies on their record
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality
    And the GP Practice has enabled allergies functionality and the patient has "0" allergies
    And I am on my record information page
    When I click the Allergies and adverse reactions section on My Record
    Then I see a message indicating that I have no information recorded for Allergies and adverse reactions on My Record

    Examples:
      |Service|
      |EMIS|
      |TPP|
      |VISION|

  Scenario Outline: A <Service> user has one or more allergy records
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality
    And the GP Practice has enabled allergies functionality and the patient has "2" allergies
    And I am on my record information page
    When I click the Allergies and adverse reactions section on My Record
    And I see one or more drug type allergies record displayed

    Examples:
      |Service|
      |EMIS|
      |TPP|

  Scenario: An EMIS user has multiple allergies with different date display formats
    Given the my record wiremocks are initialised for EMIS
    And the GP Practice has enabled demographics functionality
    And the GP Practice has enabled allergies functionality and has 5 different allergies with different date formats
    And I am on my record information page
    When I click the Allergies and adverse reactions section on My Record
    And I see 5 allergies with different date formats

  Scenario: A VISION user has a drug and non drug allergy record
    Given the my record wiremocks are initialised for VISION
    And the GP Practice has enabled demographics functionality
    And the GP Practice has enabled allergies functionality and has a drug and non drug allergy record for VISION
    And I am on my record information page
    When I click the Allergies and adverse reactions section on My Record
    Then I see a drug and non drug allergy record from VISION

  Scenario: A VISION user is shown an appropriate error message when an unknown error occurs retrieving their allergies
    Given the my record wiremocks are initialised for VISION
    And the GP Practice has enabled demographics functionality
    And there is an unknown error getting allergies for VISION
    And I am on my record information page
    When I click the Allergies and adverse reactions section on My Record
    Then I see an error occurred message with Allergies and adverse reactions on My Record
