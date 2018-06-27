@pending
Feature: Ability to cancel an appointment

  Users are given the ability to cancel an appointment.

  Background:
    Given wiremock is initialised

  @NHSO-663
  @pending  @NHSO-663
  Scenario: A user is presented with the cancel appointment screen
    Given I have upcoming appointments
    And I am on the appointments page
    When I select a "Cancel appointment" link
    Then I will be on the "Cancellation reason" screen
    And I am presented with the appointment details
    And there is a cancellation reasons drop-down with the appropriate reasons
      | No longer required |
      | Unable to attend   |

  @NHSO-663
  @pending  @NHSO-663
  Scenario: A user navigates back to the "My appointments" screen
    Given I am on the cancellation screen
    When I select "Back to my appointments" button
    Then I will be on the "My appointments" screen

  @NHSO-1027
  @backend
  Scenario: API will cancel the appointment if valid reason is provided
    Given the Emis is available to cancel an appointment
    When I send a cancellation request to the API with a valid cancellation reason
    Then I will receive a successful response

  @NHSO-1027
  @backend
  Scenario: API will not cancel the appointment if reason an invalid reason is provided
    Given the Emis is available to cancel an appointment
    When I send a cancellation request to the API with an invalid cancellation reason
    Then I receive a "Bad request" error

  @NHSO-1026
  @pending  @NHSO-1026
    # Remove cancellation reason assertion in first scenario, once this is implemented
  Scenario Outline: User cancels appointment with selected reason
    Given I am on the cancellation screen
    And I select a cancellation reason of <Reason>
    When I select "Cancel appointment" button
    Then I will be on the "My appointments" screen
    And a "Cancellation confirmed" message is displayed
    Examples:
      | Reason             |
      | No longer required |
      | Unable to attend   |

  @NHSO-1026
  @pending  @NHSO-1026
  Scenario: A validation message will be displayed if no reason is provided
    Given I am on the cancellation screen
    When I select "Cancel appointment" button
    Then I will receive a cancellation validation error

  @NHSO-722
  @pending  @NHSO-722
  Scenario: A user sees appropriate information message when there is a timeout
    Given GP system doesn't respond a timely fashion when cancelling an appointment
    And I am on the cancellation screen
    And I select any cancellation reason
    When I select "Cancel appointment" button
    Then I see appropriate information message after 10 seconds when it times-out when cancelling
    And there should be a button to go back to my appointments

  @NHSO-722
  @pending  @NHSO-722
  Scenario: A user sees appropriate information message when GP system is unavailable
    Given GP system is unavailable when cancelling an appointment
    And I am on the cancellation screen
    And I select any cancellation reason
    When I select "Cancel appointment" button
    Then I see appropriate information message when there is an error sending data when cancelling
    And there should be a button to go back to my appointments

  @NHSO-722
  @pending  @NHSO-722
  Scenario: A user sees appropriate information message when appointment has already been cancelled
    Given the appointment slot has already been cancelled
    And I am on the cancellation screen
    And I select any cancellation reason
    When I select "Cancel appointment" button
    Then I see appropriate information message when appointment has already been cancelled
    And there should be a button to go back to my appointments

  @NHSO-722
  @pending  @NHSO-722
  Scenario: A user returns directly back to their appointments after trying to cancel one already cancelled
    Given the appointment slot has already been cancelled
    And I try to cancel it again
    When I click the button to go back to my appointments
    Then I will be on the My appointments screen

  @NHSO-722
  @manual
  Scenario: A user's session expires
    Given I am on the cancellation screen
    When I allow my session to expire
    Then I should return to the sign in screen with the session expiry message
    And there should be a button to try again

  @NHSO-722
  @native
  @manual
  Scenario: A user sees appropriate information message when internet connection has been lost
    Given I am on the cancellation screen
    And internet connection drops
    When I select "Cancel appointment" button
    Then I see appropriate information message when there is no internet connection
    And there should be a button to try again

  @NHSO-722
  @native
  @manual
  Scenario: A user can try again when connection is restored
    Given I am already informed that I have lost the connection on "Cancellation" screen
    But the internet connection returns
    When I click the try again button
    Then I will be on the "Cancellation reason" screen