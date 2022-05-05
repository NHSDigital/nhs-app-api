@gp_session_on_demand
Feature: GP Session On Demand medical record

  @pending
  Scenario Outline: A <GP System> user trying to view medical record but whose gp system is unavailable has the option to retry and can view medical record when the gp system becomes available
    Given I am an <GP System> patient whose GP system is unavailable
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I see appropriate try again shutter screen for gp medical record when there is no GP session
    And The <GP System> GP system becomes available
    And I am a <GP System> user setup to use medical record version 2
    And the GP Practice has enabled all medical records for the patient
    And I click the 'Try again' button
    # todo: next step doesn't work because demographics call succeeds (gp session created automatically as
    # part of this request) then instantly we request user's record. The configuration call hasn't
    # had a chance to finish yet after the successful session creation, so the correct patient id is not sent up
    # so we get a 467.
    Then I see the medical record v2 page
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario Outline: A <GP System> user trying to view medical record but whose gp system is unavailable has the option to retry and an error screen when the gp session does not recover
    Given I am an <GP System> patient whose GP system is unavailable
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I see appropriate try again shutter screen for gp medical record when there is no GP session
    And I click the 'Try again' button
    Then I see what I can do next with a medical record error message and reference code '<Prefix>'
    Examples:
      | GP System | Prefix |
      | EMIS      | 3e     |
      | TPP       | 3t     |

  Scenario: The shutter page is shown with care plans menu item for pkb
    Given I am a patient with pkb care plans and the GP System is unavailable
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I see appropriate try again shutter screen for gp medical record when there is no GP session
    And I click the 'Try again' button
    And I see the Third Party menu item 'Care plans'

  Scenario: The shutter page is shown with health tracker menu item for pkb
    Given I am a patient with pkb health tracker and the GP System is unavailable
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I see appropriate try again shutter screen for gp medical record when there is no GP session
    And I click the 'Try again' button
    And I see the Third Party menu item 'Track your health'

  Scenario Outline: The shutter page is shown with no <Menu item text> option when there is no GP session
    Given I am a patient who does not have <Silver integration> and the GP System is unavailable
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I see appropriate try again shutter screen for gp medical record when there is no GP session
    And I click the 'Try again' button
    And I do not see the Third Party menu item '<Menu item text>'
    Examples:
      | Silver integration | Menu item text    |
      | care plans         | Care plans        |
      | health tracker     | Track your health |
