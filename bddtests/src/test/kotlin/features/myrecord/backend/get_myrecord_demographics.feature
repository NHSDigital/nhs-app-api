@my-record
@backend
Feature: Get demographic data Backend
  A user can get their demographic information

  Scenario Outline: Requesting demographics returns demographic data for the <GP System> user
    Given the my record wiremocks are initialised for <GP System>
    And I have logged in and have a valid session cookie
    And the GP Practice has enabled demographics functionality
    When I get the users demographic data
    Then I receive the demographic object

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: When GP practice has disabled demographics functionality, the <GP System> user receives Forbidden error
    Given the my record wiremocks are initialised for <GP System>
    And I have logged in and have a valid session cookie
    And the GP Practice has disabled demographics functionality
    When I get the users demographic data
    Then I receive a "Forbidden" error

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  @tech-debt  @NHSO-2549   # covered in Manual Regression Test pack
  Scenario Outline: When GP System is unavailable, the <GP System> user received Bad Gateway error
    Given the my record wiremocks are initialised for <GP System>
    And I have logged in and have a valid session cookie
    And the GP System is unavailable
    When I communicate with <Service>
    Then I get a "Bad gateway" error

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  @tech-debt  @NHSO-2549   # covered in Manual Regression Test pack
  Scenario Outline: When GP System times out, the <GP System> user receives Gateway Timeout error
    Given the my record wiremocks are initialised for <GP System>
    And I have logged in and have a valid session cookie
    But the GP System times out for <Service>
    When I communicate with <Service>
    Then I get a "Gateway timeout" error

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
