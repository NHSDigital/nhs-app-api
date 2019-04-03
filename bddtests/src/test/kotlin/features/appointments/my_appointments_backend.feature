@appointments
@my
@backend
Feature: My appointments backend
  In order control my booked appointments
  As a logged user
  I want to see my appointments

  Scenario Outline: API call for my appointments will successfully return appointments from <GP System> when they are only upcoming
    Given I have upcoming appointments before cutoff time for <GP System>
    And I have logged in and have a valid session cookie
    When my appointments are requested
    Then I will only receive upcoming appointments
    And a list of cancellation reasons if the GP Service provides the list
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: API call for my appointments will return appointments from <GP System> when they are only historical
    Given I have historical appointments for <GP System>
    And I have logged in and have a valid session cookie
    When my appointments are requested
    Then I will only receive historical appointments
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario Outline: API call for my appointments will return appointments from <GP System> when there are historical
  and upcoming
    Given I have historical and upcoming appointments for <GP System>
    And I have logged in and have a valid session cookie
    When my appointments are requested
    Then I will receive both historical and upcoming appointments
    And a list of cancellation reasons if the GP Service provides the list
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario: API call for my appointments won't filter out past appointments from VISION
    Given I have upcoming appointments for VISION, with one in the past
    And I have logged in and have a valid session cookie
    When my appointments are requested
    Then I will receive upcoming appointments with appointments in the past

  Scenario Outline: API call for my appointments from <GP System> will put appointments in progress as past appointments
    Given I have upcoming appointments for <GP System>, with one in the past
    And I have logged in and have a valid session cookie
    When my appointments are requested
    Then I will receive both historical and upcoming appointments
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario Outline: API call for my appointments will return an empty response from <GP System> when they are no appointments to retrieve
    Given I have no booked appointments for <GP System>
    And I have logged in and have a valid session cookie
    When my appointments are requested
    Then I will receive no appointments
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: Online appointment booking is not available to a particular patient of <GP System>
    Given <GP System> does not offer online booking to my patient
    And I have logged in and have a valid session cookie
    When my appointments are requested
    Then I receive a "Forbidden" error
    And the response contains an empty body
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: Appropriate error response, when <GP System> returns corrupted data
    Given <GP System> returns corrupted response for my appointments
    And I have logged in and have a valid session cookie
    When my appointments are requested
    Then I receive a "Internal Server Error" error
    And the response contains an empty body
  @bug @NHSO-4923
    Examples:
      | GP System |
      | TPP       |
    Examples:
      | GP System |
      | EMIS      |
      | VISION    |

  Scenario Outline: Requesting my appointments the <GP System> times out and returns "Gateway Timeout" error
    Given <GP System> will time out when trying to retrieve my appointments
    And I have logged in and have a valid session cookie
    When my appointments are requested
    Then I receive a "Gateway Timeout" error
    And the response contains an empty body
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: <GP System> user sees an unknown exception on booked appointments request
    Given an unknown exception occurs when I want to view my <GP System> appointments
    And I have logged in and have a valid session cookie
    When my appointments are requested
    Then I receive a "Bad Gateway" error
    And the response contains an empty body
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: Receive a "Bad Gateway" error when TPP is unavailable for <Appointment Type> appointments
    Given TPP is unavailable for <Appointment Type> appointments
    And I have logged in and have a valid session cookie
    When my appointments are requested
    Then I receive a "Bad Gateway" error
    And the response contains an empty body
    Examples:
      | Appointment Type       |
      | past                   |
      | upcoming               |
      | both past and upcoming |
