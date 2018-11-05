@appointment
@backend
Feature: Ability to cancel an appointment via api

  Scenario Outline: <GP System> API will cancel the appointment if valid reason is provided
    Given <GP System> is available to cancel a previously booked appointment
    When I send a cancellation request to the API with a valid cancellation reason
    Then I will receive a successful response
    Examples:
    | GP System |
    | EMIS      |
    | TPP       |
    | VISION    |

  Scenario: EMIS API will not cancel the appointment if reason an invalid reason is provided
    Given EMIS is available to cancel a previously booked appointment
    When I send a cancellation request to the API with an invalid cancellation reason
    Then I receive a "Bad request" error

  @NHSO-802
  Scenario Outline: Cancel a previously booked appointment the <GP System> times out and returns "Gateway Timeout" error
    Given  <GP System> will time out when trying to cancel a previously booked appointment
    When I send a cancellation request to the API with a valid cancellation reason
    Then I receive a "Gateway Timeout" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

