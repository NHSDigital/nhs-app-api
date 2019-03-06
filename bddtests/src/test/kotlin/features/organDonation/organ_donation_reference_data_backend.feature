@organ-donation
@backend
Feature: Organ Donation Reference Data Backend

  Scenario: When retrieving reference data from organ donation, the user receives a 200 and reference data in
  the response
    Given I am a EMIS api user not registered with organ donation, where the reference data call will return data
    And I have logged in and have a valid session cookie
    When I request the Organ Donation Reference Data
    Then I receive a "OK" success code
    And I receive Organ Donation Reference Data

  Scenario Outline: When retrieving reference data from organ donation, an OD response of <Error Code> will prompt a
  500 response and no retry option
    Given I am a EMIS api user registered with OD but Reference Data returns '<Error Code>' error
    And I have logged in and have a valid session cookie
    When I request the Organ Donation Reference Data
    Then I receive a "Internal Server Error" error
    And the Internal Server Error response does not include a retry option
    Examples:
      | Error Code |
      | 400        |
      | 401        |
      | 403        |
      | 405        |

  Scenario Outline: When retrieving reference data from organ donation, an OD response of <Error Code> will prompt a
  502 response and a retry option
    Given I am a EMIS api user registered with OD but Reference Data returns '<Error Code>' error
    And I have logged in and have a valid session cookie
    When I request the Organ Donation Reference Data
    Then I receive a "Bad Gateway" error
    And the Bad Gateway error response includes a retry option
    Examples:
      | Error Code |
      | 429        |
      | 503        |
      | 504        |

  Scenario Outline: When retrieving reference data from organ donation, an OD response of <Error Code> will prompt a
  502 response and no retry option
    Given I am a EMIS api user registered with OD but Reference Data returns '<Error Code>' error
    And I have logged in and have a valid session cookie
    When I request the Organ Donation Reference Data
    Then I receive a "Bad Gateway" error
    And the Bad Gateway error response does not include a retry option
    Examples:
      | Error Code |
      | 500        |
      | 502        |
