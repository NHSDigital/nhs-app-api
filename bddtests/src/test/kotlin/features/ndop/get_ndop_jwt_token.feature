@ndop
@backend
Feature: Get Ndop Token
         A user can get an Ndop JWT Token

  Scenario: User requests Ndop JWT Token
    Given I have logged into EMIS and have a valid session cookie
    When I request a Ndop Token
    Then I receive a signed JWT Token

  Scenario: A user with proof level 5 receives an 'Unauthorized' error when they attempt to request their NDOP token
    Given I am a user with proof level 5 and have a valid session cookie
    When I request a Ndop Token
    Then I receive an "Unauthorized" error
