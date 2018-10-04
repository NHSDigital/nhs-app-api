@ndop
Feature: Get Ndop Token

  A user can get an Ndop JWT Token

  @backend
  @smoketest
  Scenario Outline: User requests Ndop JWT Token
    Given I have logged into <Service> and have a valid session cookie
    When I request a Ndop Token
    Then I receive a signed JWT Token

  Examples:
  |Service|
  |EMIS|

