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
  Scenario: Each available slot contains an id, start date and time, end date and time, location identifire, appointment session identifire, clinician identifire

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
  Scenario: Requesting available appointment slots with a missing NHSO-Session-Id retuns "Bad request" error

  @wip
  Scenario: Requesting available appointment slots with a NHSO-Session-Id not in the expected format retuns "Bad request" error

  @wip
  Scenario: Requesting available appointment slots with a missing fromDate parameter returns "Bad Request" error

  @wip
  Scenario: Requesting available appointment slots with a fromDate parameter not in the expected format returns "Bad Request" error

  @wip
  Scenario: Requesting available appointment slots with a missing toDate parameter returns "Bad Request" error

  @wip
  Scenario: Requesting available appointment slots with a toDate parameter not in the expected format returns "Bad Request" error

  @wip
  Scenario: Requesting available appointment slots when the GP practice has disabled this functionality returns empty list of slots

  @wip
  Scenario: Requesting available appointment slots by patient whos session expired returns "Not authorized" error

  @wip
  Scenario: Requesting available appointment slots when GP system is unavailable returns "bad gateway" error
