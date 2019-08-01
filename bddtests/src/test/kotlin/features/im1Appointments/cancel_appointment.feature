@appointments
@cancel
Feature: Cancel Appointments Frontend

  #This test covers navigation via buttons/links

  @nativesmoketest
  Scenario Outline: <GP System> user is presented with the cancel appointment screen with a drop-down
    Given I have upcoming appointments before cutoff time for <GP System>
    And I am logged in
    And I am on the My Appointments page
    When I select a "Cancel this appointment" link
    Then I will be on the "Cancellation reason" screen
    And I am presented with the appointment details
    And there is a cancellation reasons drop-down
    Examples:
      | GP System |
      | EMIS      |

# These tests navigate directly to the pages where the features are to be tested, to save time.

  Scenario Outline: <GP System> user is presented with the cancel appointment screen with a drop-down
    Given I have upcoming appointments before cutoff time for <GP System>
    And I am logged in
    When I retrieve the 'My Appointments' page directly
    When I select a "Cancel this appointment" link
    Then I will be on the "Cancellation reason" screen
    And I am presented with the appointment details
    And there is a cancellation reasons drop-down
  @nativesmoketest
    Examples:
      | GP System |
      | VISION    |
      | MICROTEST |

  Scenario: A Vision user is presented with the cancel appointment screen without reason selected, when there is just one (appointments before cutoff time)
    Given I have upcoming appointments before cutoff time for VISION with only one cancellation reason
    And I am logged in
    When I retrieve the 'My Appointments' page directly
    When I select a "Cancel this appointment" link
    Then I will be on the "Cancellation reason" screen
    And I am presented with the appointment details
    And there is a cancellation reasons drop-down

  Scenario: A TPP user is presented with the cancel appointment screen without drop-down
    Given I have upcoming appointments before cutoff time for TPP
    And I am logged in
    When I retrieve the 'My Appointments' page directly
    When I select a "Cancel this appointment" link
    Then I will be on the "Cancellation reason" screen
    And I am presented with the appointment details
    And cancellation reasons drop-down is hidden

  Scenario Outline: A <GP System> user is presented with a validation message if no reason is selected
    Given I have upcoming appointments before cutoff time for <GP System>
    And I am logged in
    When I retrieve the 'My Appointments' page directly
    When I select a "Cancel this appointment" link
    And I select "Cancel appointment" button
    Then I will receive a cancellation validation error
    Examples:
      | GP System |
      | EMIS      |
      | VISION    |
      | MICROTEST |

  Scenario:  A VISION user is presented with a validation message if no reason is selected, even when there is just one
    Given I have upcoming appointments before cutoff time for VISION with only one cancellation reason
    And I am logged in
    When I retrieve the 'My Appointments' page directly
    When I select a "Cancel this appointment" link
    And I select "Cancel appointment" button
    Then I will receive a cancellation validation error


  Scenario Outline: <GP System> user can cancel an appointment with selected reason of <Reason>
    Given <GP System> is available to cancel a previously booked appointment before cutoff time because <Reason>
    And I am logged in
    When I retrieve the 'My Appointments' page directly
    And I select a "Cancel this appointment" link
    And I select a cancellation reason of <Reason>
    When I select "Cancel appointment" button
    Then the My Appointments page is displayed
    And a "Cancellation confirmed" message is displayed
  @smoketest
    Examples:
      | Reason             | GP System |
      | No longer required | EMIS      |

    Examples:
      | Reason             | GP System |
      | Unable to attend   | EMIS      |
      | Reason 1           | VISION    |
      | Reason 2           | VISION    |
      | No longer required | MICROTEST |
      | Unable to attend   | MICROTEST |

  Scenario:  Vision user can cancel appointment when there is just one reason
    Given VISION is available to cancel a previously booked appointment before cutoff time, with only one available reason
    And I am logged in
    When I retrieve the 'My Appointments' page directly
    And I select a "Cancel this appointment" link
    And I select the cancellation reason
    When I select "Cancel appointment" button
    Then the My Appointments page is displayed
    And a "Cancellation confirmed" message is displayed

  @smoketest
  Scenario: A TPP user can cancel an appointment
    Given TPP is available to cancel a previously booked appointment before cutoff time
    And I am logged in
    When I retrieve the 'My Appointments' page directly
    And I select a "Cancel this appointment" link
    Then I will be on the "Cancellation reason" screen
    When I select "Cancel appointment" button
    Then the My Appointments page is displayed
    And a "Cancellation confirmed" message is displayed


  Scenario Outline: <GP System> user navigates back to the "My appointments" screen
    Given I have upcoming appointments before cutoff time for <GP System>
    And I am logged in
    When I retrieve the 'My Appointments' page directly
    And I select a "Cancel this appointment" link
    When I select a "Back" button
    Then the My Appointments page is displayed
    Examples:
      | GP System |
      | TPP       |
      | VISION    |
      | MICROTEST |
  @nativesmoketest
    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: A <GP System> user sees appropriate information message when GP system is unavailable
    Given <GP System> is unavailable to cancel a previously booked appointment because <Reason>
    And I am logged in
    When I retrieve the 'My Appointments' page directly
    When I select a "Cancel this appointment" link
    And I select a cancellation reason of <Reason>
    When I select "Cancel appointment" button
    Then I see appropriate information message when there is an error sending data on appointment confirmation page
    And there should be an action to go back to my appointments
    Examples:
      | Reason           | GP System |
      | Unable to attend | EMIS      |
      | Reason 1         | VISION    |
      | Unable to attend | MICROTEST |

  @tech-debt   @NHSO-4061 # covered in Manual Regression Test pack
  Scenario: Cancelling appointment, when there is no internet connection should result with a message indicating user may have connectivity problems
    Given I have upcoming appointments before cutoff time for VISION
    And I am logged in
    When I retrieve the 'My Appointments' page directly
    And I select a "Cancel this appointment" link
    Then I will be on the "Cancellation reason" screen
    And I lose my internet connection
    When I select "Cancel appointment" button
    Then I see a message indicating user may have connectivity problems
