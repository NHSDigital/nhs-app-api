@my-record
Feature: All records enabled - Medical Record v1

  Scenario: A EMIS user can see that the medical record sections are collapsed - Medical Record v1
    Given I am a EMIS user setup to use medical record version 1
    And the GP Practice has enabled all medical records for the patient
    And I am on the medical record page
    Then I see the Acute (short-term) medications section collapsed on My Record - Medical Record v1
    And I see the Allergies and adverse reactions section collapsed on My Record - Medical Record v1
    And I see the Repeat medications: current section collapsed on My Record - Medical Record v1
    And I see the Repeat medications: discontinued section collapsed on My Record - Medical Record v1
    And I see the Immunisations section collapsed on My Record - Medical Record v1
    And I see the Problems section collapsed on My Record - Medical Record v1
    And I see the Test results section collapsed on My Record - Medical Record v1
    And I see the Documents section collapsed on My Record - Medical Record v1

  Scenario: A VISION user can see that the medical record are sections collapsed - Medical Record v1
    Given I am a VISION user setup to use medical record version 1
    And the GP Practice has enabled all medical records for the patient
    And I am on the medical record page
    Then I see the Acute (short-term) medications section collapsed on My Record - Medical Record v1
    And I see the Allergies and adverse reactions section collapsed on My Record - Medical Record v1
    And I see the Repeat medications: current section collapsed on My Record - Medical Record v1
    And I see the Repeat medications: discontinued section collapsed on My Record - Medical Record v1
    And I see the Immunisations section collapsed on My Record - Medical Record v1
    And I see the Health conditions section collapsed on My Record - Medical Record v1
    And I see the Test results section collapsed on My Record - Medical Record v1
    And I see the examinations section collapsed on My Record - Medical Record v1
    And I see the diagnosis section collapsed on My Record - Medical Record v1
    And I see the procedures section collapsed on My Record - Medical Record v1


  Scenario: A TPP user can see that the medical record sections are collapsed - Medical Record v1
    Given I am a TPP user setup to use medical record version 1
    And the GP Practice has enabled all medical records for the patient
    And I am on the medical record page
    Then I see the Acute (short-term) medications section collapsed on My Record - Medical Record v1
    And I see the Allergies and adverse reactions section collapsed on My Record - Medical Record v1
    And I see the Repeat medications: current section collapsed on My Record - Medical Record v1
    And I see the Repeat medications: discontinued section collapsed on My Record - Medical Record v1
    And I see the Consultations section collapsed on My Record - Medical Record v1
    And I see the Test results section collapsed on My Record - Medical Record v1


  Scenario: A VISION user can view medications, immunisations, problems, allergies and test results - Medical Record v1
    Given I am a VISION user setup to use medical record version 1
    And the GP Practice has enabled all medical records for the patient
    And I am on the medical record page
    When I click the Acute (short-term) medications section on My Record - Medical Record v1
    Then I see acute medication information - Medical Record v1
    When I click the Repeat medications: current section on My Record - Medical Record v1
    Then I see current repeat medication information - Medical Record v1
    When I click the Repeat medications: discontinued section on My Record - Medical Record v1
    Then I see discontinued repeat medication information - Medical Record v1
    When I click the Immunisations section on My Record - Medical Record v1
    Then I see immunisation records displayed - Medical Record v1
    When I click the Health conditions section on My Record - Medical Record v1
    Then I see health condition records displayed - Medical Record v1
    When I click the Allergies and adverse reactions section on My Record - Medical Record v1
    Then I see allergies record displayed - Medical Record v1
    When I click the Test results section on My Record - Medical Record v1
    Then I see test result information - Medical Record v1

  Scenario: A EMIS user can view medications, immunisations, problems, allergies, consultations, and test results - Medical Record v1
    Given I am a EMIS user setup to use medical record version 1
    And the GP Practice has enabled all medical records for the patient
    And I am on the medical record page
    When I click the Acute (short-term) medications section on My Record - Medical Record v1
    Then I see acute medication information - Medical Record v1
    When I click the Repeat medications: current section on My Record - Medical Record v1
    Then I see current repeat medication information - Medical Record v1
    When I click the Repeat medications: discontinued section on My Record - Medical Record v1
    Then I see discontinued repeat medication information - Medical Record v1
    When I click the Immunisations section on My Record - Medical Record v1
    Then I see immunisation records displayed - Medical Record v1
    When I click the Health conditions section on My Record - Medical Record v1
    Then I see health condition records displayed - Medical Record v1
    When I click the Allergies and adverse reactions section on My Record - Medical Record v1
    Then I see allergies record displayed - Medical Record v1
    When I click the Consultations section on My Record - Medical Record v1
    Then I see Consultations records displayed - Medical Record v1
    When I click the Test results section on My Record - Medical Record v1
    Then I see test result information - Medical Record v1


  Scenario: A TPP user can view allergies, medications, test results, and consultations - Medical Record v1
    Given I am a TPP user setup to use medical record version 1
    And the GP Practice has enabled all medical records for the patient
    And I am on the medical record page
    When I click the Acute (short-term) medications section on My Record - Medical Record v1
    Then I see acute medication information - Medical Record v1
    When I click the Repeat medications: current section on My Record - Medical Record v1
    Then I see current repeat medication information - Medical Record v1
    When I click the Repeat medications: discontinued section on My Record - Medical Record v1
    Then I see discontinued repeat medication information - Medical Record v1
    When I click the Allergies and adverse reactions section on My Record - Medical Record v1
    Then I see allergies record displayed - Medical Record v1
    When I click the Test results section on My Record - Medical Record v1
    Then I see test result information - Medical Record v1
    When I click the Consultations section on My Record - Medical Record v1
    Then I see Consultations records displayed - Medical Record v1

  @bug @NHSO-6233
  Scenario: A VISION user can view examinations, diagnosis, and procedures - Medical Record v1
    Given I am a VISION user setup to use medical record version 1
    And the GP Practice has enabled all medical records for the patient
    And I am on the medical record page
    When I click the Examinations section on My Record - Medical Record v1
    Then I see examinations information - Medical Record v1
    And I click on the Back link on the Medical Record page - Medical Record v1
    When I click the Diagnosis section on My Record - Medical Record v1
    Then I see diagnosis information - Medical Record v1
    And I click on the Back link on the Medical Record page - Medical Record v1
    When I click the Procedures section on My Record - Medical Record v1
    Then I see procedures information - Medical Record v1
