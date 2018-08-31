@my-account
Feature: View My Account Page

  Background:
    Given a patient from EMIS is defined
    And I am logged in
    And I go to the my account page

  @NHSO-1004
  Scenario: A patient can navigate to My account page
    Given I am on the My Account page
    And none of the menu buttons are highlighted

  @NHSO-1917
  Scenario: A patient can navigate to the terms and conditions page
    Given I am on the My Account page
    And I click the Terms and conditions link
    Then a new tab opens https://www.nhs.uk/

  @NHSO-1917
  Scenario: A patient can navigate to the Privacy policy page
    Given I am on the My Account page
    And I click the Privacy policy link
    Then a new tab opens https://www.nhs.uk/

  @NHSO-1917
  Scenario: A patient can navigate to the Cookies policy page
    Given I am on the My Account page
    And I click the Cookies policy link
    Then a new tab opens https://www.nhs.uk/

  @NHSO-1917
  Scenario: A patient can navigate to the Open source licenses page
    Given I am on the My Account page
    And I click the Open source licenses link
    Then a new tab opens https://www.nhs.uk/

  @NHSO-1917
  @bug @NHSO-2266
  Scenario: A patient can navigate to the Help and support page
    Given I am on the My Account page
    And I click the Help and support link
    Then a new tab opens https://www.nhs.uk/
