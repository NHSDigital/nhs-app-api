@appointments
@view
Feature: View available appointment slots
  Users can view available appointments from the available appointments Page.

  #HAPPY PATH JOURNEYS

  #GP System agnostic scenario, so only need to test with EMIS

  @nativesmoketest
  Scenario: A user who is signed in sees the appointments navigation button highlighted
    Given there are available appointment slots with different criteria for EMIS
    And I am logged in
    And I am on the Available Appointments page
    Then the appointments menu button is highlighted

  #FEATURE JOURNEYS

  Scenario Outline: A <GP System> user enters the available appointments page
    Given there are available appointment slots with different criteria for <GP System>
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    Then there is a filter for the appointment types
    And there is a filter for the appointment locations
    And there is a filter for the appointment doctors/nurses
    And there is a filter for the appointment time period
    And no available slots are displayed
    Examples:
      | GP System |
      | TPP       |
      | VISION    |
      | MICROTEST |

  @nativesmoketest
    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: A user does not see any guidance provided by <GP System>
    Given there are available appointment slots with different criteria for <GP System>
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    Then I cannot see any appointment slot guidance
    Examples:
      | GP System |
      | MICROTEST |

  Scenario Outline: A user can expand, view and collapse guidance provided by <GP System>
    Given there are available appointment slots with different criteria for <GP System> when <Content> appointment slot guidance is provided
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I expand the appointment slot guidance
    Then the appointment slot guidance content is displayed
    And the appointment slot guidance is collapsible
    Examples:
      | Content             | GP System |
      | test Emis Message   | EMIS      |
      | test TPP Message    | TPP       |
      | yet another Message | VISION    |

  Scenario Outline: A user does not see guidance if none is provided by <GP System>
    Given there are available appointment slots with different criteria for <GP System> when <Content> appointment slot guidance is provided
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    Then I cannot see any appointment slot guidance
    Examples:
      | Content           | GP System |
      | whitespace string | EMIS      |
      | empty             | TPP       |
      | empty             | VISION    |

  Scenario Outline: A user does not see any guidance when it cannot be retrieved from <GP System>, but can still progress
    Given there are available appointment slots with different criteria for <GP System> when guidance cannot be retrieved
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    Then I cannot see any appointment slot guidance
    And I am able to filter on available slots
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: A <GP System> user can filter on slot type and location and only the appropriate slots will be displayed
    Given there are available appointment slots with different slot types and locations for <GP System>
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I select a particular slot type and location
    And I select time period for 'Next four weeks'
    Then I only see results for the selected filter options
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: A <GP System> user can filter by selected clinician and only the appropriate slots will be displayed
    Given there are available appointment slots with different clinician for <GP System>
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I select a particular slot type and location
    And I select time period for 'Next four weeks'
    Then I only see results for the selected filter options
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: A <GP System> user enters the available appointments page, but only 1 appointment is available
    Given there is 1 available appointment slot for <GP System>
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    Then appointment type is not selected
    And the only location is selected
    And options for doctors/nurses remains as "no preference"
    And I select time period for 'This week'
    And no available slots are displayed
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: A <GP System> user enters the available appointments page, but appointments only available at 1 location
    Given there are available appointment slots for <GP System> for 1 location
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    Then the only location is selected
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: A <GP System> user sees appropriate information message when no slots are available at all
    Given there are no available appointment slots for <GP System>
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    Then a message is displayed indicating there are no slots available
    Examples:
      | GP System |
      | TPP       |
      | VISION    |
      | MICROTEST |

  @nativesmoketest
    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: A <GP System> user goes back when no slots are available at all
    Given there are no available appointment slots for <GP System>
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I acknowledge that there are no appointments and go back to my appointments
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

  Scenario Outline: A <GP System> user sees appropriate message, if filtering by today but no appointments are available
    Given there are available appointment slots with different criteria for <GP System>
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I select options from the filters that don't yield any results
    Then a message is displayed indicating there are no slots for selected criteria
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: A <GP System> user sees appropriate message, if filtering by tomorrow but no appointments are available
    Given there are appointment slots on some days other than tomorrow, provided by <GP System>
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I select a type and location that have available slots
    And I select time period for 'Tomorrow'
    Then a message is displayed indicating there are no slots for selected criteria
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: <GP System> user tries again after a timeout and it times-out again
    Given the <GP System> doesn't respond in a timely fashion for available appointment slots
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    Then I see appropriate information message for time-outs
    When I click the 'Try again' action
    Then I see appropriate information message for time-outs
    And there should be a button to try again
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: <GP System> user tries again after a timeout and it is now successful
    Given the <GP System> doesn't respond in a timely fashion for available appointment slots, on the first attempt
    But will respond in a timely fashion on the second attempt
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    Then I see a timeout on the appointment booking page
    When I click the 'Try again' action
    Then I am able to filter on available slots
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: <GP System> user sees appropriate information message when returns corrupt data
    Given <GP System> returns corrupt data for appointment slots
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    Then I see appropriate information message when there is a error retrieving data
    And there should not be an option to try again
    Examples:
      | GP System |
      | MICROTEST |
      | TPP       |
      | VISION    |

  @nativesmoketest
    Examples:
      | GP System |
      | EMIS      |

  # GP System agnostic scenario, so only need to test with TPP
  Scenario: A TPP user only sees days with available slots, if filtering by "Next week" but no appointments are available for some days
    Given there are appointment slots on some days next week but not others, provided by TPP
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I select a type and location that have available slots
    And I select time period for 'Next week'
    Then I only see results for days that have available slots

  # GP System agnostic scenario, so only need to test with TPP
  Scenario: A TPP user only sees days with available slots, if filtering by "This week" but no appointments are available for some days
    Given there are appointment slots on some days in This week but not others, provided by TPP
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I select a type and location that have available slots
    And I select time period for 'This week'
    Then I only see results for days that have available slots

  # GP System agnostic scenario, so only need to test with TPP
  Scenario: A TPP user only sees days with available slots, if filtering by "Next four weeks" but no appointments are available for some days
    Given there are appointment slots on some days in the next few weeks but not others, provided by TPP
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I select a type and location that have available slots
    And I select time period for 'Next four weeks'
    Then I only see results for days that have available slots

  Scenario Outline: <GP System>  user sees appropriate information message when GP system is unavailable
    Given <GP System> is unavailable for available appointment slots
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    Then I see appropriate information message when there is a error retrieving data
    And there should not be an option to try again
    Examples:
      | GP System |
      | TPP       |
      | VISION    |
      | MICROTEST |

  @nativesmoketest
    Examples:
      | GP System |
      | EMIS      |

    #    GP System agnostic scenario, so only need to test with EMIS
  Scenario: A user decides to go back even though there's available slots
    Given there are available appointment slots with different criteria for EMIS
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I decide I don't want to select an appointment and go back
    Then the My Appointments page is displayed

  @native
  @tech-debt   @NHSO-4061 # covered in Manual Regression Test pack
  Scenario: A user sees appropriate information message when internet connection has been lost
    #    GP System agnostic scenario, so only need to test with EMIS
    Given I am on the My Appointments page
    And internet connection drops
    When I press the "Book this appointment" button
    Then I see appropriate information message when there is no internet connection
    And there should be a button to try again

  @nativesmoketest
  Scenario: A user has problems with prescriptions and selects appointments and prescriptions in quick succession
    #    GP System agnostic scenario, so only need to test with EMIS
    Given there are available EMIS appointment slots with different criteria but there is a slight delay in retrieving them
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    Then I retrieve the 'My Prescriptions' page directly
    And I wait for 5 seconds
    Then I don't see filters for available slots
