@accessibility
@home-page-accessibility
Feature: Home page accessibility

  Scenario: The 'Home - prove your identity component' page is captured
    Given I am a patient with proof level 5
    And I am logged in
    Then I am asked to prove my identity
    And the Home_ProveYourIdentityComponent page is saved to disk

  Scenario: The 'Home, logged in for Native App' page is captured
    Given I am using the native app user agent
    And I am a user wishing to enable push notifications
    When I am logged in
    Then the Home_LoggedIn_NativeApp page is saved to disk

  Scenario: The 'Home, logged in for Browser & acting on behalf of a linked profile' pages are captured
    Given I am a EMIS user with linked profiles
    And I am logged in
    And the Home_LoggedIn_Browser page is saved to disk
    When I have switched to a linked profile
    Then I see the proxy home page
    And I see the yellow banner
    And the yellow banner contains details for the user I am acting on behalf of
    And the Home_ActingOnBehalfOfLinkedProfile page is saved to disk
