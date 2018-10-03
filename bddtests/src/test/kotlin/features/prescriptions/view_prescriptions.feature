@prescription
Feature: View prescriptions
  A user can view information about their prescriptions after logging in

  Scenario Outline: A <GP System> user can see the prescriptions menu button
    Given a patient from <GP System> is defined
    And I am using <GP System> GP System
    And I am logged in
    Then I see the prescriptions menu button

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  
  Scenario Outline: <GP System> patient selects the prescriptions menu button
    Given a patient from <GP System> is defined
    And I am using <GP System> GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    When I am on the prescriptions page
    Then I see prescriptions page loaded
    And the prescriptions menu button is highlighted

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: <GP System> patient with no past repeat prescriptions
    Given a patient from <GP System> is defined
    And I am using <GP System> GP System
    And I have 0 past repeat prescriptions
    And each repeat prescription contains 0 courses of which 0 are repeats
    When I am on the prescriptions page
    Then I see no prescriptions
    And I see a message indicating that I have no repeat prescriptions

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: <GP System> patient who has prescriptions totalling more than one hundred courses
    Given a patient from <GP System> is defined
    And I am using <GP System> GP System
    And I have 110 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    When I am on the prescriptions page
    Then I see 100 prescriptions

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  @smoketest
  Scenario Outline: <GP System> patient who has multiple prescription each containing one course
    Given a patient from <GP System> is defined
    And I am using <GP System> GP System
    And I have 3 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    When I am on the prescriptions page
    Then I see 3 prescriptions

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |


  Scenario Outline: <GP System> patient who has multiple prescription each containing the same repeat prescription
    Given a patient from <GP System> is defined
    And I am using <GP System> GP System
    And I have 3 past repeat prescriptions
    And each repeat prescription shares the same course
    When I am on the prescriptions page
    Then I see 3 prescriptions

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: <GP System> patient who has only one prescription containing multiple courses
    Given a patient from <GP System> is defined
    And I am using <GP System> GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 3 courses of which 3 are repeats
    When I am on the prescriptions page
    Then I see 3 prescriptions

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |


  Scenario: EMIS patient who has acute prescriptions
    Given a patient from EMIS is defined
    And I am using EMIS GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 3 courses of which 2 are repeats
    When I am on the prescriptions page
    Then I see 2 prescriptions

  Scenario Outline: The <GP System> User clicks on the Prescriptions button and the service is disabled at a GP Practice level
    Given a patient from <GP System> is defined
    And I am using <GP System> GP System
    Given prescriptions is disabled at a GP Practice level
    When I am on the prescriptions page
    Then I see a message informing me that I don't currently have access to this service

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
    
  Scenario Outline: A <GP System> user with historic prescriptions with missing quantity info
    Given a patient from <GP System> is defined
    And I am using <GP System> GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And each course has only dosage info
    When I am on the prescriptions page
    Then I see 1 prescriptions

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: A <GP System> user with historic prescriptions with missing dosage info
    Given a patient from <GP System> is defined
    And I am using <GP System> GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And each course has only quantity info
    When I am on the prescriptions page
    Then I see 1 prescriptions

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: <GP System> user with historic prescriptions with missing dosage and quantity info
    Given a patient from <GP System> is defined
    And I am using <GP System> GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And each course has no info
    When I am on the prescriptions page
    Then I see 1 prescriptions

    Examples:
      | GP System |
      | VISION    |


  Scenario: A user who has multiple prescriptions but medication status should not be displayed
    Given a patient from EMIS is defined
    And I am using EMIS GP System
    And I have 6 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And courses have status
      | Issued              |
      | Requested           |
      | ForwardedForSigning |
      | Rejected            |
      | Unknown             |
      | Cancelled           |
    When I am on the prescriptions page
    Then I see 4 prescriptions

  @backend
  Scenario Outline: <GP System> patient requesting prescriptions with correct data returns a list of prescriptions when a patient had repeat prescriptions in the last 6 months (Date 6 months ago provided)
    Given I have logged into <GP System> and have a valid session cookie
    And From date is 6 months ago and I have 10 prescriptions in the last 6 months
    When I get the users prescriptions with a valid cookie
    Then I receive a list of 10 prescriptions

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  @backend
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

  @backend
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

  @backend
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

  @backend
  Scenario Outline: <GP System> patient requesting prescriptions with a fromDate not in the expected format
    Given I have logged into <GP System> and have a valid session cookie
    But a fromDate in an unexpected format
    When I request prescriptions for the last 6 months
    Then I receive a "Bad request" error

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  @backend
  Scenario: Requesting prescriptions with a missing cookie
    # Without logging in
    When I request prescriptions for the last 6 months
    Then I receive a "Unauthorized" error

  @backend
  Scenario: Patient requesting prescriptions with a NHSO-Session-Id not in the expected format
    Given I have logged into EMIS and have a valid session cookie
    When I request prescriptions for the last 6 months with an invalid cookie
    Then I receive a "Unauthorized" error

  @backend
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

  @backend
  Scenario Outline: <GP System> GP practice has disabled prescriptions functionality
    Given I have logged into <GP System> and have a valid session cookie
    And the GP System has disabled prescriptions
    When I request prescriptions for the last 6 months
    Then I receive a "Forbidden" error

    Examples:
      | GP System |
      | EMIS      |

  @backend
  Scenario Outline: <GP System> GP system fails to return in a timely fashion
    Given I have logged into <GP System> and have a valid session cookie
    But the GP System is too slow
    When I request prescriptions for the last 6 months
    Then I receive a "Gateway Timeout" error

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
