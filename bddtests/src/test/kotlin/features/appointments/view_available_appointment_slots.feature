Feature: View available appointment slots

  Users can view available appointments from the Appointments Page.

  Background:
    Given wiremock is initialised

  Scenario: A user who is signed in sees the appointments navigation button highlighted
    Given I am on the appointments booking page
    Then the appointments menu button is highlighted

  @NHSO-71
  @pending    @NHSO-71
  @smoketest
  Scenario: A user sees available appointment slots
    Given I am on the appointments booking page
    And there are available appointment slots
    Then the appointment slots are ordered ascending start date and time then first clinician name

  @NHSO-71
  @pending    @NHSO-71
  Scenario: A user sees appropriate information message when no slots are available
    Given I am on the appointments booking page
    And there are no available slots
    Then I see appropriate information message when no slots are available

  @NHSO-71
  @pending    @NHSO-71
  Scenario: A user sees 'Book this appointment' button position unchanged when scrolling.
    Given I am logged in
    When I click appointments button in menu
    Then I see Book this appointment button position unchanged when scrolling

  @NHSO-71
  @pending    @NHSO-71
  Scenario: A user selects a second appointment slot and the first selected slot gets deselected.
    Given I am logged in
    And there are available appointment slots
    When I click appointments button in menu
    And I selected appointment slot and changed to another slot
    Then I see first selected slot gets deselected

  @NHSO-71
  @pending    @NHSO-71
  @smoketest
  Scenario: A user is navigated to 'Appointments Confirmation' screen when the user selects a slot and  clicks on 'Book this appointment' button.
    Given I am logged in
    And there are available appointment slots
    When I click appointments button in menu
    And I select appointment slot and click Book this appointment button
    Then I see the Appointment Confirmation screen

  @NHSO-71
  @pending    @NHSO-71
  Scenario: A user select the same appointment twice and the selected appointment stay selected.
    Given I am logged in
    And there are available appointment slots
    When I click appointments button in menu
    And I select same appointment twice
    Then I see selected appointment stay selected

  @NHSO-616
  Scenario: A user sees appropriate information message when there is a timeout
    Given GP system doesn't respond a timely fashion for available appointment slots
    When I try to progress to the appointments booking page
    Then I see appropriate information message for time-outs
    And there should be a button to try again

  @NHSO-616
  Scenario: A user tries again after a timeout and it times-out again
    Given GP system doesn't respond a timely fashion for available appointment slots
    And I try to progress to the appointments booking page
    When I click try again button on appointment page
    Then I see appropriate information message for time-outs
    And there should be a button to try again

  @NHSO-616
  Scenario: A user tries again after a timeout and it is now successful
    Given GP system doesn't respond a timely fashion for available appointment slots
    And I try to progress to the appointments booking page
    When GP system responds a timely fashion for available appointment slots
    And I click try again button on appointment page
    Then I see available appointment slots

  @NHSO-616
  Scenario: A user sees appropriate information message when GP system is unavailable
    Given GP system is unavailable for available appointment slots
    When I try to progress to the appointments booking page
    Then I see appropriate information message when there is a error retrieving data
    And there should not be an option to try again

  @NHSO-616
  Scenario: A user sees appropriate information message when GP system returns corrupt data
    Given GP system returns corrupt data for appointment slots
    When I try to progress to the appointments booking page
    Then I see appropriate information message when there is a error retrieving data
    And there should not be an option to try again

  @NHSO-616
  @native
  @manual
  Scenario: A user sees appropriate information message when internet connection has been lost
    Given I am on my appointments page
    And internet connection drops
    When I press the "Book this appointment" button
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

