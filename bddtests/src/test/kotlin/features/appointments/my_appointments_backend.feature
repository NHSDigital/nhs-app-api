@appointment
@backend
Feature: My appointments backend
  In order control my booked appointments
  As a logged user
  I want to see my appointments

  Scenario Outline: API call for my appointments will successfully return appointments from <GP System> when they are only upcoming
    Given I have upcoming appointments before cutoff time for <GP System>
    And I have logged into <GP System> and have a valid session cookie
    When the upcoming appointments are requested
    Then I will only receive upcoming appointments
    And a list of cancellation reasons if the GP Service provides the list
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario: API call for my appointments will return appointments from EMIS when they are only historical
    Given I have historical appointments for EMIS
    And I have logged in and have a valid session cookie
    When the upcoming appointments are requested
    Then I will only receive historical appointments

  Scenario: API call for my appointments will return appointments from EMIS when there are historical and upcoming
    Given I have historical and upcoming appointments for EMIS
    And I have logged in and have a valid session cookie
    When the upcoming appointments are requested
    Then I will receive both historical and upcoming appointments
    And a list of cancellation reasons if the GP Service provides the list

  Scenario: API call for upcoming appointments won't filter out past appointments from VISION
    Given I have upcoming appointments for VISION, with one in the past
    And I have logged into VISION and have a valid session cookie
    When the upcoming appointments are requested
    Then I will receive upcoming appointments with appointments in the past

  Scenario: API call for upcoming appointments will filter out past appointments from TPP
    Given I have upcoming appointments for TPP, with one in the past
    And I have logged into TPP and have a valid session cookie
    When the upcoming appointments are requested
    Then I will only receive upcoming appointments

  Scenario Outline: API call for my appointments will return an empty response from <GP System> when they are no appointments to retrieve
    Given I have no booked appointments for <GP System>
    And I have logged in and have a valid session cookie
    When the upcoming appointments are requested
    Then I will receive no appointments
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: Online appointment booking is not available to a particular patient of <GP System>
    Given I have logged into <GP System> and have a valid session cookie
    But <GP System> does not offer online booking to my patient
    When the upcoming appointments are requested
    Then I receive a "Forbidden" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: Appropriate error response, when <GP System> returns corrupted data
    Given I have logged into <GP System> and have a valid session cookie
    But <GP System> returns corrupted response for my appointments
    When the upcoming appointments are requested
    Then I receive a "Internal Server Error" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: Requesting my appointments the <GP System> times out and returns "Gateway Timeout" error
    Given I have logged into <GP System> and have a valid session cookie
    But <GP System> will time out when trying to retrieve my appointments
    When the upcoming appointments are requested
    Then I receive a "Gateway Timeout" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: <GP System> user sees an unknown exception on booked appointments request
    Given an unknown exception occurs when I want to view my <GP System> appointments
    And I have logged into <GP System> and have a valid session cookie
    When the upcoming appointments are requested
    Then I receive a "Bad Gateway" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |