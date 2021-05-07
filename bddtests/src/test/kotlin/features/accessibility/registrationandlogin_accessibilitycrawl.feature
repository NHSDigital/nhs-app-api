@accessibility
@registration-and-login-accessibility
Feature: Registration and login accessibility

  Scenario: The 'Login Browser version' page is captured
    Given I am a EMIS patient who has not already accepted terms and conditions
    And I am not logged in
    Then the Login_BroswerVersion page is saved to disk

  Scenario: The 'Login Native version' page is captured
    Given I am using the native app user agent
    And I am not logged in
    Then the Login_NativeVersion page is saved to disk

  Scenario: The 'Before you start' page is captured
    Given I am a patient using the native app
    When I am on the login logged-out page for the first time
    And I click the 'Continue with NHS login' button
    Then the page title is 'Before you start'
    And the Login_BeforeYouStart page is saved to disk

  Scenario: The 'Accept conditions of use' page is captured
    Given I am logged into Citizen ID but EMIS session cannot be established
    Then the Terms and Conditions page is displayed
    And the AcceptConditionsOfUse page is saved to disk

  Scenario: The 'Updated conditions of use' page is captured
    Given I am a EMIS patient who has accepted terms and conditions but updated terms and conditions exist
    And I am logged in expecting to see T&Cs
    Then the updated Terms and Conditions page is displayed
    And the UpdatedConditionsOfUse page is saved to disk

  Scenario: The 'User research panel sign up' page is captured
    Given I am a EMIS patient who has not already accepted terms and conditions
    And I am logged in expecting to see T&Cs
    Then the Terms and Conditions page is displayed
    When I check the agree to terms and conditions checkbox
    And I click the 'Continue' button
    Then the User Research page is displayed
    And the UserResearchPanelSignUp page is saved to disk

  Scenario: The 'Desktop cookie banner' page is captured
    Given I have enabled javascript
    When I am on the login logged-out page
    Then I see the cookie banner
    And the DesktopCookieBanner page is saved to disk

  Scenario: The 'Manage Notifications Prompt' page is captured
    Given I am using the native app user agent
    And I am a user wishing to enable push notifications for the first time, with my initial state undetermined
    And I am logged in
    When I navigate to the More page
    Then the More Settings links are available
    And I click the Notifications link on the More page
    And the Notifications Settings page is displayed
    And the ManageNotificationsPrompt page is saved to disk
