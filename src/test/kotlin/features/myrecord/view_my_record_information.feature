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