Feature: My appointments

  Users can view their upcoming and past appointments in the My Appointments screen.

  Background:
    Given wiremock is initialised


  @NHSO-524
  @backend
  Scenario: API appropriately filters for upcoming appointments
    Given I have upcoming appointments
    And I have logged in and have a valid session cookie
    When the API retrieves upcoming appointments from "Emis"
    Then I will only receive upcoming appointments
    And a list of cancellation reasons

  @NHSO-524
  Scenario: A user has never booked an appointment
    Given I have no upcoming appointments
    When I am on the appointments page
    Then I am informed I have no booked appointments
    But I can book an appointment

  @NHSO-196
  @NHSO-663
  @pending  @NHSO-196   @NHSO-663
  @bug  @NHSO-1591
  @smoketest
  Scenario: A user has upcoming appointments
    Given I have upcoming appointments
    When I am on the appointments page
    Then I will be on the "My appointments" screen
    And I am given the list of upcoming appointments
    And appointments are in chronological order
#    And each appointment can be cancelled
    And I can book an appointment

  @NHSO-525
  @pending  @NHSO-525
  Scenario: A user sees appropriate information message when there is a timeout
    Given GP system doesn't respond a timely fashion when retrieving my appointments
    When I am on the appointments page
    Then I see appropriate information message for time-outs
    And there should be a button to try again

  @NHSO-525
  @pending  @NHSO-525
  Scenario: A user tries again after a timeout and it times-out again
    Given GP system doesn't respond a timely fashion when retrieving my appointments
    And I am on the appointments page
    When I click try again button on appointment page
    Then I see appropriate information message for time-outs
    And there should be a button to try again

  @NHSO-525
  @pending  @NHSO-525
  Scenario: A user tries again after a timeout and it is now successful
    Given GP system doesn't respond a timely fashion when retrieving my appointments
    And I am on the appointments page
    When GP system responds in a timely fashion when retrieving my appointments
    And I click try again button on appointment page
    Then I will be on the "My appointments" screen

  @NHSO-525
  @pending  @NHSO-525
  Scenario: A user sees appropriate information message when GP system is unavailable
    Given GP system is unavailable when retrieving my appointments
    When I am on the appointments page
    Then I see appropriate information message when there is a error retrieving data
    And there should not be an option to try again

  @NHSO-525
  @pending  @NHSO-525
  Scenario: A user sees appropriate information message when GP system returns corrupt data
    Given GP system returns corrupt data when retrieving my appointments
    When I am on the appointments page
    Then I see appropriate information message when there is a error retrieving data
    And there should not be an option to try again

  @NHSO-525
  @manual
  Scenario: A user's session expires
    Given I am on the appointments page
    When I allow my session to expire
    Then I should return to the sign in screen with the session expiry message
    And there should be a button to try again

  @NHSO-525
  @native
  @manual
  Scenario: A user sees appropriate information message when internet connection has been lost
    Given I am logged in
    And internet connection drops
    When I click appointments button in menu
    Then I see appropriate information message when there is no internet connection
    And there should be a button to try again

  @NHSO-525
  @native
  @manual
  Scenario: A user can try again when connection is restored
    Given I am already informed that I have lost the connection on "My appointments" screen
    But the internet connection returns
    When I click the try again button
    Then I will be on the "My appointments" screen
