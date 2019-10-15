@my-record
@smoketest

Feature: View My Medical Record Information - All records enabled

  Scenario: A EMIS user can see that the medical record sections are collapsed when directed to medical records page
    Given the my record wiremocks are initialised for EMIS
    And the GP Practice has enabled demographics functionality
    And the GP Practice has enabled all medical records for the patient
    And I am on my record information page
    Then I see the Acute (short-term) medications section collapsed on My Record
    And I see the Allergies and adverse reactions section collapsed on My Record
    And I see the Repeat medications: current section collapsed on My Record
    And I see the Repeat medications: discontinued section collapsed on My Record
    And I see the Immunisations section collapsed on My Record
    And I see the Problems section collapsed on My Record
    And I see the Test results section collapsed on My Record
    And I see the Documents section collapsed on My Record

  Scenario: A VISION user can see that the medical record sections are collapsed when directed to medical records page
    Given the my record wiremocks are initialised for VISION
    And the GP Practice has enabled demographics functionality
    And the GP Practice has enabled all medical records for the patient
    And I am on my record information page
    Then I see the Acute (short-term) medications section collapsed on My Record
    And I see the Allergies and adverse reactions section collapsed on My Record
    And I see the Repeat medications: current section collapsed on My Record
    And I see the Repeat medications: discontinued section collapsed on My Record
    And I see the Immunisations section collapsed on My Record
    And I see the Health conditions section collapsed on My Record
    And I see the Test results section collapsed on My Record
    And I see the examinations section collapsed on My Record
    And I see the diagnosis section collapsed on My Record
    And I see the procedures section collapsed on My Record


  Scenario: A TPP user can see that the medical record sections are collapsed when directed to medical records page
    Given the my record wiremocks are initialised for TPP
    And the GP Practice has enabled demographics functionality
    And the GP Practice has enabled all medical records for the patient
    And I am on my record information page
    Then I see the Acute (short-term) medications section collapsed on My Record
    And I see the Allergies and adverse reactions section collapsed on My Record
    And I see the Repeat medications: current section collapsed on My Record
    And I see the Repeat medications: discontinued section collapsed on My Record
    And I see the Consultations section collapsed on My Record
    And I see the Test results section collapsed on My Record


  Scenario: A VISION user can view medications, immunisations, problems, allergies and test results
    Given the my record wiremocks are initialised for VISION
    And the GP Practice has enabled demographics functionality
    And the GP Practice has enabled all medical records for the patient
    And I am on my record information page
    When I click the Acute (short-term) medications section on My Record
    Then I see acute medication information
    When I click the Repeat medications: current section on My Record
    Then I see current repeat medication information
    When I click the Repeat medications: discontinued section on My Record
    Then I see discontinued repeat medication information
    When I click the Immunisations section on My Record
    Then I see immunisation records displayed
    When I click the Health conditions section on My Record
    Then I see health condition records displayed
    When I click the Allergies and adverse reactions section on My Record
    Then I see allergies record displayed
    When I click the test result section
    Then I see test result information

  Scenario: A EMIS user can view medications, immunisations, problems, allergies, consultations and test results
    Given the my record wiremocks are initialised for EMIS
    And the GP Practice has enabled demographics functionality
    And the GP Practice has enabled all medical records for the patient
    And I am on my record information page
    When I click the Acute (short-term) medications section on My Record
    Then I see acute medication information
    When I click the Repeat medications: current section on My Record
    Then I see current repeat medication information
    When I click the Repeat medications: discontinued section on My Record
    Then I see discontinued repeat medication information
    When I click the Immunisations section on My Record
    Then I see immunisation records displayed
    When I click the Health conditions section on My Record
    Then I see health condition records displayed
    When I click the Allergies and adverse reactions section on My Record
    Then I see allergies record displayed
    When I click the Consultations section on My Record
    Then I see Consultations records displayed
    When I click the test result section
    Then I see test result information


  Scenario: A TPP usercan view allergies, medications, test results and consultations
    Given the my record wiremocks are initialised for TPP
    And the GP Practice has enabled demographics functionality
    And the GP Practice has enabled all medical records for the patient
    And I am on my record information page
    When I click the Acute (short-term) medications section on My Record
    Then I see acute medication information
    When I click the Repeat medications: current section on My Record
    Then I see current repeat medication information
    When I click the Repeat medications: discontinued section on My Record
    Then I see discontinued repeat medication information
    When I click the Allergies and adverse reactions section on My Record
    Then I see allergies record displayed
    When I click the test result section
    Then I see test result information
    When I click the Consultations section on My Record
    Then I see Consultations records displayed

  @bug @NHSO-6233
  Scenario: A VISION user can view examinations, diagnosis and procedures information
    Given the my record wiremocks are initialised for VISION
    And the GP Practice has enabled demographics functionality
    And the GP Practice has enabled all medical records for the patient
    And I am on my record information page
    When I click the examinations section
    Then I see examinations information
    And I click on the Back link on my records page
    When I click the diagnosis section
    Then I see diagnosis information
    And I click on the Back link on my records page
    When I click the procedures section
    Then I see procedures information