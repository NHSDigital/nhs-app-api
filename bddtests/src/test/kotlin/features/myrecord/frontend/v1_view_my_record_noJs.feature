@my-record
@noJs
Feature: No Javascript Frontend - Medical Record v1

  Scenario Outline: A <GP System> user can view allergies, consultations, demographics, and test results - Medical Record v1
    Given I have disabled javascript
    And I am a <GP System> user setup to use medical record version 1
    And the GP Practice has enabled allergies functionality and the patient has "2" allergies
    And the GP Practice has multiple consultations
    And the GP Practice has six test results
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    Then I see the medical record page - Medical Record v1
    And I see one or more drug type allergies record displayed - Medical Record v1
    And I see Consultations records displayed - Medical Record v1
    And I see test result information - Medical Record v1

    Examples:
      | GP System |
      | EMIS    |
      | TPP     |

  Scenario Outline: A <GP System> user can view acute, current and discontinued medications - Medical Record v1
    Given I have disabled javascript
    And I am a <GP System> user setup to use medical record version 1
    And the GP Practice has enabled medications functionality
    And I am logged in
    When I retrieve the 'my record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    Then I see acute medication information - Medical Record v1
    And I see current repeat medication information - Medical Record v1
    And I see discontinued repeat medication information - Medical Record v1

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: A <GP System> user can view immunisations and health conditions - Medical Record v1
    Given I have disabled javascript
    And I am a <GP System> user setup to use medical record version 1
    And the GP Practice has enabled immunisations functionality and multiple immunisation records exist
    And the GP Practice has enabled problems functionality
    And I am logged in
    When I retrieve the 'my record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    Then I see immunisation records displayed - Medical Record v1
    And I see health condition records displayed - Medical Record v1

    Examples:
      | GP System |
      | EMIS      |
      | VISION    |

  Scenario: A VISION user can view allergies and demographics - Medical Record v1
    Given I have disabled javascript
    And I am a VISION user setup to use medical record version 1
    And the GP Practice has enabled allergies functionality and has a drug and non drug allergy record for VISION
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    Then I see the medical record page - Medical Record v1
    And I see a drug and non drug allergy record from VISION - Medical Record v1

  Scenario: A VISION user can view diagnosis information without Javascript - Medical Record v1
    Given I have disabled javascript
    And I am a VISION user setup to use medical record version 1
    And the GP Practice has multiple diagnosis
    And I am logged in
    When I retrieve the 'my record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    And I click the diagnosis section
    Then I see diagnosis information - Medical Record v1

  Scenario: A VISION user can view examinations information without Javascript - Medical Record v1
    Given I have disabled javascript
    And I am a VISION user setup to use medical record version 1
    And the GP Practice has enabled demographics functionality
    And the GP Practice has multiple examinations
    And I am logged in
    When I retrieve the 'my record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    And I click the Examinations section on My Record - Medical Record v1
    Then I see examinations information - Medical Record v1

  Scenario: A VISION user can view procedures information without Javascript - Medical Record v1
    Given I have disabled javascript
    And I am a VISION user setup to use medical record version 1
    And the GP Practice has enabled demographics functionality
    And the GP Practice has multiple procedures
    And I am logged in
    When I retrieve the 'my record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    And I click the Procedures section on My Record - Medical Record v1
    Then I see procedures information - Medical Record v1
