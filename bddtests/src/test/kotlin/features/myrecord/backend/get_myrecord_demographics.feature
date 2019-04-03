@my-record
@backend
Feature: Get demographic data
  A user can get their demographic information

  Scenario Outline: Requesting demographics returns demographic data
    Given the my record wiremocks are initialised for <Service>
    And I have logged in and have a valid session cookie
    And the GP Practice has enabled demographics functionality
    When I get the users demographic data
    Then I receive the demographic object

    Examples:
      | Service |
      | EMIS    |
      | TPP     |
      | VISION  |

  Scenario Outline: GP practice has disabled demographics functionality
    Given the my record wiremocks are initialised for <Service>
    And I have logged in and have a valid session cookie
    And the GP Practice has disabled demographics functionality
    When I get the users demographic data
    Then I receive a "Forbidden" error

    Examples:
      | Service |
      | EMIS    |
      | TPP     |
      | VISION  |

  @tech-debt  @NHSO-2549   # covered in Manual Regression Test pack
  Scenario Outline: GP System Unavailable
    Given the my record wiremocks are initialised for <Service>
    And I have logged in and have a valid session cookie
    And the GP System is unavailable
    When I communicate with <Service>
    Then I get a "Bad gateway" error

    Examples:
      | Service |
      | EMIS    |
      | TPP     |

  @tech-debt  @NHSO-2549   # covered in Manual Regression Test pack
  Scenario Outline: GP System Times Out
    Given the my record wiremocks are initialised for <Service>
    And I have logged in and have a valid session cookie
    But the GP System times out for <Service>
    When I communicate with <Service>
    Then I get a "Gateway timeout" error

    Examples:
      | Service |
      | EMIS    |
      | TPP     |
