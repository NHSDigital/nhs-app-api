@organ-donation
Feature: Organ Donation View Frontend

#  This test covers navigation via buttons/links

  Scenario: A user can navigate to the native version of 'Manage organ donation preferences' when toggle is set as so
    Given I am using the native app user agent
    And I am a EMIS user not registered with organ donation, who wishes to register
    And I am logged in
    And I retrieve the 'more' page directly
    When I choose to set my organ donation preferences
    Then the internal Organ Donation page is displayed

# These tests navigate directly to the pages where the features are to be tested, to save time.

  Scenario Outline: A <GP System> user registered with organ donation can view their existing decision to not donate their organs
    Given I am using the native app user agent
    And I am a <GP System> user registered with organ donation to not donate my organs
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    Then the Organ Donation View Registration page is displayed
    And the decision to opt out of organ donation is displayed
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: A <GP System> user registered with organ donation can view their existing decision to donate all their organs
    Given I am using the native app user agent
    And I am a <GP System> user registered with organ donation to donate all organs
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    Then the Organ Donation View Registration page is displayed
    And the decision to opt in to organ donation with all organs is displayed
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: A <GP System> user registered with organ donation can view their existing decision to donate some of their organs
    Given I am using the native app user agent
    And I am a <GP System> user registered with organ donation to donate some organs
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    Then the Organ Donation View Registration page is displayed
    And the existing decision to opt in to organ donation with some organs is displayed
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario: A user registered to donate some organs can see which organs they have not yet decided on
    Given I am using the native app user agent
    And I am a EMIS user registered with organ donation to donate some organs, but not all are decided on
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    Then the Organ Donation View Registration page is displayed
    And the existing decision to opt in to organ donation with some organs is displayed

  Scenario: A user is informed when existing registration is in conflicted state
    Given I am using the native app user agent
    And I am a EMIS user registered with organ donation but existing registration is in conflicted state
    And I am logged in
    And I navigate to the internal Organ Donation Page
    Then the organ donation decision has been found and is to be processed

  Scenario Outline: A <GP System> user registered with organ donation can view their decision of appointed representative
    Given I am using the native app user agent
    And I am a <GP System> user registered with organ donation with an appointed representative
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    Then the Organ Donation View Registration page is displayed
    And the choice of an organ donation appointed representative is displayed
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: A user registered with organ donation can view their existing faith decision of <Option>
    Given I am using the native app user agent
    And I am a EMIS user registered with organ donation to donate all organs with a faith decision of '<Option>'
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    Then the Organ Donation View Registration page is displayed
    And the decision to opt in to organ donation with all organs is displayed
    And the faith and beliefs decision of '<Option>' is displayed on the Organ Donation View Registration page
    Examples:
      | Option                            |
      | Yes - this is applicable to me    |
      | No - this is not applicable to me |
      | Prefer not to say                 |

  Scenario: A user can navigate to the external 'Share my decision' page when viewing their registration
    Given I am using the native app user agent
    And I am a EMIS user registered with organ donation to donate all organs
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    Then the Organ Donation View Registration page is displayed
    When I click the link called 'Share that you are a donor' with a url of 'https://www.organdonation.nhs.uk/app/app-share/'
    Then a new tab has been opened by the link

  Scenario: A user can navigate to the external 'Tell your family and friends' page when viewing their registration
    Given I am using the native app user agent
    And I am a EMIS user registered with organ donation to donate all organs
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    Then the Organ Donation View Registration page is displayed
    When I click the link called 'Tell your family and friends' with a url of 'https://www.organdonation.nhs.uk/app/app-tell/'
    Then a new tab has been opened by the link

  Scenario Outline: A user when navigating to Organ Donation, but OD returns a <Error Code> recoverable error when retrieving their registration, is shown an error message and can retry
    Given I am using the native app user agent
    And I am a EMIS user registered as opt-in, but on lookup OD returns recoverable <Error Code> error
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    And I see an appropriate Organ Donation error message with a retry option
    And I click the 'Try again' button
    And the decision to opt in to organ donation with all organs is displayed
    Examples:
      | Error Code |
      | 504        |

  Scenario Outline: A user when navigating to Organ Donation, but OD returns a <Error Code> non-recoverable error when retrieving their registration, is shown an error message and can't retry
    Given I am using the native app user agent
    And I am a EMIS user registered with OD, but on lookup OD returns non-recoverable <Error Code> error
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    And I see an appropriate Organ Donation error message without a retry option
    Examples:
      | Error Code |
      | 403        |

  Scenario Outline: A user when navigating to Organ Donation, but OD returns a <Error Code> recoverable error when retrieving reference data, is shown an error message and can retry
    Given I am using the native app user agent
    And I am a EMIS user registered with OD, but the ReferenceData call returns recoverable <Error Code> error
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    And I see an appropriate Organ Donation error message with a retry option
    And I click the 'Try again' button on an Organ Donation page
    And the decision to opt in to organ donation with all organs is displayed
    Examples:
      | Error Code |
      | 429        |

  Scenario Outline: A user when navigating to Organ Donation, but OD returns a <Error Code> non-recoverable error when retrieving reference data, is shown an error message and can't retry
    Given I am using the native app user agent
    And I am a EMIS user registered with OD, but the ReferenceData call returns non-recoverable <Error Code> error
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    And I see an appropriate Organ Donation error message without a retry option
    Examples:
      | Error Code |
      | 405        |
