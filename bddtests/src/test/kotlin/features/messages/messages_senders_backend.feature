@profile-messages-notifications
@messages
@backend
Feature: Messages Senders Backend

  Scenario: An api user can get all their message senders
    Given I am an api user wishing to get my messages
    And I have logged in and have a valid session cookie
    When I try to get a list of message senders
    Then I receive a "OK" success code
    And I can see a list of message senders along with a count of unread messages per sender

  Scenario: An api user attempting to retrieve their message senders without passing an access token
  will receive a 401
    Given I am an api user with an unread message
    And I have logged in and have a valid session cookie
    When I try to get the message senders without passing an access token
    Then I receive an "Unauthorized" error

  Scenario: An api user attempting to retrieve their message senders with no results returns a 204
    Given I am an api user wishing to get my messages, but I have no messages
    And I have logged in and have a valid session cookie
    When I try to get a list of message senders
    Then I receive a "No Content" success code
