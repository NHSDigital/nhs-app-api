@my-record
Feature: Warning Frontend - GP Medical Record

  Scenario: A user navigates to my record warning page - GP Medical Record
    Given I am a EMIS user setup to use medical record version 2
    And I am on the record warning page
    Then I see record warning page opened
    And I see header text is Your GP medical record
    And I see your record may contain sensitive information message
    And I see list of sensitive data information
    And I see continue
    And I see back to home
    And I see my record button on the nav bar is highlighted

  Scenario: A user navigates back to home - GP Medical Record
    Given I am a EMIS user setup to use medical record version 2
    And I am on the record warning page
    When I click the back to home
    Then I will return to the home page
    And No navigation menu bar item will be selected

  Scenario: A user tries to navigate directly to my record through url - GP Medical Record
    Given I am a EMIS user setup to use medical record version 2
    And I am on the record warning page
    When I enter url address for my record directly into the url
    Then I see record warning page opened
    And I see header text is Your GP medical record
    And I see your record may contain sensitive information message
    And I see list of sensitive data information
    And I see continue
    And I see back to home
    And I see my record button on the nav bar is highlighted

  Scenario: An EMIS user returns to my record and is taken straight to my record - GP Medical Record
    Given I am a EMIS user setup to use medical record version 2
    When I am on the record warning page
    Then I see the my record warning page
    When I click continue
    Then I am on the medical record v2 page
    When I navigate away from the medical record page
    Then I return to my medical record page
    Then I am on the medical record v2 page

  Scenario: An EMIS user accepts record and then logs out and back in and is shown the warning page - GP Medical Record
    Given I am a EMIS user setup to use medical record version 2
    When I am on the record warning page
    Then I see the my record warning page
    When I click continue
    Then I am on the medical record v2 page
    When I use the header link to log out of the website
    Then I see the login page
    When I am on the record warning page
    Then I see the my record warning page
