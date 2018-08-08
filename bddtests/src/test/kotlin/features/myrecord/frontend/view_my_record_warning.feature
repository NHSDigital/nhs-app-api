Feature: View My Medical Record Information - Warning

  @smoketest
  @NHSO-359
  Scenario Outline: A user navigates to my record warning page
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And I am on the record warning page
    Then I see record warning page opened
    And I see header text is My medical record
    And I see your record may contain sensitive information message
    And I see list of sensitive data information
    And I see agree and continue button
    And I see back to home button
    And I see my record button on the nav bar is highlighted

    Examples:
      |Service|
      |EMIS|

  @NHSO-359
  Scenario Outline: A user navigates back to home
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And I am on the record warning page
    When I click the back to home button
    Then I will return to the home page
    And No navigation menu bar item will be selected

    Examples:
      |Service|
      |EMIS|

   @NHSO-1613
   Scenario Outline: A user tries to navigate directly to my record through url
     Given the my record wiremocks are initialised for <Service>
     And the GP Practice has enabled demographics functionality for <Service>
     And I am on the record warning page
     When I enter url address for my record directly into the url
     Then I see record warning page opened
     And I see header text is My medical record
     And I see your record may contain sensitive information message
     And I see list of sensitive data information
     And I see agree and continue button
     And I see back to home button
     And I see my record button on the nav bar is highlighted

     Examples:
       |Service|
       |EMIS|
