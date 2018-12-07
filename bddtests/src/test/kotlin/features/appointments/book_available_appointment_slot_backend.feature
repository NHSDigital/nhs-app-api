@appointment
@backend
Feature: Book appointments
  In order to visit a clinician
  As a logged in user
  I want to book an appointment to see a clinician at my GP practice

  Scenario Outline: Booking an appointment with <GP System> returns a successful response
    Given an appointment booking for <GP System> can be successful
    And I have logged into <GP System> and have a valid session cookie
    When an appointment booking is submitted
    Then a successful response for appointment booking is returned
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: Booking an appointment with <GP System> returns "Bad Request" response if no slot identifier is provided
    Given I have logged into <GP System> and have a valid session cookie
    When an appointment booking is submitted with no slot identifier
    Then I receive a "Bad Request" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario Outline: Booking an appointment with <GP System> returns "Bad Request" response if empty slot identifier is provided
    Given an appointment booking for <GP System> can be successful
    And I have logged into <GP System> and have a valid session cookie
    When an appointment booking is submitted with slot identifier of 0 characters
    Then I receive a "Bad Request" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario Outline: Booking an appointment with <GP System> returns a successful response if slot identifier is 1 character
    Given an appointment booking for <GP System> can be successful with slot identifier of 1 character
    And I have logged into <GP System> and have a valid session cookie
    When an appointment booking is submitted
    Then a successful response for appointment booking is returned
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: Booking an appointment with <GP System> returns "Bad Request" response if no booking reason is provided
    Given I have logged into <GP System> and have a valid session cookie
    When an appointment booking is submitted with no booking reason
    Then I receive a "Bad Request" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario Outline: Booking an appointment with <GP System> returns "Bad Request" response if an empty booking reason is provided
    Given an appointment booking for <GP System> can be successful
    And I have logged into <GP System> and have a valid session cookie
    When an appointment booking is submitted with booking reason of 0 characters
    Then I receive a "Bad Request" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario Outline: Booking an appointment with <GP System> returns successful response if the booking reason is 1 character
    Given an appointment booking for <GP System> can be successful with booking reason of 1 character
    And I have logged into <GP System> and have a valid session cookie
    When an appointment booking is submitted
    Then a successful response for appointment booking is returned
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: Booking an appointment with <GP System> returns successful response if the booking reason is 150 characters
    Given an appointment booking for <GP System> can be successful with booking reason of 150 characters
    And I have logged into <GP System> and have a valid session cookie
    When an appointment booking is submitted
    Then a successful response for appointment booking is returned
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: Booking an appointment with <GP System> returns "Bad Request" response if the booking reason exceeds 150 characters
    Given an appointment booking for <GP System> can be successful
    And I have logged into <GP System> and have a valid session cookie
    When an appointment booking is submitted with booking reason of 151 characters
    Then I receive a "Bad Request" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario Outline: Booking an appointment with <GP System> returns "Unauthorized" response if the user session has expired
    Given an appointment booking for <GP System> can be successful, but session has expired
    When an appointment booking is submitted
    Then I receive an "Unauthorized" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: Booking an appointment with <GP System> returns "Unauthorized" response if no user session cookie was generated
    Given an appointment booking for <GP System> can be successful
    When an appointment booking is submitted
    Then I receive an "Unauthorized" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario Outline: Booking an appointment with <GP System> returns "Forbidden" response if online appointment booking is not available for this patient
    Given online appointment booking is not available to the <GP System> patient, when wanting to book an appointment
    And I have logged into <GP System> and have a valid session cookie
    When an appointment booking is submitted
    Then I receive an "Forbidden" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: Booking an appointment with <GP System> returns "Conflict" response if the chosen appointment slot is not available for booking
    Given an appointment booking for <GP System> cannot be successful because the slot is not available
    And I have logged into <GP System> and have a valid session cookie
    When an appointment booking is submitted
    Then I receive a "Conflict" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

#  Note: VISION ALLOWS To book appointments that are in past
  Scenario Outline: Booking an appointment with <GP System> returns "Conflict" response if the chosen appointment slot is in the past
    Given an appointment booking for <GP System> cannot be successful because the slot is in the past
    And I have logged into <GP System> and have a valid session cookie
    When an appointment booking is submitted
    Then I receive an "Conflict" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario Outline: Booking an appointment with <GP System> returns "Conflict" response if the chosen appointment slot has been booked by someone else
    Given an appointment booking for <GP System> cannot be successful because the slot has been booked by someone else
    And I have logged into <GP System> and have a valid session cookie
    When an appointment booking is submitted
    Then I receive an "Conflict" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: Booking an appointment with <GP System> returns "Bad Gateway" response if an unknown exception occurs
    Given an appointment booking for <GP System> generates an unknown exception
    And I have logged into <GP System> and have a valid session cookie
    When an appointment booking is submitted
    Then I receive a "Bad Gateway" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: Booking an appointment with <GP System> returns "Bad Gateway" response if the GP system is currently unavailable
    Given an appointment booking for <GP System> cannot be successful because the GP system is unavailable
    And I have logged into <GP System> and have a valid session cookie
    When an appointment booking is submitted
    Then I receive a "Bad Gateway" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: Booking an appointment with <GP System> returns "Gateway Timeout" response if the GP system did not respond in a timely fashion
    Given an appointment booking for <GP System> cannot be successful because the GP system will time out
    And I have logged into <GP System> and have a valid session cookie
    When an appointment booking is submitted
    Then I receive a "Gateway Timeout" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: <GP System> returns corrupted data
    Given <GP System> returns corrupted response for booking request
    And I have logged into <GP System> and have a valid session cookie
    When an appointment booking is submitted
    Then I receive a "Internal Server Error" error
  @bug @NHSO-3039
    Examples:
      | GP System |
      | EMIS      |
    Examples:
      | GP System |
      | TPP       |
      | VISION    |

