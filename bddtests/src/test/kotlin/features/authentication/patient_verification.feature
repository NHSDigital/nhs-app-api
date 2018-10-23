@authentication
@backend
Feature: Patient Verification
  The system validates the patient data

  
  Scenario Outline: <GP System> patient has single NHS Number
    Given I have valid credentials for a <GP System> patient with one NHS Number
    When I verify patient data
    Then I receive the expected NHS Number

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: <GP System> patient has multiple NHS Numbers
    Given I have valid credentials for a <GP System> patient with multiple NHS Numbers
    When I verify patient data
    Then I receive the expected NHS Numbers

    Examples:
      | GP System |
      | EMIS      |
      | VISION    |

  Scenario Outline: <GP System> patient has no NHS Number
    Given I have valid credentials for a <GP System> patient with no NHS Number
    When I verify patient data
    Then I receive no NHS Number

    Examples:
      | GP System |
      | EMIS      |
      | VISION    |

  
  Scenario Outline: Non-existent IM1 Connection Token for the <GP System>
    Given I have an <GP System> IM1 Connection Token that does not exist
    When I verify patient data
    Then I receive a "Bad Gateway" error

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: <GP System> IM1 Connection Token not in the expected format
    Given I have an <GP System> IM1 Connection Token that is in an invalid format
    When I verify patient data
    Then I receive a "Bad Request" error

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: No IM1 Connection Token for the <GP System>
    Given I have no IM1 Connection Token for <GP System>
    When I verify patient data
    Then I receive a "Bad Request" error

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: Non-existent ODS Code for <GP System>
    Given I have an <GP System> ODS Code that does not exists
    When I verify patient data
    Then I receive a "Not Implemented" error

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: ODS Code not in the expected format <GP System>
    Given I have an <GP System> ODS Code not in expected format
    When I verify patient data
    Then I receive a "Bad Request" error

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: No ODS Code for <GP System>
    Given I have no <GP System> ODS Code
    When I verify patient data
    Then I receive a "Bad Request" error

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: <GP System> is not available
    Given <GP System> is not available
    When I verify patient data
    Then I receive an "Bad Gateway" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  
  @NHSO-2522
  Scenario: Vision responds with security header error
    Given Vision responds with a security header error
    When I verify patient data
    Then I receive an "Internal server error" error

  
  @NHSO-2522
  Scenario: Vision responds with invalid request error
    Given Vision responds with an invalid request error
    When I verify patient data
    Then I receive an "Bad Request" error

  
  @NHSO-2522
  Scenario: Vision responds with an unknown error
    Given Vision responds with an unknown error
    When I verify patient data
    Then I receive an "Bad Gateway" error