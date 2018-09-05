@my-account
Feature: View My Account Page

  Background:
    Given a patient from EMIS is defined
    And I am logged in
    And I click the my account icon

  @NHSO-1004
  Scenario: A patient can navigate to My account page
    Given I am on the My Account page
    And none of the menu buttons are highlighted

  @NHSO-1917
  Scenario: A patient can navigate to the terms and conditions page
    Given I am on the My Account page
    When I click the Terms and conditions link
    Then a new tab opens https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/terms-of-use/

  @NHSO-1917
  Scenario: A patient can navigate to the Privacy policy page
    Given I am on the My Account page
    When I click the Privacy policy link
    Then a new tab opens https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy-policy/

  @NHSO-1917
  Scenario: A patient can navigate to the Cookies policy page
    Given I am on the My Account page
    When I click the Cookies policy link
    Then a new tab opens https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/cookies-policy/

  @NHSO-1917
  Scenario: A patient can navigate to the Open source licenses page
    Given I am on the My Account page
    When I click the Open source licenses link
    Then a new tab opens https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/open-source-licences/

  @NHSO-1917
  Scenario: A patient can navigate to the Help and support page
    Given I am on the My Account page
    And I scroll to the bottom of the page
    When I click the Help and support link
    Then a new tab opens https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/help-and-support/
