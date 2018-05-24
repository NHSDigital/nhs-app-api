Feature: EMIS Session Registration

  The application verifies the user session

  @NHSO-63
  @backend
  @bug @NHSO-922
  Scenario: Creating user session returns a valid response with session guid token http-only cookie and given name and family name in body
    Given I have a valid authCode and codeVerifier for a patient
    When I create a user session with valid details
    Then I receive a response with given name and family name
    And a cookie containing a session guid with http-only

    # Combine these Scenarios once Tech Debt is resolved
  @NHSO-63
  @tech-debt @NHSO-725
  @backend
  @bug @NHSO-922
  Scenario: Creating user session returns a valid response with session guid token tls-only cookie and given name and family name in body
    Given I have a valid authCode and codeVerifier for a patient
    When I create a user session with valid details
    Then I receive a response with given name and family name
    And a cookie containing a session guid with tls-only

  @NHSO-63
  @backend
  Scenario: OAuth details are incomplete
    Given I have incomplete OAuth details
    When I create a user session with incomplete details
    Then I receive a "Bad Request" error

  @NHSO-63
  @backend
  Scenario: OAuth details are invalid
    Given I have invalid OAuth details
    When I create a user session with invalid details
    Then I receive a "Bad Request" error

  @NHSO-63
  @bug @NHSO-720 @NHSO-922
  @backend
  Scenario: CID tokens endpoint fails to process the request
    Given I have valid OAuth details and the CID tokens endpoint fails to process the request
    When I create a user session with valid details
    Then I receive a "Bad Gateway" error

  @NHSO-63
  @bug @NHSO-720 @NHSO-922
  @backend
  Scenario: CID user profile endpoint fails to process the request
    Given I have valid OAuth details and the CID user profile endpoint fails to process the request
    When I create a user session with valid details
    Then I receive a "Bad Gateway" error

  @NHSO-63
  @backend
  @bug @NHSO-922
  Scenario: EMIS end user session fails to create
    Given I have valid OAuth details and the EMIS end user session endpoint fails to create
    When I create a user session with valid details
    Then I receive a "Bad Gateway" error

  @NHSO-63
  @backend
  @bug @NHSO-922
  Scenario: EMIS session fails to create
    Given I have valid OAuth details and the EMIS session endpoint fails to create
    When I create a user session with valid details
    Then I receive a "Bad Gateway" error

  @NHSO-63
  @backend
  @bug @NHSO-922
  Scenario: EMIS is unavailable
    Given I have valid OAuth details and EMIS is unavailable
    When I create a user session with valid details
    Then I receive a "Bad Gateway" error

  @NHSO-63
  @backend
  @bug @NHSO-922
  Scenario: CID connection token fails to authenticate with EMIS
    Given I have invalid OAuth details and CID connection token fails to authenticate with emis
    When I create a user session with valid details
    Then I receive a "Forbidden" error

  @NHSO-63
  @backend
  @bug @NHSO-922
  Scenario: EMIS fails to respond in 30 seconds
    Given I have valid OAuth details and emis fails to respond in 30 seconds
    When I create a user session with valid details
    Then I receive a "Gateway Timeout" error

  @NHSO-63
  @manual @NHSO-449
  @backend
  Scenario: EMIS session fails to be saved in cache
    Given I have valid OAuth details and the EMIS session fails to be saved in cache
    When I create a user session with valid details
    Then I receive a "Server Error" error