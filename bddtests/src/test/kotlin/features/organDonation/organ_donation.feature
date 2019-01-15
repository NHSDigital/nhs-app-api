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

  Scenario Outline: A <GP System> user can opt to donate all their organs
    Given I am a <GP System> user not registered with organ donation, who wishes to register
    And I am logged in
    And I navigate to the internal Organ Donation Choice Page
    And I choose to donate my organs
    Then the Organ Donation Your Choice page is displayed
    When I select the option to donate all my organs
    #Check can submit in future task
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario: A user can navigate back to the internal Organ Donation Choice Page page from the All or Some Organs page
    Given I am a EMIS user not registered with organ donation, who wishes to register
    And I am logged in
    And I navigate to the internal Organ Donation Choice Page
    And I choose to donate my organs
    Then the Organ Donation Your Choice page is displayed
    When I click the 'Back' button
    Then the internal Organ Donation Choice Page is displayed

  Scenario Outline: A <GP System> user can opt to not donate their organs
    Given I am a <GP System> user not registered with organ donation, who wishes to register
    And I am logged in
    When I navigate to the internal Organ Donation Choice Page
    When I choose to not donate my organs
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Continue' button
    Then the Organ Donation Check Details page is displayed
    And the choice of not wishing to donate organs is displayed on the Organ Donation Check Details page
    #Check can submit in future task
  Examples:
  | GP System |
  | EMIS      |
  | TPP       |
  | VISION    |

  Scenario: A user can choose to record their ethnicity when opting out of organ donation
    Given I am a EMIS user not registered with organ donation, who wishes to register
    And I am logged in
    And I navigate to the internal Organ Donation Choice Page
    When I choose to not donate my organs
    Then the Organ Donation Decision Additional Details page is displayed
    When I select an ethnicity to record for organ donation
    And I click the 'Continue' button
    Then the Organ Donation Check Details page is displayed
    And the choice of not wishing to donate organs is displayed on the Organ Donation Check Details page
    And my ethnicity is recorded on the Organ Donation Check Details page
    #Check can submit in future task

  Scenario: A user can choose not to record their ethnicity when opting out of organ donation
    Given I am a EMIS user not registered with organ donation, who wishes to register
    And I am logged in
    And I navigate to the internal Organ Donation Choice Page
    When I choose to not donate my organs
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Continue' button
    Then the Organ Donation Check Details page is displayed
    And the choice of not wishing to donate organs is displayed on the Organ Donation Check Details page
    And my ethnicity is recorded as not chosen on the Organ Donation Check Details page
    #Check can submit in future task

  Scenario: A user can choose to record their religion when opting out of organ donation
    Given I am a EMIS user not registered with organ donation, who wishes to register
    And I am logged in
    And I navigate to the internal Organ Donation Choice Page
    When I choose to not donate my organs
    Then the Organ Donation Decision Additional Details page is displayed
    When I select a religion to record for organ donation
    And I click the 'Continue' button
    Then the Organ Donation Check Details page is displayed
    And my religion is recorded on the Organ Donation Check Details page
    #Check can submit in future task

  Scenario: A user can choose not to record their religion when opting out of organ donation
    Given I am a EMIS user not registered with organ donation, who wishes to register
    And I am logged in
    And I navigate to the internal Organ Donation Choice Page
    When I choose to not donate my organs
    Then the Organ Donation Decision Additional Details page is displayed
    And I click the 'Continue' button
    Then the Organ Donation Check Details page is displayed
    And my religion is recorded as not chosen on the Organ Donation Check Details page
    #Check can submit in future task

  Scenario: A user can navigate back to the Organ Donation Decision page from Additional Details after opting to not
  donate their organs
    Given I am a EMIS user not registered with organ donation, who wishes to register
    And I am logged in
    And I navigate to the internal Organ Donation Choice Page
    When I choose to not donate my organs
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Back' button
    Then the internal Organ Donation page is displayed

  Scenario: A user can navigate back to the Additional Details page from Check Details page after opting to not donate
  their organs
    Given I am a EMIS user not registered with organ donation, who wishes to register
    And I am logged in
    And I navigate to the internal Organ Donation Choice Page
    When I choose to not donate my organs
    Then the Organ Donation Decision Additional Details page is displayed
    And I click the 'Continue' button
    Then the Organ Donation Check Details page is displayed
    When I click the 'Back' button
    Then the Organ Donation Decision Additional Details page is displayed

  Scenario: A user can view the privacy statement on the organ donation Check Details page
    Given I am a EMIS user not registered with organ donation, who wishes to register
    And I am logged in
    And I navigate to the internal Organ Donation Choice Page
    When I choose to not donate my organs
    Then the Organ Donation Decision Additional Details page is displayed
    And I click the 'Continue' button
    Then the Organ Donation Check Details page is displayed
    When I select the privacy statement link on the Organ Donation Check Details page
    Then a new tab opens https://www.nhsbt.nhs.uk/privacy/

