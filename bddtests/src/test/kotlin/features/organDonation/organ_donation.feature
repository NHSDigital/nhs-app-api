@organ-donation
Feature: Organ Donation

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
    
  Scenario: A user can opt to not donate their organs
    Given I am a EMIS user not registered with organ donation, who wishes to register
    And I am logged in
    When I navigate to the internal Organ Donation Choice Page
    When I choose to not donate my organs
    Then the Organ Donation Decision Additional Details page is displayed
    #Extend as part of 3487

  Scenario: A user can choose to record their ethnicity when opting out of organ donation
    Given I am a EMIS user not registered with organ donation, who wishes to register
    And I am logged in
    And I navigate to the internal Organ Donation Choice Page
    When I choose to not donate my organs
    Then the Organ Donation Decision Additional Details page is displayed
    When I select an ethnicity to record for organ donation
    #As part of 3487, click continue to ensure validation is optional
    #And I click the 'Continue' button
    #Then the Organ Donation Decision Summary page is displayed
    #And my ethnicity is recorded in the organ donation decision summary

  Scenario: A user can choose not to record their ethnicity when opting out of organ donation
    Given I am a EMIS user not registered with organ donation, who wishes to register
    And I am logged in
    And I navigate to the internal Organ Donation Choice Page
    When I choose to not donate my organs
    Then the Organ Donation Decision Additional Details page is displayed
    #As part of 3487, click continue to ensure validation is optional
    #And I click the 'Continue' button
    #Then the Organ Donation Decision Summary page is displayed
    #And my ethnicity is recorded as not chosen in the organ donation decision summary

  Scenario: A user can choose to record their religion when opting out of organ donation
    Given I am a EMIS user not registered with organ donation, who wishes to register
    And I am logged in
    And I navigate to the internal Organ Donation Choice Page
    When I choose to not donate my organs
    Then the Organ Donation Decision Additional Details page is displayed
    When I select a religion to record for organ donation
    #As part of 3487, click continue to ensure validation is optional
    #And I click the 'Continue' button
    #Then the Organ Donation Decision Summary page is displayed
    #And my religion is recorded in the organ donation decision summary

  Scenario: A user can choose not to record their religion when opting out of organ donation
    Given I am a EMIS user not registered with organ donation, who wishes to register
    And I am logged in
    And I navigate to the internal Organ Donation Choice Page
    When I choose to not donate my organs
    Then the Organ Donation Decision Additional Details page is displayed
    #As part of 3487, click continue to ensure validation is optional
    #And I click the 'Continue' button
    #Then the Organ Donation Decision Summary page is displayed
    #And my religion is recorded as not chosen in the organ donation decision summary

  Scenario: A user can navigate back to the Organ Donation Decision page from Additional Details after opting to not
  donate their organs
    Given I am a EMIS user not registered with organ donation, who wishes to register
    And I am logged in
    And I navigate to the internal Organ Donation Choice Page
    When I choose to not donate my organs
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Back' button
    Then the internal Organ Donation page is displayed
  @pending @NHSO-3487
  Scenario: A user can navigate back to the Additional Details page from Check Details page after opting to not donate
  their organs

  @pending @NHSO-3487
  Scenario: A user can view the privacy statement on the organ donation Check Details page

