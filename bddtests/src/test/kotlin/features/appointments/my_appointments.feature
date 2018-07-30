Feature: My appointments

  Users can view their upcoming and past appointments in the My Appointments screen.

  @NHSO-524
  @backend
  Scenario Outline: API appropriately filters for upcoming appointments
    Given I have upcoming appointments for <GP System>
    And I have logged in and have a valid session cookie for <GP System>
    When the API retrieves upcoming appointments
    Then I will only receive upcoming appointments
    And a list of cancellation reasons if the GP Service provides the list

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  @NHSO-524
  @appointment
  Scenario Outline: A user has never booked an appointment
    Given <GP System> logged in session started
    And I have no upcoming appointments for <GP System>
    And I am logged in as a <GP System> user
    When I am on my appointments page
    Then I am informed I have no booked appointments
    But I can book an appointment

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  @NHSO-196
  @NHSO-663
  @NHSO-1591
  @tech-debt @NHSO-1937
  Scenario Outline: A user has upcoming appointments
    Given <GP System> logged in session started
    And I have upcoming appointments for <GP System>
    And I am logged in as a <GP System> user
    When I am on my appointments page
    Then the page title is "My appointments"
    And I am given the list of upcoming appointments
    And appointments are in chronological order
    And each appointment can be cancelled
    And I can book an appointment

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

