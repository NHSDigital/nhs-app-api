Feature: Get Test Results Data

  A user can get their test results information

  @NHSO-1508
  @backend
  Scenario Outline: Requesting multiple test results returns multiple test results data for <Service>
    Given the my record wiremocks are initialised for <Service>
    And I have logged in and have a valid session cookie for <Service>
    And the GP Practice has multiple test results for <Service>
    When I get the users test results
    Then I receive <Count> test results as part of the my record object
    And the field indicating supplier is set to <Service>

    Examples:
      | Service | Count |
      | EMIS    | 2     |
      | TPP     | 6     |

  @backend
  Scenario Outline: Requesting single test result with child values with no ranges for EMIS
    Given the my record wiremocks are initialised for <Service>
    And I have logged in and have a valid session cookie for <Service>
    And the GP Practice has a single test result with multiple child values with no ranges for EMIS
    When I get the users test results
    Then I receive the test result with term set correctly to Term
    And I receive line items for each child value
    And the line item value is set correctly
    And the field indicating supplier is set to <Service>

    Examples:
      | Service |
      | EMIS    |

  @backend
  Scenario Outline: Requesting single test result with child values with ranges for EMIS
    Given the my record wiremocks are initialised for <Service>
    And I have logged in and have a valid session cookie for <Service>
    And the GP Practice has a single test result with multiple child values with ranges for EMIS
    When I get the users test results
    Then I receive the test result with term set correctly to Term
    And I receive line items for each child value
    And the line item displays text value and range
    And the field indicating supplier is set to <Service>

    Examples:
      | Service |
      | EMIS    |

  @backend
  Scenario Outline: Requesting single test result with no child items or range for EMIS
    Given the my record wiremocks are initialised for <Service>
    And I have logged in and have a valid session cookie for <Service>
    And the GP Practice has test results enabled and a single test result exists with no child values or range for EMIS
    When I get the users test results
    Then I receive a single test result with the term set correctly to Term TextValue NumericUnits
    And the field indicating supplier is set to <Service>

    Examples:
      | Service |
      | EMIS    |

  @backend
  Scenario Outline: Requesting single test result with no child items and a range for EMIS
    Given the my record wiremocks are initialised for <Service>
    And I have logged in and have a valid session cookie for <Service>
    And the GP Practice has a single test result with no child values and range for EMIS
    When I get the users test results
    Then I receive the term set correctly to Term TextValue NumericUnits Range
    And the field indicating supplier is set to <Service>

    Examples:
      | Service |
      | EMIS    |

  @NHSO-1508
  @backend
  Scenario Outline: GP Practice has disabled test results functionality
    Given the my record wiremocks are initialised for <Service>
    And I have logged in and have a valid session cookie for <Service>
    And the GP Practice has disabled test results functionality for <Service>
    When I get the users test results
    Then the flag informing that the patient has access to the test results data is set to "False"
    And the flag informing that there was an error retrieving the test results data is set to "False"
    And the field indicating supplier is set to <Service>

    Examples:
      | Service |
      | EMIS    |
      | TPP     |

  @NHSO-1508
  @backend
  Scenario Outline: Error occurs getting test results
    Given the my record wiremocks are initialised for <Service>
    And I have logged in and have a valid session cookie for <Service>
    And an error occurred retrieving the test results from <Service>
    When I get the users test results
    Then the flag informing that there was an error retrieving the test results data is set to "True"

    Examples:
      | Service |
      | EMIS    |
      | TPP     |
