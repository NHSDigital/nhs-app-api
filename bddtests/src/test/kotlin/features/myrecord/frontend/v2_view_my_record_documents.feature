@my-record
@documents
Feature: Documents Frontend - Medical Record v2

  Scenario Outline: An <GP System> user who has no Documents on their record can see a message informing them of this - Medical Record v2
    Given I am a <GP System> user setup to use medical record version 2
    And the GP Practice has no documents
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Documents link on my record - Medical Record v2
    Then I see a message that I have no information recorded for a specific record - Medical Record v2
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario: An EMIS user who does not have access to Documents on their record can see a message informing them of this - Medical Record v2
    Given I am a EMIS user setup to use medical record version 2
    And the Patient has no access to Documents
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Documents link on my record - Medical Record v2
    Then I see a message indicating that I have no access to view this section on My Record - Medical Record v2

  Scenario Outline: An <GP System> user can view a list of Documents on their record - Medical Record v2
    Given I am a <GP System> user setup to use medical record version 2
    And the GP Practice has multiple documents
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Documents link on my record - Medical Record v2
    Then I see a list of documents
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario: An EMIS user can view a list of Documents on their record if the pageCount is null
    Given I am a EMIS user setup to use medical record version 2
    And the GP Practice has a document with a null page count
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Documents link on my record - Medical Record v2
    Then I see a list of documents

  Scenario: An EMIS user can view a list of Documents on their record if the size is null
    Given I am a EMIS user setup to use medical record version 2
    And the GP Practice has a document with a null size property
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Documents link on my record - Medical Record v2
    Then I see a list of documents

  Scenario: An EMIS user who views a document with no term sees the document date as the header - Medical Record v2
    Given I am a EMIS user setup to use medical record version 2
    And the GP Practice has multiple documents with no name or term
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Documents link on my record - Medical Record v2
    Then I see a list of documents
    When I select an available document
    Then I see the document information page with the document date as the header

  Scenario: A TPP user who views a document will see the correct header
    Given I am a TPP user setup to use medical record version 2
    And the GP Practice has multiple documents with no name or term
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Documents link on my record - Medical Record v2
    Then I see a list of documents
    When I select an available document
    Then I see the document information page with the document header

  Scenario: A TPP user who views a document with comments can see them
    Given I am a TPP user setup to use medical record version 2
    And the GP Practice has multiple documents with no name or term
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Documents link on my record - Medical Record v2
    Then I see a list of documents
    When I select an available document
    Then I see the document information page with comments

  Scenario: A TPP user who views a letter will see the correct header
    Given I am a TPP user setup to use medical record version 2
    And the GP Practice has multiple letters with no name or term
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Documents link on my record - Medical Record v2
    Then I see a list of documents
    When I select an available document
    Then I see the document information page with the letter header

  Scenario: A TPP user who accesses a file that is still uploading cannot view or download it
    Given I am a TPP user setup to use medical record version 2
    And the GP practice has a file that is still uploading
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Documents link on my record - Medical Record v2
    Then I see a list of documents
    When I select an available document
    Then I see the document information page without actions

  Scenario Outline: A <GP System> user who selects a large document cannot download or view it - Medical Record v2
    Given I am a <GP System> user setup to use medical record version 2
    And the GP Practice has multiple large documents
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Documents link on my record - Medical Record v2
    Then I see a list of documents
    When I select an available document
    Then I see the document information page without actions
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario Outline: A <GP System> user who selects a document with an invalid type cannot download or view it - Medical Record v2
    Given I am a <GP System> user setup to use medical record version 2
    And the GP Practice has documents with invalid types
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Documents link on my record - Medical Record v2
    Then I see a list of documents
    When I select an available document
    Then I see the document information page without actions
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario Outline: An <GP System> user can view an individual Document from their record - Medical Record v2
    Given I am a <GP System> user setup to use medical record version 2
    And the GP Practice has multiple documents
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Documents link on my record - Medical Record v2
    Then I see a list of documents
    When I select an available document
    Then I see the document information page with actions
    When I click the View action link on the document information page
    Then I can see my document
    When I click the Back link
    Then I see the document information page with actions
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  #This test only works on Chrome
  Scenario Outline: A <GP System> user can download a document from their record - Medical Record v2
    Given I am a <GP System> user setup to use medical record version 2
    And the GP Practice has multiple documents
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Documents link on my record - Medical Record v2
    Then I see a list of documents
    When I select an available document
    Then I see the document information page with actions
    When I click the Download action link on the document information page
    Then The file has been downloaded
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario Outline: A <GP System> user can download a non-viewable document from their record - Medical Record v2
    Given I am a <GP System> user setup to use medical record version 2
    And the GP Practice has multiple non-viewable documents
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Documents link on my record - Medical Record v2
    Then I see a list of documents
    When I select an available document
    Then I see the document information page with download action only
    Examples:
      | GP System |
      | TPP       |

  Scenario: An EMIS user selecting an unavailable or invalid document will see an error page - Medical Record v2
    Given I am a EMIS user setup to use medical record version 2
    And the GP Practice has multiple documents where one has an invalid id
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Documents link on my record - Medical Record v2
    Then I see a list of documents
    When I select a document that has an invalid id
    Then I see the document information page with actions
    When I click the View action link on the document information page
    Then I see the appropriate error message for a document server error
    When I click the error '111.nhs.uk' link with a url of 'https://111.nhs.uk'
    Then a new tab has been opened by the link

  Scenario: An EMIS user has a document result with an unknown date
    Given I am a EMIS user setup to use medical record version 2
    And the EMIS GP Practice has three document results where the first record has no date
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Documents link on my record - Medical Record v2
    Then I see the expected list of documents displayed with unknown date for the last result
