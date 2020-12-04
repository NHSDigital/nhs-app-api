@messages
Feature: Messages
#The following tests are gp system agnostic

  @native
  Scenario: A logged in user using the native app can navigate to the messages screen
    Given I am a EMIS patient using the native app
    And I log in to the app expecting to see the notifications prompt
    Then I see the notifications prompt
    When I do not accept notifications and continue
    Then I see the home page
    And I navigate to Messages
    And the Messages Hub page is displayed
    And I see messages button on the nav bar is highlighted

  @smoketest
  Scenario: A user can see their read and unread messages
    Given I am using the native app user agent
    And I am a user wishing to view my messages
    And I am logged in
    When I navigate to the More page
    And I click the Messages link on the More page
    And I click the App Messages link on the messages hub page
    And the Messages Inbox page is displayed
    And the senders and latest messages are displayed on the Messages Inbox page
    And I click on a sender in the Messages Inbox
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

  Scenario: A user with proof level 5 can access their messages
    Given I am using the native app user agent
    And I am a user with proof level 5 wishing to view my messages
    And I am logged in
    When I follow the unread messages link from the home page
    And I click the App Messages link on the messages hub page
    Then the Messages Inbox page is displayed

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

  Scenario: A user can see they have unread messages on the More page
    Given I am using the native app user agent
    And I am a user wishing to view my messages and GP surgery messages
    And I am logged in
    Then I can see I have unread messages on the home page
    When I navigate to the More page
    Then I see the unread indicator on the More page

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
      | /account  |
      | /appointments/gp-appointments/booking  |
    And I am logged in
    When I navigate to the More page
    And I click the Messages link on the More page
    And I click the App Messages link on the messages hub page
    And the Messages Inbox page is displayed
    And the senders and latest messages are displayed on the Messages Inbox page
    And I click on a sender in the Messages Inbox
    Then the Messages page is displayed
    When I click on the '/account' link in the message
    Then the Account page for mobile devices is displayed
    When I navigate to the More page
    And I click the Messages link on the More page
    And I click the App Messages link on the messages hub page
    And the Messages Inbox page is displayed
    And the senders and latest messages are displayed on the Messages Inbox page
    And I click on a sender in the Messages Inbox
    Then the Messages page is displayed
    When I click on the '/appointments/gp-appointments/booking' link in the message
    Then the Available Appointments page is displayed
    When I click the 'Back' breadcrumb
    Then the Messages page is displayed

  Scenario: A user can see their plain text messages and follow an external link
    Given I am using the native app user agent
    And I am a user wishing to view my messages with content
      | https://111.nhs.uk/  |
      | 111.nhs.uk/  |
    And I am logged in
    When I follow the Messages link from the home page
    And I click the App Messages link on the messages hub page
    And the Messages Inbox page is displayed
    And the senders and latest messages are displayed on the Messages Inbox page
    And I click on a sender in the Messages Inbox
    Then the Messages page is displayed
    When I click the link called 'https://111.nhs.uk/' with a url of 'https://111.nhs.uk/'
    Then a new tab has been opened by the link
    When I click the link called '111.nhs.uk/' with a url of 'https://111.nhs.uk/'
    Then a new tab has been opened by the link

  Scenario: A user can see their plain text messages and see a mailto link
    Given I am using the native app user agent
    And I am a user wishing to view my messages with content
      | email@address.com  |
    And I am logged in
    When I navigate to the More page
    And I click the Messages link on the More page
    And I click the App Messages link on the messages hub page
    And the Messages Inbox page is displayed
    And the senders and latest messages are displayed on the Messages Inbox page
    And I click on a sender in the Messages Inbox
    Then the Messages page is displayed
    And the email address 'email@address.com' is identified as a link in the message

  Scenario: A user can see their markdown messages and follow an external link
    Given I am using the native app user agent
    And I am a user wishing to view my messages with markdown content
      | [https://111.nhs.uk/](https://111.nhs.uk/)  |
      | [111.nhs.uk/](https://111.nhs.uk/)  |
    And I am logged in
    When I follow the Messages link from the home page
    And I click the App Messages link on the messages hub page
    And the Messages Inbox page is displayed
    And the senders and latest messages are displayed on the Messages Inbox page
    And I click on a sender in the Messages Inbox
    Then the Messages page is displayed
    When I click the link called 'https://111.nhs.uk/' with a url of 'https://111.nhs.uk/'
    Then a new tab has been opened by the link
    When I click the link called '111.nhs.uk/' with a url of 'https://111.nhs.uk/'
    Then a new tab has been opened by the link

  Scenario: A user can see their markdown messages and follow an internal link
    Given I am using the native app user agent
    And I am a user wishing to view my appointments and my messages with markdown content
      | [Account](/account)  |
    And I am logged in
    When I follow the Messages link from the home page
    And I click the App Messages link on the messages hub page
    And the Messages Inbox page is displayed
    And the senders and latest messages are displayed on the Messages Inbox page
    And I click on a sender in the Messages Inbox
    Then the Messages page is displayed
    When I click the internal link called 'Account' with a url of '/account'
    Then the Account page for mobile devices is displayed

  Scenario: A user can see their markdown messages and see a mailto link
    Given I am using the native app user agent
    And I am a user wishing to view my messages with markdown content
      | [email1@address.com](mailto:email1@address.com)  |
    And I am logged in
    When I follow the Messages link from the home page
    And I click the App Messages link on the messages hub page
    And the Messages Inbox page is displayed
    And the senders and latest messages are displayed on the Messages Inbox page
    And I click on a sender in the Messages Inbox
    Then the Messages page is displayed
    And the email address 'email1@address.com' is identified as a link in the message

  Scenario: A user can see their messages and follow an incorrect internal link
    Given I am using the native app user agent
    And I am a user wishing to view my messages with content
      | /appointments/sausages  |
    And I am logged in
    When I navigate to the More page
    And I click the Messages link on the More page
    And I click the App Messages link on the messages hub page
    And the Messages Inbox page is displayed
    And the senders and latest messages are displayed on the Messages Inbox page
    And I click on a sender in the Messages Inbox
    Then the Messages page is displayed
    When I click on the '/appointments/sausages' link in the message
    Then the Page not found error is displayed
    When I click the error '111.nhs.uk' link with a url of 'https://111.nhs.uk'
    Then a new tab has been opened by the link

  Scenario: A user with no messages can navigate to the messages inbox, but sees no messages
    Given I am using the native app user agent
    And I am a user wishing to view my messages, but I have no messages
    And I am logged in
    When I navigate to the More page
    And I click the Messages link on the More page
    And I click the App Messages link on the messages hub page
    And the Messages Inbox page is displayed
    And a message is displayed indicating that there are no messages in the Messages Inbox

  Scenario: A user with messages disabled in service journey rules cannot see their messages
    Given I am using the native app user agent
    And I am a EMIS user where the journey configurations are:
      | Journey       | Value    |
      | messages      | disabled |
    And I am logged in
    When I navigate to the More page
    Then the link to NHS App Messages is not available on the Messages Hub page
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
    And I click the App Messages link on the messages hub page
    And an error with a retry button is displayed indicating that there was a problem getting messages
    And the messages in the repository can be retrieved successfully
    And I click the 'Try again' button
    Then the Messages Inbox page is displayed

  Scenario: A user getting messages from a sender when an internal server error occurs sees an error and can try again
    Given I am using the native app user agent
    And I am a user wishing to view my messages
    And I am logged in
    When I navigate to the More page
    And I click the Messages link on the More page
    And I click the App Messages link on the messages hub page
    And the senders and latest messages are displayed on the Messages Inbox page
    And retrieving the messages from the repository will cause an internal server error
    And I click on a sender in the Messages Inbox
    Then an error with a retry button is displayed indicating that there was a problem getting messages from the sender
    When the messages in the repository can be retrieved successfully
    And I click the 'Try again' button
    Then the Messages page is displayed
    And my messages from the sender are displayed

  Scenario: A desktop user can see back links on the app messages journey
    Given I am a user wishing to view my messages
    And I am logged in
    When I navigate to the More page
    And I click the Messages link on the More page
    And I click the App Messages link on the messages hub page
    And the Messages Inbox page is displayed
    And the Back link on the Messages Inbox page is displayed
    And I click on a sender in the Messages Inbox
    Then the Messages page is displayed
    And the Back link on the App Messages page is displayed
    And I click on the Back link on the App Messages page
    And the Messages Inbox page is displayed
    And I click on the Back link on the Messages Inbox page
    And the Messages Hub page is displayed

  Scenario: A native user cannot see back links on the app messages journey
    Given I am using the native app user agent
    And I am a user wishing to view my messages
    And I am logged in
    When I navigate to the More page
    And I click the Messages link on the More page
    And I click the App Messages link on the messages hub page
    And the Messages Inbox page is displayed
    And the Back link is not shown on the Messages Inbox page
    And I click on a sender in the Messages Inbox
    Then the Messages page is displayed
    And the Back link is not shown on the Messages page
