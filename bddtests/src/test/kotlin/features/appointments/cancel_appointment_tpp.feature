Feature: Ability to cancel an TPP appointment

  Background:
    Given I have upcoming appointments for TPP
    And I am logged in as a TPP user

  @NHSO-876
  @appointment
  Scenario: A TPP user is presented with the cancel appointment screen
    And I am on my appointments page
    When I select a "Cancel appointment" link
    Then I will be on the "Cancellation reason" screen
    And I am presented with the appointment details
    And cancellation reasons drop-down is hidden

  @NHSO-1026
  @appointment
  Scenario: User cancels TPP appointment
    Given TPP is available to cancel an appointment
    And I am on my appointments page
    And I select a "Cancel appointment" link
    When I select "Cancel appointment" button
    Then I will be on the My appointments screen
    And a "Cancellation confirmed" message is displayed
