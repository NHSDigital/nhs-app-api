@organ-donation
@backend
Feature: Organ Donation Lookup Backend

  Scenario Outline: When looking for a registration, a <GP System> user who opted out receives a 200 and donation details
    Given I am a <GP System> api user registered with organ donation to not donate my organs
    And I have logged in and have a valid session cookie
    When I request my organ donation details
    Then I receive organ donation details with an 'OptOut' decision
    And I receive the users demographics details
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: When looking for a registration, a <GP System> user who opted in receives a 200 and donation details
    Given I am a <GP System> api user registered with organ donation to donate all organs
    And I have logged in and have a valid session cookie
    When I request my organ donation details
    Then I receive organ donation details with an 'OptIn' decision
    And I receive the users demographics details
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: When looking for a registration, a <GP System> user who opted in with some organs receives a 200
  and donation details
    Given I am a <GP System> api user registered with organ donation to donate some organs
    And I have logged in and have a valid session cookie
    When I request my organ donation details
    Then I receive a "OK" success code
    Then I receive organ donation details with an 'OptIn' decision
    And I receive the users demographics details
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: When looking for a registration, a <GP System> user who has an appointed representative receives a 200
  and donation details
    Given I am a <GP System> api user registered with organ donation with an appointed representative
    And I have logged in and have a valid session cookie
    When I request my organ donation details
    Then I receive organ donation details with an 'AppRep' decision
    And I receive the users demographics details
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |


  Scenario Outline: When looking for an organ donation registration, an unregistered <GP System> user receives a 200
  and no donation details
    Given I am a <GP System> api user not registered with organ donation
    And I have logged in and have a valid session cookie
    When I request my organ donation details
    Then I receive a "OK" success code
    And I receive the users demographics details
    And I receive no organ donation details
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: When looking for an organ donation registration but the system has a conflict, the <GP System>
  user receives a 200 response with state value of "conflicted"
    Given I am a <GP System> api user registered with organ donation, but organ donation will conflict
    And I have logged in and have a valid session cookie
    When I request my organ donation details
    Then I receive a "Ok" success code
    And I receive an organ donation response with state value of 'conflicted'
    Examples:
      | GP System |
      | VISION    |

  Scenario Outline: When looking for an organ donation registration, an OD response of <Error Code> will prompt a 500
  response and no retry option
    Given I am a EMIS api user registered with OD, but lookup returns '<Error Code>' error
    And I have logged in and have a valid session cookie
    When I request my organ donation details
    Then I receive a "Internal Server Error" error
    And the Internal Server Error response does not include a retry option
    Examples:
      | Error Code |
      | 400        |
      | 401        |
      | 403        |
      | 405        |
      | 415        |
      | 422        |

  Scenario Outline: When looking for an organ donation registration, an OD response of <Error Code> will prompt a 502
  response and a retry option
    Given I am a EMIS api user registered with OD, but lookup returns '<Error Code>' error
    And I have logged in and have a valid session cookie
    When I request my organ donation details
    Then I receive a "Bad Gateway" error
    And the Bad Gateway error response includes a retry option
    Examples:
      | Error Code |
      | 429        |
      | 503        |
      | 504        |

  Scenario Outline: When looking for an organ donation registration, an OD response of <Error Code> will prompt a 502
  response and no retry option
    Given I am a EMIS api user registered with OD, but lookup returns '<Error Code>' error
    And I have logged in and have a valid session cookie
    When I request my organ donation details
    Then I receive a "Bad Gateway" error
    And the Bad Gateway error response does not include a retry option
    Examples:
      | Error Code |
      | 500        |
      | 502        |

  Scenario Outline: When looking for an organ donation registration but <GP System> fails to return demographics,
  the user receives a 502 response
      #The OD data needs to be placed after log in, as the demographics timeout will affect creating a session
    Given I have logged into <GP System> and have a valid session cookie
    And I am an api user registered with organ donation, but demographics will return an internal error
    When I request my organ donation details
    Then I receive a "Bad Gateway" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: When looking for an organ donation registration but <GP System> times out when returning
  demographics, the user receives a 504 response
    #The OD data needs to be placed after log in, as the demographics timeout will affect creating a session
    Given I have logged into <GP System> and have a valid session cookie
    And I am an api user registered with organ donation, but demographics will time out
    When I request my organ donation details
    Then I receive a "Gateway Timeout" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
