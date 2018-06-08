Feature: View available appointment slots

  Users can view available appointments from the Appointments Page.

  Background:
    Given wiremock is initialised

  @bug @NHSO-922
  Scenario: A user who is signed in sees the appointments button
    Given I am on the appointments booking page
    Then I see the appointments menu button

  @pending
  @native
  @mobile
  Scenario: A user sees available appointment slots
    Given I am on the appointments booking page
    And there are available appointment slots
    Then the appointment slots are ordered ascending start date and time then first clinician name

  @pending
  @native
  @mobile
  Scenario: Available slots with the location length greater than 24 characters is truncated
    Given I am on the appointments booking page
    And there are available appointment slots with long location name
    Then I see available slots with the location length greater than 24 characters is truncated

  @pending
  @native
  @mobile
  Scenario: Available slots with the location length less or equal than 24 characters is shown in full
    Given I am on the appointments booking page
    And there are available appointment slots with location name length less or equal 24 characters
    Then I see available slots with the location length less or equal than 24 characters is shown in full

  @pending
  @native
  @mobile
  Scenario: Available slots with the Clinician Name length greater than 24 characters is truncated
    Given I am on the appointments booking page
    And there are available appointment slots with long clinician name
    Then I see available slots with the Clinician Name length greater than 24 characters is truncated

  @pending
  @native
  @mobile
  Scenario: Available slots with the Clinician Name length less or equal than 24 characters is shown in full
    Given I am on the appointments booking page
    And there are available appointment slots with the Clinician Name length less or equal than 24 characters
    Then I see available slots with the Clinician Name length less or equal than 24 characters is shown in full

  @pending
  @native
  @mobile
  Scenario: Available slots with the Session Name length greater than 24 characters is truncated
    Given I am on the appointments booking page
    And there are available appointment slots with long session name
    Then I see available slots with the Session Name length greater than 24 characters is truncated

  @pending
  @native
  @mobile
  Scenario: Available slots with the Session Name length less or equal 24 characters is shown in full
    Given I am on the appointments booking page
    And there are available appointment slots with the Session Name length less or equal than 24 characters
    Then I see available slots with the Session Name length less or equal 24 characters is shown in full

  @pending
  @native
  @mobile
  Scenario: Available slots display date in correct format (day-of-the-week day month year)
    Given I am on the appointments booking page
    And there are available appointment slots
    Then I see available slots display date in correct format

  @pending
  @native
  @mobile
  Scenario: Available slots display start time in correct format includes AM or PM
    Given I am on the appointments booking page
    And there are available appointment slots with some in BST and some in GMT
    Then each slot displays the start time in the timezone effective on that date

  @pending
  @native
  @mobile
  Scenario: Available slots display start time in timezone effective on that date
    Given I am on the appointments booking page
    And there are available appointment slots with some in BST and some in GMT
    Then each slot displays the start time in the timezone effective on that date

  @pending
  @native
  @mobile
  Scenario: A user sees appropriate information message when no slots are available
    Given I am on the appointments booking page
    And there are no available slots
    Then I see appropriate information message when no slots are available

  @NHSO-616
  Scenario: A user sees appropriate information message when there is a timeout
    Given GP system doesn't respond a timely fashion for available appointment slots
    When I am on the appointments booking page
    Then I see appropriate information message for time-outs
    And there should be a button to try again

  @NHSO-616
  Scenario: A user tries again after a timeout and it times-out again
    Given GP system doesn't respond a timely fashion for available appointment slots
    And I am on the appointments booking page
    When I click try again button on appointment page
    Then I see appropriate information message for time-outs
    And there should be a button to try again

  @NHSO-616
  Scenario: A user tries again after a timeout and it is now successful
    Given GP system doesn't respond a timely fashion for available appointment slots
    And I am on the appointments booking page
    When GP system responds a timely fashion for available appointment slots
    And I click try again button on appointment page
    Then I see available appointment slots

  @NHSO-616
  Scenario: A user sees appropriate information message when GP system is unavailable
    Given GP system is unavailable for available appointment slots
    When I am on the appointments booking page
    Then I see appropriate information message when there is a error retrieving data
    And there should not be an option to try again

  @NHSO-616
  Scenario: A user sees appropriate information message when GP system returns corrupt data
    Given GP system returns corrupt data for appointment slots
    When I am on the appointments booking page
    Then I see appropriate information message when there is a error retrieving data
    And there should not be an option to try again

  @NHSO-616
  @native
  @manual
  Scenario: A user sees appropriate information message when internet connection has been lost
    Given I am logged in
    And internet connection drops
    When I click appointments button in menu
    Then I see appropriate information message when there is no internet connection
    And there should be a button to try again

  @NHSO-1168
  Scenario: A user has problems with prescriptions and selects appointments and prescriptions in quick succession
    Given there are available appointment slots
    But there is a slight delay in retrieving them
    And I am on the appointments booking page
    When I navigate to Prescriptions
    And I wait 5 seconds
    Then I don't see appointment slots

  @NHSO-1168
  Scenario: A user has different problems with prescriptions and appointments and selects appointments and prescriptions in quick succession
    Given GP system doesn't respond a timely fashion for available appointment slots
    And I am on the appointments booking page
    When I navigate to Prescriptions
    And I wait 11 seconds
    Then I don't see a time-out error

  @NHSO-617
  @pending  @NHSO-617
  @mobile
  @native
  Scenario: A user sees appropriate information message when GP system is not offering the service
    Given I am logged in
    And GP system is not offering the service
    When I click appointments button in menu
    Then I see appropriate information message when there is a error retrieving data
    And there should not be an option to try again

  @NHSO-470
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
    Then I get a response with an empty set of slots

  @NHSO-470
  @backend
  Scenario: Requesting available appointment slots returns an unknown exception, returns a Bad Gateway error
    Given I have logged in and have a valid session cookie
    But an unknown exception will occur when wanting to view appointment slots
    When the available appointment slots are retrieved for explicit date-time range
    Then I receive a "Bad Gateway" error
