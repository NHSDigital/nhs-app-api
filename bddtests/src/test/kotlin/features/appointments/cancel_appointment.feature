@appointment
Feature: Ability to cancel an appointment

  Scenario Outline: <GP System> user is presented with the cancel appointment screen
    Given I have upcoming appointments for <GP System>
    And I am logged in as a <GP System> user
    And I am on the My Appointments page
    When I select a "Cancel appointment" link
    Then I will be on the "Cancellation reason" screen
    And I am presented with the appointment details
    And there is a cancellation reasons drop-down
    Examples:
      | GP System |
      | EMIS      |
      | VISION    |

  Scenario: A Vision user is presented with the cancel appointment screen without reason selected, when there is just
  one
    Given I have upcoming appointments for VISION, but with only one cancellation reason
    And I am logged in as a VISION user
    And I am on the My Appointments page
    When I select a "Cancel appointment" link
    Then I will be on the "Cancellation reason" screen
    And I am presented with the appointment details
    And there is a cancellation reasons drop-down

  Scenario: A TPP user is presented with the cancel appointment screen
    Given I have upcoming appointments for TPP
    And I am logged in as a TPP user
    And I am on the My Appointments page
    When I select a "Cancel appointment" link
    Then I will be on the "Cancellation reason" screen
    And I am presented with the appointment details
    And cancellation reasons drop-down is hidden

  Scenario Outline: A validation message will be displayed if no reason is selected
    Given I have upcoming appointments for <GP System>
    And I am logged in as a <GP System> user
    And I am on the My Appointments page
    When I select a "Cancel appointment" link
    And I select "Cancel appointment" button
    Then I will receive a cancellation validation error
    Examples:
      | GP System |
      | EMIS      |
      | VISION    |

  Scenario:  A validation message will be displayed if no reason is selected, even when there is just one
    Given I have upcoming appointments for VISION, but with only one cancellation reason
    And I am logged in as a VISION user
    And I am on the My Appointments page
    When I select a "Cancel appointment" link
    And I select "Cancel appointment" button
    Then I will receive a cancellation validation error


  Scenario Outline: <GP System> user can cancel an appointment with selected reason of <Reason>
    Given <GP System> is available to cancel a previously booked appointment because <Reason>
    And I am logged in as a <GP System> user
    And I am on the My Appointments page
    And I select a "Cancel appointment" link
    And I select a cancellation reason of <Reason>
    When I select "Cancel appointment" button
    Then the My Appointments page is displayed
    And a "Cancellation confirmed" message is displayed
  @smoketest
    Examples:
      | Reason             | GP System |
      | No longer required | EMIS      |

    Examples:
      | Reason           | GP System |
      | Unable to attend | EMIS      |
      | Reason 1         | VISION    |
      | Reason 2         | VISION    |

  Scenario:  Vision user can cancel appointment when there is just one reason
    Given VISION is available to cancel a previously booked appointment, with only one available reason
    And I am logged in as a VISION user
    And I am on the My Appointments page
    And I select a "Cancel appointment" link
    And I select the cancellation reason
    When I select "Cancel appointment" button
    Then the My Appointments page is displayed
    And a "Cancellation confirmed" message is displayed

  @smoketest
  Scenario: A TPP user can cancel an appointment
    Given TPP is available to cancel a previously booked appointment
    And I am logged in as a TPP user
    And I am on the My Appointments page
    And I select a "Cancel appointment" link
    Then I will be on the "Cancellation reason" screen
    When I select "Cancel appointment" button
    Then the My Appointments page is displayed
    And a "Cancellation confirmed" message is displayed


  Scenario Outline: <GP System> user navigates back to the "My appointments" screen
    Given I have upcoming appointments for <GP System>
    And I am logged in as a <GP System> user
    And I am on the My Appointments page
    And I select a "Cancel appointment" link
    When I select "Back" button
    Then the My Appointments page is displayed
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: A <GP System> user sees appropriate information message when GP system is unavailable
    Given <GP System> is unavailable to cancel a previously booked appointment because <Reason>
    And I am logged in as a <GP System> user
    And I am on the My Appointments page
    When I select a "Cancel appointment" link
    And I select a cancellation reason of <Reason>
    When I select "Cancel appointment" button
    Then I see appropriate information message when there is an error sending data on appointment confirmation page
    And there should be a button to go back to my appointments
    Examples:
      | Reason           | GP System |
      | Unable to attend | EMIS      |
      | Reason 1         | VISION    |

  @long-running
  @nativepending @NHSO-2966
  Scenario: On session expiry (when on my appointments page), a user on a secure screen is automatically signed out
    Given I have upcoming appointments for VISION
    And I am logged in as a VISION user
    And I am on the My Appointments page
    And I select a "Cancel appointment" link
    Then I will be on the "Cancellation reason" screen
    When I am idle long enough for the session to expire
    Then I see the login page with the session expiry notification
    And the user login details are cleared from cookies

  @manual
  Scenario: Cancelling appointment, when there is no internet connection should result with a message indicating user may have connectivity problems
    Given I have upcoming appointments for VISION
    And I am logged in as a VISION user
    And I am on the My Appointments page
    And I select a "Cancel appointment" link
    Then I will be on the "Cancellation reason" screen
    And I lose my internet connection
    When I select "Cancel appointment" button
    Then I see a message indicating user may have connectivity problems
