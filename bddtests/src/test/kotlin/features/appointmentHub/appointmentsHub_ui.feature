@appointments
@appointmentsHub
Feature: Appointments Hub Frontend
  Users can see the Appointments Hub page and use the third party silver partner links

  Scenario: An EMIS user can see the Appointments Hub page and navigate to GP Appointments
    Given I am an EMIS user with no booked appointments
    And there are EMIS appointments available to book
    And I am logged in
    When I navigate to Appointments
    Then the Appointments Hub page is displayed
    And I click the GP Appointments link
    And the Your Appointments page is displayed

  Scenario Outline: An <GP System> user can recover their session on navigate to GP Appointments
    Given I am an <GP System> user with no booked appointments
    And there are <GP System> appointments available to book
    And I am logged in
    And <GP System> GP appointments returns unauthorized
    When I navigate to Appointments
    And I click the GP Appointments link
    Then I see appropriate try again error message when there is no GP session
    When I click the error 'Back' link
    Then the Appointments Hub page is displayed
    And I have no booked appointments for <GP System>
    And there are <GP System> appointments available to book
    When I click the GP Appointments link
    Then the Your Appointments page is displayed
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: An <GP System> user can recover their session by clicking try again on navigate to GP Appointments
    Given I am an <GP System> user with no booked appointments
    And there are <GP System> appointments available to book
    And I am logged in
    And <GP System> GP appointments returns unauthorized
    When I navigate to Appointments
    Then the Appointments Hub page is displayed
    And The <GP System> GP system is still unavailable
    When I click the GP Appointments link
    Then I see appropriate try again error message when there is no GP session
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: An <GP System> user without a GP session can recover their session on navigate to GP Appointments
    Given I am an <GP System> patient whose GP system is unavailable
    And I am logged in
    When I navigate to Appointments
    And The <GP System> GP system is still unavailable
    And I click the GP Appointments link
    Then I see appropriate try again error message when there is no GP session
    When I click the error 'Back' link
    Then the Appointments Hub page is displayed
    And The <GP System> GP system becomes available
    And I have no booked appointments for <GP System>
    And there are <GP System> appointments available to book
    When I click the GP Appointments link
    Then the Your Appointments page is displayed
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: An <GP System> user recovers their GP session before the error
    Given I am an <GP System> patient whose GP system is unavailable
    And I am logged in
    When I navigate to Appointments
    And The <GP System> GP system becomes available
    And I have no booked appointments for <GP System>
    And there are <GP System> appointments available to book
    And I click the GP Appointments link
    Then the Your Appointments page is displayed
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: An <GP System> user without a GP session is able to recover their session on try again
    Given I am an <GP System> patient whose GP system is unavailable
    And I am logged in
    When I navigate to Appointments
    And The <GP System> GP system is still unavailable
    And I click the GP Appointments link
    Then I see appropriate try again error message when there is no GP session
    And The <GP System> GP system becomes available
    And I have no booked appointments for <GP System>
    And there are <GP System> appointments available to book
    When I click the 'Try again' button
    Then the Your Appointments page is displayed
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: A <GP System> user GP session eventually becomes available
    Given I am an <GP System> patient whose GP system is unavailable
    And I am logged in
    When I navigate to Appointments
    And The <GP System> GP system is still unavailable
    And I click the GP Appointments link
    Then I see appropriate try again error message when there is no GP session
    When I click the 'Try again' button
    Then I see what I can do next with an error message and reference code '3p'
    And I click the session error back link
    And I click the GP Appointments link
    And I see what I can do next with an error message and reference code '3p'
    And I click the session error back link
    And the Appointments Hub page is displayed
    And The <GP System> GP system becomes available
    And I have no booked appointments for <GP System>
    And there are <GP System> appointments available to book
    When I click the GP Appointments link
    Then the Your Appointments page is displayed
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |
