@linkage
@backend
Feature: Linkage Post Key
  As CID I want to be able to check if a linkage key exists for a user
  or request a linkage key is created for a user

  Scenario: Linkage request POST for EMIS returns success with LinkageResponse
    Given I have valid EMIS linkage details and it's the first time a linkage key has been created for my nhs number
    When I call the EMIS Linkage POST endpoint
    Then I receive a valid linkage response

  Scenario: Linkage request POST for TPP returns success with LinkageResponse
    Given I have valid TPP linkage details
    When I call the TPP Linkage POST endpoint
    Then I receive a valid linkage response

  Scenario Outline: Linkage request POST for <GP System> returns 400 Bad Request, invalid OdsCode
    Given I have valid <GP System> linkage details apart from a not found OdsCode
    When I call the <GP System> Linkage POST endpoint
    Then I receive a "Not Implemented" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario Outline: Linkage request POST for <GP System> returns 400 Bad Request, empty NhsNumber
    Given I have valid <GP System> linkage details apart from an empty NhsNumber
    When I call the <GP System> Linkage POST endpoint
    Then I receive a "Bad Request" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario: Linkage request POST for EMIS returns 400 Bad Request, empty identity token
    Given I have valid EMIS linkage details apart from an empty identity token
    When I call the EMIS Linkage POST endpoint
    Then I receive a "Bad Request" error

  Scenario: Linkage request POST for EMIS returns 400 Bad Request, empty email address
    Given I have valid EMIS linkage details apart from an empty email address
    When I call the EMIS Linkage POST endpoint
    Then I receive a "Bad Request" error

  Scenario: Linkage request POST for TPP returns 400 Bad Request, empty surname
    Given I have valid TPP linkage details apart from an empty surname
    When I call the TPP Linkage POST endpoint
    Then I receive a "Bad Request" error

  Scenario: Linkage request POST for TPP returns 400 Bad Request, empty date of birth
    Given I have valid TPP linkage details apart from an empty date of birth
    When I call the TPP Linkage POST endpoint
    Then I receive a "Bad Request" error

  Scenario: Linkage request POST for EMIS returns 404 Not Found, not registered with practice
    Given I have valid EMIS linkage details but I'm not registered at the practice
    When I call the EMIS Linkage POST endpoint
    Then I receive a "Not Found" error

  Scenario: Linkage request POST for EMIS returns 409 Conflict when user already has an online account
    Given I have valid EMIS linkage details but I already have an online account
    When I call the EMIS Linkage POST endpoint
    Then I receive a "Conflict" error

  Scenario: Linkage request POST for EMIS returns bad request when practice not live
    Given I have valid EMIS linkage details but the practice is not live
    When I call the EMIS Linkage POST endpoint
    Then I receive a "Bad Request" error

  Scenario: Linkage request POST for EMIS returns 403 Forbidden when patient marked as archived
    Given I have valid EMIS linkage details but the GP system has marked me as archived
    When I call the EMIS Linkage POST endpoint
    Then I receive a "Forbidden" error

  Scenario Outline: Linkage request POST for <GP System> returns 403 Forbidden when patient is under 13
    Given I have valid <GP System> linkage details but I am under 13
    When I call the <GP System> Linkage POST endpoint
    Then I receive a "Forbidden" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario Outline: Linkage request POST for <GP System> returns 502, when GP system responds with 500
    Given I have valid <GP System> linkage details but the GP system responds with an internal server error creating the linkage key
    When I call the <GP System> Linkage POST endpoint
    Then I receive a "Bad Gateway" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
