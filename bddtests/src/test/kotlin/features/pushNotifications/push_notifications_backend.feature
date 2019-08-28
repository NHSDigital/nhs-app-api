@pushNotifications
@backend
Feature: Push Notifications Backend

  Scenario: An api user can register their device for push notifications
    Given I am a EMIS api user wishing to register their device for push notifications
    And I have logged in and have a valid session cookie
    When I register the device for push notifications
    Then I receive a "Created" success code
    And I receive the newly created registered device details
    And the device registration is available in the database

  Scenario: An api user registering for push notifications without a token will receive a 401
    Given I am a EMIS api user wishing to register their device for push notifications
    And I have logged in and have a valid session cookie
    When I register the device for push notifications without auth token
    Then I receive an "Unauthorized" error

  Scenario: An api user registering for push notifications with an invalid access token will receive a 401
    Given I am a EMIS api user wishing to register their device for push notifications
    And I have logged in and have a valid session cookie
    Then a registration attempt with an invalid access token will return an Unauthorised error
