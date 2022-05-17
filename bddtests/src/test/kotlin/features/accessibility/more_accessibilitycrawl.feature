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

    Scenario: 'MO More hub' page is captured
      Given I am a user who can view Join a patient participation group from Substrakt
      When I am logged in
      And I click the more icon
      And the More page is displayed
      Then the More_MOHub page is saved to disk

    Scenario: 'M001: More proxy shutter' page is captured
      Given I am logged in as a EMIS user with linked profiles and appointments provider IM1
      Then I see the home page
      When I can see and follow the Linked profiles link
      Then the linked profiles page is displayed
      And linked profiles are displayed
      And I select a linked profile with appointments enabled false, prescriptions enabled false and medical record enabled false
      And details for the selected linked profile are displayed
      When I click the Switch to this profile button for the proxy user
      Then I see the proxy home page
      When I click the more icon
      Then the more shutter page is displayed
      And the More_ProxyShutter page is saved to disk

    Scenario: 'M-LP: Linked profiles' page is captured
      Given I am a EMIS patient
      And I am logged in
      When I can see and follow the Linked profiles link
      Then the linked profiles page is displayed
      And the More_LPLinkedProfile page is saved to disk

    Scenario: 'MoreP16: EMIS Switch Profile' page is captured
      Given I am logged in as a EMIS user with linked profiles and appointments provider IM1
      Then I see the home page
      When I can see and follow the Linked profiles link
      Then the linked profiles page is displayed
      And linked profiles are displayed
      And I select a linked profile with appointments enabled false, prescriptions enabled false and medical record enabled false
      And  details for the selected linked profile are displayed
      And the More_LP16EMISSwitchProfile page is saved to disk

    Scenario: 'MoreP16: TPP Switch Profile' page is captured
      Given I am logged in as a TPP user with linked profiles and appointments provider IM1
      Then I see the home page
      When I can see and follow the Linked profiles link
      Then the linked profiles page is displayed
      And linked profiles are displayed
      When I select a linked profile
      Then details for the selected linked profile are displayed
      And the More_LP16TPPSwitchProfile page is saved to disk

    Scenario: 'M-LP17: Switch to my profile' page is captured
      Given I am logged in as a EMIS user with linked profiles and appointments provider IM1
      When I can see and follow the Linked profiles link
      And I select a linked profile
      And I click the Switch to this profile button for the proxy user
      And I see the proxy patient details of age and gp surgery
      And I click the proxy warning
      Then the switch profiles page is displayed
      And the correct proxy user details are displayed
      And the More_LP17SwitchmyProfile page is saved to disk

    Scenario: 'MoreP20: How to add a Linked Profile' page is captured
      Given I am logged in as a EMIS user with no linked profiles
      And I see the home page
      When I can see and follow the Linked profiles link
      Then I see information on how to setup a linked profile
      And the More_LP20AddLinkedProfile page is saved to disk

    Scenario: 'M-LP23: Linked Profiles Temporarily unavailable' page is captured
      Given I am an EMIS patient with linked profiles whose GP system is unavailable
      And I am logged in
      When I navigate to linked profiles from the home page via more
      And I see appropriate linked profiles try again error message when there is no GP session
      And the More_LP23LinkedProfile page is saved to disk

    Scenario: 'M-LP19: Proxy shutter linked profiles unavailable' page is captured
      Given I am an EMIS patient with linked profiles whose GP system is unavailable
      And I am logged in
      When I navigate to linked profiles from the home page via more
      And I see appropriate linked profiles try again error message when there is no GP session
      And I click the 'Try again' button
      Then I see what I can do next with a linked accounts error message and reference code '3e'
      And the More_LP19ShutterLinkedProfile page is saved to disk

