@linkage
@backend
Feature: Linkage Get Key
  As CID I want to be able to check if a linkage key exists for a user
  or request a linkage key is created for a user

  Scenario: Linkage request GET for EMIS returns success with LinkageResponse, first time retrieving
    Given I have valid EMIS linkage details and it's the first time a linkage key has been retrieved for an identity token
    When I call the EMIS Linkage GET endpoint
    Then I receive a valid linkage response

  Scenario: Linkage request GET for EMIS returns success with LinkageResponse, not the first time retrieving
    Given I have valid EMIS linkage details and it's not the first time a linkage key has been retrieved for an identity token
    When I call the EMIS Linkage GET endpoint
    Then I receive a valid linkage response

  Scenario: Linkage request GET with correct values for TPP returns 404 Not Found
    Given I have valid TPP linkage details
    When I call the TPP Linkage GET endpoint
    Then I receive a "Not Found" error

  Scenario Outline: Linkage request GET for <GP System> returns 400 Bad Request, empty OdsCode
    Given I have valid <GP System> linkage details apart from an empty OdsCode
    When I call the <GP System> Linkage GET endpoint
    Then I receive a "Bad Request" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario Outline: Linkage request GET for <GP System> returns 400 Bad Request, empty NhsNumber
    Given I have valid <GP System> linkage details apart from an empty NhsNumber
    When I call the <GP System> Linkage GET endpoint
    Then I receive a "Bad Request" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario Outline: Linkage request GET for <GP System> returns 501 Not Implemented, not found ods code
    Given I have valid <GP System> linkage details apart from a not found OdsCode
    When I call the <GP System> Linkage GET endpoint
    Then I receive a "Not Implemented" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario: Linkage request GET for EMIS returns 400 Bad Request, empty identity token
    Given I have valid EMIS linkage details apart from an empty identity token
    When I call the EMIS Linkage GET endpoint
    Then I receive a "Bad Request" error

  Scenario: Linkage request GET for EMIS returns 400 Bad Request, practice not live
    Given I have valid EMIS linkage details but the practice is not live
    When I call the EMIS Linkage GET endpoint
    Then I receive a "Bad Request" error

  Scenario: Linkage request GET for EMIS returns 400 Bad Request, marked as archived
    Given I have valid EMIS linkage details but the GP system has marked me as archived
    When I call the EMIS Linkage GET endpoint
    Then I receive a "Bad Request" error

  Scenario: Linkage request GET for EMIS returns 400 Bad Request, under 16
    Given I have valid EMIS linkage details but I am under 16
    When I call the EMIS Linkage GET endpoint
    Then I receive a "Bad Request" error

  Scenario: Linkage request GET for EMIS returns 400 Bad Request, account status invalid
    Given I have valid EMIS linkage details but my account status is invalid
    When I call the EMIS Linkage GET endpoint
    Then I receive a "Bad Request" error

  Scenario: Linkage request GET for EMIS returns 404 Not Found, patient not registered at practice
    Given I have valid EMIS linkage details but I'm not registered at the practice
    When I call the EMIS Linkage GET endpoint
    Then I receive a "Not Found" error

  Scenario: Linkage request GET for EMIS returns 404 Not Found, patient not found at practice
    Given I have valid EMIS linkage details but I am not found on the GP system
    When I call the EMIS Linkage GET endpoint
    Then I receive a "Not Found" error

  Scenario: Linkage request GET for EMIS returns 502, when GP system responds with 500
    Given I have valid EMIS linkage details but the GP system responds with an internal server error retrieving the linkage key
    When I call the EMIS Linkage GET endpoint
    Then I receive a "Bad Gateway" error
