@organ-donation
Feature: Organ Donation

  @NHSO-3485
  @nativepending @NHSO-2972
  Scenario: A user can navigate to the external version of 'Set organ donation preferences' when toggle is set as so
    Given I am a EMIS user not registered with organ donation, who wishes to register
    And I am logged in
    And I navigate to More
    When I choose to set my organ donation preferences
    Then the external Organ Donation page is displayed

  @NHSO-3485
  @nativepending @NHSO-2972
  Scenario: A user can navigate to the native version of 'Set organ donation preferences' when toggle is set as so
    Given I am a EMIS user not registered with organ donation, who wishes to register
    And I am logged in
    And I navigate to more
    And the organ donation toggle is set to target the internal page
    When I choose to set my organ donation preferences
    Then the internal Organ Donation page is displayed


