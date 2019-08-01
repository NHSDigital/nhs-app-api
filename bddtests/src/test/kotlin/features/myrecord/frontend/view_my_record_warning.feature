@my-record
Feature: View My Medical Record Information - Warning Frontend

  Scenario: A user navigates to my record warning page
    Given the my record wiremocks are initialised for EMIS
    And the GP Practice has enabled demographics functionality
    And I am on the record warning page
    Then I see record warning page opened
    And I see header text is My medical record
    And I see your record may contain sensitive information message
    And I see list of sensitive data information
    And I see continue
    And I see back to home
    And I see my record button on the nav bar is highlighted

  Scenario: A user navigates back to home
    Given the my record wiremocks are initialised for EMIS
    And the GP Practice has enabled demographics functionality
    And I am on the record warning page
    When I click the back to home
    Then I will return to the home page
    And No navigation menu bar item will be selected

  Scenario: A user tries to navigate directly to my record through url
    Given the my record wiremocks are initialised for EMIS
    And the GP Practice has enabled demographics functionality
    And I am on the record warning page
    When I enter url address for my record directly into the url
    Then I see record warning page opened
    And I see header text is My medical record
    And I see your record may contain sensitive information message
    And I see list of sensitive data information
    And I see continue
    And I see back to home
    And I see my record button on the nav bar is highlighted

  Scenario: An EMIS user returns to my record and sees the top of the my record page
    Given the my record wiremocks are initialised for EMIS
    And the GP Practice has enabled demographics functionality
    When I am on the record warning page
    Then I see the my record warning page
    When I click continue
    Then I see the my medical record page
    When I navigate away from the medical record page
    Then I return to my medical record page
    Then I see the my record warning page
    When I click continue
    Then I see the my medical record page
    And I see the top of my medical record page
