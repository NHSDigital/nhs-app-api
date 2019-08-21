@linkage
@backend
Feature: Linkage Post Key
  As CID I want to be able to check if a linkage key exists for a user
  or request a linkage key is created for a user

  Scenario: Linkage request POST for EMIS returns success with LinkageResponse and an Im1 Connection Token is cached
    Given I have valid EMIS linkage details and it's the first time a linkage key has been created for my nhs number
    And no IM1 Connection Token is currently cached
    When I call the Linkage POST endpoint
    Then I receive a "Created" success code
    And I receive a valid linkage response
    And the IM1 Connection Token is in the cache

  Scenario: Linkage request POST after delayed response from EMIS and an Im1 Connection Token is cached
    Given I have valid EMIS linkage details and it's the first time a linkage key has been created for my nhs number
    And no IM1 Connection Token is currently cached
    When I call the Linkage POST endpoint which responds after 11 seconds
    Then I receive a "Created" success code
    And I receive a valid linkage response
    And the IM1 Connection Token is in the cache

  Scenario: Linkage request EMIS POST connection closes and an Im1 Connection Token is cached
    Given I have valid EMIS linkage details and it's the first time a linkage key has been created for my nhs number
    And no IM1 Connection Token is currently cached
    When I call the Linkage POST endpoint CID connection times out
    And Wait for the request to complete
    Then the IM1 Connection Token is in the cache

  Scenario: Linkage request POST for TPP returns success with LinkageResponse and an Im1 Connection Token is cached
    Given I have valid TPP linkage details for posting
    And no IM1 Connection Token is currently cached
    When I call the Linkage POST endpoint
    Then I receive a "Created" success code
    And I receive a valid linkage response
    And the IM1 Connection Token is in the cache

  Scenario Outline: Linkage request POST for <GP System> returns success with LinkageResponse text
    Given I have valid <GP System> linkage details for posting
    When I call the Linkage POST endpoint
    Then I receive a "Created" success code
    And I receive a valid linkage response
    Examples:
      | GP System |
      | TPP       |
      | VISION    |

  Scenario: Linkage request POST for Microtest returns not found
    Given I have valid MICROTEST linkage details for posting
    When I call the Linkage POST endpoint
    Then I receive a "Not Found" error

  Scenario Outline: Linkage request POST for <GP System> returns 400 Bad Request, invalid OdsCode
    Given I have valid <GP System> linkage details apart from a not found OdsCode
    When I call the Linkage POST endpoint
    Then I receive a "Not Implemented" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: Linkage request POST for <GP System> returns 400 Bad Request, empty NhsNumber
    Given I have valid <GP System> linkage details apart from an empty NhsNumber
    When I call the Linkage POST endpoint
    Then I receive a "Bad Request" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario: Linkage request POST for EMIS returns 400 Bad Request, empty identity token
    Given I have valid EMIS linkage details apart from an empty identity token
    When I call the Linkage POST endpoint
    Then I receive a "Bad Request" error

  Scenario: Linkage request POST for EMIS returns 400 Bad Request, empty email address
    Given I have valid EMIS linkage details apart from an empty email address
    When I call the Linkage POST endpoint
    Then I receive a "Bad Request" error

  Scenario: Linkage request POST for TPP returns 400 Bad Request, empty surname
    Given I have valid TPP linkage details apart from an empty surname
    When I call the Linkage POST endpoint
    Then I receive a "Bad Request" error

  Scenario: Linkage request POST for TPP returns 400 Bad Request, empty date of birth
    Given I have valid TPP linkage details apart from an empty date of birth
    When I call the Linkage POST endpoint
    Then I receive a "Bad Request" error

  Scenario: Linkage request POST for EMIS returns 404 Not Found, not registered with practice
    Given I have valid EMIS linkage details but I'm not registered at the practice
    When I call the Linkage POST endpoint
    Then I receive a "Not Found" error

  Scenario: Linkage request POST for EMIS returns 409 Conflict when user already has an online account
    Given I have valid EMIS linkage details but I already have an online account
    When I call the Linkage POST endpoint
    Then I receive a "Conflict" error

  Scenario: Linkage request POST for EMIS returns bad request when practice not live
    Given I have valid EMIS linkage details but the practice is not live
    When I call the Linkage POST endpoint
    Then I receive a "Bad Request" error

  Scenario: Linkage request POST for EMIS returns 403 Forbidden when patient marked as archived
    Given I have valid EMIS linkage details but the GP system has marked me as archived
    When I call the Linkage POST endpoint
    Then I receive a "Forbidden" error

  Scenario Outline: Linkage request POST for <GP System> returns 403 Forbidden when patient is under 16
    Given I have valid <GP System> linkage details for POST but I am under 16
    When I call the Linkage POST endpoint
    Then I receive a "Forbidden" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: Linkage request POST for <GP System> returns 201 Created when patient is at least 16
    Given I have valid <GP System> linkage details and try to create a linkage key as 16 years old
    When I call the Linkage POST endpoint
    Then I receive a "Created" success code
    And I receive a valid linkage response
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario: Linkage request POST for Vision returns 400 Bad Request, invalid nhs number
    Given I have valid VISION linkage details but my nhs number is invalid
    When I call the Linkage POST endpoint
    Then I receive a "Bad Request" error

  Scenario: Linkage request POST for Vision returns 404 Not Found, patient record not found
    Given I have valid VISION linkage details but my patient record was not found
    When I call the Linkage POST endpoint
    Then I receive a "Not Found" error

  Scenario: Linkage request POST for Vision returns 409 Conflict, linkage key already exists
    Given I have valid VISION linkage details but a linkage key already exists
    When I call the Linkage POST endpoint
    Then I receive a "Conflict" error

  Scenario Outline: Linkage request POST for <GP System> returns 502, when GP system responds with 500
    Given I have valid <GP System> linkage details but the GP system responds with an internal server error creating the linkage key
    When I call the Linkage POST endpoint
    Then I receive a "Bad Gateway" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
