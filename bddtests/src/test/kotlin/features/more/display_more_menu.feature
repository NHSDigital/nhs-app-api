@more
Feature: Display More Menu
  In order to access functionality that is not available on the standard app menu bar
  including Organ Donation and Data Sharing preferences

  @smoketest
  @nativepending @NHSO-2972
  Scenario: A logged in user can navigate to the more screen
    Given a patient from EMIS is defined
    And I am logged in
    And I navigate to More
    And I see the more page header
    Then I see more button on the nav bar is highlighted
    And I see and can follow links within the more page body

  Scenario: A user can navigate to the 'Choose how the NHS uses your data' page
    Given a patient from EMIS is defined
    And I am logged in
    And I navigate to more
    Given I am on the More Page
    When I choose to set my data sharing preferences
    Then I am on the Data Sharing page
