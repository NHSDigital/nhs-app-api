@gp_session_on_demand
Feature: GP Session On Demand organ donation

  Scenario Outline: A user can select '<Option>' for faith and belief sharing when opting to donate all their organs after a GpSession On Demand login
    Given I am using the native app user agent
    And I am a EMIS user not registered with organ donation, who wishes to opt in with '<Option>' faith decision
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    And I choose to donate my organs
    And the Organ Donation Your Choice page is displayed
    And I select the option to donate all my organs
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

  Scenario Outline: A <GP System> user trying to view organ donation but whose gp system is unavailable has the option to retry and an advice screen when the gp session does not recover
    And I am using the native app user agent
    And Reference data is available for <GP System>
    And I am an <GP System> patient whose GP system is unavailable
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    And I see appropriate try again error message for organ donation when there is no GP session
    And I click the 'Try again' button
    Then I see what I can do next with an organ donation error message
    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: A <GP System> session is lost and regained
    And I am using the native app user agent
    And I am an <GP System> patient whose GP system is unavailable
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    And I see appropriate try again error message for organ donation when there is no GP session
    And I click the 'Try again' button
    Then I see what I can do next with an organ donation error message
    And The <GP System> GP system becomes available
    And I am a <GP System> user registered with organ donation to donate all organs
    When I retrieve the 'health record hub' page directly
    Then I am redirected to the 'health record hub' page
    When I choose to set my organ donation preferences
    Then the Organ Donation View Registration page is displayed with my existing decision to opt-in
    And I am an <GP System> patient whose GP system is unavailable
    When I retrieve the 'health record hub' page directly
    Then I am redirected to the 'health record hub' page
    When I choose to set my organ donation preferences

    Examples:
      | GP System |
      | EMIS      |

