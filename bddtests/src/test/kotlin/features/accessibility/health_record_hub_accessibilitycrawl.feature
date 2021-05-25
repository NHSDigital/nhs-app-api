@accessibility
Feature: Health record hub page accessibility
  Scenario: Health record hub page is captured
    Given I am an EMIS patient and I have access to Patients Know Best Care Plans
    And I am logged in
    When I navigate to the health record hub page
    Then I see the health records hub page
    And the HealthRecord_Hub page is saved to disk


  Scenario: Prove Your Identity shutter page for P5 user is captured
    Given I am a patient with proof level 5
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the page title is 'Your GP health record'
    And I am asked to prove my identity to access 'gp medical record'
    And the HealthRecord_PYIShutter page is saved to disk


  Scenario: Sensitive information warning page is captured
    Given I am a EMIS user setup to use medical record version 2
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    And the HealthRecord_SensitiveInfo page is saved to disk


  Scenario: Summary Care Record page is captured
    Given I am a EMIS user setup to use medical record version 2
    And the GP Practice has disabled DCR access for the patient
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    Then I see the medical record v2 page with information on asking for DCR access
    And the HealthRecord_SCR page is saved to disk


  Scenario Outline: <GP System> Your GP Health Record page is captured
    Given I am a <GP System> user setup to use medical record version 2
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I see header text is Your GP health record
    And the HealthRecord_<GP System>DCR page is saved to disk

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |


  Scenario: Allergies and adverse reactions page is captured
    Given I am a VISION user setup to use medical record version 2
    And the GP Practice has enabled allergies functionality and has a drug and non drug allergy record for VISION
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Allergies and adverse reactions link on my record - Medical Record v2
    Then I see a drug and non drug allergy record from VISION - Medical Record v2
    And the HealthRecord_Allergies page is saved to disk


  Scenario: Medicine pages are captured
    Given I am a EMIS user setup to use medical record version 2
    And the GP Practice has enabled all medical records for the patient
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Medicines link on my record - Medical Record v2
    Then I see the medical record v2 medicines page
    And the HealthRecord_Medicines page is saved to disk
    When I click the Acute medicines link - Medical Record v2
    Then I see the expected acute medicines - Medical Record v2
    And the HealthRecord_AcuteMedicines page is saved to disk
    When I click the Back link
    Then I see the medical record v2 medicines page
    When I click the Current medicines link - Medical Record v2
    Then I see the expected current medicines - Medical Record v2
    And the HealthRecord_CurrentMedicines page is saved to disk
    When I click the Back link
    Then I see the medical record v2 medicines page
    When I click the Discontinued medicines link - Medical Record v2
    Then I see the expected discontinued medicines - Medical Record v2
    And the HealthRecord_DiscontinuedMedicines page is saved to disk


  Scenario: Immunisations page is captured
    Given I am a VISION user setup to use medical record version 2
    And the GP Practice has enabled immunisations functionality and multiple immunisation records exist
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Immunisations link on my record - Medical Record v2
    Then I see the expected immunisations - Medical Record v2
    And the HealthRecord_Immunisations page is saved to disk


  Scenario: Health conditions page is captured
    Given I am a EMIS user setup to use medical record version 2
    And the GP Practice has enabled problems functionality
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Health conditions link on my record - Medical Record v2
    Then I see the expected health conditions - Medical Record v2
    And the HealthRecord_HealthConditions page is saved to disk


  Scenario: Test results page is captured
    Given I am a EMIS user setup to use medical record version 2
    And the GP Practice has a single test result with multiple child values with ranges for EMIS
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Test results link on my record - Medical Record v2
    Then I see test results with multiple child values some of which have ranges - Medical Record v2
    And the HealthRecord_TestResults page is saved to disk


  Scenario: Consultations and events page is captured
    Given I am a TPP user setup to use medical record version 2
    And the GP Practice has multiple consultations
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Consultations and events link on my record - Medical Record v2
    Then I see the expected consultations and events - Medical Record v2
    And the HealthRecord_Consultations page is saved to disk


  Scenario Outline: <GP System> documents page is captured
    Given I am a <GP System> user setup to use medical record version 2
    And the GP Practice has multiple documents
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Documents link on my record - Medical Record v2
    Then I see a list of documents
    And the HealthRecord_<GP System>Documents page is saved to disk

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |


  Scenario: Document with title is captured
    Given I am a EMIS user setup to use medical record version 2
    And the GP Practice has multiple documents
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Documents link on my record - Medical Record v2
    Then I see a list of documents
    When I select an available document
    Then I see the document information page with actions
    And the HealthRecord_DocumentWithTitle page is saved to disk


  Scenario: Document without title is captured
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
    And the HealthRecord_DocumentWithoutTitle page is saved to disk


  Scenario: Document with comments is captured
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
    And the HealthRecord_DocumentWithComments page is saved to disk


  Scenario: Viewed document is captured
    Given I am a EMIS user setup to use medical record version 2
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
    And the HealthRecord_ViewDocument page is saved to disk


  Scenario: Document not available through NHS App error is captured
    Given I am a EMIS user setup to use medical record version 2
    And the GP Practice has documents with invalid types
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Documents link on my record - Medical Record v2
    Then I see a list of documents
    When I select an available document
    Then I see the document information page without actions
    And the HealthRecord_DocumentNotAvailable page is saved to disk


  Scenario: Proxy shutter page is captured
    Given I am logged in as a EMIS user with linked profiles and appointments provider IM1
    Then I see the home page
    When I can see and follow the Linked profiles link
    Then the linked profiles page is displayed
    And linked profiles are displayed
    And I select a linked profile with appointments enabled false, prescriptions enabled false and medical record enabled false
    And details for the selected linked profile are displayed
    When I click the Switch to this profile button for the proxy user
    Then I see the proxy home page
    When I navigate to Your_Health
    And I click on the Gp medical record link
    And I click the 'Continue' button
    Then the medical record shutter page is displayed
    And the HealthRecord_ProxyShutter page is saved to disk


  Scenario: Session error page is captured
    Given I have valid OAuth details and EMIS fails to respond in 31 seconds
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    Then I see the error reference code with prefix 'ze'
    And the HealthRecord_Error page is saved to disk

