@appointment
@noJs
Feature: Appointment Service with no Javascript

  @bug @NHSO-3651
  Scenario Outline: A <GP System> user is presented with guidance when booking an appointment and can proceed to check
  their symptoms with javascript disabled
    Given I have disabled javascript
    And there are available appointment slots with different criteria for <GP System>
    And I am logged in
    And I am on the Appointment Guidance page
    When I click the 'Check symptoms' button
    Then the Check My Symptoms page is displayed
    And the Check My Symptoms page header and navigation menu are correct
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  # Only EMIS has appointment slot guidance available
  # The guidance should always be visible with no javascript, in comparison to an expandable/collapsible box
  # with javascript
  @bug @NSHO-3650
  Scenario: An EMIS user can view appointment slot guidance with javascript disabled
    Given I have disabled javascript
    And there are available appointment slots with different criteria for EMIS
    And I am logged in
    And I am on the Available Appointments page
    Then the appointment slot guidance content is displayed

  Scenario Outline: A <GP System> user can find and book available appointments with javascript disabled
    Given I have disabled javascript
    And there are multiple appointment slots at the same time, provided by <GP System>
    And I am logged in
    And I am on the My Appointments page
    Then I am informed I have no booked appointments

    When I click the 'Book' button
    Then I am given guidance as to my options before booking an appointment

    When I click the 'Book new appointment' button
    Then the Available Appointments page is displayed

    When I have filtered such that there is one time displayed that represents multiple slots
    And I click the 'Find available appointments' button
    And I have selected a time when multiple slots are available
    Then the Appointment Slot page is displayed

    When I click the 'Change this appointment' button
    Then the Available Appointments page is displayed

    When I have filtered such that there is one time displayed that represents multiple slots
    And I click the 'Find available appointments' button
    And I have selected a time when multiple slots are available
    Then the Appointment Slot page is displayed

    When I enter symptoms
    And  I click the 'Confirm and book appointment' button
    Then the Appointment Booking success message is displayed
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: A <GP System> user can cancel appointments with javascript disabled
    Given I have disabled javascript
    And <GP System> is available to cancel a previously booked appointment before cutoff time because <Reason>
    And I am logged in as a <GP System> user
    And I am on the My Appointments page
    And I select a "Cancel appointment" link
    And I select a cancellation reason of <Reason>
    When I select "Cancel appointment" button
    Then the My Appointments page is displayed
    And a "Cancellation confirmed" message is displayed
    Examples:
      | Reason             | GP System |
      | No longer required | EMIS      |
      | Reason 1           | VISION    |

  Scenario: A TPP user can cancel appointments with javascript disabled
    Given I have disabled javascript
    And TPP is available to cancel a previously booked appointment before cutoff time
    And I am logged in as a TPP user
    And I am on the My Appointments page
    And I select a "Cancel appointment" link
    Then I will be on the "Cancellation reason" screen
    When I select "Cancel appointment" button
    Then the My Appointments page is displayed
    And a "Cancellation confirmed" message is displayed
