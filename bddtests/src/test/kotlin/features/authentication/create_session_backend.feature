@authentication
@authentication-session
@backend
Feature: Create Session Backend: The application verifies the user session

  Scenario: We check the session cookie and response body for http
    Given I have a valid authCode and codeVerifier
    When I create a user session
    Then I receive a response
    And the response has a name for the EMIS patient with no title
    And the response has a session timeout
    And the response has service journey rules
    And the cookie contains a session guid with http-only

  # covered in Manual Regression Test pack
  @tech-debt @NHSO-725
  Scenario: We check the session cookie and response body for https
    Given I have a valid authCode and codeVerifier
    When I create a user session
    Then I receive a response
    And the response has a name for the EMIS patient with no title
    And the response has a session timeout
    And the response has service journey rules
    And the cookie contains a session guid with tls-only

  Scenario: OAuth details are incomplete
    Given I have incomplete OAuth details
    When I create a user session
    Then I receive a "Bad Request" error with service desk reference prefixed "3a"

  Scenario: OAuth details are invalid
    Given I have invalid OAuth details
    When I create a user session
    Then I receive a "Bad Request" error with service desk reference prefixed "3a"

  Scenario: CID tokens endpoint fails to process the request
    Given I have valid OAuth details and the CID tokens endpoint fails to process the request
    When I create a user session
    Then I receive a "Bad Gateway" error with service desk reference prefixed "3n"

  Scenario: EMIS session fails to create
    And I have valid OAuth details and the EMIS session endpoint fails to create
    When I create a user session
    Then I receive a response
    And the response has a name for the EMIS patient with no title
    And the response has a session timeout
    And the response has service journey rules

  Scenario Outline: <GP System> is unavailable when creating a session
    Given I have valid OAuth details and <GP System> is not available
    When I create a user session
    Then I receive a response
    And the response has a name for the <GP System> patient with no title
    And the response has a session timeout
    And the response has service journey rules
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: When creating a session with <GP System> an incomplete response resulting in a parsing error
    Given I have valid OAuth details and <GP System> returns with an incomplete response
    When I create a user session
    Then I receive a response
    And the response has a name for the <GP System> patient with no title
    And the response has a session timeout
    And the response has service journey rules
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: CID connection token fails to authenticate with <GP System> so the connection token is invalid
    Given I have invalid OAuth details and CID connection token fails to authenticate with <GP System>
    When I create a user session
    Then I receive a "Forbidden" error with service desk reference prefixed "3c"
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: <GP System> fails to respond in 31 seconds resulting in a timeout
    Given I have valid OAuth details and <GP System> fails to respond in 31 seconds
    When I create a user session
    Then I receive a response
    And the response has a name for the <GP System> patient with no title
    And the response has a session timeout
    And the response has service journey rules
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  # covered in Manual Regression Test pack
  @manual
  Scenario Outline: <GP System> session fails to be saved in cache
    Given I have valid OAuth details and the <GP System> session fails to be saved in cache
    When I create a user session
    Then I receive a response
    And the response has a name for the <GP System> patient with no title
    And the response has a session timeout
    And the response has service journey rules
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |
