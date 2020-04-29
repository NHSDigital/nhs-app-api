@accessibility
  Feature: Data Sharing accessibility

    Scenario: The data sharing pages are captured
      Given I am using the native app user agent
      And I am a user who wishes to manage their Data Sharing Preferences
      And I have the instructions cookie
      And I am logged in
      And I retrieve the 'Data Sharing' page directly
      Then the Data Sharing 'Overview' page is displayed
      And the DataSharing_Overview page is saved to disk
      When I click the Next button on the Data Sharing page
      Then the Data Sharing 'How confidential patient information is used' page is displayed
      And the DataSharing_ConfidentialData page is saved to disk
      When I click the Next button on the Data Sharing page
      Then the Data Sharing 'When your choice does not apply' page is displayed
      And the DataSharing_ChoiceApply page is saved to disk
      When I click the Next button on the Data Sharing page
      Then the Data Sharing 'Make your choice' page is displayed
      And the DataSharing_MakeChoice page is saved to disk
