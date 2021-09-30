@gp_session_on_demand
Feature: GP Session On Demand subject claim mismatch

  Scenario Outline: A <GP System> user trying to view prescriptions after a decoupled login is shown an error when subject claim differs between Token and UserInfo responses
    Given NHS Login returns an invalid Subject upon establishing a <GP System> GP session
    And I am a patient using the <GP System> GP System
    And I have 3 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And I am logged in
    When I retrieve the 'Your Prescriptions' page directly
    And I click the View Orders link
    And I see appropriate try again error message for prescriptions when there is no GP session
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: A <GP System> user trying to view appointments after a decoupled login is shown an error when subject claim differs between Token and UserInfo responses
    Given NHS Login returns an invalid Subject upon establishing a <GP System> GP session
    And I have upcoming appointments before cutoff time for <GP System>
    And I am logged in
    When I retrieve the 'appointment hub' page directly
    Then the Appointments Hub page is displayed
    When I click the GP Appointments link
    And I see appropriate try again error message when there is no GP session
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: A <GP System> user trying to view linked profiles after a decoupled login is shown an error when subject claim differs between Token and UserInfo responses
    Given NHS Login returns an invalid Subject upon establishing a <GP System> GP session
    And I am a patient using the <GP System> GP System
    And I am logged in
    When I can see and follow the Linked profiles link
    And I see appropriate linked profiles try again error message when there is no GP session
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario Outline: A <GP System> user trying to view medical record after a decoupled login is shown an error when subject claim differs between Token and UserInfo responses
    Given NHS Login returns an invalid Subject upon establishing a <GP System> GP session
    And I am a patient using the <GP System> GP System
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I see appropriate try again error message for gp medical record when there is no GP session
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
