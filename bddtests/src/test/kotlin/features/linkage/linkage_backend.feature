Feature: Linkage Key

  As CID I want to be able to check if a linkage key exists for a user
  or
  Request a linkage key is created for a user

  @backend
  Scenario Outline: Linkage request GET for <GP System> returns success with LinkageResponse
    Given I have a valid <GP System> OdsCode
    And I have a valid NhsNumber
    When I call the Linkage GET endpoint
    Then I receive a valid response
    Examples:
      | GP System |
      | EMIS      |

  @backend
  Scenario Outline: Linkage request GET for <GP System> returns 400 Bad Request, invalid OdsCode
    Given I have an invalid <GP System> OdsCode
    And I have a valid NhsNumber
    When I call the Linkage GET endpoint
    Then I receive a "Bad Request" error
    Examples:
      | GP System |
      | EMIS      |

  @backend
  Scenario Outline: Linkage request GET for <GP System> returns 400 Bad Request, invalid NhsNumber
    Given I have a valid <GP System> OdsCode
    And I have an invalid NhsNumber
    When I call the Linkage GET endpoint
    Then I receive a "Bad Request" error
    Examples:
      | GP System |
      | EMIS      |

  @backend
  Scenario Outline: Linkage request GET for <GP System> returns 501 Not Implemented
    Given I have a not found <GP System> OdsCode
    And I have a valid NhsNumber
    When I call the Linkage GET endpoint
    Then I receive a "Not Implemented" error
    Examples:
      | GP System |
      | EMIS      |

  @backend
  Scenario Outline: Linkage request POST for <GP System> returns success with LinkageResponse
    Given I have a valid <GP System> OdsCode
    And I have a valid NhsNumber
    When I call the Linkage POST endpoint
    Then I receive a valid response
    Examples:
      | GP System |
      | EMIS      |

  @backend
  Scenario Outline: Linkage request POST for <GP System> returns 400 Bad Request, invalid OdsCode
    Given I have an invalid <GP System> OdsCode
    And I have a valid NhsNumber
    When I call the Linkage POST endpoint
    Then I receive a "Bad Request" error
    Examples:
      | GP System |
      | EMIS      |

  @backend
  Scenario Outline: Linkage request POST for <GP System> returns 400 Bad Request, invalid NhsNumber
    Given I have a valid <GP System> OdsCode
    And I have an invalid NhsNumber
    When I call the Linkage GET endpoint
    Then I receive a "Bad Request" error
    Examples:
      | GP System |
      | EMIS      |

  @backend
  Scenario Outline: Linkage request POST for <GP System> returns 404 Not Found, NhsNumber not found
    Given I have a valid <GP System> OdsCode
    And I have a not found NhsNumber
    When I call the Linkage POST endpoint
    Then I receive a "Not Found" error
    Examples:
      | GP System |
      | EMIS      |

  @backend
  Scenario Outline: Linkage request POST for <GP System> returns 501 Not Implemented
    Given I have a not found <GP System> OdsCode
    And I have a valid NhsNumber
    When I call the Linkage POST endpoint
    Then I receive a "Not Implemented" error
    Examples:
      | GP System |
      | EMIS      |

  @backend
  Scenario Outline: Linkage request POST for <GP System> returns 409 Conflict
    Given I have a valid <GP System> OdsCode
    And I have a valid NhsNumber
    But A linkage key already exists for the user
    When I call the Linkage POST endpoint
    Then I receive a "Conflict" error
    Examples:
      | GP System |
      | EMIS      |

  @backend
  Scenario Outline: Linkage request GET for <GP System> returns 403 Forbidden
    Given I have a valid <GP System> OdsCode
    And I have a valid NhsNumber
    And My linkage key has been revoked
    When I call the Linkage GET endpoint
    Then I receive a "Forbidden" error
    Examples:
      | GP System |
      | EMIS      |
