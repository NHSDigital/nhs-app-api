@accessibility
@organ-donation-accessibility
  Feature: Organ donation accessibility

    Scenario: The organ donation journey pages are captured
      Given I am using the native app user agent
      And I am a EMIS user not registered with organ donation, who wishes to register and donate some organs
      And I am logged in
      And I navigate to the internal Organ Donation Choice Page
      Then the OrganDonationChoice page is saved to disk
      And I choose to donate my organs
      Then the Organ Donation Your Choice page is displayed
      Then the OrganDonationYourChoice page is saved to disk
      When I select the option to donate some of my organs
      And I click the 'Continue' button
      Then the Organ Donation Specific Organ Choice page is displayed
      Then the OrganDonationSpecificOrganChoice page is saved to disk
      When I choose which organs to donate
      And I click the 'Continue' button
      Then the Organ Donation Faith And Beliefs page is displayed
      Then the OrganDonationFaithAndBeliefs page is saved to disk
      When I select an option in sharing my organ donation faith and beliefs
      And I click the 'Continue' button
      Then the Organ Donation Decision Additional Details page is displayed
      Then the OrganDonationAdditionalDetails page is saved to disk
      When I click the 'Continue' button
      Then the Organ Donation Check Details page is displayed
      Then the OrganDonationCheckDetails page is saved to disk
      When I confirm that my details are accurate, and accept the privacy statement for organ donation
      And I click the 'Submit my decision' button
      Then the Organ Donation View Registration page is displayed
      Then the OrganDonationViewRegistration page is saved to disk

    # Reaffirm journey flow
    Scenario: The organ donation check details page captured for reaffirm journey
      Given I am using the native app user agent
      And I am a EMIS user registered as opt-out who wishes to reaffirm their decision
      And I am logged in
      And I navigate to the internal Organ Donation Page
      Then the Organ Donation View Registration page is displayed
      When I choose to reaffirm my organ donation decision
      Then the Organ Donation Check Details page is displayed
      Then the OrganDonationCheckDetails_ReaffirmNoAdditionalDetails page is saved to disk

    # Withdraw journey flow
    Scenario: The organ donation withdraw journey pages captured
      Given I am using the native app user agent
      And I am a EMIS user registered with organ donation with a decision to opt-in who wishes to withdraw
      And I am logged in
      And I navigate to the internal Organ Donation Page
      Then the Organ Donation View Registration page is displayed with my existing decision to opt-in
      Then the OrganDonationViewRegistration_ExistingDecision page is saved to disk
      When I choose to withdraw my organ donation decision
      Then the Organ Donation Withdraw Decision page is displayed
      Then the OrganDonationWithdrawDecision page is saved to disk
      And I select an organ donation withdrawal reason from the list
      When I click the 'Continue' button
      Then the Organ Donation Check Details page is displayed
      Then the OrganDonationCheckDetails_Withdraw page is saved to disk
      And I confirm that my details are accurate, and accept the privacy statement for organ donation
      When I click the 'Submit my decision' button
      Then the Organ Donation View Registration page is displayed with my decision to withdraw
      Then the OrganDonationViewRegistration_Withdraw page is saved to disk