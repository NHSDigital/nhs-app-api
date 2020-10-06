@more
Feature: Display More Menu
  In order to access functionality that is not available on the standard app menu bar
  including Organ Donation and Data Sharing preferences

  @native
  Scenario: A logged in user using the native app can navigate to the more screen
    Given I am a EMIS patient using the native app
    And I am logged in
    And I navigate to More
    And I see the more page header
    Then I see more button on the nav bar is highlighted

  @smoketest
  @onlineconsultations
  Scenario: A logged in user using desktop can navigate to and follow links on the more screen
    Given I am a EMIS patient
    And I am logged in
    And I have access to online consultations but they are switched off by the practice
    And I navigate to More
    Then I am on the More Page
    And I see and can follow links within the more page body
