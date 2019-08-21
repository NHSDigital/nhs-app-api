@authentication
@authentication-session
@backend
Feature: Create Session Backend

  The application verifies the user session

  Scenario: We check the session cookie and response body for http
    Given I have a valid authCode and codeVerifier
    When I create a user session
    Then I receive a response
    And the response has a name
    And the response has a session timeout
    And the response has service journey rules
    And the cookie contains a session guid with http-only

  @tech-debt @NHSO-725  # covered in Manual Regression Test pack
  Scenario: We check the session cookie and response body for https
    Given I have a valid authCode and codeVerifier
    When I create a user session
    Then I receive a response
    And the response has a name
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

  Scenario: EMIS end user session fails to create
    Given I have valid OAuth details and the EMIS end user session endpoint fails to create
    When I create a user session
    Then I receive a "Bad Gateway" error with service desk reference prefixed "3e"

  Scenario: EMIS session fails to create
    Given I have valid OAuth details and the EMIS session endpoint fails to create
    When I create a user session
    Then I receive a "Bad Gateway" error with service desk reference prefixed "3e"
    
  Scenario Outline: <GP System> is unavailable
    Given I have valid OAuth details and <GP System> is not available
    When I create a user session
    Then I receive a "Bad Gateway" error with service desk reference prefixed "<Prefix>"

    Examples:
      | GP System | Prefix |
      | EMIS      | 3e     |
      | TPP       | 3t     |
      | VISION    | 3s     |
      | MICROTEST | 3m     |

  Scenario Outline: When creating a session with <GP System> an incomplete response returns a Bad Gateway
    Given I have valid OAuth details and <GP System> returns with an incomplete response
    When I create a user session
    Then I receive a "Bad Gateway" error with service desk reference prefixed "<Prefix>"

    Examples:
      | GP System | Prefix |
      | TPP       | 3t     |

  Scenario Outline: CID connection token fails to authenticate with <GP System>
    Given I have invalid OAuth details and CID connection token fails to authenticate with <GP System>
    When I create a user session
    Then I receive a "Forbidden" error with service desk reference prefixed "3c"

    Examples:
      | GP System |
      | EMIS      |
      | VISION    |

  Scenario: CID connection token fails to authenticate with TPP
    Given I have invalid OAuth details and CID connection token fails to authenticate with TPP
    When I create a user session
    Then I receive a "Bad Gateway" error with service desk reference prefixed "3t"

  Scenario Outline: <GP System> fails to respond in 31 seconds
    Given I have valid OAuth details and <GP System> fails to respond in 31 seconds
    When I create a user session
    Then I receive a "Gateway Timeout" error with service desk reference prefixed "<Prefix>"

    Examples:
      | GP System | Prefix |
      | EMIS      | ze     |
      | TPP       | zt     |
      | VISION    | zs     |

  @manual # covered in Manual Regression Test pack
  Scenario: session fails to be saved in cache
    Given I have valid OAuth details and the <GP System> session fails to be saved in cache
    When I create a user session
    Then I get a "Server Error" error
