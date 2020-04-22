@prescription
@backend
Feature: View prescriptions backend
  A user can view information about their prescriptions after logging in

  Scenario Outline: <GP System> patient requesting prescriptions with correct data returns a list of prescriptions when a patient had repeat prescriptions in the last 6 months (Date 6 months ago provided)
    Given I have logged into <GP System> and have a valid session cookie
    And From date is 6 months ago and I have 10 prescriptions in the last 6 months
    When I request the users prescriptions with a valid cookie
    Then I receive a list of 10 prescriptions
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: <GP System> patient with repeat prescriptions in the last 6 months and no fromDate
    Given I have logged into <GP System> and have a valid session cookie
    And From date is 6 months ago and I have 10 prescriptions in the last 6 months
    But I do not request a fromDate
    When I request prescriptions for the last 6 months
    Then I get a response with a list of prescriptions for the last 6 months
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: <GP System> patient requesting prescriptions with a fromDate in the future
    Given I have logged into <GP System> and have a valid session cookie
    But a fromDate in the future
    When I request prescriptions for the last 6 months
    Then I get a response with a list of prescriptions for the last 6 months
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: <GP System> patient requesting prescriptions with a fromDate greater than 6 months ago
    Given I have logged into <GP System> and have a valid session cookie
    But a fromDate greater than 6 months ago
    When I request prescriptions for the last 6 months
    Then I get a response with a list of prescriptions for the last 6 months
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |
    
  Scenario Outline: <GP System> patient requesting prescriptions with a fromDate not in the expected format
    Given I have logged into <GP System> and have a valid session cookie
    But a fromDate in an unexpected format
    When I request prescriptions for the last 6 months
    Then I receive a "Bad request" error with service desk reference prefixed "5a"
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario: Requesting prescriptions with a missing cookie
    # Without logging in
    When I request prescriptions for the last 6 months
    Then I receive a "Unauthorized" error

  Scenario: Patient requesting prescriptions with a NHSO-Session-Id not in the expected format
    Given I have logged into EMIS and have a valid session cookie
    When I request prescriptions for the last 6 months with an invalid cookie
    Then I receive a "Unauthorized" error

  @long-running
  Scenario Outline: <GP System> patient requesting prescriptions with when their session has expired
    Given I have logged into <GP System> and have a valid session cookie
    But I allow my session to expire
    When I request prescriptions for the last 6 months
    Then I receive an "Unauthorized" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: <GP System> GP practice has disabled prescriptions functionality
    Given I have logged into <GP System> and have a valid session cookie
    And the GP System has disabled prescriptions
    When I request prescriptions for the last 6 months
    Then I receive a "Forbidden" error with service desk reference prefixed "5c"
    Examples:
      | GP System |
      | EMIS      |
      | MICROTEST |

  Scenario Outline: <GP System> GP system fails to return in a timely fashion
    Given I have logged into <GP System> and have a valid session cookie
    But the GP System is too slow
    When I request prescriptions for the last 6 months
    Then I receive a "Gateway Timeout" error with service desk reference prefixed "<Prefix>"
    Examples:
      | GP System | Prefix |
      | EMIS      | ze     |
      | TPP       | zt     |
      | VISION    | zs     |
      | MICROTEST | zm     |

  Scenario Outline: <GP System> patient who has prescriptions totalling more than one hundred courses
    Given I have logged into <GP System> and have a valid session cookie
    And I have 110 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    When I request the users prescriptions with a valid cookie
    Then I receive a list of 100 prescriptions
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: <GP System> patient who has multiple prescription each containing one course
    Given I have logged into <GP System> and have a valid session cookie
    And I have 3 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    When I request the users prescriptions with a valid cookie
    Then I receive a list of 3 prescriptions
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: <GP System> patient who has multiple prescription each containing the same repeat prescription
    Given I have logged into <GP System> and have a valid session cookie
    And I have 3 past repeat prescriptions
    And each repeat prescription shares the same course
    When I request the users prescriptions with a valid cookie
    Then I receive a list of 3 prescriptions
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: <GP System> patient who has only one prescription containing multiple courses
    Given I have logged into <GP System> and have a valid session cookie
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 3 courses of which 3 are repeats
    When I request the users prescriptions with a valid cookie
    Then I receive a list of 3 prescriptions
    Examples:
      | GP System |
      | TPP       |
      | VISION    |

  Scenario Outline: <GP System> user with historic prescriptions with missing quantity info
    Given I have logged into <GP System> and have a valid session cookie
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And each course has only dosage info
    When I request the users prescriptions with a valid cookie
    Then I receive a list of 1 prescriptions
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: <GP System> user with historic prescriptions with missing dosage info
    Given I have logged into <GP System> and have a valid session cookie
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And each course has only quantity info
    When I request the users prescriptions with a valid cookie
    Then I receive a list of 1 prescriptions
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: <GP System> patient with no past repeat prescriptions
    Given I have logged into <GP System> and have a valid session cookie
    And I have 0 past repeat prescriptions
    And each repeat prescription contains 0 courses of which 0 are repeats
    When I request the users prescriptions with a valid cookie
    Then I receive a list of 0 prescriptions
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: <GP System> patient but NHSO-Patient-Id header is not valid
    Given I have logged into <GP System> and have a valid session cookie
    But the patient id sent in the request is not valid
    And I have 3 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    When I request the users prescriptions with a valid cookie
    Then I receive a "patient id not found" error
    Examples:
      | GP System |
      | EMIS      |

  Scenario: A user with proof level 5 receives an 'Unauthorized' error when they attempt to access prescriptions
    Given I am a user with proof level 5 and have a valid session cookie
    When I request the users prescriptions with a valid cookie
    Then I receive an "Unauthorized" error
