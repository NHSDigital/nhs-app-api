Feature: Session Registration

  The application verifies the user session

  @NHSO-63
  @NHSO-415
  @backend
  Scenario: We check the session cookie and response body for http
    Given I have a valid authCode and codeVerifier
    When I create a user session
    Then I receive a response
    And the response has a name
    And the response has a session timeout
    And the cookie contains a session guid with http-only

  @NHSO-63
  @NHSO-415
  @tech-debt @NHSO-725
  @backend
  Scenario: We check the session cookie and response body for https
    Given I have a valid authCode and codeVerifier
    When I create a user session
    Then I receive a response
    And the response has a name
    And the response has a session timeout
    And the cookie contains a session guid with tls-only

  @NHSO-63
  @backend
  Scenario: OAuth details are incomplete
    Given I have incomplete OAuth details
    When I create a user session
    Then I get a "Bad Request" error

  @NHSO-63
  @backend
  Scenario: OAuth details are invalid
    Given I have invalid OAuth details
    When I create a user session
    Then I get a "Bad Request" error

  @NHSO-63
  @bug @NHSO-720
  @backend
  Scenario: CID tokens endpoint fails to process the request
    Given I have valid OAuth details and the CID tokens endpoint fails to process the request
    When I create a user session
    Then I get a "Bad Gateway" error

  @NHSO-63
  @bug @NHSO-720
  @backend
  Scenario: CID user profile endpoint fails to process the request
    Given I have valid OAuth details and the CID user profile endpoint fails to process the request
    When I create a user session
    Then I get a "Bad Gateway" error

  @NHSO-63
  @backend
  Scenario: EMIS end user session fails to create
    Given I have valid OAuth details and the EMIS end user session endpoint fails to create
    When I create a user session
    Then I get a "Bad Gateway" error

  @NHSO-63
  @backend
  Scenario: EMIS session fails to create
    Given I have valid OAuth details and the EMIS session endpoint fails to create
    When I create a user session
    Then I get a "Bad Gateway" error

  @NHSO-63
  @NHSO-992
  @backend
  Scenario Outline: <GP System> is unavailable
    Given I have valid OAuth details and <GP System> is not available
    When I create a user session
    Then I get a "Bad Gateway" error

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  @NHSO-63
  @NHSO-992
  @backend
  Scenario Outline: CID connection token fails to authenticate with <GP System>
    Given I have invalid OAuth details and CID connection token fails to authenticate with <GP System>
    When I create a user session
    Then I get a "Forbidden" error

    Examples:
      | GP System |
      | EMIS      |
      | VISION    |

  @NHSO-63
  @backend
  Scenario: CID connection token fails to authenticate with TPP
    Given I have invalid OAuth details and CID connection token fails to authenticate with TPP
    When I create a user session
    Then I get a "Bad Gateway" error

  @NHSO-63
  @NHSO-992
  @backend
  Scenario Outline: <GP System> fails to respond in 30 seconds
    Given I have valid OAuth details and <GP System> fails to respond in 30 seconds
    When I create a user session
    Then I get a "Gateway Timeout" error

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  @NHSO-63
  @manual @NHSO-449
  @NHSO-992
  @backend
  Scenario Outline: <GP System> session fails to be saved in cache
    Given I have valid OAuth details and the <GP System> session fails to be saved in cache
    When I create a user session
    Then I get a "Server Error" error

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |