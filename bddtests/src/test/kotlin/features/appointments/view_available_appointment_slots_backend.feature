Feature: View available appointment slots backend
  In order book appointment
  As a logged user
  I want to see available appointment slots

  @NHSO-470
  @NHSO-1552
  @backend
  Scenario: Requesting available appointment slots with correct data returns lists of available slots, locations, clinicians, and appointment sessions
    Given I have logged in and have a valid session cookie
    And there are available appointment slots for an explicit date-time range
    When the available appointment slots are retrieved for explicit date-time range
    Then available slots, locations, clinicians and appointment sessions are returned for the given date-time range
    And available slots are returned containing id, start date and time, end date and time, location identifier, appointment session identifier, clinician identifiers
    And available locations are returned containing an id and display name
    And available clinicians are returned containing an id and display name
    And available appointment session are returned containing an id and display name

  @NHSO-470
  @backend
  Scenario: Online appointment booking is not available to a particular patient
    Given I have logged in and have a valid session cookie
    But the practice does not offer online booking to my patient
    When the available appointment slots are retrieved for explicit date-time range
    Then I receive a "Forbidden" error

  @NHSO-470
  @backend
  Scenario: Requesting available appointment slots returns an unknown exception, returns a Bad Gateway error
    Given I have logged in and have a valid session cookie
    But an unknown exception will occur when wanting to view appointment slots
    When the available appointment slots are retrieved for explicit date-time range
    Then I receive a "Bad Gateway" error

  @NHSO-470
  @backend
  Scenario: Requesting available appointment slots by patient whose session expired returns "Unauthorized" error
    Given there are available appointment slots, but session has expired
    When the available appointment slots are retrieved for explicit date-time range
    Then I receive an "Unauthorized" error

  @NHSO-470
  @backend
  Scenario: Requesting available appointment slots with a missing NHSO-Session-Id cookie returns "Unauthorized" error
    Given there are available appointment slots for an explicit date-time range
    When the available appointment slots are retrieved for explicit date-time range without a cookie
    Then I receive an "Unauthorized" error

  @NHSO-470
  @backend
  @tech-debt  @NHSO-1595
  Scenario: Requesting available appointment slots without fromDate and toDate parameters returns set of appointment slots for the next 2 weeks from now
    Given there are available appointment slots within the next two weeks
    And I have logged in and have a valid session cookie
    When the available appointment slots are retrieved without a given date-time range
    #This last line is based on breaking down the request and asserting details from that. This seems incorrect
    Then available slots, locations, clinicians and appointment sessions are returned for the next two weeks

  @NHSO-470
  @backend
  Scenario: Requesting available appointment slots with only fromDate parameter returns set of appointment slots for 2 weeks from specified start date
    Given there are available appointment slots two weeks from a specific from date
    And I have logged in and have a valid session cookie
    When the available appointment slots are retrieved with just a from date
    Then available slots, locations, clinicians and appointment sessions are returned for the two weeks following the from date

  @NHSO-470
  @backend
  Scenario: Requesting available appointment slots with only toDate parameter returns set of appointment slots for 2 weeks from start day 2 weeks before end date
    Given there are available appointment slots two weeks preceding a specific to date
    And I have logged in and have a valid session cookie
    When the available appointment slots are retrieved with just a to date
    Then available slots, locations, clinicians and appointment sessions are returned for the two weeks preceding the to date

  @NHSO-470
  @backend
  Scenario: Requesting available appointment slots with fromDate parameter that is after toDate parameter returns "Bad request"
    Given I have logged in and have a valid session cookie
    When I try to retrieve appointment slots with fromDate after the toDate
    Then I receive a "Bad Request" error

  @NHSO-470
  @backend
  Scenario: Requesting available appointment slots with fromDate and toDate parameter in the past returns empty set of appointment slots
    Given I have logged in and have a valid session cookie
    When I try to retrieve appointment slots only from the past
    Then I receive a "Bad Request" error

  @NHSO-470
  @backend
  Scenario: Requesting available appointment slots with a fromDate parameter not in the expected format returns "Bad Request" error
    Given I have logged in and have a valid session cookie
    When I try to retrieve appointment slots with a malformed from Date
    Then I receive a "Bad Request" error

  @NHSO-470
  @backend
  Scenario: Requesting available appointment slots with a toDate parameter not in the expected format returns "Bad Request" error
    Given I have logged in and have a valid session cookie
    When I try to retrieve appointment slots with a malformed to Date
    Then I receive a "Bad Request" error

  @NHSO-470
  @backend
  Scenario: Requesting available appointment slots when GP system is unavailable returns "Bad gateway" error
    Given I have logged in and have a valid session cookie
    When the available appointment slots are retrieved for explicit date-time range
    Then I receive a "Bad Gateway" error

  @NHSO-470
  @backend
  Scenario: Requesting available appointment slots the GP system times out and returns "Gateway Timeout" error
    Given the system will time out when trying to retrieve appointment slots
    And I have logged in and have a valid session cookie
    When the available appointment slots are retrieved for explicit date-time range
    Then I receive a "Gateway Timeout" error
