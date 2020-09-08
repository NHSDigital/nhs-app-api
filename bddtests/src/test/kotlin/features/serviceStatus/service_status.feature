@backend
Feature: Service Status

  Scenario: The service journey rules service readiness endpoint returns healthy
    When the service journey rules service readiness is requested
    Then the response from the service readiness endpoint has status code 204

  Scenario: The users service readiness endpoint returns healthy
    When the users service readiness is requested
    Then the response from the service readiness endpoint has status code 204

  Scenario: The user info service readiness endpoint returns healthy
    When the user info service readiness is requested
    Then the response from the service readiness endpoint has status code 204

  Scenario: The messages service readiness endpoint returns healthy
    When the messages service readiness is requested
    Then the response from the service readiness endpoint has status code 204

  Scenario: The logs service readiness endpoint returns healthy
    When the logs service readiness is requested
    Then the response from the service readiness endpoint has status code 204
