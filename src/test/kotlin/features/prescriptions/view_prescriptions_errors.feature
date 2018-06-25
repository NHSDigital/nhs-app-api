Feature: View prescriptions (errors)

  A user can view information about their prescriptions after logging in

  Background:
    Given wiremock is initialised
    And I am logged in

  @pending
  @NHSO-498
  Scenario: A user tries to navigate to the prescriptions page, but the request to retrieve the prescriptions times out
    Given The prescriptions endpoint is timing out
    When I navigate to prescriptions
    And I wait for 11 seconds
    Then I see the appropriate error message for a prescription timeout

  @NHSO-498
  Scenario: A user tries to navigate to the prescriptions page, but the request to retrieve the prescriptions throws a server error
    Given The prescriptions endpoint is throwing a server error
    When I navigate to prescriptions
    Then I see the appropriate error message for a prescription server error

  # No yellow banner showing, this is a bug with NHSO-415, so this test will fail until this is resolved
  @NHSO-498
  @bug
  Scenario: A user tries to navigate to the prescriptions page, but the session has timed out
    Given My session has expired
    When I navigate to prescriptions
    Then I am kicked back to the login page

  @pending
  @NHSO-513
  Scenario: A user tried to navigate to the 'Order a Repeat Prescription' page, but the request to retrieve the repeat prescriptions to order times out
    Given I have 10 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    But The courses endpoint is timing out
    When I navigate to prescriptions
    And I click 'Order a repeat prescription'
    And I wait for 12 seconds
    Then I see the appropriate error message for a prescription timeout

  @NHSO-513
  Scenario: A user tried to navigate to the 'Order a Repeat Prescription' page, but the request to retrieve the repeat prescriptions to order throws a server error
    Given I have 10 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    But The courses endpoint is throwing a server error
    When I navigate to prescriptions
    And I click 'Order a repeat prescription'
    Then I see the appropriate error message for a prescription server error

  @NHSO-514
  Scenario: A user tries to place an order for a repeat subscription, but the request times out
    Given I have 10 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And I have 10 assigned prescriptions
    And 10 of my prescriptions are of type repeat
    And 10 of my prescriptions can be requested
    But The prescription submission endpoint is timing out
    When I navigate to prescriptions
    And I click 'Order a repeat prescription'
    And I select 1 prescription to order
    And I wait for 11 seconds
    Then I see the appropriate error message for a course request error

  @NHSO-514
  Scenario: A user tries to place an order for a repeat subscription, but the request throws a server error
    Given I have 10 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And I have 10 assigned prescriptions
    And 10 of my prescriptions are of type repeat
    And 10 of my prescriptions can be requested
    But The prescription submission endpoint is throwing a server error
    When I navigate to prescriptions
    And I click 'Order a repeat prescription'
    And I select 1 prescription to order
    Then I see the appropriate error message for a course request error