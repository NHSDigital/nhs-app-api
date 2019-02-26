@my-account
Feature: View My Account Page

  Background:
    Given I am a EMIS patient
    And I am logged in
    And I click the my account icon

  @nativesmoketest
  Scenario: A patient can navigate to My account page
    Given I am on the My Account page
    And none of the menu buttons are highlighted

  @nativesmoketest
  Scenario: My details are shown on the My Account page
    Given I am on the My Account page
    Then I see my personal details

  @nativesmoketest
  Scenario: The app version is on the My Account Page
    Given I am on the My Account page
    Then I see the current app version

  Scenario: A patient can navigate to the Terms of use page
    Given I am on the My Account page
    When I click the link called 'Terms of use' with a url of 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/terms-of-use/'
    Then a new tab has been opened by the link

  Scenario: A patient can navigate to the Privacy policy page
    Given I am on the My Account page
    When I click the link called 'Privacy policy' with a url of 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy-policy/'
    Then a new tab has been opened by the link

  Scenario: A patient can navigate to the Cookies policy page
    Given I am on the My Account page
    When I click the link called 'Cookies policy' with a url of 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/cookies-policy/'
    Then a new tab has been opened by the link

  Scenario: A patient can navigate to the Open source licences page
    Given I am on the My Account page
    When I click the link called 'Open source licences' with a url of 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/open-source-licences/'
    Then a new tab has been opened by the link

  Scenario: A patient can navigate to the Help and support page
    Given I am on the My Account page
    When I click the link called 'Help and support' with a url of 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/help-and-support/'
    Then a new tab has been opened by the link

  Scenario: A patient can navigate to the Accessibility statement page
    Given I am on the My Account page
    When I click the link called 'Accessibility statement' with a url of 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/accessibility-statement/'
    Then a new tab has been opened by the link
