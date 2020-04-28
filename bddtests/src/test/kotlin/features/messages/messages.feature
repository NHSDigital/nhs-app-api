@messages
Feature: Messages
#The following tests are gp system agnostic

@smoketest
  Scenario: A user can see their read and unread messages
    Given I am using the native app user agent
    And I am a user wishing to view my messages
    And I am logged in
    When I navigate to the More page
    And I click the Messages link on the More page
    Then the Messages Hub page is displayed
    And I click on the NHS App Messages link on the Messages Hub page
    Then the Messages Inbox page is displayed
    And the senders and latest messages are displayed on the Messages Inbox page
    When I click on a sender in the Messages Inbox
    Then the Messages page is displayed
    And my messages from the sender are displayed
    # We need to check the functionality to mark the messages as read with the back button,
    # to ensure that the state is updated
    When I click the 'Back' breadcrumb
    Then the Messages Inbox page is displayed
    And the viewed messages are marked as read on the Messages Inbox page
    When I click on a sender in the Messages Inbox
    Then the Messages page is displayed
    And my messages from the sender are displayed as read

  Scenario: A user can see their messages and follow an internal link
    Given I am using the native app user agent
    And I am a user wishing to view my appointments and my messages with content
      | /account  |
      | /appointments/booking-guidance  |
    And I am logged in
    When I navigate to the More page
    And I click the Messages link on the More page
    Then the Messages Hub page is displayed
    And I click on the NHS App Messages link on the Messages Hub page
    Then the Messages Inbox page is displayed
    And the senders and latest messages are displayed on the Messages Inbox page
    When I click on a sender in the Messages Inbox
    Then the Messages page is displayed
    When I click on the '/account' link in the message
    Then the Account page for mobile devices is displayed
    When I navigate to the More page
    And I click the Messages link on the More page
    Then the Messages Hub page is displayed
    And I click on the NHS App Messages link on the Messages Hub page
    Then the Messages Inbox page is displayed
    And the senders and latest messages are displayed on the Messages Inbox page
    When I click on a sender in the Messages Inbox
    Then the Messages page is displayed
    When I click on the '/appointments/booking-guidance' link in the message
    Then I am on the Appointments Guidance page
    When I click the 'Back' breadcrumb
    Then the Messages page is displayed

  Scenario: A user can see their messages and follow an external link
    Given I am using the native app user agent
    And I am a user wishing to view my messages with content
      | https://111.nhs.uk/  |
      | 111.nhs.uk/  |
    And I am logged in
    When I navigate to the More page
    And I click the Messages link on the More page
    Then the Messages Hub page is displayed
    And I click on the NHS App Messages link on the Messages Hub page
    Then the Messages Inbox page is displayed
    And the senders and latest messages are displayed on the Messages Inbox page
    When I click on a sender in the Messages Inbox
    Then the Messages page is displayed
    When I click the link called 'https://111.nhs.uk/' with a url of 'https://111.nhs.uk/'
    Then a new tab has been opened by the link
    When I click the link called '111.nhs.uk/' with a url of 'https://111.nhs.uk/'
    Then a new tab has been opened by the link

  Scenario: A user can see their messages and see a mailto link
    Given I am using the native app user agent
    And I am a user wishing to view my messages with content
      | email@address.com  |
    And I am logged in
    When I navigate to the More page
    And I click the Messages link on the More page
    Then the Messages Hub page is displayed
    And I click on the NHS App Messages link on the Messages Hub page
    Then the Messages Inbox page is displayed
    And the senders and latest messages are displayed on the Messages Inbox page
    When I click on a sender in the Messages Inbox
    Then the Messages page is displayed
    Then the email address 'email@address.com' is identified as a link in the message

  Scenario: A user can see their messages and follow an incorrect internal link
    Given I am using the native app user agent
    And I am a user wishing to view my messages with content
      | /appointments/sausages  |
    And I am logged in
    When I navigate to the More page
    And I click the Messages link on the More page
    Then the Messages Hub page is displayed
    And I click on the NHS App Messages link on the Messages Hub page
    Then the Messages Inbox page is displayed
    And the senders and latest messages are displayed on the Messages Inbox page
    When I click on a sender in the Messages Inbox
    Then the Messages page is displayed
    When I click on the '/appointments/sausages' link in the message
    Then the Page not found error is displayed

  Scenario: A user with no messages can navigate to the messages inbox, but sees no messages
    Given I am using the native app user agent
    And I am a user wishing to view my messages, but I have no messages
    And I am logged in
    When I navigate to the More page
    And I click the Messages link on the More page
    Then the Messages Hub page is displayed
    And I click on the NHS App Messages link on the Messages Hub page
    Then the Messages Inbox page is displayed
    And a message is displayed indicating that there are no messages in the Messages Inbox

  Scenario: A user with messages disabled in service journey rules cannot see their messages
    Given I am using the native app user agent
    And I am a EMIS user where the journey configurations are:
      | Journey       | Value    |
      | messages      | disabled |
    And I am logged in
    When I navigate to the More page
    And I click the Messages link on the More page
    Then the link to NHS App Messages link is not displayed
    When I browse to the pages at the following urls I see the home page
      | /messages/app-messaging  |
      | /messages/app-messaging?source=ios  |
      | /messages/app-messaging?source=android  |
      | /messages/app-messaging/app-message  |
      | /messages/app-messaging/app-message?source=ios  |
      | /messages/app-messaging/app-message?source=android  |

  Scenario: A user getting their summary messages when an internal server error occurs sees an error and can try again
    Given I am using the native app user agent
    And I am a user wishing to view my messages but retrieving the messages will cause an internal server error
    And I am logged in
    When I navigate to the More page
    And I click the Messages link on the More page
    Then the Messages Hub page is displayed
    And I click on the NHS App Messages link on the Messages Hub page
    Then an error with a retry button is displayed indicating that there was a problem getting messages
    When the messages in the repository can be retrieved successfully
    And I click the 'Try again' button
    Then the Messages Inbox page is displayed

  Scenario: A user getting messages from a sender when an internal server error occurs sees an error and can try again
    Given I am using the native app user agent
    And I am a user wishing to view my messages
    And I am logged in
    When I navigate to the More page
    And I click the Messages link on the More page
    Then the Messages Hub page is displayed
    And I click on the NHS App Messages link on the Messages Hub page
    Then the senders and latest messages are displayed on the Messages Inbox page
    When retrieving the messages from the repository will cause an internal server error
    And I click on a sender in the Messages Inbox
    Then an error with a retry button is displayed indicating that there was a problem getting messages from the sender
    When the messages in the repository can be retrieved successfully
    And I click the 'Try again' button
    Then the Messages page is displayed
    And my messages from the sender are displayed