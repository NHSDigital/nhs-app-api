Feature: View My Medical Record Information

  Background:
    Given wiremock is initialised

  @NHSO-361
  Scenario: An EMIS user with access navigates to the patient record information page
    Given I am logged in
    Given the GP Practice has enabled demographics functionality
    And I am on the record warning page
    When I click agree and continue
    Then the my record information screen is loaded

  @NHSO-361
  Scenario: An EMIS user navigates to patient information page
    Given I am logged in
    Given the GP Practice has enabled demographics functionality
    And I am on the record warning page
    Then I click agree and continue
    And I see header text is My medical record
    And I see heading My details
    And I see patient information details
    And I see my record button on the nav bar is highlighted

  @NHSO-361
  Scenario: An EMIS user collapses the patient details section
    Given I am logged in
    Given the GP Practice has enabled demographics functionality
    And I am on my record information page
    When I click My details heading
    Then I do not see patient information details

  @pending
  @NHSO-678
  Scenario: An EMIS user views acute medications
    Given I am logged in
    Given the GP Practice has enabled summary care record functionality
    And I am on my record information page
    And I see heading Acute medications
    When I click acute medications
    Then I see acute medication information

  @pending
  @NHSO-678
  Scenario: An EMIS user views current repeat medications
    Given I am logged in
    Given the GP Practice has enabled summary care record functionality
    And I am on my record information page
    And I see heading Current repeat medications
    When I click current repeat medications
    Then I see current repeat medication information

  @pending
  @NHSO-678
  Scenario: An EMIS user views discontinued repeat medications
    Given I am logged in
    Given the GP Practice has enabled summary care record functionality
    And I am on my record information page
    And I see heading Discontinued repeat medications
    When I click discontinued repeat medications
    Then I see current repeat medication information

  @pending
  @NHSO-678
  Scenario: An EMIS user has no acute medications
    Given I am logged in
    Given the GP Practice has enabled summary care record functionality
    And I am on my record information page
    When I click acute medications
    Then I see a message indicating that I have no medications

  @pending
  @NHSO-678
  Scenario: An EMIS user has no current repeat medications
    Given I am logged in
    Given the GP Practice has enabled summary care record functionality
    And I am on my record information page
    When I click current repeat medications
    Then I see a message indicating that I have no medications

  @pending
  @NHSO-678
  Scenario: An EMIS user has no discontinued repeat medications
    Given I am logged in
    Given the GP Practice has enabled summary care record functionality
    And I am on my record information page
    When I click discontinued repeat medications
    Then I see a message indicating that I have no medications

  @pending
  @NHSO-678
  Scenario: An EMIS user has no access to view acute medications
    Given I am logged in
    Given the GP Practice has disabled summary care record functionality
    And I am on my record information page
    Then I see a message indicating that I have no access to view my record

  @pending
  @NHSO-678
  Scenario: An EMIS user has no access to view current repeat medications
    Given I am logged in
    Given the GP Practice has disabled summary care record functionality
    And I am on my record information page
    Then I see a message indicating that I have no access to view my record

  @pending
  @NHSO-678
  Scenario: An EMIS user has no access to view discontinued repeat medications
    Given I am logged in
    Given the GP Practice has disabled summary care record functionality
    And I am on my record information page
    Then I see a message indicating that I have no access to view my record