Feature: View My Record Warning

  @pending
  @NHSO-359
  Scenario: The one where my record warning page is navigated to
    Given I am logged in
    When I click my record button on menu bar
    And I navigate to my record via the url
    Then I see record warning page opened
    And  I see header text is My record
    And  I see Your record may contain sensitive information message
    And I see sensitive information message highlighted yellow
    And I see list of sensitive data information
    And I see agree and continue button
    And I see back to home button
    And I see my record button on the nav bar is highlighted

  @pending
  @NHSO-359
  Scenario: The one where a user navigates to the patient record information page
    Given I am on the record warning page
    When I click agree and continue
    And I have access to view my information
    Then the my record information screen is loaded

  @pending
  @NHSO-359
  Scenario: The one where the user navigates back to home
    Given I am on the record warning page
    When I click the back to home button
    Then I will return to the home page
    And No navigation menu bar item will be selected
