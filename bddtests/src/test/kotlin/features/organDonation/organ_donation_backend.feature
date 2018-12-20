@organ-donation
@backend
Feature: Organ Donation Backend

  Scenario Outline: When looking for an organ donation registration, a registered <GP System> user receives a 200
  and donation details
    Given I am a <GP System> user registered with organ donation
    And I have logged in and have a valid session cookie
    When I request my organ donation details
    And I receive organ donation details
    And I receive the users demographics details
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: When looking for an organ donation registration, an unregistered <GP System> user receives a 200
  and no donation details
    Given I am a <GP System> user not registered with organ donation
    And I have logged in and have a valid session cookie
    When I request my organ donation details
    And I receive the users demographics details
    And I receive no organ donation details
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: When looking for an organ donation registration but the system has a conflict, the <GP System>
  user receives a 409 response
    Given I am a <GP System> user registered with organ donation, but organ donation will conflict
    And I have logged in and have a valid session cookie
    When I request my organ donation details
    Then I receive a "conflict" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: When looking for an organ donation registration but the call returns internal error, the
  <GP System> user receives a 502 response
    Given I am a <GP System> user registered with organ donation, but organ donation call will return an internal error
    And I have logged in and have a valid session cookie
    When I request my organ donation details
    Then I receive a "bad gateway" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: When looking for an organ donation registration but the call times out, the <GP System> user
  receives a 504 response
    Given I am a <GP System> user registered with organ donation, but organ donation call will time out
    And I have logged in and have a valid session cookie
    When I request my organ donation details
    Then I receive a "gateway timeout" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |


  Scenario Outline: When looking for an organ donation registration with an invalid session, the <GP System> user
  receives a 401 response
    Given I am a <GP System> user registered with organ donation
    And I have logged in and have a valid session cookie
    And I am idle long enough for the backend session to expire
    When I request my organ donation details
    Then I receive a "unauthorized" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: When looking for an organ donation registration but <GP System> fails to return demographics,
  the user receives a 502 response
      #The OD data needs to be placed after log in, as the demographics timeout will affect creating a session
    Given I have logged into <GP System> and have a valid session cookie
    Given I am a <GP System> user registered with organ donation, but demographics will return an internal error
    When I request my organ donation details
    Then I receive a "bad gateway" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: When looking for an organ donation registration but <GP System> times out when returning
  demographics, the user receives a 504 response
      #The OD data needs to be placed after log in, as the demographics timeout will affect creating a session
      Given I have logged into <GP System> and have a valid session cookie
      And I am a <GP System> user registered with organ donation, but demographics will time out
      When I request my organ donation details
      Then I receive a "gateway timeout" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
