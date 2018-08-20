@appointment
Feature: Ability to cancel an TPP appointment

  @NHSO-876
  Scenario: A TPP user is presented with the cancel appointment screen
    Given I have upcoming appointments for TPP
    And I am logged in as a TPP user
    And I am on my appointments page
    When I select a "Cancel appointment" link
    Then I will be on the "Cancellation reason" screen
    And I am presented with the appointment details
    And cancellation reasons drop-down is hidden

  @NHSO-1026
  @smoketest
  Scenario: A TPP user can cancel an appointment
    Given TPP is available to cancel a previously booked appointment
    And I am logged in as a TPP user
    And I am on my appointments page
    And I select a "Cancel appointment" link
    Then I will be on the "Cancellation reason" screen
    When I select "Cancel appointment" button
    Then I will be on the My appointments screen
    And a "Cancellation confirmed" message is displayed
