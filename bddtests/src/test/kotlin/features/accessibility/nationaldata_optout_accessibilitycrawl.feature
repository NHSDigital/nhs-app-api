@accessibility
@national-data-opt-out-accessibility
Feature: National Data Opt-Out accessibility

  Scenario: The data sharing pages are captured
    Given I am using the native app user agent
    And I am a user who wishes to manage their Data Sharing Preferences
    And I am logged in
    And I retrieve the 'Data Sharing' page directly
    Then the Data Sharing 'Overview' page is displayed
    And the NationalDataOptOut_Overview page is saved to disk
    When I click the Next button on the Data Sharing page
    Then the Data Sharing 'How confidential patient information is used' page is displayed
    And the NationalDataOptOut_HowConfidentialDataIsUsed page is saved to disk
    When I click the Next button on the Data Sharing page
    Then the Data Sharing 'When your choice does not apply' page is displayed
    And the NationalDataOptOut_WhenYourChoiceDoesNotApply page is saved to disk
    When I click the Next button on the Data Sharing page
    Then the Data Sharing 'Make your choice' page is displayed
    And the NationalDataOptOut_MakeYourChoice page is saved to disk
