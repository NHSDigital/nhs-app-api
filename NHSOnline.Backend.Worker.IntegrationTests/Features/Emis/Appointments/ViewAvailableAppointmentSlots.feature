Feature: View available appointment slots
  In order book appointment
  As a logged user
  I want to see available appointment slots

  #NHSO-Session-Id expected format is GUID
  #fromDate parameter format is date-time
  #toDate parameter format is date-time

  @wip
  Scenario: Requesting available appointment slots with correct data returns lists of available slots, locations, clinicians, and appointment sessions

  @wip
  Scenario: Each available slot contains an id, start date and time, end date and time, location identifier, appointment session identifier, clinician identifier

  @wip
  Scenario: Each available location contains an id and display name

  @wip
  Scenario: Each available clinician contains an id and display name

  @wip
  Scenario: Each available appointment session contains an id and display name

  @wip
  Scenario: Requesting available appointment slots when no slots are available returns empty set of slots

  @wip
  Scenario: Requesting available appointment slots when no location are available returns empty set of locations

  @wip
  Scenario: Requesting available appointment slots when no clinician are available returns empty set of clinicians

  @wip
  Scenario: Requesting available appointment slots when no appointment session are available returns empty set of appointment sessions

  @wip
  Scenario: Requesting available appointment slots when online appointment booking is not offered by the practice returns empty set of slots

  @wip
  Scenario: Requesting available appointment slots when online appointment booking is offered by the practice, but not for this patient returns empty set of slots

  @wip
  Scenario: Requesting available appointment slots when online appointment booking is offered by practice, and patient, but there will never never be any slots available returns empty set of slots

  @wip
  Scenario: Requesting available appointment slots by patient whose session expired returns "Unauthorized" error

  @wip
  Scenario: Requesting available appointment slots with a missing NHSO-Session-Id cookie returns "Unauthorized" error

  @wip
  Scenario: Requesting available appointment slots with a NHSO-Session-Id cookie not in the expected format returns "Unauthorized" error

  @wip
  Scenario: Requesting available appointment slots without fromDate and toDate parameters returns set of appointment slots for the next 2 weeks from now

  @wip
  Scenario: Requesting available appointment slots with fromDate and toDate parameters returns set of appointment slots within specified date range

  @wip
  Scenario: Requesting available appointment slots with only fromDate parameter returns set of appointment slots for 2 weeks from specified start date

  @wip
  Scenario: Requesting available appointment slots with only toDate parameter returns set of appointment slots for 2 weeks from start day 2 weeks before end date

  @wip
  Scenario: Requesting available appointment slots with fromDate parameter that is after toDate parameter returns "Bad request"

  @wip
  Scenario: Requesting available appointment slots with fromDate and toDate parameter in the past returns empty set of appointment slots

  @wip
  Scenario: Requesting available appointment slots with a fromDate parameter not in the expected format returns "Bad Request" error

  @wip
  Scenario: Requesting available appointment slots with a toDate parameter not in the expected format returns "Bad Request" error

  @wip
  Scenario: Requesting available appointment slots when the GP practice has disabled this functionality returns empty list of slots

  @wip
  Scenario: Requesting available appointment slots when GP system is unavailable returns "Bad gateway" error

  @wip
  Scenario: Requesting available appointment slots the GP system times out and returns "Gateway Timeout" error
