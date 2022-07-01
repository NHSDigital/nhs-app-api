Feature: User Research

  Scenario: A newly registered patient after failing to make a choice can accept to be part of user research
    Given I am a EMIS patient who has not already accepted terms and conditions
    And I am logged in expecting to see T&Cs
    Then the Terms and Conditions page is displayed
    When I check the agree to terms and conditions checkbox
    And I click the 'Continue' button
    Then the User Research page is displayed
    When I click the 'Continue' button
    Then I see 'There is a problem' form error summary heading
    And I see 'Select yes or no' form error summary reason item
    And I see 'Select yes or no' inline error
    When I click the 'Yes' radio button
    And I click the 'Continue' button
    Then I see the home page

  Scenario: A newly registered patient can reject to be part of user research
    Given I am a EMIS patient who has not already accepted terms and conditions
    And I am logged in expecting to see T&Cs
    Then the Terms and Conditions page is displayed
    When I check the agree to terms and conditions checkbox
    And I click the 'Continue' button
    Then the User Research page is displayed
    When I click the 'No' radio button
    And I click the 'Continue' button
    Then I see the home page

  Scenario: A newly registered patient can accept to be part of user research
    Given I am a EMIS patient who has not already accepted terms and conditions
    And I am logged in expecting to see T&Cs
    Then the Terms and Conditions page is displayed
    When I check the agree to terms and conditions checkbox
    And I click the 'Continue' button
    Then the User Research page is displayed
    When I click the 'Yes' radio button
    And I click the 'Continue' button
    Then I see the home page

  Scenario: A newly registered patient can navigate to the User Research Privacy policy page
    Given I am a EMIS patient who has not already accepted terms and conditions
    And 'NHS UK' responds to requests for '/privacy'
    And I am logged in expecting to see T&Cs
    Then the Terms and Conditions page is displayed
    When I check the agree to terms and conditions checkbox
    And I click the 'Continue' button
    Then the User Research page is displayed
    When I click the link called 'Read our privacy policy' with a url of 'http://stubs.local.bitraft.io:8080/external/nhsuk/privacy'
    Then a new tab has been opened by the link
