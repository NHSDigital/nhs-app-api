@biometrics
Feature: Biometric status update

  Scenario Outline: A patient can navigate to the <Biometric Type> page on their native device and register their face id
    Given I am a EMIS patient using the native app
    And I am logged in
    When I retrieve the 'more' page directly
    Then the More page for mobile devices is displayed
    And the <Biometric Type> more link is displayed
    And I click the <Biometric Type> link on the more page
    When I click the Login with <Biometric Type> toggle
    Then I see my <Biometric Type> registration was successful
    Examples:
      | Biometric Type|
      | Face ID       |
      | Touch ID      |
      | fingerprint   |

  Scenario Outline: A patient can navigate to the <Biometric Type> page on their native device and deregister their <Biometric Type>
    Given I am a EMIS patient using the native app
    And I am logged in
    And I have already registered for biometrics
    When I retrieve the 'more' page directly
    Then the More page for mobile devices is displayed
    And the <Biometric Type> more link is displayed
    And I click the <Biometric Type> link on the more page
    When I click the Login with <Biometric Type> toggle
    Then I see my <Biometric Type> deregistration was successful
    Examples:
      | Biometric Type|
      | Face ID       |
      | Touch ID      |
      | fingerprint   |

  Scenario Outline: A patient that gets an error due to an inability to find their <Biometric Type> state when changing their <Biometric Type> state
    Given I am a EMIS patient using the native app
    And I am logged in
    And I have already registered for biometrics
    When I retrieve the 'more' page directly
    Then the More page for mobile devices is displayed
    And the <Biometric Type> more link is displayed
    And I click the <Biometric Type> link on the more page
    When I click the Login with <Biometric Type> toggle
    Then I see my <Biometric Type> deregistration was unsuccessful as it could not be found
    Examples:
      | Biometric Type|
      | Face ID       |
      | Touch ID      |
      | fingerprint   |

  Scenario Outline: A patient that gets an error due to an inability to change their <Biometric Type> state
    Given I am a EMIS patient using the native app
    And I am logged in
    And I have already registered for biometrics
    When I retrieve the 'more' page directly
    Then the More page for mobile devices is displayed
    And the <Biometric Type> more link is displayed
    And I click the <Biometric Type> link on the more page
    When I click the Login with <Biometric Type> toggle
    Then I see my <Biometric Type> deregistration was unsuccessful as it could not be changed
    Examples:
      | Biometric Type|
      | Face ID       |
      | Touch ID      |
      | fingerprint   |


