@appointment
@backend
Feature: My appointments backend
  In order control my booked appointments
  As a logged user
  I want to see my upcoming appointments

  @backend
  Scenario Outline: API call for upcoming appointments will only request upcoming appointments from <GP System>
    Given I have upcoming appointments for <GP System>
    And I have logged into <GP System> and have a valid session cookie
    When the upcoming appointments are requested
    Then I will only receive upcoming appointments
    And a list of cancellation reasons if the GP Service provides the list
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  @backend
  Scenario Outline: API call for upcoming appointments won't filter out past appointments from <GP System>
    Given I have upcoming appointments for <GP System>, with one in the past
    And I have logged into <GP System> and have a valid session cookie
    When the upcoming appointments are requested
    Then I will receive upcoming appointments with appointments in the past
    Examples:
      | GP System |
      | EMIS      |
      | VISION    |

  @backend
  Scenario: API call for upcoming appointments will filter out past appointments from TPP
    Given I have upcoming appointments for TPP, with one in the past
    And I have logged into TPP and have a valid session cookie
    When the upcoming appointments are requested
    Then I will only receive upcoming appointments

  @backend
  Scenario Outline: Online appointment booking is not available to a particular patient
    Given I have logged into <GP System> and have a valid session cookie
    But <GP System> does not offer online booking to my patient
    When the upcoming appointments are requested
    Then I receive a "Forbidden" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION       |

  @backend
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

  @backend
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

  @backend
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