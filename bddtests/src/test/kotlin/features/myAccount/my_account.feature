@my-account
Feature: View My Account Page

  @nativesmoketest
  Scenario: A patient can navigate to Settings page
    Given I am a EMIS patient
    And I am logged in
    And I click the settings icon
    Then the Account page is displayed
    Then none of the menu buttons are highlighted

  @nativesmoketest
  Scenario: The app version is on the My Account Page
    Given I am a EMIS patient
    And I am logged in
    And I click the settings icon
    Then the Account page is displayed
    Then I see the current app version

  Scenario: A patient can see the linked accounts and cookies link
    Given I am a EMIS patient
    And I am logged in
    And I click the settings icon
    Then the Account page is displayed
    Then the Linked Profiles link is displayed
    And the Cookies link is displayed

  Scenario: A patient can navigate to the Terms of use page
    Given I am a EMIS patient
    And I am logged in
    And I click the settings icon
    Then the Account page is displayed
    When I click the link called 'Terms of use' with a url of 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/terms/'
    Then a new tab has been opened by the link

  Scenario: A patient can navigate to the Privacy policy page
    Given I am a EMIS patient
    And I am logged in
    And I click the settings icon
    Then the Account page is displayed
    When I click the link called 'Privacy policy' with a url of 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/'
    Then a new tab has been opened by the link

  Scenario: A patient can navigate to the Open source licences page
    Given I am a EMIS patient
    And I am logged in
    And I click the settings icon
    Then the Account page is displayed
    When I click the link called 'Open source licences' with a url of 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/open-source/'
    Then a new tab has been opened by the link

  Scenario: A patient can navigate to the Help and support page
    Given I am a EMIS patient
    And I am logged in
    And I click the settings icon
    Then the Account page is displayed
    When I click the link called 'Help and support' with a url of 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/help/'
    Then a new tab has been opened by the link

  Scenario: A patient can navigate to the Accessibility statement page
    Given I am a EMIS patient
    And I am logged in
    And I click the settings icon
    Then the Account page is displayed
    When I click the link called 'Accessibility statement' with a url of 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/accessibility/'
    Then a new tab has been opened by the link

  Scenario: A patient can navigate to Settings page and can not see the linked account link
    Given I am logged in as a EMIS user with no linked profiles
    And I click the settings icon
    Then the Account page is displayed
    Then the Linked Profiles link is not displayed
    And the Cookies link is displayed