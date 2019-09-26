@messages
@pending @NHSO-6498
Feature: Messages
#The following tests are gp system agnostic

  Scenario: A user can see their messages
    Given I am a user wishing to view my messages
    And I am logged in
    When I navigate to the More page for mobile devices
    And I click the Messages link on the More page
    Then I am on the Messages page
    And my messages are displayed
    # Check the full page
    # Title (messages from)
    # unread Bar
    # all messages and content
    # times beneath messages

  Scenario: A user with no messages can see navigate to the messages page, but sees no messages
    Given I am a user wishing to view my messages
    And I am logged in
    When I navigate to the More page for mobile devices
    And I click the Messages link on the More page
    Then I am on the Messages page
    And no messages are displayed

  Scenario: A desktop user cannot see their messages
    Given I am a user wishing to view my messages
    And I am logged in
    When I browse to the pages at the following urls I see the home page
      | /messages  |

  Scenario: A user with messages disabled in service journey rules cannot see their messages
    Given I am a EMIS user where the journey configurations are:
      | Journey       | Value    |
      | messages      | disabled |
    And I am logged in
    When I navigate to the More page for mobile devices
    Then the link to Messages is not available on the More page
    When I browse to the pages at the following urls I see the home page
      | /messages  |
      | /messages?source=ios  |
      | /messages?source=android  |