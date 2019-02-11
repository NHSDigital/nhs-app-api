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

  Scenario: A user is informed when existing registration is in conflicted state
    Given I am a EMIS user registered with organ donation but existing registration is in conflicted state
    And I am logged in
    And I navigate to the internal Organ Donation Page
    Then the organ donation decision has been found and is to be processed
    
  Scenario Outline: A <GP System> user registered with organ donation can view their decision of appointed representative
    Given I am a <GP System> user registered with organ donation with an appointed representative
    And I am logged in
    And I navigate to the internal Organ Donation Page
    Then the Organ Donation Confirmation page is displayed
    And the choice of an organ donation appointed representative is displayed
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

    #Depending on timeline, either add an 'Amend' test here, or add 'App Rep' test into 'Amend'
