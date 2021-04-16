@gp_session_on_demand
Feature: GP Session On Demand
  Users can view their upcoming and past appointments in the Your Appointments screen.

  Scenario Outline: A <GP System> user can see the Gp Appointments page after a decoupled GpSession login
    Given I have a Decoupled GP User Session
    And I have upcoming appointments before cutoff time for <GP System>
    And I am logged in
    When I retrieve the 'appointment hub' page directly
    Then the Appointments Hub page is displayed
    When I click the GP Appointments link
    Then I SSO to NhsLogin
    And the page title is "Your GP appointments"
    And I am given the list of upcoming appointments
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: A <GP System> user can see prescriptions after a decoupled GpSession login
    Given I have a Decoupled GP User Session
    And I am a patient using the <GP System> GP System
    And I have 3 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And I am logged in
    When I retrieve the 'Your Prescriptions' page directly
    And I click the View Orders link
    Then I SSO to NhsLogin
    And I see 3 prescriptions
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: A <GP System> user can see the order a repeat prescription after a decoupled GpSession login
    Given I have a Decoupled GP User Session
    And I am a <GP System> patient
    And I have historic prescriptions
    And there are 4 repeatable prescriptions available
    And I am logged in
    And I navigate to prescriptions
    When I click 'Order a new repeat prescription'
    Then I SSO to NhsLogin
    And I see the available repeatable prescriptions
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: A <Gp Provider> user who supports proxy can see their linked profiles after a decoupled GpSession login
    Given I have a Decoupled GP User Session
    And I am a <Gp Provider> patient
    And I am logged in
    When I can see and follow the Linked profiles link
    Then I SSO to NhsLogin
    And the linked profiles page is displayed
    And linked profiles are displayed
    Examples:
      | Gp Provider |
      | EMIS        |
      | TPP         |

