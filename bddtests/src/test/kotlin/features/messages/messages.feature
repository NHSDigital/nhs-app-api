@profile-messages-notifications
@messages
Feature: Messages
#The following tests are gp system agnostic

  @smoketest
  Scenario: A user can see their read and unread messages
    Given I am using the native app user agent
    And I am a user wishing to view my messages
    And I am logged in
    When I navigate to the Messages Hub page
    And I click the App Messages link on the messages hub page
    Then the Message Senders page is displayed
    And the senders are displayed on the Messages Inbox page
    And the Sender One Canonical sender is displayed as unread
    And the unread message and sender count is displayed on the page
    When I click on Sender One Canonical sender
    Then the Sender Messages page is displayed
    And the unread messages title is displayed
    And my messages from the sender are displayed
    # We need to check the functionality to mark the messages as read with the back button,
    # to ensure that the state is updated
    When I click on the unread message on the Sender Messages page
    Then the Message page is displayed
    When I click the 'Back' breadcrumb
    Then the Sender Messages page is displayed
    And the read messages title is displayed
    And my messages from the sender are displayed as read
    When I click the 'Back' breadcrumb
    Then the Message Senders page is displayed
    And the Sender One Canonical sender is displayed as read

  Scenario: A user with proof level 5 can access their messages
    Given I am using the native app user agent
    And I am a user with proof level 5 wishing to view my messages
    And I am logged in
    When I follow the unread messages link from the home page
    And I click the App Messages link on the messages hub page
    Then the Message Senders page is displayed

  Scenario: A user with proof level 5 can see they have unread messages on the home page
    Given I am using the native app user agent
    And I am a user with proof level 5 wishing to view my messages
    And I am logged in
    Then I can see I have unread messages on the home page

  Scenario: A user with proof level 5 whose access token is about to expire can see they have unread messages
    Given I am using the native app user agent
    And I am a user with proof level 5 whose access token is about to expire wishing to view my messages
    And I am logged in
    Then I can see I have unread messages on the home page

  Scenario: A user can see they have unread messages on the Messages Hub
    Given I am using the native app user agent
    And I am a user wishing to view my messages and GP surgery messages
    And I am logged in
    Then I can see I have unread messages on the home page
    When I follow the unread messages link from the home page
    Then the Messages Hub page is displayed with the unread indicator for app messaging

  Scenario: A user can see their plain text messages and follow an internal link
    Given I am using the native app user agent
    And I am a user wishing to view my appointments and my messages with content
      | /more  |
      | /appointments/gp-appointments/booking  |
    And I am logged in
    When I navigate to the Messages Hub page
    And I click the App Messages link on the messages hub page
    And the Message Senders page is displayed
    And the senders are displayed on the Messages Inbox page
    And I click on Sender One Canonical sender
    Then the Sender Messages page is displayed
    When I click on message 1 message on the Sender Messages page
    Then the Message page is displayed
    When I click on the '/more' link in the message
    Then the More page for mobile devices is displayed
    When I navigate to the Messages Hub page
    And I click the App Messages link on the messages hub page
    And the Message Senders page is displayed
    And I click on Sender One Canonical sender
    Then the Sender Messages page is displayed
    When I click on message 2 message on the Sender Messages page
    Then the Message page is displayed
    When I click on the '/appointments/gp-appointments/booking' link in the message
    Then the Available Appointments page is displayed
    When I click the 'Back' breadcrumb
    Then the Message page is displayed

  Scenario: A user can see their plain text messages and follow an external link
    Given I am using the native app user agent
    And '111' responds to requests for '/home'
    And I am a user wishing to view my messages with content
      | http://stubs.local.bitraft.io:8080/external/111/home  |
      | stubs.local.bitraft.io:8080/external/111/home  |
    And I am logged in
    When I follow the Messages link from the home page
    And I click the App Messages link on the messages hub page
    And the Message Senders page is displayed
    And the senders are displayed on the Messages Inbox page
    And I click on Sender One Canonical sender
    Then the Sender Messages page is displayed
    When I click on message 1 message on the Sender Messages page
    Then the Message page is displayed
    When I click the link called 'http://stubs.local.bitraft.io:8080/external/111/home' with a url of 'http://stubs.local.bitraft.io:8080/external/111/home'
    Then a new tab has been opened by the link
    When I click the 'Back' breadcrumb
    Then the Sender Messages page is displayed
    When I click on message 2 message on the Sender Messages page
    Then the Message page is displayed
    When I click the link called 'stubs.local.bitraft.io:8080/external/111/home' with a url of 'https://stubs.local.bitraft.io:8080/external/111/home'
    Then a new tab has been opened by the link

  Scenario: A user can see their plain text messages and see a mailto link
    Given I am using the native app user agent
    And I am a user wishing to view my messages with content
      | email@address.com  |
    And I am logged in
    When I navigate to the Messages Hub page
    And I click the App Messages link on the messages hub page
    And the Message Senders page is displayed
    And the senders are displayed on the Messages Inbox page
    And I click on Sender One Canonical sender
    Then the Sender Messages page is displayed
    When I click on message 1 message on the Sender Messages page
    Then the Message page is displayed
    And the email address 'email@address.com' is identified as a link in the message

  Scenario: A user can see their markdown messages and follow some external links
    Given I am using the native app user agent
    And '111' responds to requests for '/home'
    And I am a user wishing to view my messages with markdown content
      | [http://stubs.local.bitraft.io:8080/external/111/home](http://stubs.local.bitraft.io:8080/external/111/home)  |
      | [stubs.local.bitraft.io:8080/external/111/home](https://stubs.local.bitraft.io:8080/external/111/home)  |
    And I am logged in
    When I follow the Messages link from the home page
    And I click the App Messages link on the messages hub page
    And the Message Senders page is displayed
    And the senders are displayed on the Messages Inbox page
    And I click on Sender One Canonical sender
    Then the Sender Messages page is displayed
    When I click on message 1 message on the Sender Messages page
    Then the Message page is displayed
    When I click the link called 'http://stubs.local.bitraft.io:8080/external/111/home' with a url of 'http://stubs.local.bitraft.io:8080/external/111/home'
    Then a new tab has been opened by the link
    When I click the 'Back' breadcrumb
    Then the Sender Messages page is displayed
    When I click on message 2 message on the Sender Messages page
    Then the Message page is displayed
    When I click the link called 'stubs.local.bitraft.io:8080/external/111/home' with a url of 'https://stubs.local.bitraft.io:8080/external/111/home'
    Then a new tab has been opened by the link

  Scenario: A user can see their markdown messages and follow a link to a nhs partner site via a redirector url
    Given I am using the native app user agent
    And I am a user wishing to view my messages with markdown content
      | [pkb.stubs.local.bitraft.io](http://pkb.stubs.local.bitraft.io:8080/nhs-login/login?phrPath=%2Fauth%2FgetInbox.action%3Ftab%3Dmessages)  |
    And I am logged in
    When I follow the Messages link from the home page
    And I click the App Messages link on the messages hub page
    And the Message Senders page is displayed
    And the senders are displayed on the Messages Inbox page
    And I click on Sender One Canonical sender
    Then the Sender Messages page is displayed
    When I click on message 1 message on the Sender Messages page
    Then the Message page is displayed
    When I click the internal link called 'pkb.stubs.local.bitraft.io' with a url of '/redirector?redirect_to=http%3A%2F%2Fpkb.stubs.local.bitraft.io%3A8080%2Fnhs-login%2Flogin%3FphrPath%3D%252Fauth%252FgetInbox.action%253Ftab%253Dmessages'
    Then a new tab has been opened by the link

  Scenario: A user can see their markdown messages and follow an internal link
    Given I am using the native app user agent
    And I am a user wishing to view my appointments and my messages with markdown content
      | [More](/more)  |
    And I am logged in
    When I follow the Messages link from the home page
    And I click the App Messages link on the messages hub page
    And the Message Senders page is displayed
    And the senders are displayed on the Messages Inbox page
    And I click on Sender One Canonical sender
    Then the Sender Messages page is displayed
    When I click on message 1 message on the Sender Messages page
    Then the Message page is displayed
    When I click the internal link called 'More' with a url of '/more'
    Then the More page for mobile devices is displayed

  Scenario: A user can see their markdown messages and see a mailto link
    Given I am using the native app user agent
    And I am a user wishing to view my messages with markdown content
      | [email1@address.com](mailto:email1@address.com)  |
    And I am logged in
    When I follow the Messages link from the home page
    And I click the App Messages link on the messages hub page
    And the Message Senders page is displayed
    And the senders are displayed on the Messages Inbox page
    And I click on Sender One Canonical sender
    Then the Sender Messages page is displayed
    When I click on message 1 message on the Sender Messages page
    Then the Message page is displayed
    And the email address 'email1@address.com' is identified as a link in the message

  Scenario: A user can see their messages and follow an incorrect internal link
    Given I am using the native app user agent
    And '111' responds to requests for '/home'
    And I am a user wishing to view my messages with content
      | /appointments/sausages  |
    And I am logged in
    When I navigate to the Messages Hub page
    And I click the App Messages link on the messages hub page
    And the Message Senders page is displayed
    And the senders are displayed on the Messages Inbox page
    And I click on Sender One Canonical sender
    Then the Sender Messages page is displayed
    When I click on message 1 message on the Sender Messages page
    Then the Message page is displayed
    When I click on the '/appointments/sausages' link in the message
    Then the Page not found error is displayed
    When I click the error '111.nhs.uk' link with a url of 'http://stubs.local.bitraft.io:8080/external/111/home'
    Then a new tab has been opened by the link

  Scenario: A user with no messages can navigate to the messages inbox, but sees no messages
    Given I am using the native app user agent
    And I am a user wishing to view my messages, but I have no messages
    And I am logged in
    When I navigate to the Messages Hub page
    And I click the App Messages link on the messages hub page
    And the Message Senders page is displayed
    And a message is displayed indicating that there are no messages in the Messages Inbox

  Scenario: A user with messages disabled in service journey rules cannot see their messages
    Given I am using the native app user agent
    And I am a EMIS user where the journey configurations are:
      | Journey       | Value    |
      | messages      | disabled |
    And I am logged in
    When I navigate to the Messages Hub page
    Then the link to NHS App Messages is not available on the Messages Hub page
    When I browse to the pages at the following urls I see the home page
      | /messages/app-messaging  |
      | /messages/app-messaging?source=ios  |
      | /messages/app-messaging?source=android  |
      | /messages/app-messaging/app-message  |
      | /messages/app-messaging/app-message?source=ios  |
      | /messages/app-messaging/app-message?source=android  |

  Scenario: A user getting their message senders when a server error occurs sees an error and can try again
    Given I am using the native app user agent
    And I am a user wishing to view my messages
    And I am logged in
    When I navigate to the Messages Hub page
    And retrieving the messages will cause a server error
    And I click the App Messages link on the messages hub page
    Then an error is displayed indicating that there was a problem getting message senders
    When the messages can be retrieved successfully
    And I click the 'Try again' button
    Then the Message Senders page is displayed

  Scenario: A user getting messages from a sender when a server error occurs sees an error and can try again
    Given I am using the native app user agent
    And I am a user wishing to view my messages
    And I am logged in
    When I navigate to the Messages Hub page
    And I click the App Messages link on the messages hub page
    Then the senders are displayed on the Messages Inbox page
    When retrieving the messages will cause a server error
    And I click on Sender One Canonical sender
    Then an error is displayed indicating that there was a problem getting messages from Sender One Canonical
    When the messages can be retrieved successfully
    And I click the 'Try again' button
    Then the Sender Messages page is displayed
    And my messages from the sender are displayed

  Scenario: A user getting a message when a server error occurs sees an error and can try again
    Given I am using the native app user agent
    And I am a user wishing to view my messages
    And I am logged in
    When I navigate to the Messages Hub page
    And I click the App Messages link on the messages hub page
    Then the senders are displayed on the Messages Inbox page
    When I click on Sender One Canonical sender
    Then the Sender Messages page is displayed
    When retrieving the messages will cause a server error
    And I click on a message on the Sender Messages page
    Then an error is displayed indicating that there was a problem getting a message
    When the messages can be retrieved successfully
    And I click the 'Try again' button
    Then the Message page is displayed

  Scenario: A desktop user can see back links on the app messages journey
    Given I am a user wishing to view my messages
    And I am logged in
    When I navigate to the Messages Hub page
    And I click the App Messages link on the messages hub page
    And the Message Senders page is displayed
    And I click on Sender One Canonical sender
    Then the Sender Messages page is displayed
    When I click on a message on the Sender Messages page
    Then the Message page is displayed
    When I click the Back link
    Then the Sender Messages page is displayed
    When I click the Back link
    Then the Message Senders page is displayed
    When I click the Back link
    Then the Messages Hub page is displayed

  Scenario: A native user cannot see back links on the app messages journey
    Given I am using the native app user agent
    And I am a user wishing to view my messages
    And I am logged in
    When I navigate to the Messages Hub page
    And I click the App Messages link on the messages hub page
    Then the Message Senders page is displayed
    And I can't see the Back link
    When I click on Sender One Canonical sender
    Then the Sender Messages page is displayed
    And I can't see the Back link
    When I click on a message on the Sender Messages page
    Then the Message page is displayed
    And I can't see the Back link

  Scenario: A user can see a message with a questionnaire and respond to it
    Given I am a user wishing to view messages that require a reply
    And I am logged in
    When I navigate to the Messages Hub page
    And I click the App Messages link on the messages hub page
    And the Message Senders page is displayed
    And I click on Sender One Canonical sender
    Then the Sender Messages page is displayed
    When I click on a message on the Sender Messages page
    Then the Message page is displayed
    When I click on the 'Reply to this message' button
    And I select an option to reply
    And I send the reply
    And I can see the response message

  Scenario: A user can see a message which has already been replied
    Given I am a user wishing to view messages that is already replied to
    And I am logged in
    When I navigate to the Messages Hub page
    And I click the App Messages link on the messages hub page
    And the Message Senders page is displayed
    And I click on Sender One Canonical sender
    Then the Sender Messages page is displayed
    When I click on a message on the Sender Messages page
    And I can see the response message
