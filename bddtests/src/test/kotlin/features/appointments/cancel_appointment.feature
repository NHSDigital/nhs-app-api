@appointment
Feature: Ability to cancel an appointment

  Scenario Outline: <GP System> user is presented with the cancel appointment screen
    Given I have upcoming appointments for <GP System>
    And I am logged in as a <GP System> user
    And I am on my appointments page
    When I select a "Cancel appointment" link
    Then I will be on the "Cancellation reason" screen
    And I am presented with the appointment details
    And there is a cancellation reasons drop-down
    Examples:
      | GP System |
      | EMIS      |
      | VISION    |

  Scenario Outline: A validation message will be displayed if no reason is provided
    Given I have upcoming appointments for <GP System>
    And I am logged in as a <GP System> user
    And I am on my appointments page
    When I select a "Cancel appointment" link
    And I select "Cancel appointment" button
    Then I will receive a cancellation validation error
    Examples:
      | GP System |
      | EMIS      |
      | VISION    |

  Scenario Outline: <GP System> user can cancel an appointment with selected reason of <Reason>
    Given <GP System> is available to cancel a previously booked appointment because <Reason>
    And I am logged in as a <GP System> user
    And I am on my appointments page
    And I select a "Cancel appointment" link
    And I select a cancellation reason of <Reason>
    When I select "Cancel appointment" button
    Then I will be on the My appointments screen
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

  Scenario Outline: <GP System> user navigates back to the "My appointments" screen
    Given I have upcoming appointments for <GP System>
    And I am logged in as a <GP System> user
    And I am on my appointments page
    And I select a "Cancel appointment" link
    When I select "Back" button
    Then I will be on the My appointments screen
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |


#    TPP Specific scenarios
  Scenario: A TPP user is presented with the cancel appointment screen
    Given I have upcoming appointments for TPP
    And I am logged in as a TPP user
    And I am on my appointments page
    When I select a "Cancel appointment" link
    Then I will be on the "Cancellation reason" screen
    And I am presented with the appointment details
    And cancellation reasons drop-down is hidden

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

  @long-running
  @NHSO-803
  Scenario: On session expiry (when on my appointments page), a user on a secure screen is automatically signed out
    Given I have upcoming appointments for VISION
    And I am logged in as a VISION user
    And I am on my appointments page
    And I select a "Cancel appointment" link
    Then I will be on the "Cancellation reason" screen
    When I am idle long enough for the session to expire
    Then I see the login page with the session expiry notification
    And the user login details are cleared from cookies

  @manual
  @NHSO-803
  Scenario: Cancelling appointment, when there is no internet connection should result with a message indicating user may have connectivity problems
    Given I have upcoming appointments for VISION
    And I am logged in as a VISION user
    And I am on my appointments page
    And I select a "Cancel appointment" link
    Then I will be on the "Cancellation reason" screen
    And I lose my internet connection
    When I select "Cancel appointment" button
    Then I see a message indicating user may have connectivity problems
