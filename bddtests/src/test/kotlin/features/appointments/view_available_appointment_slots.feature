Feature: View available appointment slots

  Users can view available appointments from the Appointments Page.

  Background:
    Given wiremock is initialised

  Scenario: A user who is signed in sees the appointments navigation button highlighted
    Given I am on the appointments booking page
    Then the appointments menu button is highlighted

  @NHSO-71
  @NHSO-870
  @pending    @NHSO-71
  @smoketest
  Scenario Outline: A user enters the appointments booking page
    Given there are available appointment slots with different criteria for <GP System>
    When I am on the appointments booking page
    Then there is a filter for the appointment types
    And there is a filter for the location
    And there is a filter for the doctors/nurses
    And there is a filter for the time period
    And no available slots are displayed
    Examples:
      | GP System |
      | EMIS      |

  @NHSO-71
  @NHSO-870
  @pending    @NHSO-71
  Scenario Outline: A user enters the appointments booking page, but only 1 appointment is available
    Given there is 1 available appointment slot for <GP System>
    When I am on the appointments booking page
    Then appointment type is not selected
    And the only location is selected
    And options for doctors/nurses remains as "no preference"
    And time period remains as that for this week
    And no available slots are displayed
    Examples:
      | GP System |
      | EMIS      |

  @NHSO-71
  @NHSO-870
  @pending    @NHSO-71
  Scenario Outline: A user enters the appointments booking page, but only appointments only available at 1 location
    Given there are available appointment slot for <GP System> for 1 location
    When I am on the appointments booking page
    Then the only location is selected
    Examples:
      | GP System |
      | EMIS      |

  @NHSO-71
  @NHSO-870
  @pending    @NHSO-71
  Scenario Outline: A user sees appropriate information message when no slots are available at all
    Given there are no available appointment slots for <GP System>
    When I am on the appointments booking page
    Then a message is displayed indicating there are no slots to select
    Examples:
      | GP System |
      | EMIS      |

  @NHSO-71
  @NHSO-870
  @pending    @NHSO-71
  Scenario Outline: A user goes back when no slots are available at all
    Given there are no available appointment slots for <GP System>
    When I am on the appointments booking page
    And I acknowledge that there are no appointments and go back to my appointments
    Then I will be on the My appointments screen
    Examples:
      | GP System |
      | EMIS      |

  @NHSO-71
  @NHSO-870
  @pending    @NHSO-71
  Scenario Outline: A user refines criteria to isolate ideal appointment slots
    Given there are available appointment slots with different criteria for <GP System>
    When I am on the appointments booking page
    And I select an option from each of the filters
    Then available slots are displayed that meet the new criteria
    Examples:
      | GP System |
      | EMIS      |

  @NHSO-71
  @NHSO-870
  @pending    @NHSO-71
  Scenario Outline: A user refines criteria but no slots are available
    Given there are available appointment slots with different criteria for <GP System>
    When I am on the appointments booking page
    And I select options from the filters that doesn't yield any results
    Then a message is displayed indicating there are no slots to select
    Examples:
      | GP System |
      | EMIS      |

  @NHSO-71
  @pending    @NHSO-71
  Scenario: A user selects a second appointment slot and the first selected slot gets deselected.
    Given there are available appointment slots with different criteria for any GP System
    And I select the appointment I want
    When I select a different appointment
    Then the slot is highlighted
    And the first slot is no longer highlighted

  @NHSO-71
  @pending    @NHSO-71
  Scenario: A user select the same appointment twice and the selected appointment stay selected.
    Given there are available appointment slots with different criteria for any GP System
    And I select the appointment I want
    When I select the same appointment
    Then the slot remains highlighted

  @NHSO-71
  @pending    @NHSO-71
  Scenario: A user tries to progress without selecting an appointment type or location
    Given there are available appointment slots with different criteria for any GP System
    When I try to progress without selecting an appointment type or location
    Then I see an appropriate message informing that I need to select an appointment type and location

  @NHSO-71
  @pending    @NHSO-71
  Scenario: A user tries to progress without selecting an appointment type
    Given there are available appointment slots with different criteria for any GP System
    When I try to progress without selecting an appointment type
    Then I see an appropriate message informing that I need to select an appointment type

  @NHSO-71
  @pending    @NHSO-71
  Scenario: A user tries to progress without selecting a location
    Given there are available appointment slots with different criteria for any GP System
    When I try to progress without selecting a location
    Then I see an appropriate message informing that I need to select an location

  @NHSO-71
  @pending    @NHSO-71
  Scenario: A user tries to progress without selecting an appointment
    Given there are available appointment slots with different criteria for any GP System
    When I try to progress without selecting an appointment
    Then I see an appropriate message informing that I need to select an appointment

  @NHSO-71
  @pending    @NHSO-71
  Scenario: A user decides to go back even though there's available slots
    Given there are available appointment slots with different criteria for any GP System
    When I decide I don't want to select an appointment and go back
    Then I will be on the My appointments screen

  @NHSO-616
  @NHSO-870
  Scenario: A user sees appropriate information message when there is a timeout
    Given GP system doesn't respond a timely fashion for available appointment slots
    When I try to progress to the appointments booking page
    Then I see appropriate information message for time-outs
    And there should be a button to try again

  @NHSO-616
  @NHSO-870
  Scenario: A user tries again after a timeout and it times-out again
    Given GP system doesn't respond a timely fashion for available appointment slots
    And I try to progress to the appointments booking page
    When I click try again button on appointment page
    Then I see appropriate information message for time-outs
    And there should be a button to try again

  @NHSO-616
  @NHSO-870
  Scenario: A user tries again after a timeout and it is now successful
    Given GP system doesn't respond a timely fashion for available appointment slots
    And I try to progress to the appointments booking page
    When GP system responds a timely fashion for available appointment slots
    And I click try again button on appointment page
    Then I see available appointment slots

  @NHSO-616
  @NHSO-870
  Scenario: A user sees appropriate information message when GP system is unavailable
    Given GP system is unavailable for available appointment slots
    When I try to progress to the appointments booking page
    Then I see appropriate information message when there is a error retrieving data
    And there should not be an option to try again

  @NHSO-616
  @NHSO-870
  Scenario: A user sees appropriate information message when GP system returns corrupt data
    Given GP system returns corrupt data for appointment slots
    When I try to progress to the appointments booking page
    Then I see appropriate information message when there is a error retrieving data
    And there should not be an option to try again

  @NHSO-616
  @native
  @manual
  Scenario: A user sees appropriate information message when internet connection has been lost
    Given I am on my appointments page
    And internet connection drops
    When I press the "Book this appointment" button
    Then I see appropriate information message when there is no internet connection
    And there should be a button to try again

  @NHSO-1168
  Scenario: A user has problems with prescriptions and selects appointments and prescriptions in quick succession
    Given there are available appointment slots
    But there is a slight delay in retrieving them
    And I am on the appointments booking page
    When I navigate to Prescriptions
    And I wait 5 seconds
    Then I don't see appointment slots

  @NHSO-1168
  Scenario: A user has different problems with prescriptions and appointments and selects appointments and prescriptions in quick succession
    Given GP system doesn't respond a timely fashion for available appointment slots
    And I am on the appointments booking page
    When I navigate to Prescriptions
    And I wait 11 seconds
    Then I don't see a time-out error

