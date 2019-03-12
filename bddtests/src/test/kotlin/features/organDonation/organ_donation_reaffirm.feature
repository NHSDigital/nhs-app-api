@organ-donation
Feature: Organ Donation - Reaffirm

  Scenario: A user registered with organ donation can reaffirm their decision to not donate
    Given I am a EMIS user registered as opt-out who wishes to reaffirm their decision
    And I am logged in
    And I navigate to the internal Organ Donation Page
    Then the Organ Donation View Registration page is displayed
    When I choose to reaffirm my organ donation decision
    Then the Organ Donation Check Details page is displayed
    And there is no additional details section displayed on the Organ Donation Check Details page
    #Rest of the flow is amend flow

  Scenario: A user registered with organ donation can reaffirm their decision to donate all their organs
    Given I am a EMIS user registered as opt-in who wishes to reaffirm their decision
    And I am logged in
    And I navigate to the internal Organ Donation Page
    Then the Organ Donation View Registration page is displayed
    When I choose to reaffirm my organ donation decision
    Then the Organ Donation Check Details page is displayed
    And there is no additional details section displayed on the Organ Donation Check Details page
    #Rest of the flow is amend flow

  Scenario: A user registered with organ donation can reaffirm their decision to donate some of their organs
    Given I am a EMIS user registered as opt-in with some organs who wishes to reaffirm their decision
    And I am logged in
    And I navigate to the internal Organ Donation Page
    Then the Organ Donation View Registration page is displayed
    When I choose to reaffirm my organ donation decision
    Then the Organ Donation Your Choice page is displayed
    And the some organs option is selected
    And I click the 'Continue' button
    Then the Organ Donation Specific Organ Choice page is displayed
    And my previous decisions are displayed on the Organ Donation Specific Organ Choice page
    When I choose which organs to donate
    And I click the 'Continue' button
    Then the Organ Donation Faith And Beliefs page is displayed
    And the previous option on the Organ Donation Faith And Beliefs page is selected
    And I click the 'Continue' button
    Then the Organ Donation Decision Additional Details page is displayed
    When I select an ethnicity to record for organ donation
    And  I select a religion to record for organ donation
    And I click the 'Continue' button
    Then the Organ Donation Check Details page is displayed
    And my specific organ donation choices are displayed on the Organ Donation Check Details page
    And the additional details section is displayed on the Organ Donation Check Details page
    #Rest of the flow is amend flow

  Scenario: A user can navigate back through the reaffirm opt out journey
    Given I am a EMIS user registered as opt-out who wishes to reaffirm their decision
    And I am logged in
    And I navigate to the internal Organ Donation Page
    Then the Organ Donation View Registration page is displayed
    When I choose to reaffirm my organ donation decision
    Then the Organ Donation Check Details page is displayed
    When I click the 'Back' button
    Then the Organ Donation View Registration page is displayed

  Scenario: A user can navigate back through the reaffirm opt in journey
    Given I am a EMIS user registered as opt-in who wishes to reaffirm their decision
    And I am logged in
    And I navigate to the internal Organ Donation Page
    Then the Organ Donation View Registration page is displayed
    When I choose to reaffirm my organ donation decision
    Then the Organ Donation Check Details page is displayed
    When I click the 'Back' button
    Then the Organ Donation View Registration page is displayed

  Scenario: A user can navigate back through the reaffirm opt in with some organs journey
    Given I am a EMIS user registered as opt-in with some organs who wishes to reaffirm their decision
    And I am logged in
    And I navigate to the internal Organ Donation Page
    Then the Organ Donation View Registration page is displayed
    When I choose to reaffirm my organ donation decision
    Then the Organ Donation Your Choice page is displayed
    And the some organs option is selected
    And I click the 'Continue' button
    Then the Organ Donation Specific Organ Choice page is displayed
    When I choose which organs to donate
    And I click the 'Continue' button
    Then the Organ Donation Faith And Beliefs page is displayed
    And I click the 'Continue' button
    Then the Organ Donation Decision Additional Details page is displayed
    And I click the 'Continue' button
    Then the Organ Donation Check Details page is displayed
    When I click the 'Back' button
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Back' button
    Then the Organ Donation Faith And Beliefs page is displayed
    When I click the 'Back' button
    Then the Organ Donation Specific Organ Choice page is displayed
    When I click the 'Back' button
    Then the Organ Donation Your Choice page is displayed
    When I click the 'Back' button
    Then the Organ Donation View Registration page is displayed
