@appointment
@backend
Feature: View available appointment slots backend
  In order book appointment
  As a logged user
  I want to see available appointment slots

  @NHSO-470
  @NHSO-1552
  @NHSO-870
  @tech-debt @NHSO-1937
  Scenario Outline: Requesting available <GP System> appointment slots with correct data returns lists of available slots
    Given I have logged into <GP System> and have a valid session cookie
    And there are available appointment slots for an explicit date-time range
    When the available appointment slots are retrieved for explicit date-time range
    Then available slots are returned for the given date-time range
    And available slots are returned containing id, start date and time, end date and time, location, clinicians, type
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  @NHSO-470
    # GP System agnostic, depends on what status code we get back
  Scenario: Requesting available appointment slots returns an unknown exception, returns a Bad Gateway error
    Given I have logged into EMIS and have a valid session cookie
    But an unknown exception will occur when wanting to view appointment slots
    When the available appointment slots are retrieved for explicit date-time range
    Then I receive a "Bad Gateway" error

  @NHSO-470
    # GP System agnostic as GP System shouldn't be hit
  Scenario: Requesting available appointment slots by patient whose session expired returns "Unauthorized" error
    Given there are available EMIS appointment slots, but session has expired
    When the available appointment slots are retrieved for explicit date-time range
    Then I receive an "Unauthorized" error

  @NHSO-470
    # GP System agnostic as GP System shouldn't be hit
  Scenario: Requesting available appointment slots with a missing NHSO-Session-Id cookie returns "Unauthorized" error
    When the available appointment slots are retrieved for explicit date-time range without a cookie
    Then I receive an "Unauthorized" error

  @NHSO-470
  @NHSO-870
  @tech-debt  @NHSO-1595
  Scenario: Requesting available appointment slots without fromDate and toDate parameters returns set of appointment slots for the next 4 weeks from now
    Given I have logged into EMIS and have a valid session cookie
    And there are available appointment slots within the next four weeks
    When the available appointment slots are retrieved without a given date-time range
    #This last line is based on breaking down the request and asserting details from that. This seems incorrect
    Then available slots are returned for the next four weeks

  @NHSO-470
  @NHSO-870
  @tech-debt @NHSO-1937
  Scenario Outline: Requesting available <GP System> appointment slots with only fromDate parameter returns set of appointment slots for 4 weeks from specified start date
    And I have logged into <GP System> and have a valid session cookie
    And there are available appointment slots four weeks from a specific from date
    When the available appointment slots are retrieved with just a from date
    Then available slots are returned for four weeks based on the date provided
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  @NHSO-470
  @NHSO-870
  @tech-debt @NHSO-1937
  Scenario Outline: Requesting available appointment slots with only toDate parameter returns set of appointment slots for 2 weeks from start day 2 weeks before end date
    Given I have logged into <GP System> and have a valid session cookie
    And there are available appointment slots four weeks preceding a specific to date
    When the available appointment slots are retrieved with just a to date
    Then available slots are returned for four weeks based on the date provided
  Examples:
  | GP System |
  | EMIS      |
  | TPP       |

  @NHSO-470
    # GP System agnostic as GP System shouldn't be hit
  Scenario: Requesting available appointment slots with fromDate parameter that is after toDate parameter returns "Bad request"
    Given I have logged into EMIS and have a valid session cookie
    When I try to retrieve appointment slots with fromDate after the toDate
    Then I receive a "Bad Request" error

  @NHSO-470
    # GP System agnostic as GP System shouldn't be hit
  Scenario: Requesting available appointment slots with fromDate and toDate parameter in the past returns "Bad Request" error
    Given I have logged into EMIS and have a valid session cookie
    When I try to retrieve appointment slots only from the past
    Then I receive a "Bad Request" error

  @NHSO-470
    # GP System agnostic as GP System shouldn't be hit
  Scenario: Requesting available appointment slots with a fromDate parameter not in the expected format returns "Bad Request" error
    Given I have logged into EMIS and have a valid session cookie
    When I try to retrieve appointment slots with a malformed from Date
    Then I receive a "Bad Request" error

  @NHSO-470
    # GP System agnostic as GP System shouldn't be hit
  Scenario: Requesting available appointment slots with a toDate parameter not in the expected format returns "Bad Request" error
    Given I have logged into EMIS and have a valid session cookie
    When I try to retrieve appointment slots with a malformed to Date
    Then I receive a "Bad Request" error

  @NHSO-470
    # GP System agnostic as GP System shouldn't be hit
  Scenario: Requesting available appointment slots when GP system is unavailable returns "Bad gateway" error
    Given I have logged into EMIS and have a valid session cookie
    When the available appointment slots are retrieved for explicit date-time range
    Then I receive a "Bad Gateway" error

  @NHSO-470
    # GP System agnostic as GP System shouldn't be hit
  Scenario: Requesting available appointment slots the GP system times out and returns "Gateway Timeout" error
    Given the system will time out when trying to retrieve appointment slots
    And I have logged into EMIS and have a valid session cookie
    When the available appointment slots are retrieved for explicit date-time range
    Then I receive a "Gateway Timeout" error
