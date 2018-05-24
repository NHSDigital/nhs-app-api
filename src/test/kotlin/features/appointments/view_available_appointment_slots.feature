Feature: View available appointment slots

  Users can view available appointments from the Appointments Page.

  Background:
    Given wiremock is initialised


  @bug @NHSO-922
  Scenario: A user who is signed in sees the appointments button
    Given I am on the appointments page
    Then I see the appointments menu button

  @pending
  Scenario: A user sees available appointment slots
    Given I am on the appointments page
    And there are available appointment slots
    Then the appointment slots are ordered ascending start date and time then first clinician name

  @pending
  Scenario: Available slots with the location length greater than 24 characters is truncated
    Given I am on the appointments page
    And there are available appointment slots with long location name
    Then I see available slots with the location length greater than 24 characters is truncated

  @pending
  Scenario: Available slots with the location length less or equal than 24 characters is shown in full
    Given I am on the appointments page
    And there are available appointment slots with location name length less or equal 24 characters
    Then I see available slots with the location length less or equal than 24 characters is shown in full

  @pending
  Scenario: Available slots with the Clinician Name length greater than 24 characters is truncated
    Given I am on the appointments page
    And there are available appointment slots with long clinician name
    Then I see available slots with the Clinician Name length greater than 24 characters is truncated

  @pending
  Scenario: Available slots with the Clinician Name length less or equal than 24 characters is shown in full
    Given I am on the appointments page
    And there are available appointment slots with the Clinician Name length less or equal than 24 characters
    Then I see available slots with the Clinician Name length less or equal than 24 characters is shown in full

  @pending
  Scenario: Available slots with the Session Name length greater than 24 characters is truncated
    Given I am on the appointments page
    And there are available appointment slots with long session name
    Then I see available slots with the Session Name length greater than 24 characters is truncated

  @pending
  Scenario: Available slots with the Session Name length less or equal 24 characters is shown in full
    Given I am on the appointments page
    And there are available appointment slots with the Session Name length less or equal than 24 characters
    Then I see available slots with the Session Name length less or equal 24 characters is shown in full

  @pending
  Scenario: Available slots display date in correct format (day-of-the-week day month year)
    Given I am on the appointments page
    And there are available appointment slots
    Then I see available slots display date in correct format

  @pending
  Scenario: Available slots display start time in correct format includes AM or PM
    Given I am on the appointments page
    And there are available appointment slots with some in BST and some in GMT
    Then each slot displays the start time in the timezone effective on that date

  @pending
  Scenario: Available slots display start time in timezone effective on that date
    Given I am on the appointments page
    And there are available appointment slots with some in BST and some in GMT
    Then each slot displays the start time in the timezone effective on that date

  @pending
  Scenario: A user sees appropriate information message when no slots are available
    Given I am on the appointments page
    And there are no available slots
    Then I see appropriate information message when no slots are available

  @pending
  Scenario: A user sees appropriate information message when GP system is unavailable
    Given I am on the appointments page
    And GP system is unavailable
    Then I see appropriate information message when GP system is unavailable

  @pending
  @backend
  Scenario: Valid EMIS data
    Given I have a valid EMIS request
    When I communicate with EMIS
    Then I get a response with available appointment slots
    And each slot contains an id
    And each slot contains a start date and time
    And each slot contains an end date and time
    And each slot contains a location identifier
    And each slot contains an appointment session identifier
    And each slot contains a clinician identifier
    And each available location contains an id and display name
    And each available clinician contains an id and display name
    And each available appointment session contains an id and display name


  @pending
  @backend
  Scenario: There are no slots available
    Given I have a valid EMIS request
    But there are no available slots
    When I communicate with EMIS
    Then I get a response with an empty set of slots

  @pending
  @backend
  Scenario: There are no locations available
    Given I have a valid EMIS request
    But there are no available locations
    When I communicate with EMIS
    Then I get a response with an empty set of locations

  @pending
  @backend
  Scenario: There are no clinicians available
    Given I have a valid EMIS request
    But there are no available clinicians
    When I communicate with EMIS
    Then I get a response with an empty set of clinicians

  @pending
  @backend
  Scenario: There are no appointment sessions available
    Given I have a valid EMIS request
    But there are no available appointment sessions
    When I communicate with EMIS
    Then I get a response with an empty set of appointment sessions

  @pending
  @backend
  Scenario: Online appointment booking is not offered
    Given I have a valid EMIS request
    But the practice does not offer online booking
    When I communicate with EMIS
    Then I get a response with an empty set of slots

  @pending
  @backend
  Scenario: Online appointment booking is not offered to a particular patient
    Given I have a valid EMIS request
    But the practice does not offer online booking to my patient
    When I communicate with EMIS
    Then I get a response with an empty set of slots

  @pending
  @backend
  Scenario: The patient's session has expired
    Given I have a valid EMIS request
    But the patient's session has expired
    When I communicate with EMIS
    Then I get a "Unauthorized" error

  @pending
  @backend
  Scenario: Missing NHSO-Session-Id cookie
    Given I have a valid EMIS request
    But no NHSO-Session-Id cookie
    When I communicate with EMIS
    Then I get a "Unauthorized" error

  @pending
  @backend
  Scenario: NHSO-Session-Id cookie in unexpected format
    Given I have a valid EMIS request
    But the NHSO-Session-Id cookie is in an unexpected format
    When I communicate with EMIS
    Then I get a "Unauthorized" error

  @pending
  @backend
  Scenario: No fromDate and toDate
    Given I have a valid EMIS request
    But no fromDate
    And no toDate
    When I communicate with EMIS
    Then Then I get a response with slots for the next two weeks

  @pending
  @backend
  Scenario: fromDate and toDate
    Given I have a valid EMIS request
    And fromDate of <now>
    And toDate of <now+three weeks>
    When I communicate with EMIS
    Then Then I get a response with slots for the next three weeks

  @pending
  @backend
  Scenario: fromDate only
    Given I have a valid EMIS request
    And fromDate of <now>
    But no toDate
    When I communicate with EMIS
    Then I get a response with slots for the next two weeks

  @pending
  @backend
  Scenario: toDate only
    Given I have a valid EMIS request
    And toDate of <now+three weeks>
    But no fromDate
    When I communicate with EMIS
    Then I get a response with slots for two weeks before <now+three weeks>

  @pending
  @backend
  Scenario: toDate before fromDate
    Given I have a valid EMIS request
    And fromDate of <now+two weeks>
    And toDate of <now>
    When I communicate with EMIS
    Then I get a "Bad request" error

  @pending
  @backend
  Scenario: fromDate and toDate in the past
    Given I have a valid EMIS request
    And fromDate of <now-three weeks>
    And toDate of <now-two weeks>
    When I communicate with EMIS
    Then Then I get a response with an empty set of slots

  @pending
  @backend
  Scenario: fromDate in unexpected format
    Given I have a valid EMIS request
    But the fromDate is in an unexpected format
    When I communicate with EMIS
    Then I get a "Bad Request" error

  @pending
  @backend
  Scenario: toDate in unexpected format
    Given I have a valid EMIS request
    But the toDate is in an unexpected format
    When I communicate with EMIS
    Then I get a "Bad Request" error

  @pending
  @backend
  Scenario: GP practice has disabled slots functionality
    Given I have a valid EMIS request
    But the GP Practice has disabled slots functionality
    When I communicate with EMIS
    Then I get a response with an empty set of slots

  @pending
  @backend
  Scenario: GP System Unavailable
    Given I have a valid EMIS request
    But the GP System is unavailable
    When I communicate with EMIS
    Then I get a "Bad gateway" error

  @pending
  @backend
  Scenario: GP System Times Out
    Given I have a valid EMIS request
    But the GP System times out
    When I communicate with EMIS
    Then I get a "Gateway timeout" error
