Feature: View My Medical Record Information

  Background:
    Given wiremock is initialised
    And the my record wiremocks are initialised

  @smoketest
  @NHSO-685
  Scenario: An EMIS user has immunisations on their record
    Given I am logged in
    And the GP Practice has enabled demographics functionality
    And the GP Practice has enabled immunisations functionality and multiple immunisation records exist
    And I am on my record information page
    And I see heading Immunisations
    When I click the Immunisations section
    Then I see immunisation records displayed

  @NHSO-685
  Scenario: An EMIS user has no immunisations on their record
    Given I am logged in
    And the GP Practice has enabled demographics functionality
    And no immunisation records exist for the patient
    And I am on my record information page
    And I see heading Immunisations
    When I click the Immunisations section
    Then I see message No information recorded for this section

  @NHSO-685
  Scenario: An EMIS user does not have access to immunisations
    Given I am logged in
    And the GP Practice has enabled demographics functionality
    And the user does not have access to view immunisations
    And I am on my record information page
    And I see heading Immunisations
    When I click the Immunisations section
    Then I see message You do not have access to this section

  @NHSO-685
  Scenario: An Error occurs retrieving immunisations data
    Given I am logged in
    And the GP Practice has enabled demographics functionality
    And there is an error retrieving immunisations data
    And I am on my record information page
    And I see heading Immunisations
    When I click the Immunisations section
    Then I see message An error has occurred trying to retrieve this data
