@data-sharing
@more
@nativesmoketest
Feature: Data Sharing Frontend
  A user can access Data Sharing

  Scenario: A user can navigate through the Data Sharing Preferences pages via the Next/Previous buttons
    Given I am using the native app user agent
    And I am a user who wishes to manage their Data Sharing Preferences
    And I am logged in
    When I navigate to the health record hub page
    Then I see the health records hub page
    When I click the Data Sharing link on the health hub page
    Then the Data Sharing 'Overview' page is displayed
    And the content on the Data Sharing 'Overview' page is correct
    When I click the Next button on the Data Sharing page
    Then the content on the Data Sharing 'How confidential patient information is used' page is correct
    When I click the Next button on the Data Sharing page
    Then the content on the Data Sharing 'When your choice does not apply' page is correct
    When I click the Next button on the Data Sharing page
    Then the content on the Data Sharing 'Make your choice' page is correct
    When I click the Previous button on the Data Sharing page
    Then the Data Sharing 'When your choice does not apply' page is displayed
    When I click the Previous button on the Data Sharing page
    Then the Data Sharing 'How confidential patient information is used' page is displayed
    When I click the Previous button on the Data Sharing page
    Then the Data Sharing 'Overview' page is displayed

  Scenario: A user can navigate through the Data Sharing Preferences pages via the Contents links
    Given I am using the native app user agent
    And I am a user who wishes to manage their Data Sharing Preferences
    And I am logged in
    And I retrieve the 'Data Sharing' page directly
    Then the Data Sharing 'Overview' page is displayed
    When I click the 'Make your choice' contents link on the Data Sharing page
    Then the Data Sharing 'Make your choice' page is displayed
    When I click the 'How confidential patient information is used' contents link on the Data Sharing page
    Then the Data Sharing 'How confidential patient information is used' page is displayed
    When I click the 'Overview' contents link on the Data Sharing page
    Then the Data Sharing 'Overview' page is displayed
    When I click the 'When your choice does not apply' contents link on the Data Sharing page
    Then the Data Sharing 'When your choice does not apply' page is displayed

  Scenario: A user can navigate to the NHS website to find out more information on Data Sharing
    Given I am using the native app user agent
    And I am a user who wishes to manage their Data Sharing Preferences
    And I am logged in
    And I retrieve the 'Data Sharing' page directly
    Then the Data Sharing 'Overview' page is displayed
    When I click the link called 'Visit the NHS website' with a url of 'https://www.nhs.uk/your-nhs-data-matters/'
    Then a new tab has been opened by the link

  Scenario: A user can navigate to the NHS website to find out more information on Managing their Choice
    Given I am using the native app user agent
    And I am a user who wishes to manage their Data Sharing Preferences
    And I am logged in
    And I retrieve the 'Data Sharing Make Your Choice' page directly
    Then the Data Sharing 'Make your choice' page is displayed
    When I click the link called 'NHS website' with a url of 'https://www.nhs.uk/your-nhs-data-matters/manage-your-choice/other-ways-to-manage-your-choice/'
    Then a new tab has been opened by the link

  Scenario: A user chooses to manage their Data Sharing preferences
    Given I am using the native app user agent
    And I am a user who wishes to manage their Data Sharing Preferences
    And I am logged in
    And I retrieve the 'Data Sharing' page directly
    Then the Data Sharing 'Overview' page is displayed
    When I click the 'Make your choice' contents link on the Data Sharing page
    And the Data Sharing 'Make your choice' page is displayed
    And I click the Start Now button on the Data Sharing page
    Then the NDOP website is displayed

  Scenario: A desktop user is directed to the NHS website to find out more information on Data Sharing
    Given I am a user who wishes to manage their Data Sharing Preferences
    And I am logged in
    When I browse to the pages at the following urls I see the home page
      | /data-sharing                   |
      | /data-sharing/where-used        |
      | /data-sharing/does-not-apply    |
      | /data-sharing/make-your-choice  |
    And I retrieve the 'health record hub' page directly
    And I click the link called 'Choose if data from your health records is shared for research and planning' with a url of 'https://www.nhs.uk/your-nhs-data-matters/'
    Then a new tab has been opened by the link
