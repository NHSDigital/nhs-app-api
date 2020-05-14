@appointments
@cancel
Feature: Cancel Appointments Frontend

  #This test covers navigation via buttons/links

  @nativesmoketest
  Scenario: An EMIS user is presented with the cancel appointment screen with a drop-down
    Given I have upcoming appointments before cutoff time for EMIS
    And I am logged in
    And I am on the Your Appointments page
    When I select a "Cancel this appointment" link
    Then I will be on the "Cancellation reason" screen
    And I am presented with the appointment details
    And there is a cancellation reasons drop-down

# These tests navigate directly to the pages where the features are to be tested, to save time.

  @nativesmoketest
  Scenario Outline: <GP System> user is presented with the cancel appointment screen with a drop-down
    Given I have upcoming appointments before cutoff time for <GP System>
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    And I select a "Cancel this appointment" link
    Then I will be on the "Cancellation reason" screen
    And I am presented with the appointment details
    And there is a cancellation reasons drop-down
    Examples:
      | GP System |
      | VISION    |
      | MICROTEST |

  Scenario: A Vision user is presented with the cancel appointment screen without reason selected, when there is just one (appointments before cutoff time)
    Given I have upcoming appointments before cutoff time for VISION with only one cancellation reason
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    And I select a "Cancel this appointment" link
    Then I will be on the "Cancellation reason" screen
    And I am presented with the appointment details
    And there is a cancellation reasons drop-down

  Scenario: A TPP user is presented with the cancel appointment screen without drop-down
    Given I have upcoming appointments before cutoff time for TPP
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    And I select a "Cancel this appointment" link
    Then I will be on the "Cancellation reason" screen
    And I am presented with the appointment details
    And cancellation reasons drop-down is hidden

  Scenario Outline: A <GP System> user is presented with a validation message if no reason is selected
    Given I have upcoming appointments before cutoff time for <GP System>
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    And I select a "Cancel this appointment" link
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
    When I retrieve the 'Your GP Appointments' page directly
    And I select a "Cancel this appointment" link
    And I select "Cancel appointment" button
    Then I will receive a cancellation validation error

  Scenario Outline: <GP System> user can cancel an appointment with selected reason of <Reason>
    Given <GP System> is available to cancel a previously booked appointment before cutoff time because <Reason>
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    And I select a "Cancel this appointment" link
    And I select a cancellation reason of <Reason>
    And I select "Cancel appointment" button
    Then the Appointment Cancel success page is displayed
    And I select the back to home link on the appointments page
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

  Scenario: A Vision user can cancel appointment when there is just one reason
    Given VISION is available to cancel a previously booked appointment before cutoff time, with only one available reason
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    And I select a "Cancel this appointment" link
    And I select the cancellation reason
    And I select "Cancel appointment" button
    Then the Appointment Cancel success page is displayed
    And I select the back to home link on the appointments page

  Scenario: A TPP user can cancel an appointment
    Given TPP is available to cancel a previously booked appointment before cutoff time
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    And I select a "Cancel this appointment" link
    Then I will be on the "Cancellation reason" screen
    When I select "Cancel appointment" button
    Then the Appointment Cancel success page is displayed
    And I select the back to home link on the appointments page

  Scenario Outline: <GP System> user navigates back to the "Your appointments" screen
    Given I have upcoming appointments before cutoff time for <GP System>
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    And I select a "Cancel this appointment" link
    Then I will be on the "Cancellation reason" screen
    When I click the 'Your GP appointments' breadcrumb
    Then the Your Appointments page is displayed
    Examples:
      | GP System |
      | TPP       |
      | VISION    |
      | MICROTEST |
    @nativesmoketest
    Examples:
      | GP System |
      | EMIS      |

  #403
  Scenario Outline: A <GP System> user sees appropriate error message when it is not allowed to cancel
    Given <GP System> user is not allowed to cancel appointments with '<Reason>'
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    And I select a "Cancel this appointment" link
    And I select a cancellation reason of <Reason>
    And I select "Cancel appointment" button
    Then I see an appropriate error message when not allowed to cancel
    When I click the error 'Back' link
    Then the Your Appointments page is displayed
    Examples:
      | Reason           | GP System |
      | Unable to attend | EMIS      |

  #409
  Scenario Outline: A <GP System> user sees appropriate error message when booking has been cancelled already
    Given <GP System> prevents cancellation of previously booked appointment with '<Reason>' because it is already cancelled
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    And I select a "Cancel this appointment" link
    And I select a cancellation reason of <Reason>
    And I select "Cancel appointment" button
    Then I see an appropriate error message when it is already cancelled
    When I click the error 'Back' link
    Then the Your Appointments page is displayed
    Examples:
      | Reason           | GP System |
      | Unable to attend | EMIS      |

  #461
  Scenario: A TPP user sees appropriate error message when it is too late to cancel
    Given TPP prevents cancellation of previously booked appointment because it is too late
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    And I select a "Cancel this appointment" link
    And I select "Cancel appointment" button
    Then I see an appropriate error message when it is too late to cancel
    When I click the error 'Back' link
    Then the Your Appointments page is displayed

  #500
  Scenario Outline: VISION user sees appropriate error message when it returns corrupt data when cancelling appointment
    Given VISION returns corrupt data when cancelling appointment with '<Reason>'
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    And I select a "Cancel this appointment" link
    And I select a cancellation reason of <Reason>
    And I select "Cancel appointment" button
    Then I see appropriate submit error message when there is an error with '<Prefix>'
    When I click the error 'Back' link
    Then the Your Appointments page is displayed
    Examples:
      | Reason   | Prefix |
      | Reason 1 | xx     |

  #502
  Scenario Outline: EMIS user sees appropriate error message when it returns unknown exception when cancelling appointment
    Given EMIS returns unknown exception when cancelling appointment with '<Reason>'
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    And I select a "Cancel this appointment" link
    And I select a cancellation reason of <Reason>
    And I select "Cancel appointment" button
    Then I see appropriate submit error message when there is an error with '<Prefix>'
    When I click the error 'Contact us' link with a url of 'https://www.nhs.uk/contact-us/nhs-app-contact-us'
    Then a new tab has been opened by the link
    Examples:
      | Reason           | Prefix |
      | Unable to attend | 4e     |

  #504
  Scenario Outline: A <GP System> user sees appropriate information message when there is a timeout
    Given  <GP System> will time out when trying to cancel with '<Reason>'
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    And I select a "Cancel this appointment" link
    And I select a cancellation reason of <Reason>
    And I select "Cancel appointment" button
    Then I see appropriate submit error message when there is an error with '<Prefix>'
    When I click the error 'Contact us' link with a url of 'https://www.nhs.uk/contact-us/nhs-app-contact-us'
    Then a new tab has been opened by the link
    Examples:
      | Reason             | Prefix | GP System |
      | No longer required | zm     | MICROTEST |

    # covered in Manual Regression Test pack
  @tech-debt   @NHSO-4061
  Scenario: Cancelling appointment, when there is no internet connection should result with a message indicating user may have connectivity problems
    Given I have upcoming appointments before cutoff time for VISION
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    And I select a "Cancel this appointment" link
    Then I will be on the "Cancellation reason" screen
    And I lose my internet connection
    When I select "Cancel appointment" button
    Then I see a message indicating user may have connectivity problems
