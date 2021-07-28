@home
Feature: Home

  Scenario Outline: A patient can see the <Biometric Text> button on their native device and navigate to the <Biometric Type> page
    Given I am a EMIS patient using the native app
    And I am logged in
    And the <Biometric Text> button is displayed
    When I click the <Biometric Text> biometrics button
    Then I see the account and settings <Biometric Type> biometric page
    And The biometrics page url ends with <Biometric Url>
    Examples:
      | Biometric Type  | Biometric Text       | Biometric Url                           |
      | Face ID         | Set up Face ID       | more/account-and-settings/face-id       |
      | Touch ID        | Set up Touch ID      | more/account-and-settings/touch-id      |
      | Fingerprint     | Set up fingerprint   | more/account-and-settings/fingerprint   |
      | Login options   | Open login settings  | more/account-and-settings/login-options |
