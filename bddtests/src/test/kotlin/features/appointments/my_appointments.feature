@appointment
Feature: My appointments
  Users can view their upcoming and past appointments in the My Appointments screen.

  @NHSO-797
  @backend
  Scenario Outline: API appropriately filters for upcoming appointments
    Given I have upcoming appointments for <GP System>
    And I have logged into <GP System> and have a valid session cookie
    When the API retrieves upcoming appointments
    Then I will only receive upcoming appointments
    And a list of cancellation reasons if the GP Service provides the list
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  @backend
  Scenario Outline: Online appointment booking is not available to a particular patient
    Given I have logged into <GP System> and have a valid session cookie
    But the <GP System> does not offer online booking to my patient
    When the API retrieves upcoming appointments
    Then I receive a "Forbidden" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  @backend
  @NHSO-802
  Scenario Outline: Appropriate error response, when <GP System> returns corrupted data
    Given I have logged into <GP System> and have a valid session cookie
    But <GP System> returns corrupted response for my appointments
    When the API retrieves upcoming appointments
    Then I receive a "Internal Server Error" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  @backend
  @NHSO-802
  Scenario Outline: Requesting my appointments the <GP System> times out and returns "Gateway Timeout" error
    Given I have logged into <GP System> and have a valid session cookie
    But <GP System> will time out when trying to retrieve my appointments
    When the API retrieves upcoming appointments
    Then I receive a "Gateway Timeout" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
#      | VISION    |

  @NHSO-797
  Scenario Outline: A user has never booked an appointment
    Given I have no upcoming appointments for <GP System>
    And I am logged in as a <GP System> user
    When I am on my appointments page
    Then I am informed I have no booked appointments
    But I can book an appointment

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  @smoketest
  @NHSO-797
  Scenario Outline: A <GP System> user can see their upcoming appointments
    Given I have upcoming appointments for <GP System>
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
      | VISION    |


  @manual
  @NHSO-802
  Scenario: Requesting list of appointments, when there is no internet connection should result with a message indicating user may have connectivity problems