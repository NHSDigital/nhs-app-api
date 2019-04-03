@ndop
@backend
Feature: Get Ndop Token

  A user can get an Ndop JWT Token

  @smoketest
  Scenario: User requests Ndop JWT Token
    Given I have logged into EMIS and have a valid session cookie
    When I request a Ndop Token
    Then I receive a signed JWT Token
