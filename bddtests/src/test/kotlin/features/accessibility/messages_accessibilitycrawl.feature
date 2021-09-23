@accessibility
@messages-accessibility
Feature: Messages accessibility

  Scenario: The 'Messages hub' page is captured
    Given I am a patient using the EMIS GP System
    And I am logged in
    And I navigate to Messages
    Then the Messages Hub page is displayed
    And the Messages_Hub page is saved to disk

  Scenario: The 'Messages proxy shutter' page is captured
    Given I am logged in as a EMIS user with linked profiles and appointments provider IM1
    Then I see the home page
    When I can see and follow the Linked profiles link
    Then the linked profiles page is displayed
    And linked profiles are displayed
    And I select a linked profile with appointments enabled false, prescriptions enabled false and medical record enabled false
    And details for the selected linked profile are displayed
    When I click the Switch to this profile button for the proxy user
    Then I see the proxy home page
    When I navigate to Messages
    Then the messages shutter page is displayed
    And the Messages_ProxyShutter page is saved to disk

  Scenario: The messages page is captured with messages
    Given I am using the native app user agent
    And I am a user wishing to view my messages
    And I am logged in
    When I follow the unread messages link from the home page
    And I click the App Messages link on the messages hub page
    Then the Message Senders page is displayed
    And the Messages_Senders page is saved to disk
    When I click on Sender One sender
    Then the Sender Messages page is displayed
    And the Messages_Sender_ReadAndUnread page is saved to disk
    When I click on a message on the Sender Messages page
    Then the Message page is displayed
    And the Messages_Details page is saved to disk

  Scenario: The messages page is captured with no messages
    Given I am using the native app user agent
    And I am a user wishing to view my messages, but I have no messages
    And I am logged in
    When I follow the Messages link from the home page
    And I click the App Messages link on the messages hub page
    Then the Message Senders page is displayed
    And a message is displayed indicating that there are no messages in the Messages Inbox
    And the Messages_NoMessages page is saved to disk
