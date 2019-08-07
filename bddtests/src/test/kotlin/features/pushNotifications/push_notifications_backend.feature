@pushNotifications
@backend
Feature: Push Notifications Backend

  Scenario Outline: A <GP System> api user can register their device for push notifications
    Given I am a <GP System> api user wishing to register their device for push notifications
    And I have logged in and have a valid session cookie
    When I register the device for push notifications
    Then I receive a "Created" success code
    And I receive the newly created registered device details
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: A <GP System> api user registering for push notifications without a token will receive a 401
    Given I am a <GP System> api user wishing to register their device for push notifications
    And I have logged in and have a valid session cookie
    When I register the device for push notifications without auth token
    Then I receive an "Unauthorized" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |