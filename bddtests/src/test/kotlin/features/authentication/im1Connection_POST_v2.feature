@authentication
@backend
@im1v2post
Feature: Im1 Connection V2 POST
  A user can create a new NHS account from the login page, allowing them to access the app

  Scenario Outline: A <GP System> user can successfully register with Im1 with full linkage details
    Given I am a <GP System> user wishing to register with full linkage details
    And no IM1 Connection Token is currently cached
    When I register the user's IM1 credentials using the v2 endpoint
    Then I receive a "Created" success code
    And the Im1 connection response has the expected connection token
    And the Im1 connection response has the expected NHS numbers
    And the IM1 Connection Token is in the cache
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: A <GP System> user can successfully register with retrieved linkage details
    Given I am a <GP System> user wishing to register with retrieved linkage details
    And no IM1 Connection Token is currently cached
    When I register the user's IM1 credentials using the v2 endpoint
    Then I receive a "Created" success code
    And the Im1 connection response has the expected connection token
    And the Im1 connection response has the expected NHS numbers
    And the IM1 Connection Token is in the cache
    Examples:
      | GP System |
      | EMIS      |
      | VISION    |
    #TPP cannot retrieve linkage keys, so this test would be invalid
    #MICROTEST users connection token is based on generated guids so we cant use the assert on the connection token

  Scenario: A MICROTEST user can successfully register, getting retrieved linkage details
    Given I am a MICROTEST user wishing to register with retrieved linkage details
    And no IM1 Connection Token is currently cached
    When I register the user's IM1 credentials using the v2 endpoint
    Then I receive a "Created" success code
    And the Im1 connection response has the expected NHS numbers
    And the IM1 Connection Token is in the cache

  Scenario Outline: A <GP System> user can successfully register with created linkage details
    Given I am a <GP System> user wishing to register with created linkage details
    And no IM1 Connection Token is currently cached
    When I register the user's IM1 credentials using the v2 endpoint
    Then I receive a "Created" success code
    And the Im1 connection response has the expected connection token
    And the Im1 connection response has the expected NHS numbers
    And the IM1 Connection Token is in the cache
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: A <GP System> user missing <Field> receives a 400 response on registering with Im1
    Given I am a <GP System> user wishing to register but missing <Field>
    When I register the user's IM1 credentials using the v2 endpoint
    Then I receive a '400' IM1 error status code with code '106' and GP System 'Unknown'
    Examples:
      | GP System | Field   |
      | EMIS      | Surname |
      | EMIS      | Odscode |
      | EMIS      | Dob     |
      | TPP       | Surname |
      | TPP       | Odscode |
      | TPP       | Dob     |
      | VISION    | Surname |
      | VISION    | Odscode |
      | VISION    | Dob     |

  Scenario Outline: A <GP System> user registering can get an error code <ExpectedCode> from a GP code <GPCode> and
  '<Message>' when retrieving linkage details returns <GPHttpCode>
    Given I am a <GP System> user registering but getting linkage details returns '<GPHttpCode>' '<GPCode>' '<Message>'
    When I register the user's IM1 credentials using the v2 endpoint
    Then I receive a '<ExpectedStatus>' IM1 error status code with code '<ExpectedCode>'
    Examples:
      | GP System | GPHttpCode | GPCode | ExpectedStatus | ExpectedCode | Message |
      | EMIS     | 400        |	1001    | 403            | 101 ||
      | EMIS     | 400        |	1401    | 403            | 101 ||
      | EMIS     | 400	      | 1552    | 403            | 102 ||
      | EMIS     | 400        |	1554    | 403            | 102 ||
      | EMIS     | 400        |	1107    | 403            | 102 ||
      | EMIS     | 400        |	1109    | 403            | 102 ||
      | EMIS     | 400	      | 1553    | 403            | 104 ||
      | EMIS     | 400        |	1555    | 409            | 105 ||
      | EMIS     | 403	      | 1030    | 403            | 101 | Patient Facing Services API v2 is not enabled at this practice |
      | EMIS     | 403	      | 1030    | 403            | 101 | Patient Facing Services are not enabled by this practice |
      | EMIS     | 400        | 1       | 502            | 100 | Unmapped Error |
      | VISION   | 400        | 1       | 502            | 100 | Unmapped Error |
      | VISION   | 400        |	V4205   | 400            | 108 ||
      | VISION   | 404        |	V4205   | 400            | 108 ||
      | MICROTEST| 400        |	        | 400            | 200 ||
      | MICROTEST| 500        |	        | 400            | 200 ||

  Scenario Outline: A <GP System> user can successfully register with created linkage details after <GPCode> error
    Given I am a <GP System> user registering with created linkage after a get linkage returns '<GPHttpCode>' '<GPCode>' '<Message>'
    And no IM1 Connection Token is currently cached
    When I register the user's IM1 credentials using the v2 endpoint
    Then I receive a "Created" success code
    And the Im1 connection response has the expected connection token
    And the Im1 connection response has the expected NHS numbers
    And the IM1 Connection Token is in the cache
    Examples:
      | GP System | GPHttpCode | GPCode | Message |
      #195
      | EMIS     | 404        |	1551    | |
      #199
      | EMIS     | 404	      | 1104    | |
      #198
      | VISION   | 404	      | VY806   |  |

  Scenario Outline: A <GP System> user can successfully register with created linkage details after <GPCode> error with <Message>
    Given I am a <GP System> user registering with created linkage after a get linkage returns '<GPHttpCode>' '<GPCode>' '<Message>'
    And no IM1 Connection Token is currently cached
    When I register the user's IM1 credentials using the v2 endpoint
    Then I receive a "Created" success code
    And the Im1 connection response has the expected connection token
    And the Im1 connection response has the expected NHS numbers
    And the IM1 Connection Token is in the cache
    Examples:
      | GP System | GPHttpCode | GPCode | Message |
      #196
      | VISION   | 404	      | V2210   | No API key associated with the nhs number.|
      #197
      | VISION   | 404	      | V2210   | No user associated with the nhs number.|

  Scenario Outline: A <GP System> user registering with no linkage key can get a <ExpectedCode> error when creating linkage details after <GPCode> error
    Given I am a <GP System> user registering but creating my linkage key will return a '<GPHttpCode>' '<GPCode>' error
    When I register the user's IM1 credentials using the v2 endpoint
    Then I receive a '<ExpectedStatus>' IM1 error status code with code '<ExpectedCode>'
    Examples:
      | GP System | GPHttpCode | GPCode | ExpectedStatus | ExpectedCode |
      | TPP       | 200        | 8      | 500            | 100          |
      | TPP       | 200        | 6      | 500            | 100          |
      | EMIS      | 400        | 1107   | 403            | 102          |
      | EMIS      | 400        | 1553   | 403            | 104          |
      | EMIS      | 409        | 1110   | 409            | 105          |
      | VISION    | 409        | V2214  | 409            | 105          |
      | TPP       | 200        | 5      | 400            | 109          |
  #Unmapped Errors
      | EMIS      | 400        | 1      | 502            | 100          |
      | TPP       | 400        | 1      | 502            | 100          |
      | VISION    | 400        | 1      | 502            | 100          |

  Scenario Outline: A <GP System> user under minimum age attempting to register with no linkage key will receive an error when creating linkage details
    Given I am a <GP System> user registering but creating my linkage key fail because I am under minimum age
    When I register the user's IM1 credentials using the v2 endpoint
    Then I receive a '<ExpectedStatusCode>' IM1 error status code with code '<ExpectedCode>'
    Examples:
      | GP System | ExpectedStatusCode | ExpectedCode |
      | EMIS      | 403                | 104          |

  Scenario Outline: A <GP System> user registering with retrieved linkage can get a <ExpectedCode> error from a <GPCode> and <GPHttpCode> error
    Given I am a <GP System> user with retrieved linkage but registering returns '<GPHttpCode>' '<GPCode>' '<Message>'
    And no IM1 Connection Token is currently cached
    When I register the user's IM1 credentials using the v2 endpoint
    Then I receive a '<ExpectedStatus>' IM1 error status code with code '<ExpectedCode>'
    Examples:
      | GP System | GPHttpCode | GPCode | ExpectedStatus | ExpectedCode | Message |
      | TPP       | 200        | 8      | 500            | 100  ||
      | EMIS      | 400        | 1001   | 403            | 101  ||
      | EMIS      | 400        | 1401   | 403            | 101  ||
      | EMIS      | 400        | 1107   | 403            | 102  ||
      | VISION    | 200        | -34    | 403            | 102  ||
      | EMIS      | 400        | 1552   | 403            | 102  ||
      | VISION    | 200        | -19    | 403            | 102  ||
      | EMIS      | 400        | 1106   | 404            | 103  ||
      | EMIS      | 404        | 1104   | 404            | 103  ||
      | VISION    | 200        | -33    | 404            | 103  ||
      | EMIS      | 400        | 1108   | 409            | 105  ||
      | VISION    | 200        | -2     | 409            | 105  ||
      | EMIS      | 400        | 1105   | 400            | 106  ||
      | VISION    | 200        | -31    | 400            | 106  ||
      | VISION    | 400        | V4205  | 400            | 108  ||
      | VISION    | 404        | V4205  | 400            | 108  ||

  Scenario Outline: A <GP System> user registering with retrieved linkage can get a <ExpectedCode> error from a <GPCode> error with <Message>
    Given I am a <GP System> user with retrieved linkage but registering returns '<GPHttpCode>' '<GPCode>' '<Message>'
    And no IM1 Connection Token is currently cached
    When I register the user's IM1 credentials using the v2 endpoint
    Then I receive a '<ExpectedStatus>' IM1 error status code with code '<ExpectedCode>'
    Examples:
      | GP System | GPHttpCode | GPCode | ExpectedStatus | ExpectedCode | Message |
      | EMIS      | 403        | 1030   | 403            | 101  | Patient Facing Services API v2 is not enabled at this practice|
      | EMIS      | 403        | 1030   | 403            | 101  | Patient Facing Services are not enabled by this practice      |
      | EMIS      | 400        |        | 400            | 106  | Other length outside of valid range.|
      | VISION    | 200        | -100   | 502            | 107  | Connection to external service failed |
      | EMIS      | 400        |        | 400            | 110  | AccountId length outside of valid range.|
      | EMIS      | 400        |        | 400            | 111  | LinkageKey length outside of valid range. |
      | VISION    | 200        | -100   | 502            | 100  | Unknown Error |
      | EMIS      | 400        | 1      | 502            | 100  | Unmapped Error |
      | TPP       | 400        | 1      | 502            | 100  | Unmapped Error |
      | VISION    | 400        | 1      | 502            | 100  | Unmapped Error |

  Scenario Outline: A <GP System> user registering with created linkage details can get <ExpectedCode> error from a <GPCode> and <GPHttpCode> error
    Given I am a <GP System> user with created linkage key but registering returns '<GPHttpCode>' '<GPCode>' '<Message>'
    And no IM1 Connection Token is currently cached
    When I register the user's IM1 credentials using the v2 endpoint
    Then I receive a '<ExpectedStatus>' IM1 error status code with code '<ExpectedCode>'
    Examples:
      | GP System | GPHttpCode | GPCode | ExpectedStatus | ExpectedCode | Message |
      | EMIS      | 400        | 1001   | 403            | 101  ||
      | EMIS      | 400        | 1401   | 403            | 101  ||
      | EMIS      | 400        | 1107   | 403            | 102  ||
      | VISION    | 200        | -34    | 403            | 102  ||
      | EMIS      | 400        | 1552   | 403            | 102  ||
      | VISION    | 200        | -19    | 403            | 102  ||
      | EMIS      | 400        | 1106   | 404            | 103  ||
      | EMIS      | 404        | 1104   | 404            | 103  ||
      | VISION    | 200        | -33    | 404            | 103  ||
      | EMIS      | 400        | 1108   | 409            | 105  ||
      | VISION    | 200        | -2     | 409            | 105  ||
      | EMIS      | 400        | 1105   | 400            | 106  ||
      | VISION    | 200        | -31    | 400            | 106  ||
      | VISION    | 400        | V4205  | 400            | 108  ||
      | VISION    | 404        | V4205  | 400            | 108  ||
    #If TPP has a successful linkage creation, the register endpoint is not called

  Scenario Outline: A <GP System> user registering with created linkage details can get <ExpectedCode> error from a <GPCode> error with <Message>
    Given I am a <GP System> user with created linkage key but registering returns '<GPHttpCode>' '<GPCode>' '<Message>'
    And no IM1 Connection Token is currently cached
    When I register the user's IM1 credentials using the v2 endpoint
    Then I receive a '<ExpectedStatus>' IM1 error status code with code '<ExpectedCode>'
    Examples:
      | GP System | GPHttpCode | GPCode | ExpectedStatus | ExpectedCode | Message |
      | EMIS      | 403        | 1030   | 403            | 101  | Patient Facing Services API v2 is not enabled at this practice|
      | EMIS      | 403        | 1030   | 403            | 101  | Patient Facing Services are not enabled by this practice      |
      | EMIS      | 400        |        | 400            | 106  | Other length outside of valid range.|
      | VISION    | 200        | -100   | 502            | 107  | Connection to external service failed |
      | EMIS      | 400        |        | 400            | 110  | AccountId length outside of valid range.|
      | EMIS      | 400        |        | 400            | 111  | LinkageKey length outside of valid range. |
      | VISION    | 200        | -100   | 502            | 100  | Unknown Error |
      | EMIS      | 400        | 1      | 502            | 100  | Unmapped Error |
      | VISION    | 400        | 1      | 502            | 100  | Unmapped Error |

  Scenario Outline: A <GP System> user registering with provided linkage details can get a <ExpectedCode> error from a <GPCode> error
    Given I am a <GP System> user with provided linkage key but registering returns '<GPHttpCode>' '<GPCode>' '<Message>'
    When I register the user's IM1 credentials using the v2 endpoint
    Then I receive a '<ExpectedStatus>' IM1 error status code with code '<ExpectedCode>'
    Examples:
      | GP System | GPHttpCode | GPCode | ExpectedStatus | ExpectedCode | Message |
      | EMIS      | 400        | 1001   | 403            | 101  ||
      | EMIS      | 400        | 1401   | 403            | 101  ||
      | EMIS      | 400        | 1107   | 403            | 102  ||
      | VISION    | 200        | -34    | 403            | 102  ||
      | EMIS      | 400        | 1552   | 403            | 102  ||
      | VISION    | 200        | -19    | 403            | 102  ||
      | EMIS      | 400        | 1106   | 404            | 103  ||
      | EMIS      | 404        | 1104   | 404            | 103  ||
      | VISION    | 200        | -33    | 404            | 103  ||
      | EMIS      | 400        | 1108   | 409            | 105  ||
      | VISION    | 200        | -2     | 409            | 105  ||
      | EMIS      | 400        | 1105   | 400            | 106  ||
      | VISION    | 200        | -31    | 400            | 106  ||
      | TPP       | 200        | 8      | 400            | 106  ||
      | VISION    | 400        | V4205  | 400            | 108  ||
      | VISION    | 404        | V4205  | 400            | 108  ||

  Scenario Outline: A <GP System> user registering with provided linkage details can get a <ExpectedCode> error from a <GPCode> error with <Message>
    Given I am a <GP System> user with provided linkage key but registering returns '<GPHttpCode>' '<GPCode>' '<Message>'
    When I register the user's IM1 credentials using the v2 endpoint
    Then I receive a '<ExpectedStatus>' IM1 error status code with code '<ExpectedCode>'
    Examples:
      | GP System | GPHttpCode | GPCode | ExpectedStatus | ExpectedCode | Message |
      | EMIS      | 403        | 1030   | 403            | 101  | Patient Facing Services API v2 is not enabled at this practice|
      | EMIS      | 403        | 1030   | 403            | 101  | Patient Facing Services are not enabled by this practice      |
      | EMIS      | 400        |        | 400            | 106  | Other length outside of valid range.|
      | VISION    | 200        | -100   | 502            | 107  | Connection to external service failed |
      | EMIS      | 400        |        | 400            | 110  | AccountId length outside of valid range.|
      | EMIS      | 400        |        | 400            | 111  | LinkageKey length outside of valid range. |
      | VISION    | 200        | -100   | 502            | 100  | Unknown Error |
      | EMIS      | 400        | 1      | 502            | 100  | Unmapped Error |
      | TPP       | 400        | 1      | 502            | 100  | Unmapped Error |
      | VISION    | 400        | 1      | 502            | 100  | Unmapped Error |

  @long-running
  Scenario Outline: A <GP System> user registering with Im1, when the request times out, receives a 504 timeout error
    Given I am a <GP System> user wishing to register but the request will timeout
    And no IM1 Connection Token is currently cached
    When I register the user's IM1 credentials using the v2 endpoint
    Then I receive a '504' IM1 error status code with code '100' and GP System 'Unknown'
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
