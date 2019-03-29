@navigation-bar
Feature: Use the navigation header bar

  Background:
    Given I am a EMIS patient
    And I am logged in

  @pending
  Scenario: A patient can access the help and support page by clicking the help icon
    Given I see the header
    When I click the help icon
    Then a new tab opens https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/help-and-support/

  @native-smoketest
  Scenario: A patient can access the my account page by clicking the my account icon
    Given I see the header
    When I click the my account icon
    Then I am on the My Account page

  @native-smoketest
  Scenario: A patient can access the home page by clicking the nhs logo
    Given I see the header
    And I navigate away from the home page
    When I click the nhs logo
    And I wait for 2 seconds
    Then I see the home page
