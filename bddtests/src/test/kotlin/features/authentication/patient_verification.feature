Feature: Patient Verification

  The system validates the patient data

  @backend
  Scenario Outline: <GP System> patient has single NHS Number
    Given I have valid credentials for a <GP System> patient with one NHS Number
    When I verify patient data
    Then I receive the expected NHS Number

    Examples:
    |GP System |
    |EMIS |
    |TPP  |

  @backend
  Scenario Outline: <GP System> patient has multiple NHS Numbers
    Given I have valid credentials for a <GP System> patient with multiple NHS Numbers
    When I verify patient data
    Then I receive the expected NHS Numbers

    Examples:
    |GP System |
    |EMIS |

  @backend
  Scenario Outline: <GP System> patient has no NHS Number
    Given I have valid credentials for a <GP System> patient with no NHS Number
    When I verify patient data
    Then I receive no NHS Number

    Examples:
    |GP System |
    |EMIS |

  @backend
  Scenario Outline: Non-existent IM1 Connection Token for the <GP System>
    Given I have an <GP System> IM1 Connection Token that does not exist
    When I verify patient data
    Then I receive a "Bad Gateway" error

    Examples:
    |GP System |
    |EMIS |
    |TPP  |

  @backend
  Scenario Outline: <GP System> IM1 Connection Token not in the expected format
    Given I have an <GP System> IM1 Connection Token that is in an invalid format
    When I verify patient data
    Then I receive a "Bad Request" error

    Examples:
    |GP System |
    |EMIS |
    |TPP  |

  @backend
  Scenario Outline: No IM1 Connection Token for the <GP System>
    Given I have no IM1 Connection Token for <GP System>
    When I verify patient data
    Then I receive a "Bad Request" error

    Examples:
    |GP System |
    |EMIS |
    |TPP  |

  @backend
  Scenario Outline: Non-existent ODS Code for <GP System>
    Given I have an <GP System> ODS Code that does not exists
    When I verify patient data
    Then I receive a "Not Found" error

  Examples:
    |GP System |
    |EMIS |
    |TPP  |

  @backend
  Scenario Outline: ODS Code not in the expected format <GP System>
    Given I have an <GP System> ODS Code not in expected format
    When I verify patient data
    Then I receive a "Bad Request" error
  
  Examples:
    |GP System |
    |EMIS |
    |TPP  |

  @backend
  Scenario Outline: No ODS Code for <GP System>
    Given I have no <GP System> ODS Code
    When I verify patient data
    Then I receive a "Bad Request" error

  Examples:
  |GP System |
  |EMIS |
  |TPP  |

  @backend
  Scenario Outline: <GP System> is unavailable
    Given EMIS is unavailable
    When I verify patient data
    Then I receive an "Bad Gateway" error

  Examples:
  |GP System|
  |EMIS |
  |TPP  |
