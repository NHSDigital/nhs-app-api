@messages
Feature: Messages
#The following tests are gp system agnostic

  Scenario: A user can see their messages
    Given I am a user wishing to view my messages
    And I am logged in
    When I navigate to the More page for mobile devices
    And I click the Messages link on the More page
    Then the Messages page is displayed
    And my messages are displayed

  Scenario: A user with no messages can navigate to the messages page, but sees no messages
    Given I am a user wishing to view my messages, but I have no messages
    And I am logged in
    When I navigate to the More page for mobile devices
    And I click the Messages link on the More page
    Then the Messages page is displayed
    And no messages are displayed

  Scenario: A desktop user cannot see their messages
    Given I am a user wishing to view my messages
    And I am logged in
    When I navigate to the More page for desktop
    Then the link to Messages is not available on the More page
    When I browse to the pages at the following urls I see the home page
      | /messaging/messages  |

  Scenario: A user with messages disabled in service journey rules cannot see their messages
    Given I am a EMIS user where the journey configurations are:
      | Journey       | Value    |
      | messages      | disabled |
    And I am logged in
    When I navigate to the More page for mobile devices
    Then the link to Messages is not available on the More page
    When I browse to the pages at the following urls I see the home page
      | /messaging/messages  |
      | /messaging/messages?source=ios  |
      | /messaging/messages?source=android  |