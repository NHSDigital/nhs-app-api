Feature: Book an available appointment slot
  In order to complete a booking appointment
  As a logged in user
  I want to be able to select, confirm and book selected appointment

  Background:
    Given wiremock is initialised
    And there are available appointment slots

  @pending  @NHSO-986
  @native
  @mobile
  Scenario: A user sees 'Book this appointment' button is disabled when no appointment slot is selected.
    Given I am logged in
    When I click appointments button in menu
    Then I see Book this appointment button is disabled

  @pending  @NHSO-986
  @native
  @mobile
  Scenario: A user sees 'Book this appointment' button is enabled when an appointment slot is selected.
    Given I am logged in
    And there are available appointment slots
    When I click appointments button in menu
    And I selected appointment slot
    Then I see Book this appointment button is enabled

  @pending  @NHSO-986
  @manual   @NHSO-528
  @native
  @mobile
  Scenario: A user sees 'Book this appointment' button position unchanged when scrolling.
    Given I am logged in
    When I click appointments button in menu
    Then I see Book this appointment button position unchanged when scrolling

  @pending  @NHSO-986
  @native
  @mobile
  Scenario: A user selects a second appointment slot and the first selected slot gets deselected.
    Given I am logged in
    And there are available appointment slots
    When I click appointments button in menu
    And I selected appointment slot and changed to another slot
    Then I see first selected slot gets deselected

  @pending  @NHSO-986
  @native
  @mobile
  Scenario: A user is navigated to 'Appointments Confirmation' screen when the user selects a slot and  clicks on 'Book this appointment' button.
    Given I am logged in
    And there are available appointment slots
    When I click appointments button in menu
    And I select appointment slot and click Book this appointment button
    Then I see the Appointment Confirmation screen

  @pending  @NHSO-986
  @native
  @mobile
  Scenario: A user sees 'Change this appointment' button on 'Appointments Confirmation' screen and the button is enabled.
    Given I am logged in
    And there are available appointment slots
    When I click appointments button in menu
    And I select appointment slot and click Book this appointment button
    Then I see Change this appointment button on Appointment Confirmation screen

  @pending  @NHSO-986
  @native
  @mobile
  Scenario: A user sees the 'Appointments' link is highlighted on the navigation menu when on the appointment confirmation page
    Given I am logged in
    And there are available appointment slots
    When I click appointments button in menu
    And I select appointment slot and click Book this appointment button
    Then I see Appointments link is highlighted on the navigation menu when on the appointment Confirmation screen

  @pending  @NHSO-986
  @native
  @mobile
  Scenario: A user is navigated back to the 'Book this appointment' screen when 'Change this appointment' button selected.
    Given I am logged in
    And there are available appointment slots
    When I click appointments button in menu
    And I select appointment slot and click Book this appointment button
    Then I see Book this appointment screen when Change this appointment button is clicked

  @pending  @NHSO-986
  @pending  @NHSO-919
  @native
  @mobile
  Scenario: A user select the same appointment twice and the selected appointment stay selected.
    Given I am logged in
    And there are available appointment slots
    When I click appointments button in menu
    And I select same appointment twice
    Then I see selected appointment stay selected

  @NHSO-72
  Scenario: A user tries to book an appointment without describing symptoms
    Given I am on the appointments page
    And I have selected an appointment slot to book
    When I click the 'Confirm and book appointment' button
    Then an error is displayed that "Describe your symptoms" is mandatory

  @NHSO-72
  Scenario: A user tries to book an appointment describing symptoms at least 1 character
    Given I am on the appointments page
    And I have selected an appointment slot to book
    And I enter symptoms of 1 characters
    When I click the 'Confirm and book appointment' button
    Then Appointment Booking confirmation screen is displayed
    And booking request is successfully made with valid details

  @NHSO-72
  Scenario: A user tries to book an appointment describing symptoms no more 150 characters
    Given I am on the appointments page
    And I have selected an appointment slot to book
    And I enter symptoms of 150 characters
    When I click the 'Confirm and book appointment' button
    Then Appointment Booking confirmation screen is displayed
    And booking request is successfully made with valid details

  @NHSO-72
  Scenario: A user tries to enter symptoms with over 150 characters
    Given I am on the appointments page
    And I have selected an appointment slot to book
    When I enter symptoms of 151 characters
    Then only the first 150 characters will be displayed

  @NHSO-72
  Scenario: A user tries to paste symptoms with over 150 characters
    Given I am on the appointments page
    And I have selected an appointment slot to book
    When I paste symptoms of 151 characters
    Then only the first 150 characters will be displayed

  @NHSO-72
  Scenario: A user who books successfully, but only the first 150 characters of the symptoms are sent
    Given I am on the appointments page
    And I have selected an appointment slot to book
    And I enter symptoms of 151 characters
    When I click the 'Confirm and book appointment' button
    Then Appointment Booking confirmation screen is displayed
    And booking request is successfully made with valid details


        # To be removed once NHSO-517 is complete
  @NHSO-72
  Scenario: A user tries to book an appointment when there is a problem
    Given I am on the appointments page
    And I have selected an appointment slot to book
    And GP system doesn't respond a timely fashion for booking an appointment
    When I enter symptoms of 150 characters
    And I click the 'Confirm and book appointment' button
    Then I see appropriate information message when there is an error on appointment confirmation page
    

  @NHSO-517
  @pending  @NHSO-517
  Scenario: A user sees appropriate information message when there is a timeout
    Given GP system doesn't respond a timely fashion when booking an appointment
    And I have selected an appointment slot to book
    When I enter symptoms
    And  click the 'Confirm and book appointment' button
    Then I see appropriate information message after 10 seconds when it times-out
    And there should be a button to go back to my appointments

  @NHSO-517
  @pending  @NHSO-517
  Scenario: A user tries again after a timeout and it times-out again
    Given I have tried to book an appointment and it has timed-out
    And GP system still doesn't respond a timely fashion when booking an appointment
    And I have selected an appointment slot to book
    When I enter symptoms
    And  click the 'Confirm and book appointment' button
    Then I see appropriate information message after 10 seconds when it times-out
    And there should be a button to go back to my appointments

  @NHSO-517
  @pending  @NHSO-517
  Scenario: A user tries again after a timeout and it is now successful
    Given I have tried to book an appointment and it has timed-out
    And GP system will now allow the appointment to be successfully booked
    And I have selected an appointment slot to book
    When I enter symptoms
    And  click the 'Confirm and book appointment' button
    Then booking request is successfully made
    And Appointment Booking confirmation screen is displayed

  @NHSO-517
  @pending  @NHSO-517
  Scenario: A user sees appropriate information message when GP system is unavailable
    Given GP system is unavailable when booking an appointment
    And I have selected an appointment slot to book
    When I enter symptoms
    And  click the 'Confirm and book appointment' button
    Then I see appropriate information message when there is a error retrieving data
    And there should be a button to go back to my appointments

  @NHSO-517
  @pending  @NHSO-517
  Scenario: A user sees appropriate information message when appointment has already been booked
    Given I have selected an appointment slot to book
    And the appointment slot has already been booked by somebody else
    When I enter symptoms
    And  click the 'Confirm and book appointment' button
    Then I see appropriate information message when there is a error retrieving data
    And there should be a button to go back to my appointments

  @NHSO-517
  @pending  @NHSO-517
  Scenario: A user can return directly back to their appointments after trying to book one already booked
    Given I have tried to an appointment slot that has already been booked by somebody else
    When I click the button to go back to my appointments
    Then I will be on the "My Appointments" screen
