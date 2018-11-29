@authentication
@authentication-session-extend
@backend
Feature: Session Extend Backend

  Scenario Outline: When the session extend endpoint is called with a valid session, the user receives a 200 response.
    Given a <GP System> user expecting a "Success" response when extending their session
    When I have logged into <GP System> and have a valid session cookie
    When I try to extend my session
    Then I receive an "Ok" success code
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: When the session extend endpoint is called with a valid session but the system throws an
  exception due to an internal error and the user receives a 502 response.
    Given a <GP System> user expecting a "bad gateway" response when extending their session
    When I have logged into <GP System> and have a valid session cookie
    When I try to extend my session
    Then I receive a "bad gateway" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

    #Special case that the mock comes after login or the 504 will occur too early
  Scenario Outline: When the session extend endpoint is called with a valid session but the call times out. The system
  throws an exception and the user receives a 504 response.
    Given I have logged into <GP System> and have a valid session cookie
    When a <GP System> user expecting a "gateway timeout" response when extending their session
    When I try to extend my session
    Then I receive a "gateway timeout" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario Outline: When the session extend endpoint is called but the session has become invalid, the user receives a
  401 response.
    Given a <GP System> user expecting a "unauthorized" response when extending their session
    And I have logged into <GP System> and have a valid session cookie
    When I am idle long enough for the session to expire
    When I try to extend my session
    Then I receive a "unauthorized" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
