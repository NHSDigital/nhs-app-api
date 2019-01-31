@organ-donation
Feature: Organ Donation

  Scenario Outline: A <GP System> user registered with organ donation can view their existing decision to not donate
  their organs
    Given I am a <GP System> user registered with organ donation to not donate my organs
    And I am logged in
    And I navigate to the internal Organ Donation Page
    Then the Organ Donation Confirmation page is displayed
    And the decision to opt out of organ donation is displayed
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: A <GP System> user registered with organ donation can view their existing decision to donate all
  their organs
    Given I am a <GP System> user registered with organ donation to donate all organs
    And I am logged in
    And I navigate to the internal Organ Donation Page
    Then the Organ Donation Confirmation page is displayed
    And the decision to opt in to organ donation with all organs is displayed
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: A <GP System> user registered with organ donation can view their existing decision to donate some
  of their organs
    Given I am a <GP System> user registered with organ donation to donate some organs
    And I am logged in
    And I navigate to the internal Organ Donation Page
    Then the Organ Donation Confirmation page is displayed
    And the decision to opt in to organ donation with some organs is displayed
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
