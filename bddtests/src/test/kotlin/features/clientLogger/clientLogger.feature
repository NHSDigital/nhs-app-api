@clientLogger
@backend
Feature: Client Logger Backend

  Scenario: An api user can post a valid log event to the client logger endpoint
    When I post a valid log event to the logger
    Then I receive a "OK" success code

  Scenario: An api user can post an invalid log event to the client logger endpoint
    When I post an invalid log event to the logger
    Then I receive a "BAD REQUEST" error