Feature: View prescriptions error cases

  A user can view information about their prescriptions after logging in

  @NHSO-498
  @prescription
  Scenario Outline: A <GP System> user tries to navigate to the prescriptions page, but the request to retrieve the prescriptions times out
    Given <GP System> is initialised
    And I am using <GP System> GP System
    And I am logged in
    Given The prescriptions endpoint is timing out
    When I navigate to prescriptions
    And I wait for 20 seconds
    Then I see the appropriate error message for a prescription timeout

    Examples:
      | GP System |
      | EMIS      |

  @NHSO-498
  @prescription
  Scenario Outline: A <GP System> user tries to navigate to the prescriptions page, but the request to retrieve the prescriptions throws a server error
    Given <GP System> is initialised
    And I am using <GP System> GP System
    And I am logged in
    And The prescriptions endpoint is throwing a server error
    When I navigate to prescriptions
    Then I see the appropriate error message for a prescription server error

    Examples:
      | GP System |
      | EMIS      |

  # No yellow banner showing, this is a bug with NHSO-415, so this test will fail until this is resolved
  @NHSO-498
  @bug
  @prescription
  Scenario: A user tries to navigate to the prescriptions page, but the session has timed out
    Given My session has expired
    When I navigate to prescriptions
    Then I am kicked back to the login page

  @NHSO-513
  @prescription
  Scenario Outline: A <GP System> user tried to navigate to the 'Order a Repeat Prescription' page, but the request to retrieve the repeat prescriptions to order times out
    Given <GP System> is initialised
    And I am using <GP System> GP System
    And I am logged in
    And I have 10 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    But The courses endpoint is timing out
    When I navigate to prescriptions
    And I click 'Order a repeat prescription'
    And I wait for 20 seconds
    Then I see the appropriate error message for a prescription timeout

    Examples:
      | GP System |
      | EMIS      |

  @NHSO-513
  @prescription
  Scenario Outline: A <GP System> user tried to navigate to the 'Order a Repeat Prescription' page, but the request to retrieve the repeat prescriptions to order throws a server error
    Given <GP System> is initialised
    And I am using <GP System> GP System
    And I am logged in
    And I have 10 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    But The courses endpoint is throwing a server error
    When I navigate to prescriptions
    And I click 'Order a repeat prescription'
    Then I see the appropriate error message for a prescription server error

    Examples:
      | GP System |
      | EMIS      |

  @NHSO-858
  @NHSO-514
  @prescription
  Scenario Outline: A <GP System> user tries to place an order for a repeat subscription, but the request times out
    Given <GP System> is initialised
    And I am using <GP System> GP System
    And I am logged in
    And I have 10 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And I have 10 <GP System> assigned prescriptions
    And 10 of my prescriptions are of type repeat
    And 10 of my prescriptions can be requested
    But The prescription submission endpoint is timing out
    When I navigate to prescriptions
    And I click 'Order a repeat prescription'
    And I select 1 prescription to order
    And I wait for 20 seconds
    Then I see the appropriate error message for a course request error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  @NHSO-858
  @prescription
  Scenario Outline: A <GP System> user tries to place an order for a repeat subscription, but the request throws a server error
    Given <GP System> is initialised
    And I am using <GP System> GP System
    And I am logged in
    And I have 10 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And I have 10 <GP System> assigned prescriptions
    And 10 of my prescriptions are of type repeat
    And 10 of my prescriptions can be requested
    But The prescription submission endpoint is throwing a server error
    When I navigate to prescriptions
    And I click 'Order a repeat prescription'
    And I select 1 prescription to order
    Then I see the appropriate error message for a course request error
    Examples:
      | GP System |
      | EMIS      |

  @NHSO-858
  @prescription
  Scenario Outline: A <GP System> user tries to place an order for a repeat subscription, but request returns an already ordered response
    Given <GP System> is initialised
    And I am using <GP System> GP System
    And I am logged in
    And I have 10 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And I have 10 <GP System> assigned prescriptions
    And 10 of my prescriptions are of type repeat
    And 10 of my prescriptions can be requested
    But The prescription submission endpoint is throwing an already ordered exception
    When I navigate to prescriptions
    And I click 'Order a repeat prescription'
    And I select 1 prescription to order
    Then I see the appropriate error message for a course request error
    Examples:
      | GP System |
      | TPP       |

  @NHSO-858
  @prescription
  Scenario Outline: A <GP System> user tries to place an order for a repeat subscription, but request returns an invalid guid error
    Given <GP System> is initialised
    And I am using <GP System> GP System
    And I am logged in
    And I have 10 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And I have 10 <GP System> assigned prescriptions
    And 10 of my prescriptions are of type repeat
    And 10 of my prescriptions can be requested
    But The prescription submission endpoint is throwing an invalid guid exception
    When I navigate to prescriptions
    And I click 'Order a repeat prescription'
    And I select 1 prescription to order
    Then I see the appropriate error message for a course request error
    Examples:
      | GP System |
      | TPP       |