@my-record
Feature: Warning Frontend - Medical Record v2

  Scenario: A user navigates to my record warning page - Medical Record v2
    Given I am a EMIS user setup to use medical record version 2
    And I am on the Medical Record Warning page
    Then I see the Medical Record Warning page
    And I see header text is Your GP medical record
    And I see your record may contain sensitive information message
    And I see list of sensitive data information
    And I see continue
    And I see back to home
    And I see my record button on the nav bar is highlighted

  Scenario: A user navigates back to home - Medical Record v2
    Given I am a EMIS user setup to use medical record version 2
    And I am on the Medical Record Warning page
    When I click back to home
    Then I see the home page
    And No navigation menu bar item will be selected

  Scenario: A user tries to navigate directly to my record through url - Medical Record v2
    Given I am a EMIS user setup to use medical record version 2
    And I am on the Medical Record Warning page
    When I enter url address for my record directly into the url
    Then I see the Medical Record Warning page
    And I see header text is Your GP medical record
    And I see your record may contain sensitive information message
    And I see list of sensitive data information
    And I see continue
    And I see back to home
    And I see my record button on the nav bar is highlighted

  Scenario: An EMIS user returns to my record and is taken straight to my record - Medical Record v2
    Given I am a EMIS user setup to use medical record version 2
    When I am on the Medical Record Warning page
    Then I see the Medical Record Warning page
    When I click continue
    Then I see the medical record v2 page
    When I navigate away from the medical record page
    Then I return to my medical record page
    Then I see the medical record v2 page

  Scenario: An EMIS user accepts record and then logs out and back in and is shown the warning page - Medical Record v2
    Given I am a EMIS user setup to use medical record version 2
    When I am on the Medical Record Warning page
    Then I see the Medical Record Warning page
    When I click continue
    Then I see the medical record v2 page
    When I use the header link to log out of the website
    Then I see the login page
    When I am on the Medical Record Warning page
    Then I see the Medical Record Warning page
