@gp-session-expiry
Feature: GP Session Expiry

  Scenario Outline: The <GP System> GP practice session has expired user visits the prescription type page
    Given I am a patient using the <GP System> GP System
    And I am using the native app user agent
    And I log in to the app expecting to see the notifications prompt
    Then I see the notifications prompt
    When I click the 'No, do not send notifications on this device' radio button
    And I click the 'Continue' button
    Then I see the home page
    And the GP System session has expired when viewing prescriptions
    When I retrieve the 'Your Prescriptions' page directly
    And I click the Order a prescription button
    Then I see appropriate try again error message for prescriptions when there is no GP session
    Examples:
      | GP System |
      | TPP       |
    @bug @NHSO-7780
    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: The <GP System> GP practice session has expired and user visits the prescription repeat courses page
    Given I am a patient using the <GP System> GP System
    And I am using the native app user agent
    And I log in to the app expecting to see the notifications prompt
    Then I see the notifications prompt
    When I click the 'No, do not send notifications on this device' radio button
    And I click the 'Continue' button
    Then I see the home page
    And the GP System session has expired when viewing prescriptions
    When I retrieve the 'prescription repeat courses' page directly
    Then I see appropriate try again error message for prescriptions when there is no GP session
    Examples:
      | GP System |
      | TPP       |
