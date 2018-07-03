Feature: View My Medical Record Warning

  @smoketest
  @NHSO-359
  Scenario Outline: An EMIS user navigates to my record warning page
    Given the my record wiremocks are initialised for <Service>
    And I am logged in
    And the GP Practice has enabled demographics functionality for <Service>
    When I click my record button on menu bar
    Then I see record warning page opened
    And I see header text is My medical record
    And I see your record may contain sensitive information message
    And I see sensitive information message highlighted yellow
    And I see list of sensitive data information
    And I see agree and continue button
    And I see back to home button
    And I see my record button on the nav bar is highlighted

    Examples:
      |Service|
      |EMIS|

  @NHSO-359
  Scenario Outline: An EMIS user navigates back to home
    Given the my record wiremocks are initialised for <Service>
    And I am logged in
    And the GP Practice has enabled demographics functionality for <Service>
    And I am on the record warning page
    When I click the back to home button
    Then I will return to the home page
    And No navigation menu bar item will be selected

    Examples:
      |Service|
      |EMIS|