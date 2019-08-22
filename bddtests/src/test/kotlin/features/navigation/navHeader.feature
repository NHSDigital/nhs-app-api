@navigation-bar
Feature: Use the navigation header bar

  Background:
    Given I am a EMIS patient
    And I am logged in

  @pending
  Scenario: A patient can access the help and support page by clicking the help icon
    Given I see the header
    When I click the help icon
    Then a new tab has been opened by the link

  @nativesmoketest
  Scenario: A patient can access the my account page by clicking the my account icon
    Given I see the header
    When I click the my account icon
    Then the Account page is displayed

  @nativesmoketest
  Scenario: A patient can access the home page by clicking the home icon
    Given I see the header
    And I navigate away from the home page
    When I click the home icon
    Then I see the home page
