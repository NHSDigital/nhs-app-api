@prescription
Feature: View prescriptions error cases
  A user can view information about their prescriptions after logging in

  Scenario Outline: A <GP System> user tries to navigate to the prescriptions page, but the request to retrieve the prescriptions times out
    Given I am a <GP System> patient
    And I am using <GP System> GP System
    And I am logged in
    Given The prescriptions endpoint is timing out
    When I navigate to prescriptions
    And I wait for 20 seconds
    Then I see the appropriate error message for a prescription timeout
    Examples:
      | GP System |
      | TPP       |
      | VISION    |

  @native-smoketest
    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: A <GP System> user tries to navigate to the prescriptions page, but the request to retrieve the prescriptions throws a server error
    Given I am a <GP System> patient
    And I am using <GP System> GP System
    And I am logged in
    And The prescriptions endpoint is throwing a server error
    When I navigate to prescriptions
    Then I see the appropriate error message for a prescription server error
    Examples:
      | GP System |
      | TPP       |
      | VISION    |

  @native-smoketest
    Examples:
      | GP System |
      | EMIS      |

  @nativepending @NHSO-2974
  Scenario: A user navigates to the prescriptions page and the session times out
    Given I am a EMIS patient
    And I am using EMIS GP System
    And I have 3 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    When I am on the prescriptions page
    And My session has expired
    Then I see the login page with the session expiry notification

  Scenario Outline: A <GP System> user tried to navigate to the 'Order a Repeat Prescription' page, but the request to retrieve the repeat prescriptions to order times out
    Given I am a <GP System> patient
    And I am using <GP System> GP System
    And I am logged in
    And I have 10 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    When I navigate to prescriptions
    But The courses endpoint is timing out
    And I click 'Order a new repeat prescription'
    And I wait for 20 seconds
    Then I see the appropriate error message for a prescription timeout
    Examples:
      | GP System |
      | TPP       |
      | VISION    |

  @native-smoketest
    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: A <GP System> user tried to navigate to the 'Order a Repeat Prescription' page, but the request to retrieve the repeat prescriptions to order throws a server error
    Given I am a <GP System> patient
    And I am using <GP System> GP System
    And I am logged in
    And I have 10 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    When I navigate to prescriptions
    But The courses endpoint is throwing a server error
    And I click 'Order a new repeat prescription'
    Then I see the appropriate error message for a prescription server error
    Examples:
      | GP System |
      | TPP       |
      | VISION    |

  @native-smoketest
    Examples:
      | GP System |
      | EMIS      |

  @nativepending @NHSO-2974
  Scenario Outline: A <GP System> user tries to place an order for a repeat subscription, but the request times out
    Given I am a <GP System> patient
    And I am using <GP System> GP System
    And I am logged in
    And I have 10 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And I have 10 <GP System> assigned prescriptions
    And 10 of my prescriptions are of type repeat
    And 10 of my prescriptions can be requested
    But The prescription submission endpoint is timing out
    When I navigate to prescriptions
    And I click 'Order a new repeat prescription'
    And I select 1 prescription to order
    And I wait for 20 seconds
    Then I see the appropriate error message for a course request error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: A <GP System> user tries to place an order for a repeat subscription, but the request throws a server error
    Given I am a <GP System> patient
    And I am using <GP System> GP System
    And I am logged in
    And I have 10 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And I have 10 <GP System> assigned prescriptions
    And 10 of my prescriptions are of type repeat
    And 10 of my prescriptions can be requested
    But The prescription submission endpoint is throwing a server error
    When I navigate to prescriptions
    And I click 'Order a new repeat prescription'
    And I select 1 prescription to order
    Then I see the appropriate error message for a course request error
    Examples:
      | GP System |
      | TPP       |
      | VISION    |

  @native-smoketest
    Examples:
      | GP System |
      | EMIS      |

  @native-smoketest
  Scenario Outline: A <GP System> user tries to place an order for a repeat subscription, but request returns an already ordered response
    Given I am a <GP System> patient
    And I am using <GP System> GP System
    And I am logged in
    And I have 10 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And I have 10 <GP System> assigned prescriptions
    And 10 of my prescriptions are of type repeat
    And 10 of my prescriptions can be requested
    But The prescription submission endpoint is throwing an already ordered exception
    When I navigate to prescriptions
    And I click 'Order a new repeat prescription'
    And I select 1 prescription to order
    Then I see the appropriate error message for a course request error
    Examples:
      | GP System |
      | TPP       |

  @native-smoketest
  Scenario Outline: A <GP System> user tries to place an order for a repeat subscription, but request returns an invalid guid error
    Given I am a <GP System> patient
    And I am using <GP System> GP System
    And I am logged in
    And I have 10 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And I have 10 <GP System> assigned prescriptions
    And 10 of my prescriptions are of type repeat
    And 10 of my prescriptions can be requested
    But The prescription submission endpoint is throwing an invalid guid exception
    When I navigate to prescriptions
    And I click 'Order a new repeat prescription'
    And I select 1 prescription to order
    Then I see the appropriate error message for a course request error
    Examples:
      | GP System |
      | TPP       |
