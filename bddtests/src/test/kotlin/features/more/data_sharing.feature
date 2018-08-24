Feature: Data Sharing

  A user can access Data Sharing

  Background:
    Given a patient from EMIS is defined
    And I am logged in
    And I navigate to more

  @pending
  @NHSO-2203
  Scenario: User clicks on the data sharing feature
    Given I am on the More Page
    And Ndop wiremock is set up
    When I choose to set my data sharing preferences
    And I am on the data sharing final page
    And I click to complete data sharing
    Then a new web page opens for Ndop
