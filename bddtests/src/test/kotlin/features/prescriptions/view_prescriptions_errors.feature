@prescription
Feature: View prescriptions error cases
  A user can view information about their prescriptions after logging in

# These tests navigate directly to the pages where the features are to be tested, to save time.

  @nativesmoketest
  Scenario Outline: A <GP System> user tries to navigate to the prescriptions page, but the request to retrieve the prescriptions times out
    Given I am patient using the <GP System> GP System
    And I am logged in
    And The prescriptions endpoint is timing out
    When I retrieve the 'Your Prescriptions' page directly
    And I click the View Orders link
    And I wait for 20 seconds
    Then I see the appropriate error message for a prescription timeout
    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: A <GP System> user sees a reference code when they login without a GP system due to a timeout
    Given I have valid OAuth details and <GP System> fails to respond in 31 seconds
    And I am logged in
    When I retrieve the 'Prescription Repeat Courses' page directly
    Then I see appropriate try again error message for prescriptions when there is no GP session
    When I click the 'Try again' button
    Then I see the error reference code with prefix '<Prefix>'
    And I click the report a problem link
    And a new tab has been opened by the link
    Examples:
      | GP System | Prefix |
      | EMIS      | ze     |
      | TPP       | zt     |
      | VISION    | zs     |
      | MICROTEST | zm     |

  @nativesmoketest
  Scenario Outline: A <GP System> user tries to navigate to the prescriptions page, but the request to retrieve the prescriptions throws a server error
    Given I am patient using the <GP System> GP System
    And I am logged in
    And The prescriptions endpoint is throwing a server error
    When I retrieve the 'Your Prescriptions' page directly
    And I click the View Orders link
    Then I see the appropriate error message for a prescription server error
    Examples:
      | GP System |
      | VISION    |

  @long-running
  Scenario: A user navigates to the prescriptions page and the session times out
    Given I am patient using the EMIS GP System
    And I have 3 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And I am logged in
    When I retrieve the 'Your Prescriptions' page directly
    And I click the Order a repeat prescription button
    And My session has expired
    Then I see the login page with the session expiry notification

  Scenario Outline: A <GP System> user tried to navigate to the 'Order a Repeat Prescription' page, but the request to retrieve the repeat prescriptions to order times out
    Given I am patient using the <GP System> GP System
    And I am logged in
    And I have 10 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    Then I retrieve the 'Your Prescriptions' page directly
    And The courses endpoint is timing out
    When I retrieve the 'Prescription Repeat Courses' page directly
    And I wait for 20 seconds
    Then I see the appropriate error message for a prescription timeout
    Examples:
      | GP System |
      | TPP       |
      | VISION    |

  @nativesmoketest
    Examples:
      | GP System |
      | EMIS      |

  @nativesmoketest
  Scenario Outline: A <GP System> user tried to navigate to the 'Order a Repeat Prescription' page, but the request to retrieve the repeat prescriptions to order throws a server error
    Given I am patient using the <GP System> GP System
    And I am logged in
    And I have 10 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    Then I retrieve the 'Your Prescriptions' page directly
    But The courses endpoint is throwing a server error
    When I retrieve the 'Prescription Repeat Courses' page directly
    Then I see the appropriate error message for a prescription server error
    Examples:
      | GP System |
      | TPP       |

  Scenario Outline: A <GP System> user tries to place an order for a repeat subscription, but the request times out
    Given I am patient using the <GP System> GP System
    And I am logged in
    And I have 10 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And I have 10 assigned prescriptions
    And 10 of my prescriptions are of type repeat
    And 10 of my prescriptions can be requested
    Then I retrieve the 'Your Prescriptions' page directly
    And the prescription submission endpoint is timing out
    When I retrieve the 'Prescription Repeat Courses' page directly
    And I select 1 prescription to order
    And I wait for 20 seconds
    Then I see the appropriate error message for a course request error
    Examples:
      | GP System |
      | VISION    |

  @nativesmoketest
  Scenario Outline: A <GP System> user tries to place an order for a repeat subscription, but the request throws a server error
    Given I am patient using the <GP System> GP System
    And I am logged in
    And I have 10 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And I have 10 assigned prescriptions
    And 10 of my prescriptions are of type repeat
    And 10 of my prescriptions can be requested
    Then I retrieve the 'Your Prescriptions' page directly
    And The prescription submission endpoint is throwing a server error
    When I retrieve the 'Prescription Repeat Courses' page directly
    And I select 1 prescription to order
    Then I see the appropriate error message for a course request error
    Examples:
      | GP System |
      | EMIS      |

  @nativesmoketest
  Scenario Outline: A <GP System> user tries to place an order for a repeat subscription, but request returns an already ordered response
    Given I am patient using the <GP System> GP System
    And I am logged in
    And I have 10 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And I have 10 assigned prescriptions
    And 10 of my prescriptions are of type repeat
    And 10 of my prescriptions can be requested
    Then I retrieve the 'Your Prescriptions' page directly
    And The prescription submission endpoint is throwing an already ordered exception
    When I retrieve the 'Prescription Repeat Courses' page directly
    And I select 1 prescription to order
    Then I see the appropriate error message for a course request error
    Examples:
      | GP System |
      | TPP       |

  @nativesmoketest
  Scenario Outline: A <GP System> user tries to place an order for a repeat subscription, but request returns an invalid guid error
    Given I am patient using the <GP System> GP System
    And I am logged in
    And I have 10 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And I have 10 assigned prescriptions
    And 10 of my prescriptions are of type repeat
    And 10 of my prescriptions can be requested
    Then I retrieve the 'Your Prescriptions' page directly
    And The prescription submission endpoint is throwing an invalid guid exception
    When I retrieve the 'Prescription Repeat Courses' page directly
    And I select 1 prescription to order
    Then I see the appropriate error message for a course request error
    Examples:
      | GP System |
      | TPP       |
