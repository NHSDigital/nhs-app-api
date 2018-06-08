Feature: View available appointment slots backend
  In order book appointment
  As a logged user
  I want to see available appointment slots

  @backend
  Scenario: Requesting available appointment slots with correct data returns lists of available slots, locations, clinicians, and appointment sessions
    Given there are available appointment slots for an explicit date-time range
    And I have logged in and have a valid session cookie
    When the available appointment slots are retrieved for explicit date-time range
    Then available slots, locations, clinicians and appointment sessions are returned for the given date-time range

  @backend
  Scenario: Each available slot contains an id, start date and time, end date and time, location identifier, appointment session identifier, clinician identifiers
    Given there are available appointment slots for an explicit date-time range
    And I have logged in and have a valid session cookie
    When the available appointment slots are retrieved for explicit date-time range
    Then available slots are returned containing id, start date and time, end date and time, location identifier, appointment session identifier, clinician identifiers

  @backend
  Scenario: Each available location contains an id and display name
    Given there are available appointment slots for an explicit date-time range
    And I have logged in and have a valid session cookie
    When the available appointment slots are retrieved for explicit date-time range
    Then available locations are returned containing an id and display name

  @backend
  Scenario: Each available clinician contains an id and display name
    Given there are available appointment slots for an explicit date-time range
    And I have logged in and have a valid session cookie
    When the available appointment slots are retrieved for explicit date-time range
    Then available clinicians are returned containing an id and display name

  @backend
  Scenario: Each available appointment session contains an id and display name
    Given there are available appointment slots for an explicit date-time range
    And I have logged in and have a valid session cookie
    When the available appointment slots are retrieved for explicit date-time range
    Then available appointment session are returned containing an id and display name

  @backend
  Scenario: Requesting available appointment slots when online appointment booking is not available to the patient, returns empty set of slots
    Given online appointment booking is not available to the patient, when wanting to view appointment slots
    And I have logged in and have a valid session cookie
    When the available appointment slots are retrieved for explicit date-time range
    Then empty sets of data are returned

  @backend
  Scenario: Requesting available appointment slots returns an unknown exception, returns a Bad Gateway error
    Given unknown exception will occur when wanting to view appointment slots
    And I have logged in and have a valid session cookie
    When the available appointment slots are retrieved for explicit date-time range
    Then I receive a "Bad Gateway" error

  @backend
  Scenario: Requesting available appointment slots by patient whose session expired returns "Unauthorized" error
    Given there are available appointment slots, but session has expired
    When the available appointment slots are retrieved for explicit date-time range
    Then I receive an "Unauthorized" error

  @backend
  Scenario: Requesting available appointment slots with a missing NHSO-Session-Id cookie returns "Unauthorized" error
    Given there are available appointment slots for an explicit date-time range
    When the available appointment slots are retrieved for explicit date-time range without a cookie
    Then I receive an "Unauthorized" error

  @backend
  @pending
  Scenario: Requesting available appointment slots without fromDate and toDate parameters returns set of appointment slots for the next 2 weeks from now
    Given there are available appointment slots within the next two weeks
    And I have logged in and have a valid session cookie
    When the available appointment slots are retrieved without a given date-time range
    #This last line is based on breaking down the request and asserting details from that. This seems incorrect
    Then available slots, locations, clinicians and appointment sessions are returned for the next two weeks

  @backend
  Scenario: Requesting available appointment slots with only fromDate parameter returns set of appointment slots for 2 weeks from specified start date
    Given there are available appointment slots two weeks from a specific from date
    And I have logged in and have a valid session cookie
    When the available appointment slots are retrieved with just a from date
    Then available slots, locations, clinicians and appointment sessions are returned for the two weeks following the from date

  @backend
  Scenario: Requesting available appointment slots with only toDate parameter returns set of appointment slots for 2 weeks from start day 2 weeks before end date
    Given there are available appointment slots two weeks preceding a specific to date
    And I have logged in and have a valid session cookie
    When the available appointment slots are retrieved with just a to date
    Then available slots, locations, clinicians and appointment sessions are returned for the two weeks preceding the to date

  @backend
  Scenario: Requesting available appointment slots with fromDate parameter that is after toDate parameter returns "Bad request"
    Given I have logged in and have a valid session cookie
    When I try to retrieve appointment slots with fromDate after the toDate
    Then I receive a "Bad Request" error

  @backend
  Scenario: Requesting available appointment slots with fromDate and toDate parameter in the past returns empty set of appointment slots
    Given I have logged in and have a valid session cookie
    When I try to retrieve appointment slots only from the past
    Then I receive a "Bad Request" error

  @backend
  Scenario: Requesting available appointment slots with a fromDate parameter not in the expected format returns "Bad Request" error
    Given I have logged in and have a valid session cookie
    When I try to retrieve appointment slots with a malformed from Date
    Then I receive a "Bad Request" error

  @backend
  Scenario: Requesting available appointment slots with a toDate parameter not in the expected format returns "Bad Request" error
    Given I have logged in and have a valid session cookie
    When I try to retrieve appointment slots with a malformed to Date
    Then I receive a "Bad Request" error

  @backend
  Scenario: Requesting available appointment slots when GP system is unavailable returns "Bad gateway" error
    Given I have logged in and have a valid session cookie
    When the available appointment slots are retrieved for explicit date-time range
    Then I receive a "Bad Gateway" error

  @backend
  Scenario: Requesting available appointment slots the GP system times out and returns "Gateway Timeout" error
    Given the system will time out when trying to retrieve appointment slots
    And I have logged in and have a valid session cookie
    When the available appointment slots are retrieved for explicit date-time range
    Then I receive a "Gateway Timeout" error
