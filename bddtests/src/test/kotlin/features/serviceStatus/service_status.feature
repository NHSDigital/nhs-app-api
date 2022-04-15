@backend
Feature: Service Status

  Scenario: The service journey rules service readiness endpoint returns healthy
    When the service journey rules service readiness is requested
    Then the response from the service readiness endpoint has status code 204
