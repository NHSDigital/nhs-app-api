@my-account
Feature: View My Account Page

  Background:
    Given a patient from EMIS is defined
    And I am logged in
    And I click the my account icon

  Scenario: A patient can navigate to My account page
    Given I am on the My Account page
    And none of the menu buttons are highlighted

  Scenario: My details are shown on the My Account page
    Given I am on the My Account page
    Then I see my personal details

  Scenario: The app version is on the My Account Page
    Given I am on the My Account page
    Then I see the current app version

  @nativepending @NHSO-2972
  Scenario: A patient can navigate to the Terms of use page
    Given I am on the My Account page
    When I click the Terms of use link
    Then a new tab opens https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/terms-of-use/

  @nativepending @NHSO-2972
  Scenario: A patient can navigate to the Privacy policy page
    Given I am on the My Account page
    When I click the Privacy policy link
    Then a new tab opens https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy-policy/

  @nativepending @NHSO-2972
  Scenario: A patient can navigate to the Cookies policy page
    Given I am on the My Account page
    When I click the Cookies policy link
    Then a new tab opens https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/cookies-policy/

  @nativepending @NHSO-2972
  Scenario: A patient can navigate to the Open source licences page
    Given I am on the My Account page
    When I click the Open source licences link
    Then a new tab opens https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/open-source-licences/

  @nativepending @NHSO-2972
  Scenario: A patient can navigate to the Help and support page
    Given I am on the My Account page
    When I click the Help and support link
    Then a new tab opens https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/help-and-support/

  @nativepending @NHSO-2972
  Scenario: A patient can navigate to the Accessibility statement page
    Given I am on the My Account page
    When I click the Accessibility statement link
    Then a new tab opens https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/accessibility-statement/
