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

  Scenario Outline: Cancel a previously booked appointment the <GP System> times out and returns "Gateway Timeout" error
    Given  <GP System> will time out when trying to cancel a previously booked appointment
    When I send a cancellation request to the API with a valid cancellation reason
    Then I receive a "Gateway Timeout" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: <GP System> returns corrupted data when trying to cancel a previously booked appointment
    Given <GP System> returns corrupted response when trying to cancel a previously booked appointment
    And I have logged into <GP System> and have a valid session cookie
    When I send a cancellation request to the API with a valid cancellation reason
    Then I receive a "Internal Server Error" error
  @bug @NHSO-3039
    Examples:
      | GP System |
      | EMIS      |
    Examples:
      | GP System |
      | TPP       |
      | VISION    |

  Scenario: VISION API will return a Conflict when cancelling an appointment booked by someone else
    Given as a VISION user I want to cancel an appointment booked by someone else
    When I send a cancellation request to the API with a valid cancellation reason
    Then I receive a "Conflict" error

  Scenario: VISION API will return a Conflict when cancelling an appointment that doesn't exist
    Given as a VISION user I want to cancel an appointment that doesn't exist
    When I send a cancellation request to the API with a valid cancellation reason
    Then I receive a "Conflict" error
