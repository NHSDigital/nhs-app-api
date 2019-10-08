@messages
@backend
Feature: Messages Backend
  Scenario: An api user can get their messages
    Given I am an api user wishing to get my messages
    And I have logged in and have a valid session cookie
    When I get my messages from the api
    Then I receive a "OK" success code
    And I receive my messages

  Scenario: An api user getting their messages where no messages are stored will receive a 204
    Given I am an api user wishing to get my messages, but I have no messages
    And I have logged in and have a valid session cookie
    When I get my messages from the api
    Then I receive a "No Content" success code

  Scenario: An api user getting their messages without an access token will receive a 401
    Given I am an api user wishing to get my messages
    And I have logged in and have a valid session cookie
    When I get my messages from the api without an auth token
    Then I receive an "Unauthorized" error

  Scenario: An api user getting their messages with an invalid access token will receive a 401
    Given I am an api user wishing to get my messages
    And I have logged in and have a valid session cookie
    Then an attempt to get messages with an invalid access token will return an Unauthorised error

  Scenario: An api user can post messages
    Given I am an api user wishing to post a message
    When I post a message to the api
    Then I receive a "Created" success code
    And the message is available in the database

  Scenario: An api user posting incomplete messages will receive a 400
    Given I am an api user wishing to post a message
    When I post a message to the api
    Then an attempt to post incomplete messages will return a Bad Request error
