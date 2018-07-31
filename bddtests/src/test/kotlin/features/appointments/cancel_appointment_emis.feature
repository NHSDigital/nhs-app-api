@appointment
Feature: Ability to cancel an EMIS appointment

  Background:
    Given I have upcoming appointments for EMIS
    And I am logged in as a EMIS user

  @NHSO-663
  Scenario: A user is presented with the cancel appointment screen
    Given I am on my appointments page
    When I select a "Cancel appointment" link
    Then I will be on the "Cancellation reason" screen
    And I am presented with the appointment details
    And there is a cancellation reasons drop-down

  @NHSO-663
  Scenario: A user navigates back to the "My appointments" screen
    Given I am on my appointments page
    And I select a "Cancel appointment" link
    When I select "Back" button
    Then I will be on the My appointments screen

  @NHSO-1026
  @tech-debt @NHSO-1937
  Scenario Outline: User cancels appointment with selected reason
    Given EMIS is available to cancel an appointment for <Reason>
    And I am on my appointments page
    And I select a "Cancel appointment" link
    And I select a cancellation reason of <Reason>
    When I select "Cancel appointment" button
    Then I will be on the My appointments screen
    And a "Cancellation confirmed" message is displayed
    Examples:
      | Reason             |
      | No longer required |
      | Unable to attend   |

  @NHSO-1026
  @tech-debt @NHSO-1937
  Scenario: A validation message will be displayed if no reason is provided
    Given I am on my appointments page
    And I select a "Cancel appointment" link
    When I select "Cancel appointment" button
    Then I will receive a cancellation validation error
