@more
Feature: View More Page

  @nativesmoketest
  Scenario: A patient can navigate to More page
    Given I am a EMIS patient
    And I am logged in
    When I click the more icon
    Then the More page is displayed
    And none of the menu buttons are highlighted

  @nativesmoketest
  Scenario: The app version is on the More Page
    Given I am a EMIS patient
    And I am logged in
    When I click the more icon
    Then the More page is displayed
    And I see the current app version

  Scenario Outline: A patient can see the linked accounts and cookies link and the nhs login link
    Given I am a <Gp System> patient
    And I am logged in
    When I click the more icon
    Then the More page is displayed
    And the Linked Profiles link is displayed
    And the Cookies link is displayed
    And the NHS login link is displayed
    Examples:
      | Gp System |
      | EMIS      |
      | TPP       |

  Scenario: A patient can navigate to the nhs login more page to update their account details
    Given I am a EMIS patient
    And I am logged in
    When I click the more icon
    Then the More page is displayed
    And the NHS login link is displayed
    When I click the NHS login link on the more page
    Then the nhs login account settings page has opened in a new tab

  Scenario Outline: A patient can navigate to the <Link> page
    Given I am a EMIS patient
    And I am logged in
    When I click the more icon
    Then the More page is displayed
    When I click the link called <Link> on the More page
    Then a new tab has been opened by the link
    Examples:
    | Link                    |
    | Terms of use            |
    | Privacy policy          |
    | Open source licences    |
    | Help and support        |
    | Accessibility statement |

  Scenario Outline: A patient can navigate to More page and can not see the linked account link
    Given I am logged in as a <Gp System> user
    When I click the more icon
    Then the More page is displayed
    And the Linked Profiles link is not displayed
    And the Cookies link is displayed
    Examples:
      | Gp System |
      | VISION    |
      | MICROTEST |

  Scenario Outline: A patient can navigate to the <Biometric Type> page on their native device
    Given I am a EMIS patient using the native app
    And I am logged in
    When I retrieve the 'more' page directly
    Then the More page for mobile devices is displayed
    And the <Biometric Type> more link is displayed
    And I click the <Biometric Type> link on the more page
    And I see the <Biometric Type> login settings page
    Examples:
      | Biometric Type|
      | Login options |
      | Face ID       |
      | Touch ID      |
      | Fingerprint   |

    Scenario: A patient with proof level 5 navigates to More page
      Given I am a patient with proof level 5
      And I am logged in
      When I click the more icon
      Then the More page is displayed
      And the Linked Profiles link is not displayed

