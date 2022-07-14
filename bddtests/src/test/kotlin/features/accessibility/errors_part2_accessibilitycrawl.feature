@accessibility
@errors-part2-accessibility
Feature: Errors part2 accessibility

  Scenario: The 'SC30 accept NHS login terms of use to continue' page is captured
    Given I am a user who does not accept the NHS login terms and conditions
    And I am logging in
    Then I see an error informing me to accept NHS login terms and conditions
    And the Errors_Part2_SC30_AcceptNHSLoginTermsOfUseToContinue page is saved to disk

  Scenario: The 'H-GP08a Test results (TPP)' page is captured
    Given I am a TPP user setup to use medical record version 2
    And the GP Practice has 6 test results
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Test results link on my record - Medical Record v2
    Then I see 6 test results - Medical Record v2
    And the Errors_Part2_HGP08a_TestResultsTPP page is saved to disk

  Scenario: The 'M-HI01 Your health service messages' page is captured
    Given I am using the native app user agent
    And I am a user wishing to view my messages
    And I am logged in
    When I navigate to the Messages Hub page
    And I click the App Messages link on the messages hub page
    Then the Message Senders page is displayed
    And the senders are displayed on the Messages Inbox page
    And the Errors_Part2_MHI01_YourHealthServiceMessages page is saved to disk

  Scenario: The 'HI02 Sender list' page is captured
    Given I am using the native app user agent
    And I am a user wishing to view my messages
    And I am logged in
    When I navigate to the Messages Hub page
    And I click the App Messages link on the messages hub page
    Then the Message Senders page is displayed
    And the senders are displayed on the Messages Inbox page
    And the Sender One Canonical sender is displayed as unread
    When I click on Sender One Canonical sender
    Then the Sender Messages page is displayed
    And my messages from the sender are displayed
    And the Errors_Part2_HI02_SenderList page is saved to disk

  Scenario: The 'HI03 message details' page is captured
    Given I am using the native app user agent
    And I am a user wishing to view my messages
    And I am logged in
    When I navigate to the Messages Hub page
    And I click the App Messages link on the messages hub page
    Then the Message Senders page is displayed
    And the senders are displayed on the Messages Inbox page
    And the Sender One Canonical sender is displayed as unread
    When I click on Sender One Canonical sender
    Then the Sender Messages page is displayed
    And my messages from the sender are displayed
    When I click on the unread message on the Sender Messages page
    Then the Message page is displayed
    And the Errors_Part2_HI03_MessageDetails page is saved to disk
