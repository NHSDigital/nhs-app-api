@accessibility
@organ-donation-accessibility
Feature: Organ donation accessibility

  Scenario: The organ donation journey pages are captured
    Given I am using the native app user agent
    And I am a EMIS user not registered with organ donation, who wishes to register and donate some organs
    And I am logged in
    When I navigate to the health record hub page
    And I navigate to the internal Organ Donation Choice Page
    Then the OrganDonation_RegisterNewOrganDonationDecision page is saved to disk
    And I choose to donate my organs
    And the Organ Donation Your Choice page is displayed
    And the OrganDonation_AllOrSomeOrgans_YourChoice page is saved to disk
    When I select the option to donate some of my organs
    And I click the 'Continue' button
    Then the Organ Donation Specific Organ Choice page is displayed
    And the OrganDonation_ChooseOrgans page is saved to disk
    When I choose which organs to donate
    And I click the 'Continue' button
    Then the Organ Donation Faith And Beliefs page is displayed
    And the OrganDonation_FaithAndBeliefs page is saved to disk
    When I select an option in sharing my organ donation faith and beliefs
    And I click the 'Continue' button
    Then the Organ Donation Decision Additional Details page is displayed
    And the OrganDonation_AdditionalDetails page is saved to disk
    When I click the 'Continue' button
    Then the Organ Donation Check Details page is displayed
    And the OrganDonation_CheckDetailsAndSubmit page is saved to disk
    When I confirm that my details are accurate, and accept the privacy statement for organ donation
    And I click the 'Submit my decision' button
    Then the Organ Donation View Registration page is displayed
    And the OrganDonation_AllOrgansConfirmation page is saved to disk

  Scenario: The organ donation check details page captured for reaffirm journey
    Given I am using the native app user agent
    And I am a EMIS user registered as opt-out who wishes to reaffirm their decision
    And I am logged in
    When I navigate to the health record hub page
    And I navigate to the internal Organ Donation Page
    Then the Organ Donation View Registration page is displayed
    When I choose to reaffirm my organ donation decision
    Then the Organ Donation Check Details page is displayed
    And the OrganDonation_CheckDetails_ReaffirmNoAdditionalDetails page is saved to disk

  Scenario: The organ donation withdraw journey pages captured
    Given I am using the native app user agent
    And I am a EMIS user registered with organ donation with a decision to opt-in who wishes to withdraw
    And I am logged in
    When I navigate to the health record hub page
    And I navigate to the internal Organ Donation Page
    Then the Organ Donation View Registration page is displayed with my existing decision to opt-in
    And the OrganDonation_ViewRegisteredDecision page is saved to disk
    When I choose to withdraw my organ donation decision
    Then the Organ Donation Withdraw Decision page is displayed
    And the OrganDonation_WithdrawYourDecisionGiveReason page is saved to disk
    And I select an organ donation withdrawal reason from the list
    When I click the 'Continue' button
    Then the Organ Donation Check Details page is displayed
    And the OrganDonation_CheckDetails_ChooseToWithdraw page is saved to disk
    And I confirm that my details are accurate, and accept the privacy statement for organ donation
    When I click the 'Submit my decision' button
    Then the Organ Donation View Registration page is displayed with my decision to withdraw
    And the OrganDonation_WithdrawDecisionConfirmation page is saved to disk

  Scenario: The 'Organ Donation Register API timeout' page is captured
    Given I am using the native app user agent
    And I am a EMIS user who wishes to register as opt out, but OD takes too long to respond
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    And I follow the opt-out journey to the 'Check Details' page
    And I confirm that my details are accurate, and accept the privacy statement for organ donation
    And I click the 'Submit my decision' button on an Organ Donation page
    And I wait for 15 seconds
    And I see an appropriate Organ Donation decision processing message without a retry option
    And the OrganDonation_RegisterAPITimeout page is saved to disk

  Scenario: The 'Organ donation - generic error' page is captured
    Given I am using the native app user agent
    And I am a EMIS user who wishes to register as opt out, but OD returns non-recoverable 400 error
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    And I follow the opt-out journey to the 'Check Details' page
    And I confirm that my details are accurate, and accept the privacy statement for organ donation
    And I click the 'Submit my decision' button on an Organ Donation page
    And I see an appropriate Organ Donation error message without a retry option
    And the OrganDonation_GenericError page is saved to disk

  Scenario: The 'Organ decision received (but not yet processed)' page is captured
    Given I am using the native app user agent
    And I am a EMIS user registered as opt-in with organ donation, who wishes to opt-out but will cause a conflict
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    Then the Organ Donation View Registration page is displayed with my existing decision to opt-in
    When I choose to amend my Organ Donation decision
    Then the amend Organ Donation Choice Page is displayed
    When I follow the opt-out journey to the 'Confirmation' page and see 'Decision Submitted'
    Then the organ donation decision has been submitted and is to be processed
    And the OrganDonation_OrganDecisionReceived page is saved to disk

  Scenario: The 'About organs and tissue' page is captured
    Given I am using the native app user agent
    And I am a EMIS user not registered with organ donation, who wishes to register and donate some organs
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    And I choose to donate my organs
    Then the Organ Donation Your Choice page is displayed
    When I select the option to donate some of my organs
    And I click the 'Continue' button on an Organ Donation page
    Then the Organ Donation Specific Organ Choice page is displayed
    When I click the Find out more about organs and tissue link
    Then the Find Out More About Organs And Tissue Page is displayed
    And the OrganDonation_AboutOrgansAndTissue page is saved to disk
