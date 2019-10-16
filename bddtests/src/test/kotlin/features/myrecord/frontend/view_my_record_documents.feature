@my-record
@documents
Feature: View My Medical Record Information - Documents Frontend

  Scenario: An EMIS user who has no Documents on their record can see a message informing them of this
    Given the my record wiremocks are initialised for EMIS
    And the GP Practice has enabled demographics functionality
    And the GP Practice has no documents
    And I am on my record information page
    Then I see the Documents heading on My Record
    When I click the Documents section on My Record
    Then I see a message indicating that I have no information recorded for Documents on My Record

  Scenario: An EMIS user who does not have access to Documents on their record can see a message informing them of this
    Given the my record wiremocks are initialised for EMIS
    And the GP Practice has enabled demographics functionality
    And the Patient has no access to Documents
    And I am on my record information page
    Then I see the Documents heading on My Record
    When I click the Documents section on My Record
    Then I see a message indicating that I have no access to view Documents on My Record

  Scenario: An EMIS user can view a list of Documents on their record
    Given the my record wiremocks are initialised for EMIS
    And the GP Practice has enabled demographics functionality
    And the GP Practice has multiple documents
    And I am on my record information page
    When I select to view my documents
    Then I see a list of documents

  Scenario: An EMIS user who views a document with no name sees the document date as the header
    Given the my record wiremocks are initialised for EMIS
    And the GP Practice has enabled demographics functionality
    And the GP Practice has multiple documents with no name
    And I am on my record information page
    When I select to view my documents
    Then I see a list of documents
    When I select an available document
    Then I see the document information page with the document date as the header

  Scenario: An EMIS user can view an individual Document from their record
    Given the my record wiremocks are initialised for EMIS
    And the GP Practice has enabled demographics functionality
    And the GP Practice has multiple documents
    And I am on my record information page
    When I select to view my documents
    Then I see a list of documents
    When I select an available document
    Then I see the document information page
    When I click the View action link on the document information page
    Then I can see my document
    When I click the Back link
    Then I see a list of documents

  Scenario: An EMIS user selecting an unavailable or invalid document will see an error page
    Given the my record wiremocks are initialised for EMIS
    And the GP Practice has enabled demographics functionality
    And the GP Practice has multiple documents where one has an invalid id
    And I am on my record information page
    When I select to view my documents
    Then I see a list of documents
    When I select a document that has an invalid id
    Then I see the document information page
    When I click the View action link on the document information page
    Then I see the appropriate error message for a document server error