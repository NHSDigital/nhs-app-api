Feature: View My Medical Record Information

  Background:
    Given wiremock is initialised

  @NHSO-678
  Scenario: An EMIS user views acute medications
    Given I am logged in
    And the GP Practice has enabled demographics functionality
    And the GP Practice has enabled medications functionality
    And I am on my record information page
    And I see heading Acute medications
    When I click acute medications
    Then I see acute medication information

  @NHSO-678
  Scenario: An EMIS user views current repeat medications
    Given I am logged in
    And the GP Practice has enabled demographics functionality
    And the GP Practice has enabled medications functionality
    And I am on my record information page
    When I click current repeat medications
    Then I see current repeat medication information

  @NHSO-678
  Scenario: An EMIS user views discontinued repeat medications
    Given I am logged in
    And the GP Practice has enabled demographics functionality
    And the GP Practice has enabled medications functionality
    And I am on my record information page
    When I click discontinued repeat medications
    Then I see discontinued repeat medication information

  @NHSO-678
  Scenario: An EMIS user has no acute medications
    Given I am logged in
    And the GP Practice has enabled demographics functionality
    And the GP Practice has enabled medication functionality and the patient has no medications
    And I am on my record information page
    When I click acute medications
    Then I see a message indicating that I have no "acute" medications

  @NHSO-678
  Scenario: An EMIS user has no current repeat medications
    Given I am logged in
    And the GP Practice has enabled demographics functionality
    And the GP Practice has enabled medication functionality and the patient has no medications
    And I am on my record information page
    When I click current repeat medications
    Then I see a message indicating that I have no "current repeat" medications

  @NHSO-678
  Scenario: An EMIS user has no discontinued repeat medications
    Given I am logged in
    And the GP Practice has enabled demographics functionality
    And the GP Practice has enabled medication functionality and the patient has no medications
    And I am on my record information page
    When I click discontinued repeat medications
    Then I see a message indicating that I have no "discontinued repeat" medications

  @NHSO-678
  Scenario: An EMIS user has no access to view acute medications
    Given I am logged in
    And the GP Practice has enabled demographics functionality
    And the GP Practice has disabled summary care record functionality
    And I am on my record information page
    Then I see a message indicating that I have no access to view my record

  @NHSO-678
  Scenario: An EMIS user has no access to view current repeat medications
    Given I am logged in
    And the GP Practice has enabled demographics functionality
    And the GP Practice has disabled summary care record functionality
    And I am on my record information page
    Then I see a message indicating that I have no access to view my record

  @NHSO-678
  Scenario: An EMIS user has no access to view discontinued repeat medications
    Given I am logged in
    And the GP Practice has enabled demographics functionality
    And the GP Practice has disabled summary care record functionality
    And I am on my record information page
    Then I see a message indicating that I have no access to view my record