@my-record
@documents
Feature: Documents Frontend - Medical Record v2

  Scenario: An EMIS user who has no Documents on their record can see a message informing them of this - Medical Record v2
    Given I am a EMIS user setup to use medical record version 2
    And the GP Practice has no documents
    And I am on the medical record page
    When I click the Documents link on my record - Medical Record v2
    Then I see a message that I have no information recorded for a specific record - Medical Record v2

  Scenario: An EMIS user who does not have access to Documents on their record can see a message informing them of this - Medical Record v2
    Given I am a EMIS user setup to use medical record version 2
    And the Patient has no access to Documents
    And I am on the medical record page
    When I click the Documents link on my record - Medical Record v2
    Then I see a message indicating that I have no access to view this section on My Record - Medical Record v2

  Scenario: An EMIS user can view a list of Documents on their record - Medical Record v2
    Given I am a EMIS user setup to use medical record version 2
    And the GP Practice has multiple documents
    And I am on the medical record page
    When I click the Documents link on my record - Medical Record v2
    Then I see a list of documents

  Scenario: An EMIS user can view a list of Documents on their record if the pageCount is null
    Given I am a EMIS user setup to use medical record version 2
    And the GP Practice has a document with a null page count
    And I am on the medical record page
    When I click the Documents link on my record - Medical Record v2
    Then I see a list of documents

  Scenario: An EMIS user who views a document with no name sees the document date as the header - Medical Record v2
    Given I am a EMIS user setup to use medical record version 2
    And the GP Practice has multiple documents with no name or term
    And I am on the medical record page
    When I click the Documents link on my record - Medical Record v2
    Then I see a list of documents
    When I select an available document
    Then I see the document information page with the document date as the header

  Scenario: An EMIS user who selects a large document cannot download or view it - Medical Record v2
    Given I am a EMIS user setup to use medical record version 2
    And the GP Practice has multiple large documents
    And I am on the medical record page
    When I click the Documents link on my record - Medical Record v2
    Then I see a list of documents
    When I select an available document
    Then I see the document information page without actions

  Scenario: An EMIS user who selects a document with an invalid type cannot download or view it - Medical Record v2
    Given I am a EMIS user setup to use medical record version 2
    And the GP Practice has documents with invalid types
    And I am on the medical record page
    When I click the Documents link on my record - Medical Record v2
    Then I see a list of documents
    When I select an available document
    Then I see the document information page without actions

  Scenario: An EMIS user can view an individual Document from their record - Medical Record v2
    Given I am a EMIS user setup to use medical record version 2
    And the GP Practice has multiple documents
    And I am on the medical record page
    When I click the Documents link on my record - Medical Record v2
    Then I see a list of documents
    When I select an available document
    Then I see the document information page with actions
    When I click the View action link on the document information page
    Then I can see my document
    When I click the Back link
    Then I see the document information page with actions

  Scenario: An EMIS user can download a document from their record - Medical Record v2
    Given I am a EMIS user setup to use medical record version 2
    And the GP Practice has multiple documents
    And I am on the medical record page
    When I click the Documents link on my record - Medical Record v2
    Then I see a list of documents
    When I select an available document
    Then The download action item is enabled

  Scenario: An EMIS user selecting an unavailable or invalid document will see an error page - Medical Record v2
    Given I am a EMIS user setup to use medical record version 2
    And the GP Practice has multiple documents where one has an invalid id
    And I am on the medical record page
    When I click the Documents link on my record - Medical Record v2
    Then I see a list of documents
    When I select a document that has an invalid id
    Then I see the document information page with actions
    When I click the View action link on the document information page
    Then I see the appropriate error message for a document server error