@pushNotifications
@backend
Feature: Push Notifications Backend

  Scenario: An api user can register their device for push notifications
    Given I am an api user wishing to register their device for push notifications
    And I have logged in and have a valid session cookie
    When I register the device for push notifications
    Then I receive a "Created" success code
    And I receive the newly created registered device details
    And the device registration is available in the device repository

  Scenario: An api user registering for push notifications without a token will receive a 401
    Given I am an api user wishing to register their device for push notifications
    And I have logged in and have a valid session cookie
    When I register the device for push notifications without auth token
    Then I receive an "Unauthorized" error

  Scenario: An api user registering for push notifications with an invalid access token will receive a 401
    Given I am an api user wishing to register their device for push notifications
    And I have logged in and have a valid session cookie
    Then a registration attempt with an invalid access token will return an Unauthorised error

  Scenario: An api user registering for push notifications without a pns token will receive a 400
    Given I am an api user wishing to register their device for push notifications
    And I have logged in and have a valid session cookie
    When I register the device for push notifications without a pns token
    Then I receive an "Bad Request" error

  Scenario: An api user can register two different devices for push notifications
    Given I am an api user who has registered their device for notifications, and I have another device to register
    And I have logged in and have a valid session cookie
    When I register the device for push notifications
    Then I receive a "Created" success code
    And I receive the newly created registered device details
    And both device registrations are available in the device repository

  Scenario: An api user can get their registration for push notifications
    Given I am an api user who has registered their device for push notifications
    And I have logged in and have a valid session cookie
    When I get the registration for push notifications
    Then I receive a "No Content" success code

  Scenario: An api user with no registration getting their notifications registration receives a 404
    Given I am an api user who has not registered their device for push notifications
    And I have logged in and have a valid session cookie
    When I get the registration for push notifications with an unregistered device pns token
    Then I receive a "Not Found" response

  Scenario: An api user getting their registration for notifications without an access token will receive a 401
    Given I am an api user who has registered their device for push notifications
    And I have logged in and have a valid session cookie
    When I get the registration for push notifications without auth token
    Then I receive an "Unauthorized" error

  Scenario: An api user getting their registration for notifications with an invalid access token will receive a 401
    Given I am an api user who has registered their device for push notifications
    And I have logged in and have a valid session cookie
    Then getting a notification registration with an invalid access token will return an Unauthorised error

  Scenario: An api user getting their registration for notifications without a pns token will receive a 400
    Given I am an api user who has registered their device for push notifications
    And I have logged in and have a valid session cookie
    When I get the registration for push notifications without a pns token
    Then I receive an "Bad Request" error

  Scenario: An api user getting their notifications registration where azure record has been removed will receive a 404
    # This scenario handles multiple users registering on the same device.
    # User 1 registers, and a record is made in azure and cosmos
    # User 2 registers, and a record is made in azure and cosmos, and User 1's record in azure is deleted
    # User 1 gets their registration, on finding no record in azure, the cosmos record is deleted
    Given I am an api user whose notification registration in azure has been overridden by another user
    And I have logged in and have a valid session cookie
    When I get the registration for push notifications
    Then I receive a "Not Found" success code
    And my registration is not in the device repository, but the superseding registration exists

  Scenario: An api user can delete their registration for push notifications
   Given I am an api user wishing to delete their registration for push notifications
   And I have logged in and have a valid session cookie
   When I delete the registration for push notifications
   Then I receive a "No Content" success code
   And the device registration is not available in the device repository

  Scenario: An api user deleting their registration for notifications without an access token will receive a 401
    Given I am an api user wishing to delete their registration for push notifications
    And I have logged in and have a valid session cookie
    When I delete the registration for push notifications without auth token
    Then I receive an "Unauthorized" error

  Scenario: An api user deleting their registration for notifications with an invalid access token will receive a 401
    Given I am an api user wishing to delete their registration for push notifications
    And I have logged in and have a valid session cookie
    Then deleting a notification registration with an invalid access token will return an Unauthorised error

  Scenario: An api user with no registration deleting a registration for notifications will receive a 404
    Given I am an api user who has not registered their device for push notifications
    And I have logged in and have a valid session cookie
    When I delete the registration for push notifications with an unregistered device pns token
    Then I receive a "Not Found" success code

  Scenario: An api user deleting their registration for notifications without a pns token will receive a 400
    Given I am an api user who has registered their device for push notifications
    And I have logged in and have a valid session cookie
    When I delete the registration for push notifications without a pns token
    Then I receive an "Bad Request" error