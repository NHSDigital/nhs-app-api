@gp_session_on_demand
Feature: GP Session On Demand medical record

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

  @pending
  Scenario Outline: A <GP System> user trying to view medical record but whose gp system is unavailable has the option to retry and can view medical record when the gp system becomes available
    Given I have a Decoupled GP User Session
    And I am an <GP System> patient whose GP system is unavailable
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    Then I SSO to NhsLogin
    And I see the generic try again error message
    And The <GP System> GP system becomes available
    And I am a <GP System> user setup to use medical record version 2
    And the GP Practice has enabled all medical records for the patient
    When I click the 'Try again' button
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
    Given I have a Decoupled GP User Session
    And I am an <GP System> patient whose GP system is unavailable
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    Then I SSO to NhsLogin
    And I see the generic try again error message
    When I click the 'Try again' button
    Then I see the generic medical record error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
