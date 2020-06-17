@appointments
@noJs
@pending
Feature: Appointments Service With Javascript Disabled

  #This test covers navigation via buttons/links
  @tech-debt
  Scenario Outline: A <GP System> user can find and book available appointments with javascript disabled
    Given I have disabled javascript
    And there are multiple appointment slots at the same time, provided by <GP System>
    And I am logged in
    And I am on the Your Appointments page
    Then I am informed I have no upcoming appointments

    When I click the 'Book an appointment' button
    Then I am given guidance as to my options with OLC enabled before booking

    When I click the 'Book an appointment' button
    Then the Available Appointments page is displayed

    When I have filtered such that there is one time displayed that represents multiple slots
    And I click the 'Find available appointments' button
    And I have selected a time when multiple slots are available
    Then the Appointment Slot page is displayed

    When I click the Back link
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

  # These tests navigate directly to the pages where the features are to be tested, to save time.

  Scenario: An EMIS user is presented with guidance when booking an appointment and can proceed to check
  their symptoms with javascript disabled
    Given I have disabled javascript
    And there are available appointment slots with different criteria for EMIS
    And I am logged in
    When I retrieve the 'Appointment Guidance' page directly
    And I select Appointment Guidance Page Check symptoms button
    Then the Symptoms page is displayed
    And the Symptoms page header and navigation menu are correct

  # Only EMIS has appointment slot guidance available
  # The guidance should always be visible with no javascript, in comparison to an expandable/collapsible box
  # with javascript
  @bug @NHSO-3650
  Scenario: An EMIS user can view appointment slot guidance with javascript disabled
    Given I have disabled javascript
    And there are available appointment slots with different criteria for EMIS
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    Then the appointment slot guidance content is displayed

  @tech-debt
  Scenario Outline: A <GP System> user can cancel appointments with javascript disabled
    Given I have disabled javascript
    And <GP System> is available to cancel a previously booked appointment before cutoff time because <Reason>
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    And I select a "Cancel this appointment" link
    And I select a cancellation reason of <Reason>
    And I select "Cancel appointment" button
    Then the Your Appointments page is displayed
    And a "Cancellation confirmed" message is displayed
    Examples:
      | Reason             | GP System |
      | No longer required | EMIS      |
      | Reason 1           | VISION    |

  @tech-debt
  Scenario: A TPP user can cancel appointments with javascript disabled
    Given I have disabled javascript
    And TPP is available to cancel a previously booked appointment before cutoff time
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    And I select a "Cancel this appointment" link
    Then I will be on the "Cancellation reason" screen
    When I select "Cancel appointment" button
    Then the Your Appointments page is displayed
    And a "Cancellation confirmed" message is displayed
