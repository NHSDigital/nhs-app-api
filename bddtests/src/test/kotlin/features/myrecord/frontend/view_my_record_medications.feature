@my-record
Feature: View My Medical Record Information - Medications

  @NHSO-678
  Scenario Outline: A <Service> user views acute medications
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has enabled medications functionality for <Service>
    And I am on my record information page
    And I see heading Acute medications
    When I click acute medications
    Then I see acute medication information

    Examples:
      | Service |
      | EMIS    |
      | TPP     |

  @NHSO-678
  Scenario Outline: A <Service> user views current repeat medications
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has enabled medications functionality for <Service>
    And I am on my record information page
    When I click current repeat medications
    Then I see current repeat medication information

    Examples:
      | Service |
      | EMIS    |
      | TPP     |

  @NHSO-678
  Scenario Outline: A <Service> user views discontinued repeat medications
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has enabled medications functionality for <Service>
    And I am on my record information page
    When I click discontinued repeat medications
    Then I see discontinued repeat medication information

    Examples:
      | Service |
      | EMIS    |
      | TPP     |

  @NHSO-678
  Scenario Outline: A <Service> user has no acute medications
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has enabled medication functionality and the patient has no medications for <Service>
    And I am on my record information page
    When I click acute medications
    Then I see a message indicating that I have no "acute" medications

    Examples:
      | Service |
      | EMIS    |
      | TPP     |

  @NHSO-678
  Scenario Outline: A <Service> user has no current repeat medications
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has enabled medication functionality and the patient has no medications for <Service>
    And I am on my record information page
    When I click current repeat medications
    Then I see a message indicating that I have no "current repeat" medications

    Examples:
      | Service |
      | EMIS    |
      | TPP     |

  @NHSO-678
  Scenario Outline: A <Service> user has no discontinued repeat medications
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has enabled medication functionality and the patient has no medications for <Service>
    And I am on my record information page
    When I click discontinued repeat medications
    Then I see a message indicating that I have no "discontinued repeat" medications

    Examples:
      | Service |
      | EMIS    |
      | TPP     |

  @NHSO-678
  Scenario Outline: A <Service> user has no access to view medications
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has disabled summary care record functionality for <Service>
    And I am on my record information page
    Then I see a message indicating that I have no access to view my summary care record

    Examples:
      | Service |
      | EMIS    |
      | TPP     |