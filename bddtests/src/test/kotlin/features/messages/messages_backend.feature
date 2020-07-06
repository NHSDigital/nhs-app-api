@messages
@backend
Feature: Messages Backend

  Scenario: An api user can get a summary of their messages
    Given I am an api user wishing to get my messages
    And I have logged in and have a valid session cookie
    When I get a summary of my messages from the api
    Then I receive a "OK" success code
    And I receive a summary of my messages

  Scenario: An api user getting a summary of their messages where no messages are stored will receive a 204
    Given I am an api user wishing to get my messages, but I have no messages
    And I have logged in and have a valid session cookie
    When I get a summary of my messages from the api
    Then I receive a "No Content" success code

  Scenario: An api user getting a summary of their messages without an access token will receive a 401
    Given I am an api user wishing to get my messages
    And I have logged in and have a valid session cookie
    When I get a summary of my messages from the api without an access token
    Then I receive an "Unauthorized" error

  Scenario: An api user getting a summary of their messages with an invalid access token will receive a 401
    Given I am an api user wishing to get my messages
    And I have logged in and have a valid session cookie
    Then an attempt to get a summary of my messages with an invalid access token will return an Unauthorised error

  Scenario: An api user can get all their messages from a sender
    Given I am an api user wishing to get my messages
    And I have logged in and have a valid session cookie
    When I get my messages from a sender from the api
    Then I receive a "OK" success code
    And I receive my messages from a sender

  Scenario: An api user getting their messages from a sender where no messages are stored will receive a 204
    Given I am an api user wishing to get my messages, but I have no messages
    And I have logged in and have a valid session cookie
    When I get my messages from a sender from the api
    Then I receive a "No Content" success code

  Scenario: An api user getting their messages from a sender without an access token will receive a 401
    Given I am an api user wishing to get my messages
    And I have logged in and have a valid session cookie
    When I get my messages from a sender from the api without an access token
    Then I receive an "Unauthorized" error

  Scenario: An api user getting their messages from a sender with an invalid access token will receive a 401
    Given I am an api user wishing to get my messages
    And I have logged in and have a valid session cookie
    Then an attempt to get messages from a sender with an invalid access token will return an Unauthorised error

  Scenario: An api user getting a summary of their messages from a single sender will receive a 502
    Given I am an api user wishing to get my messages
    And I have logged in and have a valid session cookie
    When I get my messages with a summary flag and a target sender
    Then I receive an "Bad Request" error

  Scenario: An api user getting neither a summary of their messages or messages from a single sender will receive a 502
    Given I am an api user wishing to get my messages
    And I have logged in and have a valid session cookie
    When I get my messages without a summary flag and without a target sender
    Then I receive an "Bad Request" error

  Scenario: An api user can post messages
    Given I am an api user wishing to post a message
    When I post a message to the api
    Then I receive a "Created" success code
    And I receive the message id
    And the message is available in the database

  Scenario: An api user posting incomplete messages will receive a 400
    Given I am an api user wishing to post a message
    Then an attempt to post incomplete messages will return a Bad Request error

  Scenario: An api user posting messages without the api key will receive a 401
    Given I am an api user wishing to post a message
    When I post a message to the api without the api key
    Then I receive a "Unauthorized" error

  Scenario: An api user can mark a message as read
    Given I am an api user wishing to mark a message as read
    And I have logged in and have a valid session cookie
    When I patch the message to indicate that it has been read
    Then I receive a "No Content" success code
    And the message has been marked as read in the repository

  Scenario: An api user attempting to mark a message as read without an access token will receive a 401
    Given I am an api user wishing to mark a message as read
    And I have logged in and have a valid session cookie
    When I patch the message to indicate that it has been read without an access token
    Then I receive an "Unauthorized" error

  Scenario: An api user attempting to mark a message as read with an invalid access token will receive a 401
    Given I am an api user wishing to mark a message as read
    And I have logged in and have a valid session cookie
    Then an attempt to mark a message as read with an invalid access token will return an Unauthorised error

  Scenario: An api user with proof level 5 can successfully get a summary of their messages
    Given I am an api user with proof level 5 wishing to get my messages
    And I have logged in and have a valid session cookie
    When I get a summary of my messages from the api
    Then I receive a "OK" success code
    And I receive a summary of my messages
