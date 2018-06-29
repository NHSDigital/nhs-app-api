Feature: Ability to cancel an appointment

  Users are given the ability to cancel an appointment.

  Background:
    Given wiremock is initialised

  @NHSO-663
  Scenario: A user is presented with the cancel appointment screen
    Given I have upcoming appointments
    And I am on the appointments page
    When I select a "Cancel appointment" link
    Then I will be on the "Cancellation reason" screen
    And I am presented with the appointment details
    And there is a cancellation reasons drop-down

  @NHSO-663
  Scenario: A user navigates back to the "My appointments" screen
    Given I am on the appointment cancellation screen
    When I select "Back" button
    Then I will be on the My appointments screen

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
  Scenario Outline: User cancels appointment with selected reason
    Given I am on the appointment cancellation screen
    And the Emis is available to cancel an appointment for <Reason>
    And I select a cancellation reason of <Reason>
    When I select "Cancel appointment" button
    Then I will be on the My appointments screen
    And a "Cancellation confirmed" message is displayed
    Examples:
      | Reason             |
      | No longer required |
      | Unable to attend   |

  @NHSO-1026
  Scenario: A validation message will be displayed if no reason is provided
    Given I am on the appointment cancellation screen
    When I select "Cancel appointment" button
    Then I will receive a cancellation validation error
