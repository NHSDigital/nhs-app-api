@authentication
@backend
Feature: Im1 Connection V2
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
    Then I receive a "400" error status code
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

  Scenario Outline: A <GP System> user registering can get a <ExpectedErrorCode> error when retrieving linkage details
    Given I am a <GP System> user registering but getting linkage details returns '<GPHttpCode>' '<GPCode>' '<Message>'
    When I register the user's IM1 credentials using the v2 endpoint
    Then I receive a "<ExpectedStatus>" error status code with code "<ExpectedErrorCode>"
    Examples:
      | GP System | GPHttpCode | GPCode | ExpectedStatus | ExpectedErrorCode | Message |
      | EMIS     | 400        |	1001    | 400            | 103 ||
      | EMIS     | 400        |	1401    | 400            | 105 ||
      | VISION   | 400        |	V4205   | 400            | 106 ||
      | VISION   | 404        |	V4205   | 400            | 106 ||
      | EMIS     | 403	      | 1030    | 400            | 108 | Patient Facing Services API v2 is not enabled at this practice |
      | EMIS     | 403	      | 1030    | 400            | 109 | Patient Facing Services are not enabled by this practice |
      | EMIS     | 400	      | 1552    | 403            | 112 ||
      | EMIS     | 400	      | 1553    | 403            | 114 ||
      | EMIS     | 400        |	1554    | 400            | 115 ||
      | EMIS     | 400        |	1555    | 400            | 116 ||
      | EMIS     | 400        |	1107    | 403            | 117 ||
      | EMIS     | 400        |	1109    | 400            | 123 ||

  Scenario Outline: A <GP System> user can successfully register with created linkage details after '<GPCode>' error
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
      #196
      | VISION   | 404	      | V2210   | No API key associated with the nhs number.|
      #197
      | VISION   | 404	      | V2210   | No user associated with the nhs number.|
      #198
      | VISION   | 404	      | VY806   |  |

  Scenario Outline: A <GP System> user registering with no linkage key can get a <ExpectedErrorCode> error when creating
  linkage details
    Given I am a <GP System> user registering but creating my linkage key will return a '<GPHttpCode>' '<GPCode>' error
    When I register the user's IM1 credentials using the v2 endpoint
    Then I receive a "<ExpectedStatus>" error status code with code "<ExpectedErrorCode>"
    Examples:
      | GP System | GPHttpCode | GPCode | ExpectedStatus | ExpectedErrorCode |
      | EMIS      | 400        | 1553   | 403            | 114               |
      | TPP       | 200        | 5      | 403            | 118               |
      | TPP       | 200        | 8      | 404            | 119               |
      | TPP       | 200        | 6      | 404            | 119               |
      | EMIS      | 409        | 1110   | 400            | 120               |
      | EMIS      | 400        | 1107   | 403            | 121               |
      | VISION    | 409        | V2214  | 409            | 122               |

  Scenario Outline: A <GP System> user under minimum age attempting to register with no linkage key will receive an error when
  creating linkage details
    Given I am a <GP System> user registering but creating my linkage key fail because I am under minimum age
    When I register the user's IM1 credentials using the v2 endpoint
    Then I receive a "<ExpectedStatusCode>" error status code with code "<ExpectedErrorCode>"
    Examples:
      | GP System | ExpectedStatusCode | ExpectedErrorCode |
      | EMIS      | 403                | 114            |


  Scenario Outline: A <GP System> user registering with retrieved linkage details can get a <ExpectedErrorCode> error
    Given I am a <GP System> user with retrieved linkage but registering returns '<GPHttpCode>' '<GPCode>' '<Message>'
    And no IM1 Connection Token is currently cached
    When I register the user's IM1 credentials using the v2 endpoint
    Then I receive a "<ExpectedStatus>" error status code with code "<ExpectedErrorCode>"
    Examples:
      | GP System | GPHttpCode | GPCode | ExpectedStatus | ExpectedErrorCode | Message |
      | EMIS      | 400        | 1105   | 400            | 101            |         |
      | VISION    | 200        | -31    | 400            | 101            |         |
      | EMIS      | 400        | 1106   | 404            | 102            |         |
      | EMIS      | 400        | 1001   | 400            | 103            |         |
      | EMIS      | 400        | 1107   | 403            | 104            |         |
      | VISION    | 200        | -34    | 403            | 104            |         |
      | EMIS      | 400        | 1401   | 400            | 105            |         |
      | VISION    | 400        | V4205  | 400            | 106            |         |
      | VISION    | 404        | V4205  | 400            | 106            |         |
      | EMIS      | 403        | 1030   | 400            | 108            | Patient Facing Services API v2 is not enabled at this practice|
      | EMIS      | 403        | 1030   | 400            | 109            | Patient Facing Services are not enabled by this practice      |
      | EMIS      | 404        | 1104   | 404            | 110            |         |
      | VISION    | 200        | -33    | 404            | 110            |         |
      | EMIS      | 400        | 1108   | 409            | 111            |         |
      | VISION    | 200        | -2     | 409            | 111            |         |
      | EMIS      | 400        | 1552   | 403            | 112            |         |
      | VISION    | 200        | -19    | 403            | 113            |         |
      | VISION    | 200        | -100   | 400            | 124            | Connection to external service failed |
      | EMIS      | 400        |        | 400            | 125            | AccountId length outside of valid range.|
      | EMIS      | 400        |        | 400            | 126            | LinkageKey length outside of valid range. |
      | EMIS      | 400        |        | 400            | 127            | Other length outside of valid range.|
      | VISION    | 200        | -100   | 502            |                | Unknown Error |


  Scenario Outline: A <GP System> user registering with created linkage details can get a <ExpectedErrorCode> error
    Given I am a <GP System> user with created linkage key but registering returns '<GPHttpCode>' '<GPCode>' '<Message>'
    And no IM1 Connection Token is currently cached
    When I register the user's IM1 credentials using the v2 endpoint
    Then I receive a "<ExpectedStatus>" error status code with code "<ExpectedErrorCode>"
    Examples:
      | GP System | GPHttpCode | GPCode | ExpectedStatus | ExpectedErrorCode | Message |
      | EMIS      | 400        | 1105   | 400            | 101  ||
      | VISION    | 200        | -31    | 400            | 101  ||
      | EMIS      | 400        | 1106   | 404            | 102  ||
      | EMIS      | 400        | 1001   | 400            | 103  ||
      | EMIS      | 400        | 1107   | 403            | 104  ||
      | VISION    | 200        | -34    | 403            | 104  ||
      | EMIS      | 400        | 1401   | 400            | 105  ||
      | VISION    | 400        | V4205  | 400            | 106  ||
      | VISION    | 404        | V4205  | 400            | 106  ||
      | EMIS      | 403        | 1030   | 400            | 108  | Patient Facing Services API v2 is not enabled at this practice|
      | EMIS      | 403        | 1030   | 400            | 109  | Patient Facing Services are not enabled by this practice      |
      | EMIS      | 404        | 1104   | 404            | 110  ||
      | VISION    | 200        | -33    | 404            | 110  ||
      | EMIS      | 400        | 1108   | 409            | 111  ||
      | VISION    | 200        | -2     | 409            | 111  ||
      | EMIS      | 400        | 1552   | 403            | 112  ||
      | VISION    | 200        | -19    | 403            | 113  ||
      | VISION    | 200        | -100   | 400            | 124  | Connection to external service failed |
      | EMIS      | 400        |        | 400            | 125  | AccountId length outside of valid range.|
      | EMIS      | 400        |        | 400            | 126  | LinkageKey length outside of valid range. |
      | EMIS      | 400        |        | 400            | 127  | Other length outside of valid range.|
      | VISION    | 200        | -100   | 502            |      | Unknown Error |
    #If TPP has a successful linkage creation, the register endpoint is not called

  Scenario Outline: A <GP System> user registering with provided linkage details can get a <ExpectedErrorCode> error
    Given I am a <GP System> user with provided linkage key but registering returns '<GPHttpCode>' '<GPCode>' '<Message>'
    When I register the user's IM1 credentials using the v2 endpoint
    Then I receive a "<ExpectedStatus>" error status code with code "<ExpectedErrorCode>"
    Examples:
      | GP System | GPHttpCode | GPCode | ExpectedStatus | ExpectedErrorCode | Message |
      | EMIS      | 400        | 1105   | 400            | 101  ||
      | VISION    | 200        | -31    | 400            | 101  ||
      | EMIS      | 400        | 1106   | 404            | 102  ||
      | EMIS      | 400        | 1001   | 400            | 103  ||
      | EMIS      | 400        | 1107   | 403            | 104  ||
      | VISION    | 200        | -34    | 403            | 104  ||
      | EMIS      | 400        | 1401   | 400            | 105  ||
      | VISION    | 400        | V4205  | 400            | 106  ||
      | VISION    | 404        | V4205  | 400            | 106  ||
      | TPP       | 200        | 8      | 400            | 107  ||
      | EMIS      | 403        | 1030   | 400            | 108  | Patient Facing Services API v2 is not enabled at this practice|
      | EMIS      | 403        | 1030   | 400            | 109  | Patient Facing Services are not enabled by this practice      |
      | EMIS      | 404        | 1104   | 404            | 110  ||
      | VISION    | 200        | -33    | 404            | 110  ||
      | EMIS      | 400        | 1108   | 409            | 111  ||
      | VISION    | 200        | -2     | 409            | 111  ||
      | EMIS      | 400        | 1552   | 403            | 112  ||
      | VISION    | 200        | -19    | 403            | 113  ||
      | VISION    | 200        | -100   | 400            | 124  | Connection to external service failed |
      | EMIS      | 400        |        | 400            | 125  | AccountId length outside of valid range.|
      | EMIS      | 400        |        | 400            | 126  | LinkageKey length outside of valid range. |
      | EMIS      | 400        |        | 400            | 127  | Other length outside of valid range.|
      | VISION    | 200        | -100   | 502            |      | Unknown Error |