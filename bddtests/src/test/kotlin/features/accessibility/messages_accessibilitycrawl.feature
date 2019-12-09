@accessibility
@messages-accessibility
  Feature: Messages accessibility

    Scenario: The messages page is captured with messages
      Given I am using the native app user agent
      And I am a user wishing to view my messages
      And I am logged in
      When I navigate to the More page
      And I click the Messages link on the More page
      Then the Messages Inbox page is displayed
      Then the MessagesInbox page is saved to disk
      When I click on a sender in the Messages Inbox
      Then the Messages page is displayed
      Then the Messages_ReadAndUnread page is saved to disk
      When I click the 'Back' breadcrumb
      Then the Messages Inbox page is displayed
      When I click on a sender in the Messages Inbox
      Then the Messages page is displayed
      Then the Messages_Read page is saved to disk

    Scenario: The messages page is captured with no messages
      Given I am using the native app user agent
      And I am a user wishing to view my messages, but I have no messages
      And I am logged in
      When I navigate to the More page
      And I click the Messages link on the More page
      Then the Messages Inbox page is displayed
      And a message is displayed indicating that there are no messages in the Messages Inbox
      Then the Messages_NoMessages page is saved to disk