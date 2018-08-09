Feature: View My Medical Record Information - Test Results

  @NHSO-686
  Scenario Outline: A user can view test result section
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And I am on my record information page
    Then I see the test result heading
    And I see the test result section collapsed

    Examples:
      | Service |
      | EMIS    |
      | TPP     |

  @smoketest
  @NHSO-686
  Scenario Outline: A user can view test result information
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has six test results for <Service>
    And I am on my record information page
    When I click the test result section
    Then I see test result information

    Examples:
      | Service |
      | EMIS    |
      | TPP     |

  @NHSO-686
  Scenario Outline: A user has no access to test result section
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And I do not have access to test results for <Service>
    And I am on my record information page
    When I click the test result section
    Then I see a message indicating that I have no access to view test result

    Examples:
      | Service |
      | EMIS    |
      | TPP     |

  @NHSO-686
  Scenario Outline: A user has no test results
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And I have no test results for <Service>
    And I am on my record information page
    When I click the test result section
    Then I see a message indicating that I have no information recorded for this section

    Examples:
      | Service |
      | EMIS    |
      | TPP     |

  @NHSO-686
  Scenario Outline: An error occurs when trying to retrieve test result data from EMIS
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And an error occurred retrieving the test results from <Service>
    And I am on my record information page
    When I click the test result section
    Then I see an error occured message

    Examples:
      | Service |
      | EMIS    |
      | TPP     |

  @NHSO-686
  Scenario Outline: An EMIS user has one test result with one value
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has a single test result with single child values with no ranges for <Service>
    And I am on my record information page
    When I click the test result section
    Then I see one test result with one value

    Examples:
      | Service |
      | EMIS    |

  @NHSO-686
  Scenario Outline: An EMIS user has one test result with one value and a range
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has a single test result with single child value with A range for <Service>
    And I am on my record information page
    When I click the test result section
    Then I see one test result with one value and a range

    Examples:
      | Service |
      | EMIS    |

  @NHSO-686
  Scenario Outline: An EMIS user has one test result with multiple child values
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has a single test result with multiple child values with no ranges for <Service>
    And I am on my record information page
    When I click the test result section
    Then I see one test result with multiple child values

    Examples:
      | Service |
      | EMIS    |

  @NHSO-686
  Scenario Outline: An EMIS user has test results with multiple child values which have ranges
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has a single test result with multiple child values with ranges for <Service>
    And I am on my record information page
    When I click the test result section
    Then I see test results with multiple child values some of which have ranges

    Examples:
      | Service |
      | EMIS    |

  Scenario Outline: A user has multiple test results
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has six test results for <Service>
    And I am on my record information page
    When I click the test result section
    Then I see <Count> test results

    Examples:
      | Service | Count |
      | TPP     | 6     |

  Scenario Outline: An exception occurs retrieving test result detail
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has six test results for <Service>
    And an error occurs retrieving the test result detail
    And I am on my record information page
    When I click the test result section
    And I click a test result
    Then I see the appropriate error message for retrieving test result detail

    Examples:
      | Service |
      | TPP     |