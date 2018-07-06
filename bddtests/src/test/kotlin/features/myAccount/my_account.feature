Feature: View My Account Page

  Background:
    Given wiremock is initialised

  @NHSO-1004
  Scenario: A patient can navigate to My account page after visiting a secure page
    Given I am logged in
    When I navigate to appointments
    And I click the head icon button
    Then I see the sign out button
    And none of the menu buttons are highlighted