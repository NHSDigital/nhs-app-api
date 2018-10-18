@linkage
@backend
Feature: Linkage Key

  As CID I want to be able to check if a linkage key exists for a user
  or
  Request a linkage key is created for a user

  # GET
  Scenario Outline: Linkage request GET for <GP System> returns success with LinkageResponse, first time retrieving
    Given I have valid <GP System> linkage details
    And It's the first time a linkage key has been retrieved for an identity token
    When I call the <GP System> Linkage GET endpoint
    Then I receive a valid response
    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: Linkage request GET for <GP System> returns success with LinkageResponse, not the first time retrieving
    Given I have valid <GP System> linkage details
    And It's not the first time a linkage key has been retrieved for an identity token
    When I call the <GP System> Linkage GET endpoint
    Then I receive a valid response
    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: Linkage request GET for <GP System> returns 400 Bad Request, empty OdsCode
    Given I have valid <GP System> linkage details apart from an empty OdsCode
    When I call the <GP System> Linkage GET endpoint
    Then I receive a "Bad Request" error
    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: Linkage request GET for <GP System> returns 400 Bad Request, empty NhsNumber
    Given I have valid <GP System> linkage details apart from an empty NhsNumber
    When I call the <GP System> Linkage GET endpoint
    Then I receive a "Bad Request" error
    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: Linkage request GET for <GP System> returns 400 Bad Request, empty identity token
    Given I have valid <GP System> linkage details apart from an empty identity token
    When I call the <GP System> Linkage GET endpoint
    Then I receive a "Bad Request" error
    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: Linkage request GET for <GP System> returns 501 Not Implemented, not found ods code
    Given I have valid <GP System> linkage details apart from a not found OdsCode
    When I call the <GP System> Linkage GET endpoint
    Then I receive a "Not Implemented" error
    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: Linkage request GET for <GP System> returns 400 Bad Request, practice not live
    Given I have valid <GP System> linkage details
    But The practice is not live
    When I call the <GP System> Linkage GET endpoint
    Then I receive a "Bad Request" error
    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: Linkage request GET for <GP System> returns 400 Bad Request, marked as archived
    Given I have valid <GP System> linkage details
    But The GP system has marked me as archived
    When I call the <GP System> Linkage GET endpoint
    Then I receive a "Bad Request" error
    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: Linkage request GET for <GP System> returns 400 Bad Request, under 16
    Given I have valid <GP System> linkage details
    But I am under 16
    When I call the <GP System> Linkage GET endpoint
    Then I receive a "Bad Request" error
    Examples:
      | GP System |
      | EMIS      |


  Scenario Outline: Linkage request GET for <GP System> returns 400 Bad Request, account status invalid
    Given I have valid <GP System> linkage details
    But My account status is invalid
    When I call the <GP System> Linkage GET endpoint
    Then I receive a "Bad Request" error
    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: Linkage request GET for <GP System> returns 404 Not Found, patient not registered at practice
    Given I have valid <GP System> linkage details
    But I'm not registered at the practice
    When I call the <GP System> Linkage GET endpoint
    Then I receive a "Not Found" error
    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: Linkage request GET for <GP System> returns 404 Not Found, patient not found at practice
    Given I have valid <GP System> linkage details
    But I am not found on the GP system
    When I call the <GP System> Linkage GET endpoint
    Then I receive a "Not Found" error
    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: Linkage request GET for <GP System> returns 502, when GP system responds with 500
    Given I have valid <GP System> linkage details
    But The GP system responds with an internal server error retrieving the linkage key
    When I call the <GP System> Linkage GET endpoint
    Then I receive a "Bad Gateway" error
    Examples:
      | GP System |
      | EMIS      |


  # POST
  Scenario Outline: Linkage request POST for <GP System> returns success with LinkageResponse
    Given I have valid <GP System> linkage details
    And It's the first time a linkage key has been created for my nhs number
    When I call the <GP System> Linkage POST endpoint
    Then I receive a valid response
    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: Linkage request POST for <GP System> returns 400 Bad Request, invalid OdsCode
    Given I have valid <GP System> linkage details apart from a not found OdsCode
    When I call the <GP System> Linkage POST endpoint
    Then I receive a "Not Implemented" error
    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: Linkage request POST for <GP System> returns 400 Bad Request, empty NhsNumber
    Given I have valid <GP System> linkage details apart from an empty NhsNumber
    When I call the <GP System> Linkage POST endpoint
    Then I receive a "Bad Request" error
    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: Linkage request POST for <GP System> returns 400 Bad Request, empty identity token
    Given I have valid <GP System> linkage details apart from an empty identity token
    When I call the <GP System> Linkage POST endpoint
    Then I receive a "Bad Request" error
    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: Linkage request POST for <GP System> returns 400 Bad Request, empty email address
    Given I have valid <GP System> linkage details apart from an empty email address
    When I call the <GP System> Linkage POST endpoint
    Then I receive a "Bad Request" error
    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: Linkage request POST for <GP System> returns 404 Not Found, not registered with practice
    Given I have valid <GP System> linkage details
    But I'm not registered at the practice
    When I call the <GP System> Linkage POST endpoint
    Then I receive a "Not Found" error
    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: Linkage request POST for <GP System> returns 409 Conflict when user already has an online account
    Given I have valid <GP System> linkage details
    But I already have an online account
    When I call the <GP System> Linkage POST endpoint
    Then I receive a "Conflict" error
    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: Linkage request POST for <GP System> returns bad request when practice not live
    Given I have valid <GP System> linkage details
    But The practice is not live
    When I call the <GP System> Linkage POST endpoint
    Then I receive a "Bad Request" error
    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: Linkage request POST for <GP System> returns bad request when patient marked as archived
    Given I have valid <GP System> linkage details
    But The GP system has marked me as archived
    When I call the <GP System> Linkage POST endpoint
    Then I receive a "Bad Request" error
    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: Linkage request POST for <GP System> returns bad request when patient is under 16
    Given I have valid <GP System> linkage details
    But I am under 16
    When I call the <GP System> Linkage POST endpoint
    Then I receive a "Bad Request" error
    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: Linkage request POST for <GP System> returns 502, when GP system responds with 500
    Given I have valid <GP System> linkage details
    But The GP system responds with an internal server error creating the linkage key
    When I call the <GP System> Linkage POST endpoint
    Then I receive a "Bad Gateway" error
    Examples:
      | GP System |
      | EMIS      |
