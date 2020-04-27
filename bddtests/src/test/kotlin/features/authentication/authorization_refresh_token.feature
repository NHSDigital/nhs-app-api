@authentication
Feature: Authorisation refresh token

  Scenario: An API User can refresh their access token
    Given I am an API user who wishes to refresh their access token
    And I have logged in and have a valid session cookie
    When I call the refresh access token endpoint
    Then I receive a "Ok" success code
    And I receive a refreshed access token

  Scenario: An API User fails to refresh their access token when NhsLogin fails
    Given I am an API user who wishes to refresh their access token but NhsLogin will fail
    And I have logged in and have a valid session cookie
    When I call the refresh access token endpoint
    Then I receive a "Bad Gateway" error

  Scenario: An API User without a valid session will receive a 401
    Given I am an API user who wishes to refresh their access token
    When I call the refresh access token endpoint
    Then I receive a "Unauthorized" error