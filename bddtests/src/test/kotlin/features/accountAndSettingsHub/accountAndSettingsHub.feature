@accountAndSettingsHub
Feature: Account and Settings Hub

  Scenario: A patient can navigate to the Account and settings page
    Given I am a EMIS patient
    And I am logged in
    When I navigate to the Account and settings page
    Then the Account and settings Hub page is displayed

  Scenario: A patient can navigate to the Legal and cookies page
    Given I am an EMIS patient
    And I am logged in
    When I navigate to the Account and settings page
    Then the Account and settings Hub page is displayed
    And the Legal and cookies link is displayed
    When I click the Legal and cookies link on the account and settings page
    Then the Legal and cookies Hub page is displayed

  Scenario: A patient can navigate to the manage nhs login page
    Given I am a EMIS patient
    And I am logged in
    When I navigate to the Account and settings page
    Then the Account and settings Hub page is displayed
    And the Manage NHS account link is displayed
    When I click the Manage NHS account link on the account and settings page
    Then the nhs account settings page has opened in a new tab

  Scenario: A patient can navigate to the manage notifications page on their native device
    Given I am a EMIS patient using the native app
    And I am logged in
    When I navigate to the Account and settings page
    Then the Account and settings Hub page is displayed
    And the Manage notifications link is displayed
    When I click the Manage notifications link on the account and settings page
    Then the Notifications Settings page is displayed

  Scenario Outline: A patient can navigate to the <Biometric Type> page on their native device
    Given I am a EMIS patient using the native app
    And I am logged in
    When I navigate to the Account and settings page
    Then the Account and settings Hub page is displayed
    And the <Biometric Type> account and settings link is displayed
    And I click the <Biometric Type> link on the account and settings page
    And I see the account and settings <Biometric Type> biometric page
    Examples:
      | Biometric Type|
      | Login options |
      | Face ID       |
      | Touch ID      |
      | Fingerprint   |

  Scenario Outline: A patient can see the cookies link and the manage nhs login link
    Given I am a <Gp System> patient
    And I am logged in
    When I click the more icon
    Then the More page is displayed
    And I click the Account and settings link on the More page
    And the Legal and cookies link is displayed
    And the Manage NHS account link is displayed
    Examples:
      | Gp System |
      | EMIS      |
      | TPP       |
