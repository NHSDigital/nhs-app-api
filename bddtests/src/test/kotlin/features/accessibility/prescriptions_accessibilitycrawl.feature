@accessibility
@prescription-accessibility
  Feature: Prescription accessibility

    Scenario: The prescriptions journey pages are captured
      Given the scenario is submit prescription
      And I am using TPP GP System to submit my prescription
      And I have 1 historic prescriptions in this scenario
      And I am logged in
      When I retrieve the 'Your Prescriptions' page directly
      Then the MyPrescriptions_TPP page is saved to disk
      And I select 5 repeatable prescriptions to order
      Then the OrderPrescriptions page is saved to disk
      And I click Continue on the Order a repeat prescription page
      Then the OrderPrescriptionsConfirm page is saved to disk
      When I click Confirm and order repeat prescription
      Then the PrescriptionsConfirmation page is saved to disk

    Scenario: The EMIS my prescriptions page is captured
      Given the scenario is submit prescription
      And I am using EMIS GP System to submit my prescription
      And I have 1 historic prescriptions in this scenario
      And I am logged in
      When I retrieve the 'Your Prescriptions' page directly
      Then the MyPrescriptions_EMIS page is saved to disk
