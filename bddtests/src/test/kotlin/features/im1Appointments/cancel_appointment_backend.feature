@appointments
@cancel
@backend
Feature: Cancel Appointments Backend

  Scenario Outline: <GP System> API will cancel the appointment if valid reason is provided
    Given <GP System> is available to cancel a previously booked appointment before cutoff time
    When I send a cancellation request to the API with a valid cancellation reason
    Then I will receive a successful response
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario: EMIS API will not cancel the appointment if an invalid reason is provided
    Given EMIS is available to cancel a previously booked appointment before cutoff time
    When I send a cancellation request to the API with an invalid cancellation reason
    Then I receive a "Bad request" error with service desk reference prefixed "4a"

  @smoketest
  Scenario Outline: Cancel a previously booked appointment the <GP System> times out and returns "Gateway Timeout" error
    Given  <GP System> will time out when trying to cancel a previously booked appointment
    When I send a cancellation request to the API with a valid cancellation reason
    Then I receive a "Gateway Timeout" error with service desk reference prefixed "<Prefix>"
    Examples:
      | GP System | Prefix |
      | EMIS      | ze     |
      | TPP       | zt     |
      | VISION    | zs     |
      | MICROTEST | zm     |

  Scenario Outline: <GP System> returns corrupted data when trying to cancel a previously booked appointment
    Given <GP System> returns corrupted response when trying to cancel a previously booked appointment
    And I have logged in and have a valid session cookie
    When I send a cancellation request to the API with a valid cancellation reason
    Then I receive a "Internal Server Error" error with service desk reference prefixed "4k"
  @bug @NHSO-3039
    Examples:
      | GP System |
      | EMIS      |
  @bug @NHSO-4923
    Examples:
      | GP System |
      | TPP       |
    Examples:
      | GP System |
      | VISION    |

  Scenario: VISION API will return a Conflict when cancelling an appointment booked by someone else
    Given as a VISION user I want to cancel an appointment booked by someone else
    When I send a cancellation request to the API with a valid cancellation reason
    Then I receive a "Conflict" error with service desk reference prefixed "4f"

  Scenario: VISION API will return a Conflict when cancelling an appointment that doesn't exist
    Given as a VISION user I want to cancel an appointment that doesn't exist
    When I send a cancellation request to the API with a valid cancellation reason
    Then I receive a "Conflict" error with service desk reference prefixed "4f"
