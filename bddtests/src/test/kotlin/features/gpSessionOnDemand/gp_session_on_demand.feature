@gp_session_on_demand
Feature: GP Session On Demand

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

  Scenario Outline: A <Gp Provider> user can view his Medical Record after a GpSession On Demand login
    Given I have a Decoupled GP User Session
    And I am a <Gp Provider> user setup to use medical record version 2
    And the GP Practice has enabled all medical records for the patient
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    Then I SSO to NhsLogin
    And I see the medical record v2 page
    When I click the Medicines link on my record - Medical Record v2
    Then I see the medical record v2 medicines page
    When I click the Acute medicines link - Medical Record v2
    Then I see the expected acute medicines - Medical Record v2
    When I click the Back link
    Then I see the medical record v2 medicines page
    When I click the Back link
    And I click the Consultations and events link on my record - Medical Record v2
    Then I see the expected consultations and events - Medical Record v2
    When I click the Back link
    And I click the Test results link on my record - Medical Record v2
    Then I see the correct number of test results for current the supplier - Medical Record v2
    Examples:
      | Gp Provider |
      | EMIS        |
      | TPP         |

  Scenario Outline: A user can select '<Option>' for faith and belief sharing when opting to donate all their organs after a GpSession On Demand login
    Given I have a Decoupled GP User Session
    And I am using the native app user agent
    And I am a EMIS user not registered with organ donation, who wishes to opt in with '<Option>' faith decision
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    Then I SSO to NhsLogin
    And I choose to donate my organs
    And the Organ Donation Your Choice page is displayed
    When I select the option to donate all my organs
    And I click the 'Continue' button on an Organ Donation page
    Then the Organ Donation Faith And Beliefs page is displayed
    And no options on the Organ Donation Faith And Beliefs page are selected
    When I select the option '<Option>' to share my organ donation faith and beliefs
    And I click the 'Continue' button on an Organ Donation page
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Continue' button on an Organ Donation page
    Then the Organ Donation Check Details page is displayed
    And the choice of wishing to donate organs is displayed on the Organ Donation Check Details page
    And my choice of '<Option>' to share my faith and beliefs is displayed on the Organ Donation Check Details page
    When I confirm that my details are accurate, and accept the privacy statement for organ donation
    And I click the 'Submit my decision' button on an Organ Donation page
    Then the Organ Donation View Registration page is displayed
    And the decision to opt in to organ donation has been successfully created
    And the faith and beliefs decision of '<Option>' is displayed on the Organ Donation View Registration page
    Examples:
      | Option                            |
      | Yes - this is applicable to me    |
      | No - this is not applicable to me |
      | Prefer not to say                 |

