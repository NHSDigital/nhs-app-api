@appointment
@backend
Feature: View available appointment slots backend
  In order book appointment
  As a logged user
  I want to see available appointment slots

  @bug @NHSO-2795
  Scenario Outline: Requesting available <GP System> appointment slots with correct data returns lists of available slots
    Given there are available <GP System> appointment slots for an explicit date-time range
    And I have logged into <GP System> and have a valid session cookie
    When the available appointment slots are retrieved for explicit date-time range
    Then available slots are returned for the given date-time range
    And available slots are returned containing id, start date and time, end date and time, location, clinicians, type
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |


    # GP System agnostic, depends on what status code we get back
  Scenario: Requesting available appointment slots returns an unknown exception, returns a Bad Gateway error
    Given an unknown exception will occur when wanting to view appointment slots
    And I have logged into EMIS and have a valid session cookie
    When the available appointment slots are retrieved
    Then I receive a "Bad Gateway" error

    # GP System agnostic as GP System shouldn't be hit
  Scenario: Requesting available appointment slots by patient whose session expired returns "Unauthorized" error
    Given there are available EMIS appointment slots, but session has expired
    When the available appointment slots are retrieved
    Then I receive an "Unauthorized" error

    # GP System agnostic as GP System shouldn't be hit
  Scenario: Requesting available appointment slots with a missing NHSO-Session-Id cookie returns "Unauthorized" error
    When the available appointment slots are retrieved without a cookie
    Then I receive an "Unauthorized" error

    # GP System agnostic as GP System shouldn't be hit
  Scenario: Requesting available appointment slots with fromDate parameter that is after toDate parameter returns "Bad request"
    Given I have logged into EMIS and have a valid session cookie
    When I try to retrieve appointment slots with fromDate after the toDate
    Then I receive a "Bad Request" error

    # GP System agnostic as GP System shouldn't be hit
  Scenario: Requesting available appointment slots with fromDate and toDate parameter in the past returns "Bad Request" error
    Given I have logged into EMIS and have a valid session cookie
    When I try to retrieve appointment slots only from the past
    Then I receive a "Bad Request" error

    # GP System agnostic as GP System shouldn't be hit
  Scenario: Requesting available appointment slots with a fromDate parameter not in the expected format returns "Bad Request" error
    Given I have logged into EMIS and have a valid session cookie
    When I try to retrieve appointment slots with a malformed from Date
    Then I receive a "Bad Request" error

    # GP System agnostic as GP System shouldn't be hit
  Scenario: Requesting available appointment slots with a toDate parameter not in the expected format returns "Bad Request" error
    Given I have logged into EMIS and have a valid session cookie
    When I try to retrieve appointment slots with a malformed to Date
    Then I receive a "Bad Request" error

    # GP System agnostic as GP System shouldn't be hit
  Scenario: Requesting available appointment slots when GP system is unavailable returns "Bad gateway" error
    Given I have logged into EMIS and have a valid session cookie
    When the available appointment slots are retrieved
    Then I receive a "Bad Gateway" error

    # GP System agnostic as GP System shouldn't be hit
  Scenario: Requesting available appointment slots the GP system times out and returns "Gateway Timeout" error
    Given I have logged into EMIS and have a valid session cookie
    And the system will time out when trying to retrieve appointment slots
    When the available appointment slots are retrieved
    Then I receive a "Gateway Timeout" error

  @manual
  Scenario: Requesting available appointment slots without fromDate and toDate parameters returns set of appointment slots for the next 4 weeks from now
    # GP System agnostic as GP System shouldn't be hit
    # This is covered by a unit test