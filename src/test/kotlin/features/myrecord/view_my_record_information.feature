Feature: View My Record Information

  @pending
  @NHSO-361
  Scenario: The one where the user has no access to view record information page
    Given I am on the the record warning page
    And I click the agree and continue button
    Then I see the no access message

  @pending
  @NHSO-361
  Scenario: The one where the patient information page is navigated to
    Given I am logged in
    And I am on the record warning page
    Then I click agree and continue
    And  I see header text is My record
    And I see heading Patient
    And I see patient information details
    And I see my record button on the nav bar is highlighted

  @pending
  @NHSO-361
  Scenario: The one where a user collapses the patient details
    Given I am logged in
    And I am on my record information page
    When I click Patient heading
    Then I do not see patient information details