@organ-donation
Feature: Organ Donation View

  @nativepending @NHSO-2972
  Scenario: A user can navigate to the external version of 'Set organ donation preferences' when toggle is set as so
    Given I am a EMIS user not registered with organ donation, who wishes to register
    And I am logged in
    And I navigate to More
    When I choose to set my organ donation preferences
    Then the external Organ Donation page is displayed

  Scenario: A user can navigate to the native version of 'Set organ donation preferences' when toggle is set as so
    Given I am a EMIS user not registered with organ donation, who wishes to register
    And I am logged in
    And I navigate to more
    And the organ donation toggle is set to target the internal page
    When I choose to set my organ donation preferences
    Then the internal Organ Donation page is displayed

  Scenario Outline: A <GP System> user registered with organ donation can view their existing decision to not donate
  their organs
    Given I am a <GP System> user registered with organ donation to not donate my organs
    And I am logged in
    And I navigate to the internal Organ Donation Page
    Then the Organ Donation View Registration page is displayed
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
    Then the Organ Donation View Registration page is displayed
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
    Then the Organ Donation View Registration page is displayed
    And the existing decision to opt in to organ donation with some organs is displayed
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario: A user registered to donate some organs can see which organs they have not yet decided on
    Given I am a EMIS user registered with organ donation to donate some organs, but not all are decided on
    And I am logged in
    And I navigate to the internal Organ Donation Page
    Then the Organ Donation View Registration page is displayed
    And the existing decision to opt in to organ donation with some organs is displayed

  Scenario: A user is informed when existing registration is in conflicted state
    Given I am a EMIS user registered with organ donation but existing registration is in conflicted state
    And I am logged in
    And I navigate to the internal Organ Donation Page
    Then the organ donation decision has been found and is to be processed

  Scenario Outline: A <GP System> user registered with organ donation can view their decision of appointed representative
    Given I am a <GP System> user registered with organ donation with an appointed representative
    And I am logged in
    And I navigate to the internal Organ Donation Page
    Then the Organ Donation View Registration page is displayed
    And the choice of an organ donation appointed representative is displayed
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario: A user can navigate to the external 'Share my decision' page when viewing their registration
    Given I am a EMIS user registered with organ donation to donate all organs
    And I am logged in
    And I navigate to the internal Organ Donation Page
    Then the Organ Donation View Registration page is displayed
    When I select the 'Share that you are a donor' link on the Organ Donation View Registration page
    Then a new tab opens https://www.organdonation.nhs.uk/share-my-decision

  Scenario: A user can navigate to the external 'Tell your family' page when viewing their registration
    Given I am a EMIS user registered with organ donation to donate all organs
    And I am logged in
    And I navigate to the internal Organ Donation Page
    Then the Organ Donation View Registration page is displayed
    When I select the 'Tell your family' link on the Organ Donation View Registration page
    Then a new tab opens https://www.organdonation.nhs.uk/share-my-decision/how-to-discuss-my-decision/

  Scenario Outline: A user when navigating to Organ Donation, but OD returns a <Error Code> recoverable error when
  retrieving their registration, is shown an error message and can retry
    Given I am a EMIS user registered as opt-in, but on lookup OD returns recoverable <Error Code> error
    And I am logged in
    And I navigate to the internal Organ Donation Page
    And I see an appropriate Organ Donation error message with a retry option
    When I click the 'Try again' button
    And the decision to opt in to organ donation with all organs is displayed
    Examples:
      | Error Code |
      | 504        |

  Scenario Outline: A user when navigating to Organ Donation, but OD returns a <Error Code> non-recoverable error when
  retrieving their registration, is shown an error message and can't retry
    Given I am a EMIS user registered with OD, but on lookup OD returns non-recoverable <Error Code> error
    And I am logged in
    And I navigate to the internal Organ Donation Page
    And I see an appropriate Organ Donation error message without a retry option
    Examples:
      | Error Code |
      | 403        |

  Scenario Outline: A user when navigating to Organ Donation, but OD returns a <Error Code> recoverable error when
  retrieving reference data, is shown an error message and can retry
    Given I am a EMIS user registered with OD, but the ReferenceData call returns recoverable <Error Code> error
    And I am logged in
    And I navigate to the internal Organ Donation Page
    And I see an appropriate Organ Donation error message with a retry option
    When I click the 'Try again' button
    And the decision to opt in to organ donation with all organs is displayed
    Examples:
      | Error Code |
      | 429        |

  Scenario Outline: A user when navigating to Organ Donation, but OD returns a <Error Code> non-recoverable error when
  retrieving reference data, is shown an error message and can't retry
    Given I am a EMIS user registered with OD, but the ReferenceData call returns non-recoverable <Error Code> error
    And I am logged in
    And I navigate to the internal Organ Donation Page
    And I see an appropriate Organ Donation error message without a retry option
    Examples:
      | Error Code |
      | 405        |
