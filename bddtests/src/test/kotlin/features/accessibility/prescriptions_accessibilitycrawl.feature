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
      And the OrderPrescriptions page is saved to disk
      And I click Continue on the Order a repeat prescription page
      And the OrderPrescriptionsConfirm page is saved to disk
      When I click Confirm and order repeat prescription
      Then I see the Prescription Ordered success page
      And the PrescriptionsConfirmation page is saved to disk

    Scenario: The EMIS my prescriptions page is captured
      Given the scenario is submit prescription
      And I am using EMIS GP System to submit my prescription
      And I have 1 historic prescriptions in this scenario
      And I am logged in
      When I retrieve the 'Your Prescriptions' page directly
      Then the MyPrescriptions_EMIS page is saved to disk

    #Scenario: The Microtest prescriptions partial order success page is captured
    Scenario: The Microtest prescriptions partial order success page is captured
      Given the scenario is submit prescription
      And I am using MICROTEST GP System to submit my prescription
      And I have 1 historic prescriptions in this scenario
      And I am logged in
      When I retrieve the 'Your Prescriptions' page directly
      And I select 1 repeatable prescriptions to order
      And I click Continue on the Order a repeat prescription page
      But the GP system responds with an error indicating the order was partially successful
      Then the MicrotestParialSuccess page is saved to disk

