@authentication
@backend
@im1v2get
Feature: Im1 Connection GET V2
  The system validates the patients connection token and ods code using the v2 endpoint


  Scenario Outline: A <GP System> patient with a single NHS Number tries to verify using the v2 endpoint receives their NHS number and an OK status code
    Given I have valid credentials for a <GP System> patient with one NHS Number
    When I verify patient data using the v2 endpoint
    Then I receive the expected NHS Number
    And I receive a "OK" success code
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: A <GP System> user verifying their im1 Connection can get a <ExpectedCode> error from a <GPCode> error
    Given I am a <GP System> user and verifying my im1 connection returns '<GPHttpCode>' '<GPCode>' '<Message>'
    When I verify patient data using the v2 endpoint
    Then I receive a '<ExpectedStatus>' IM1 error status code with code '<ExpectedCode>'
    Examples:
      | GP System | GPHttpCode | GPCode | ExpectedStatus | ExpectedCode | Message                                                        |
      | EMIS      | 403        | 1030   | 403            | 101          | Patient Facing Services API v2 is not enabled at this practice |
      | TPP       | 200        | 6      | 400            | 112          | Problem logging in                                             |
      | TPP       | 200        | 9      | 400            | 112          | Problem logging in                                             |
      | VISION    | 200        | -100   | 502            | 107          | Connection to external service failed                          |
      | VISION    | 200        | -15    | 502            | 113          | User record unavailable                                        |
      | VISION    | 200        | -30    | 400            | 106          | Invalid user credentials                                       |

  Scenario Outline: A MICROTEST user verifying their im1 Connection can get a <ExpectedCode> error when demographics fails with a <GPHttpCode> error
    Given I am a MICROTEST user and verifying my im1 connection returns '<GPHttpCode>' '<GPCode>' '<Message>'
    When I verify patient data using the v2 endpoint
    Then I receive a '<ExpectedStatus>' IM1 error status code with code '<ExpectedCode>'
    Examples:
      | GPHttpCode | GPCode    | ExpectedStatus | ExpectedCode | Message |
      | 502        |           | 502            | 100          | Unknown Error |
      | 403        |           | 404            | 103          | Patient not found |
      | 500        |           | 404            | 103          | Patient not found |

    # Returning NHS numbers from TPP covered with EMIS BDD
    # and TPP structure is covered with unit tests
    # MICROTEST users must have an NHS number to verify
  Scenario Outline: A <GP System> patient has multiple NHS Numbers tries verification using the v2 endpoint and succesfully retrieve all their NHS numbers
    Given I have valid credentials for a <GP System> patient with multiple NHS Numbers
    When I verify patient data using the v2 endpoint
    Then I receive the expected NHS Numbers
    And I receive a "OK" success code
    Examples:
      | GP System |
      | EMIS      |
      | VISION    |

    # Returning NHS numbers from TPP covered with EMIS BDD
    # and TPP structure is covered with unit tests
    # MICROTEST users must have an NHS number to verify
  Scenario Outline: A <GP System> patient has no NHS Number tries verification using the v2 endpoint and receives no NHS number
    Given I have valid credentials for a <GP System> patient with no NHS Number
    When I verify patient data using the v2 endpoint
    Then I receive no NHS Number
    And I receive a "OK" success code
    Examples:
      | GP System |
      | EMIS      |
      | VISION    |

  Scenario Outline: A Non-existent IM1 Connection Token for the <GP System> tries verification using the v2 endpoint and receives a <Http Error> error
    Given I have an <GP System> IM1 Connection Token that does not exist
    When I verify patient data using the v2 endpoint
    Then I receive a "<Http Error>" error
    Examples:
      | GP System | Http Error  |
      | EMIS      | Bad Request |
      | TPP       | Bad Gateway |
      | VISION    | Bad Request |



  Scenario Outline: No ODS Code for a <GP System> user when trying verification using the v2 endpoint and receives a Bad Request error
    Given I have valid credentials for a <GP System> patient with one NHS Number
    When I verify patient data without sending the ODS Code
    Then I receive a "Bad Request" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: A <GP System> user with an IM1 Connection Token not in the expected format tries verification using the v2 endpoint and receives a Bad Request error
    Given I have an <GP System> IM1 Connection Token that is in an invalid format
    When I verify patient data using the v2 endpoint
    Then I receive a "Bad Request" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |