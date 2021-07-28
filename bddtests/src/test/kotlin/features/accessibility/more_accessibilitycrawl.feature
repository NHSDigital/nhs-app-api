@accessibility
@notifications-accessibility
  Feature: More section accessibility

    Scenario: The more page is captured
      Given I am a EMIS patient
      When I am logged in
      And I click the more icon
      Then the more page is saved to disk

    Scenario: The account and settings page is captured
      Given I am an EMIS patient
      And I am logged in
      When I click the more icon
      And I click the Account and settings link on the More page
      Then the Account and settings Hub page is displayed
      And the AccountAndSettings page is saved to disk

    Scenario: The manage notifications page is captured
      Given I am using the native app user agent
      And I am a user wishing to enable push notifications
      And I am logged in
      When I navigate to the More page
      And I click the Account and settings link on the More page
      And I click the Manage notifications link on the account and settings page
      Then the Notifications Settings page is displayed
      And I change the notifications toggle to on
      And the notifications toggle is displayed as on
      And the ManageNotifications page is saved to disk

    Scenario Outline: The account and settings biometric verification links are captured
      Given I am a EMIS patient using the native app
      And I am logged in
      When I navigate to the More page
      And I click the Account and settings link on the More page
      Then the Account and settings Hub page is displayed
      And the <Biometric Type> account and settings link is displayed
      And I click the <Biometric Type> link on the account and settings page
      And I see the account and settings <Biometric Type> biometric page
      And the LoginSettings page is saved to disk
      Examples:
        | Biometric Type |
        | Login options  |
        | Face ID        |
        | Touch ID       |
        | Fingerprint    |

    Scenario: The legal and cookies page is captured
      Given I am a EMIS patient
      And I am logged in
      When I click the more icon
      And I click the Account and settings link on the More page
      And I click the Legal and cookies link on the account and settings page
      Then the Legal and cookies Hub page is displayed
      And the LegalAndCookies page is saved to disk

    Scenario: The manage cookies page is captured
      Given I am a EMIS patient
      And I am logged in
      When I click the more icon
      And I click the Account and settings link on the More page
      And I click the Legal and cookies link on the account and settings page
      And I click Manage Cookies
      Then the Legal and cookies Manage cookies page is displayed
      And the LegalAndCookiesMangeCookies page is saved to disk
