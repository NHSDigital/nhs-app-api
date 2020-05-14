@my-record
Feature: Combined Frontend - Medical Record v1

  Scenario Outline: A <GP System> user can view allergies, consultations, demographics and test results - Medical Record v1
    Given I am a <GP System> user setup to use medical record version 1
    And the GP Practice has enabled allergies functionality and the patient has "2" allergies
    And the GP Practice has multiple consultations
    And the GP Practice has six test results
    When I am on the Medical Record Warning page
    Then I see the Medical Record Warning page
    When I click continue
    Then I see the medical record page - Medical Record v1
    When I click the Allergies and adverse reactions section on My Record - Medical Record v1
    Then I see one or more drug type allergies record displayed - Medical Record v1
    When I click the Consultations section on My Record - Medical Record v1
    Then I see Consultations records displayed - Medical Record v1
    When I click the Test results section on My Record - Medical Record v1
    Then I see test result information - Medical Record v1
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario Outline: A <GP System> user receives bad allergies and consultations data, demographics and OK test results data - Medical Record v1
    Given I am a <GP System> user setup to use medical record version 1
    And the GP practice returns a bad allergies response
    And the GP practice returns bad consultations data
    And the GP Practice has six test results
    And I am logged in
    When I retrieve the 'my record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    Then I see the medical record page - Medical Record v1
    When I click the Allergies and adverse reactions section on My Record - Medical Record v1
    Then I see an error occurred message with Allergies and adverse reactions on My Record - Medical Record v1
    When I click the Consultations section on My Record - Medical Record v1
    Then I see an error occurred message with Consultations on My Record - Medical Record v1
    When I click the Test results section on My Record - Medical Record v1
    Then I see test result information - Medical Record v1
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |


  Scenario Outline: A <GP System> user receives bad allergies, consultations, demographics and test results data - Medical Record v1
    Given I am a <GP System> user setup to use medical record version 1
    And the GP practice returns a bad allergies response
    And the GP practice returns bad consultations data
    And the GP Practice sends a bad test results response
    And I am logged in
    When I retrieve the 'my record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    Then I see the medical record page - Medical Record v1
    When I click the Allergies and adverse reactions section on My Record - Medical Record v1
    Then I see an error occurred message with Allergies and adverse reactions on My Record - Medical Record v1
    When I click the Consultations section on My Record - Medical Record v1
    Then I see an error occurred message with Consultations on My Record - Medical Record v1
    When I click the Test results section on My Record - Medical Record v1
    Then I see an error occurred message with Test results on My Record - Medical Record v1
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |


  Scenario Outline: A <GP System> user receives bad acute, current and discontinued medications data - Medical Record v1
    Given I am a <GP System> user setup to use medical record version 1
    And the GP practice returns bad medications data
    And I am logged in
    When I retrieve the 'my record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    And I click the Acute (short-term) medications section on My Record - Medical Record v1
    Then I see an error occurred message with Acute (short-term) medications on My Record - Medical Record v1
    When I click the Repeat medications: current section on My Record - Medical Record v1
    Then I see an error occurred message with Repeat medications: current on My Record - Medical Record v1
    When I click the Repeat medications: discontinued section on My Record - Medical Record v1
    Then I see an error occurred message with Repeat medications: discontinued on My Record - Medical Record v1
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: A <GP System> user can view acute, current and discontinued medications - Medical Record v1
    Given I am a <GP System> user setup to use medical record version 1
    And the GP Practice has enabled medications functionality
    And I am logged in
    When I retrieve the 'my record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    And I click the Acute (short-term) medications section on My Record - Medical Record v1
    Then I see acute medication information - Medical Record v1
    When I click the Repeat medications: current section on My Record - Medical Record v1
    Then I see current repeat medication information - Medical Record v1
    When I click the Repeat medications: discontinued section on My Record - Medical Record v1
    Then I see discontinued repeat medication information - Medical Record v1
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |


  Scenario Outline: A <GP System> user receives bad immunisations data and OK problems data - Medical Record v1
    Given I am a <GP System> user setup to use medical record version 1
    And the GP practice returns a bad immunisations response
    And the GP Practice has enabled problems functionality
    And I am logged in
    When I retrieve the 'my record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    And I click the Immunisations section on My Record - Medical Record v1
    Then I see an error occurred message with Immunisations on My Record - Medical Record v1
    When I click the Health conditions section on My Record - Medical Record v1
    Then I see health condition records displayed - Medical Record v1
    Examples:
      | GP System |
      | EMIS      |
      | VISION    |

  Scenario Outline: A <GP System> user can view immunisations and problems - Medical Record v1
    Given I am a <GP System> user setup to use medical record version 1
    And the GP Practice has enabled immunisations functionality and multiple immunisation records exist
    And the GP Practice has enabled problems functionality
    And I am logged in
    When I retrieve the 'my record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    And I click the Immunisations section on My Record - Medical Record v1
    Then I see immunisation records displayed - Medical Record v1
    When I click the Health conditions section on My Record - Medical Record v1
    Then I see health condition records displayed - Medical Record v1
    Examples:
      | GP System |
      | EMIS      |
      | VISION    |

  Scenario Outline: A <GP System> user receives bad immunisations and problems - Medical Record v1
    Given I am a <GP System> user setup to use medical record version 1
    And the GP practice returns a bad immunisations response
    And there is bad problems data returned
    And I am logged in
    When I retrieve the 'my record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    And I click the Immunisations section on My Record - Medical Record v1
    Then I see an error occurred message with Immunisations on My Record - Medical Record v1
    When I click the Health conditions section on My Record - Medical Record v1
    Then I see an error occurred message with Health conditions on My Record - Medical Record v1
    Examples:
      | GP System |
      | EMIS      |
      | VISION    |

  Scenario: A VISION user can view allergies and demographics - Medical Record v1
    Given I am a VISION user setup to use medical record version 1
    And the GP Practice has enabled allergies functionality and has a drug and non drug allergy record for VISION
    And I am logged in
    When I retrieve the 'my record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    Then I see the medical record page - Medical Record v1
    When I click the Allergies and adverse reactions section on My Record - Medical Record v1
    Then I see a drug and non drug allergy record from VISION - Medical Record v1
