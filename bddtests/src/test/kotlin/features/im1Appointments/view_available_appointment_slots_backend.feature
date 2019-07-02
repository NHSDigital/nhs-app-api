@appointments
@view
@backend
Feature: View available appointment slots backend
  In order book appointment
  As a logged user
  I want to see available appointment slots

  Scenario Outline: Requesting available <GP System> appointment slots with correct data returns lists of available slots
    Given there are available appointment slots with different criteria for <GP System>
    And I have logged in and have a valid session cookie
    When the available appointment slots are retrieved
    Then available slots are returned for the full date range
    And available slots are returned containing id, start date and time, end date and time, location, clinicians, type
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
  @smoketest
    Examples:
      | GP System |
      | MICROTEST |


  Scenario Outline: Requesting available <GP System> appointment slots returns an unknown exception, returns a Bad Gateway error
    Given an unknown exception will occur when wanting to view <GP System> appointment slots
    And I have logged in and have a valid session cookie
    When the available appointment slots are retrieved
    Then I receive a "Bad Gateway" error
    And the response contains an empty body
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

    # GP System agnostic as GP System shouldn't be hit
  Scenario: Requesting available appointment slots by patient whose session expired returns "Unauthorized" error
    Given there are available EMIS appointment slots, but session has expired
    When the available appointment slots are retrieved
    Then I receive an "Unauthorized" error
    And the response contains an empty body

    # GP System agnostic as GP System shouldn't be hit
  Scenario: Requesting available appointment slots with a missing NHSO-Session-Id cookie returns "Unauthorized" error
    When the available appointment slots are retrieved without a cookie
    Then I receive an "Unauthorized" error
    And the response contains an empty body

  Scenario Outline: Requesting available appointment slots when <GP system> is unavailable returns "Bad gateway" error
    Given I have logged into <GP System> and have a valid session cookie
    When the available appointment slots are retrieved
    Then I receive a "Bad Gateway" error
    And the response contains an empty body
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: Requesting available appointment slots when <GP System> is not accessible returns "Forbidden" error
    Given the system will respond with forbidden when trying to retrieve <GP System> appointment slots
    And I have logged into <GP System> and have a valid session cookie
    And the available appointment slots are retrieved
    Then I receive a "Forbidden" error
    And the response contains an empty body
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  @long-running
  Scenario Outline: Requesting available appointment slots the <GP System> times out and returns "Gateway Timeout" error
    Given the system will time out when trying to retrieve <GP System> appointment slots
    And I have logged in and have a valid session cookie
    When the available appointment slots are retrieved
    Then I receive a "Gateway Timeout" error
    And the response contains an empty body
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

    # Only specific to EMIS and MICROTEST as we only look to retrieve them for these systems at the moment
  Scenario Outline: A <GP System> user who has <User's Telephone Numbers> phone number(s) stored are retrieved with available slots
    Given I have <User's Telephone Numbers> telephone number(s) stored for <GP System>
    And there are available appointment slots with different criteria for <GP System>
    And I have logged in and have a valid session cookie
    When the available appointment slots are retrieved
    Then <Retrieved Numbers> telephone number(s) are retrieved
    Examples:
      | GP System | User's Telephone Numbers        | Retrieved Numbers |
      | EMIS      | no                              | 0                 |
      | EMIS      | only first                      | 1                 |
      | EMIS      | only second                     | 1                 |
      | EMIS      | both first and second           | 2                 |
      | MICROTEST | no                              | 0                 |
      | MICROTEST | only first                      | 1                 |
      | MICROTEST | only second                     | 1                 |
      | MICROTEST | both first and second           | 2                 |

