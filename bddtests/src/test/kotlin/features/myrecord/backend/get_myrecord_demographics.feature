Feature: Get demographic data

  A user can get their demographic information

  @NHSO-691
  @backend
  Scenario Outline: Requesting demographics returns demographic data
    Given the my record wiremocks are initialised for <Service>
    And I have logged in and have a valid session cookie for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    When I get the users demographic data
    Then I receive the demographic object
    And the field indicating supplier is set to <Service>

    Examples:
      | Service |
      | EMIS    |
      | TPP     |

  @NHSO-691
  @backend
  Scenario Outline: GP practice has disabled demographics functionality
    Given the my record wiremocks are initialised for <Service>
    And I have logged in and have a valid session cookie for <Service>
    And the GP Practice has disabled demographics functionality for <Service>
    When I get the users demographic data
    Then I receive a "Forbidden" error
    And the field indicating supplier is set to <Service>

    Examples:
      | Service |
      | EMIS    |
      | TPP     |

  @NHSO-691
  @pending
  @backend
  Scenario Outline: GP System Unavailable
    Given the my record wiremocks are initialised for <Service>
    And I have logged in and have a valid session cookie for <Service>
    And the GP System is unavailable
    When I communicate with <Service>
    Then I get a "Bad gateway" error
    And the field indicating supplier is set to <Service>

    Examples:
      | Service |
      | EMIS    |
      | TPP     |

  @NHSO-691
  @pending
  @backend
  Scenario Outline: GP System Times Out
    Given the my record wiremocks are initialised for <Service>
    And I have logged in and have a valid session cookie for <Service>
    But the GP System times out for <Service>
    When I communicate with <Service>
    Then I get a "Gateway timeout" error
    And the field indicating supplier is set to <Service>

    Examples:
      | Service |
      | EMIS    |
      | TPP     |
