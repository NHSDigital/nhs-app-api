Feature: View My Medical Record Information

  Background:
    Given wiremock is initialised
    And the my record wiremocks are initialised

  @NHSO-677
  Scenario: An EMIS user can view allergies and adverse reactions section
    Given I am logged in
    And the GP Practice has enabled demographics functionality
    And I am on my record information page
    Then I see the Allergies and Adverse Reactions heading
    And I see the Allergies and Adverse Reactions section collapsed

  @NHSO-677
  Scenario: An EMIS user does not have Summary Care Record access who is disabled at practice-level or patient level
    Given I am logged in
    And the GP Practice has enabled demographics functionality
    But the GP Practice has disabled allergies functionality
    And I am on my record information page
    Then I see Service not offered by GP or to specific user or access revoked warning message

  @NHSO-677
  Scenario: An EMIS user has no allergies on their record
    Given I am logged in
    And the GP Practice has enabled demographics functionality
    And the GP Practice has enabled allergies functionality and the patient has "0" allergies
    And I am on my record information page
    When I click the Allergies and Adverse Reactions section
    Then I see a message indicating I have no allergies

  @NHSO-677
  Scenario: An EMIS user has one or more allergy records
    Given I am logged in
    And the GP Practice has enabled demographics functionality
    And the GP Practice has enabled allergies functionality and the patient has "2" allergies
    And I am on my record information page
    When I click the Allergies and Adverse Reactions section
    And I see one or more drug type allergies record displayed

  @NHSO-677
  Scenario: An EMIS user has multiple allergies with different date display formats
    Given I am logged in
    And the GP Practice has enabled demographics functionality
    And the GP Practice has enabled allergies functionality and has 5 different allergies with different date formats
    And I am on my record information page
    When I click the Allergies and Adverse Reactions section
    And I see 5 allergies with different date formats