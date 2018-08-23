@appointment
Feature: View available appointment slots
  Users can view available appointments from the available appointments Page.

    #    GP System agnostic scenario, so only need to test with EMIS
  Scenario: A user who is signed in sees the appointments navigation button highlighted
    Given there are available appointment slots with different criteria for EMIS
    And I am logged in
    And I am on the available appointments page
    Then the appointments menu button is highlighted

  Scenario Outline: A <GP System> user enters the available appointments page
    Given there are available appointment slots with different criteria for <GP System>
    And I am logged in
    When I am on the available appointments page
    Then there is a filter for the appointment types
    And there is a filter for the appointment locations
    And there is a filter for the appointment doctors/nurses
    And there is a filter for the appointment time period
    And no available slots are displayed
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario: A user can expand, view and collapse guidance provided by EMIS
    Given there are available appointment slots with different criteria for EMIS
    And I am logged in
    And I am on the available appointments page
    When I expand the appointment slot guidance
    Then the appointment slot guidance content is displayed
    And the appointment slot guidance is collapsible

  Scenario: A user does not see guidance if none is provided by EMIS
    Given there are available appointment slots with different criteria for EMIS when no appointment slot guidance is provided
    And I am logged in
    When I am on the available appointments page
    Then I cannot see any appointment slot guidance

  Scenario: A user does not see any guidance provided by TPP
    Given there are available appointment slots with different criteria for TPP
    And I am logged in
    When I am on the available appointments page
    Then I cannot see any appointment slot guidance

  Scenario: A user does not see any guidance when it cannot be retrieved from EMIS, but can still progress
    Given there are available appointment slots with different criteria for EMIS when guidance cannot be retrieved
    And I am logged in
    When I am on the available appointments page
    Then I cannot see any appointment slot guidance
    And I am able to filter on available slots

  Scenario Outline: A <GP System> user enters the available appointments page, but only 1 appointment is available
    Given there is 1 available appointment slot for <GP System>
    And I am logged in
    When I am on the available appointments page
    Then appointment type is not selected
    And the only location is selected
    And options for doctors/nurses remains as "no preference"
    And time period remains as that for this week
    And no available slots are displayed
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: A <GP System> user enters the available appointments page, but appointments only available at 1 location
    Given there are available appointment slots for <GP System> for 1 location
    And I am logged in
    When I am on the available appointments page
    Then the only location is selected
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: A <GP System> user sees appropriate information message when no slots are available at all
    Given there are no available appointment slots for <GP System>
    And I am logged in
    When I am on the available appointments page
    Then a message is displayed indicating there are no slots available
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: A <GP System> user goes back when no slots are available at all
    Given there are no available appointment slots for <GP System>
    And I am logged in
    When I am on the available appointments page
    And I acknowledge that there are no appointments and go back to my appointments
    Then I will be on the My appointments screen
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: A <GP System> user sees appropriate message, if filtering by today but no appointments are available
    Given there are available appointment slots with different criteria for <GP System>
    And I am logged in
    When I am on the available appointments page
    And I select options from the filters that don't yield any results
    Then a message is displayed indicating there are no slots for selected criteria
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: A <GP System> user sees appropriate message, if filtering by tomorrow but no appointments are available
    Given there are appointment slots on some days other than tomorrow, provided by <GP System>
    And I am logged in
    And I am on the available appointments page
    When I select a type and location that have available slots
    And I select time period for 'Tomorrow'
    Then a message is displayed indicating there are no slots for selected criteria
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: A <GP System> user still sees the remainder of the current week, if filtering by this week but no appointments are available for some days
    Given there are appointment slots on some days this week but not others, provided by <GP System>
    And I am logged in
    And I am on the available appointments page
    When I select a type and location that have available slots
    And I select time period for 'This week'
    Then I see results for each of the remaining days for this week, with an appropriate message when there are no slots
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: A <GP System> user still sees the whole of week, if filtering by next week but no appointments are available for some days
    Given there are appointment slots on some days next week but not others, provided by <GP System>
    And I am logged in
    And I am on the available appointments page
    When I select a type and location that have available slots
    And I select time period for 'Next week'
    Then I see results for each of the days for next week, with an appropriate message when there are no slots
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: A <GP System> user only sees days with available slots, if filtering by "All available" but no appointments are available for some days
    Given there are appointment slots on some days in the next few weeks but not others, provided by <GP System>
    And I am logged in
    And I am on the available appointments page
    When I select a type and location that have available slots
    And I select time period for 'All available'
    Then I only see results for days that have available slots
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

    #    GP System agnostic scenario, so only need to test with EMIS
  Scenario: A user decides to go back even though there's available slots
    Given there are available appointment slots with different criteria for EMIS
    And I am logged in
    And I am on the available appointments page
    When I decide I don't want to select an appointment and go back
    Then I will be on the My appointments screen

  Scenario: A user sees appropriate information message when there is a timeout
    #    GP System agnostic scenario, so only need to test with EMIS
    Given EMIS doesn't respond a timely fashion for available appointment slots
    And I am logged in
    When I try to progress to the available appointments page
    Then I see appropriate information message for time-outs
    And there should be a button to try again

  Scenario: A user tries again after a timeout and it times-out again
    #    GP System agnostic scenario, so only need to test with EMIS
    Given EMIS doesn't respond a timely fashion for available appointment slots
    And I am logged in
    And I try to progress to the available appointments page
    When I click try again button on appointment page
    Then I see appropriate information message for time-outs
    And there should be a button to try again

  Scenario: A user tries again after a timeout and it is now successful
    #    GP System agnostic scenario, so only need to test with EMIS
    Given EMIS doesn't respond a timely fashion for available appointment slots
    And I am logged in
    And I try to progress to the available appointments page
    Then I see a timeout on the appointment booking page
    When EMIS responds a timely fashion for available appointment slots
    And I click try again button on appointment page
    Then I am able to filter on available slots

  Scenario: A user sees appropriate information message when GP system is unavailable
    #    GP System agnostic scenario, so only need to test with EMIS
    Given EMIS is unavailable for available appointment slots
    And I am logged in
    When I try to progress to the available appointments page
    Then I see appropriate information message when there is a error retrieving data
    And there should not be an option to try again

  Scenario: A user sees appropriate information message when EMIS returns corrupt data
    #    GP System agnostic scenario, so only need to test with EMIS
    Given EMIS returns corrupt data for appointment slots
    And I am logged in
    When I try to progress to the available appointments page
    Then I see appropriate information message when there is a error retrieving data
    And there should not be an option to try again

  @manual
  Scenario: A user sees appropriate information message when internet connection has been lost
    #    GP System agnostic scenario, so only need to test with EMIS
    Given I am on my appointments page
    And internet connection drops
    When I press the "Book this appointment" button
    Then I see appropriate information message when there is no internet connection
    And there should be a button to try again

  Scenario: A user has problems with prescriptions and selects appointments and prescriptions in quick succession
    #    GP System agnostic scenario, so only need to test with EMIS
    Given there are available EMIS appointment slots with different criteria but there is a slight delay in retrieving them
    And I am logged in
    And I am on the available appointments page
    When I navigate to Prescriptions
    And I wait 5 seconds
    Then I don't see filters for available slots
