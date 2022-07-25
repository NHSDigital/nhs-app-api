@accessibility
@repeat-prescriptions-and-your-orders-accessibility
Feature: Repeat prescriptions and Your orders accessibility

  Scenario: The 'Service not offered by GP or to specific user' page is captured
    Given I am a patient using the TPP GP System
    And prescriptions is disabled at a GP Practice level
    And I am logged in
    When I retrieve the 'Your Prescriptions' page directly
    And I click 'Order a prescription'
    Then I see a message informing me that I don't currently have access to this service
    And the Prescriptions_RepeatNotAvailable page is saved to disk

  Scenario: The repeat prescriptions journey pages are captured
    Given the scenario is submit prescription
    And I am using TPP GP System to submit my prescription
    And I have 1 historic prescriptions in this scenario
    And I am logged in
    When I retrieve the 'Your Prescriptions' page directly
    And I select 5 repeatable prescriptions to order
    And the Prescriptions_SelectMedication page is saved to disk
    And I click Continue on the Order a repeat prescription page
    And the Prescriptions_ConfirmBeforeAdding page is saved to disk
    And I click Confirm and order repeat prescription
    Then I see the Order Success page
    And the Prescriptions_OrderConfirmation_General page is saved to disk

  Scenario: The non repeat prescriptions journey pages are captured
    Given the scenario is submit prescription
    And I am using TPP GP System to submit my prescription
    And I have 1 historic prescriptions in this scenario
    And I am logged in
    When I retrieve the 'Your Prescriptions' page directly
    Then the Prescriptions Hub page is displayed
    And the Prescriptions_Hub page is saved to disk
    When I click the Order a prescription button
    Then the Type of Prescriptions page is displayed
    And the Type_Of_Prescriptions page is saved to disk
    When I select the option to order a non repeat prescription
    Then the Contact Your GP page is displayed
    And the Contact_Your_GP page is saved to disk

  Scenario: The 'User with High street pharmacy variation has ordered a prescription' page is captured
    Given the scenario is submit prescription
    And I am using EMIS GP System to submit my prescription
    And I have 1 historic prescriptions in this scenario
    And my GP Practice is EPS enabled
    And I have a P1 typed nominated pharmacy with SW11XR OdsCode
    And I am logged in
    When I retrieve the 'Your Prescriptions' page directly
    And I select 1 repeatable prescriptions to order
    And I click Continue on the Order a repeat prescription page
    Then I see nominated pharmacy information is shown and correct
    When I click Confirm and order repeat prescription
    Then I see the Order Success page with a playback of my order and what happens next for nominated pharmacy
    And the Prescriptions_OrderConfirmation_WithHighstreetPharmacy page is saved to disk

  Scenario: The 'User with On-line Only  pharmacy variation has ordered a prescription' page is captured
    Given the scenario is submit prescription
    And I am using EMIS GP System to submit my prescription
    And I have 1 historic prescriptions in this scenario
    And my GP Practice is EPS enabled
    And I have a P1 typed Internet pharmacy with SW11XR OdsCode
    And I am logged in
    When I retrieve the 'Your Prescriptions' page directly
    And I select 1 repeatable prescriptions to order
    And I click Continue on the Order a repeat prescription page
    And I click Confirm and order repeat prescription
    Then I see the Order Success page with a playback of my order and what happens next for Internet pharmacy
    And the Prescriptions_OrderConfirmation_OnlineOnlyPharmacy page is saved to disk

  Scenario: The 'ordered a prescription for another person in proxy mode' page is captured
    Given I am logged in as a TPP user with linked profiles and appointments provider IM1
    And the scenario is submit prescription
    Then I see the home page
    When I can see and follow the Linked profiles link
    Then the linked profiles page is displayed
    And linked profiles are displayed
    When I select a linked profile
    Then details for the selected linked profile are displayed
    When I click the Switch to this profile button for the proxy user
    Then I see the proxy home page
    And I am using TPP GP System to submit my prescription
    And I have 1 historic prescriptions in this scenario
    And there are 5 repeatable prescriptions available
    And I navigate to prescriptions
    And the Prescriptions Hub page is displayed
    And I select 1 repeatable prescriptions to order
    And I click Continue on the Order a repeat prescription page
    When I click Confirm and order repeat prescription
    Then I see the Order Success page with what happens next for proxy
    And the Prescriptions_OrderConfirmationInProxyMode page is saved to disk

  Scenario: The 'Select medication - none available' page is captured
    Given I am a EMIS patient
    And I have historic prescriptions
    And I have 0 assigned prescriptions
    And 0 of my prescriptions are of type repeat
    And 0 of my prescriptions can be requested
    And I am logged in
    And I navigate to prescriptions
    When I click 'Order a prescription'
    Then the Type of Prescriptions page is displayed
    When I select the option to order a repeat prescription
    Then the 'No repeat prescriptions available to order' Header is displayed
    And the PrescriptionsHub_SelectMedicationNoneAvailable page is saved to disk

  Scenario: The 'Error submitting request - prescription order' page is captured
    Given I am a patient using the TPP GP System
    And I am logged in
    And I have 10 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And I have 10 assigned prescriptions
    And 10 of my prescriptions are of type repeat
    And 10 of my prescriptions can be requested
    Then I retrieve the 'Your Prescriptions' page directly
    And The prescription submission endpoint is throwing an already ordered exception
    When I retrieve the 'Prescription Repeat Courses' page directly
    And I select 1 prescription to order
    Then I see the appropriate error message for a course request error
    And the Prescriptions_ErrorSubmittingOrder page is saved to disk

  Scenario: The 'Medication issued within last 30 days' page is captured
    Given the scenario is submit prescription
    And I am using EMIS GP System to submit my prescription
    And I have 1 historic prescriptions in this scenario
    And I am logged in
    When I retrieve the 'Your Prescriptions' page directly
    And I select 1 repeatable prescriptions to order
    And I click Continue on the Order a repeat prescription page
    But EMIS responds with an error indicating an included course has already been ordered in the last 30 days when submitting the repeat prescription
    When I click Confirm and order repeat prescription
    Then I see a message indicating I've previously ordered one of the selected medications within the last 30 days
    And the Prescriptions_MedicationIssuedWithinLast30Days page is saved to disk

  Scenario: The 'Repeat prescriptions proxy shutter' page is captured
    Given I am logged in as a EMIS user with linked profiles and appointments provider IM1
    Then I see the home page
    When I can see and follow the Linked profiles link
    Then the linked profiles page is displayed
    And linked profiles are displayed
    And I select a linked profile with appointments enabled false, prescriptions enabled false and medical record enabled false
    And details for the selected linked profile are displayed
    When I click the Switch to this profile button for the proxy user
    Then I see the proxy home page
    When I navigate to Prescriptions
    And I click the View Orders link
    Then the prescriptions shutter page is displayed
    And the Prescriptions_ProxyShutter page is saved to disk

  Scenario: The 'Prescriptions - prove your identity shutter' page for a 'P5 user' is captured
    Given I am a patient with proof level 5
    And I am logged in
    When I retrieve the 'your prescriptions' page directly
    Then the page title is 'Prescriptions'
    And I am asked to prove my identity to access 'your prescriptions'
    And the Prescriptions_ProveYourIdentityShutter page is saved to disk

  Scenario: The 'Repeat prescriptions GP session error - temporary problem' page is captured
    Given I am an EMIS patient whose GP system is unavailable
    And I am logged in
    When I retrieve the 'Your Prescriptions' page directly
    And The EMIS GP system is still unavailable
    And I click the View Orders link
    Then I see appropriate try again shutter screen for prescriptions when there is no GP session
    And the Prescriptions_GPSessionError_TemporaryProblem page is saved to disk

  Scenario: The 'Repeat prescriptions GP session error - other things you can do' page is captured
    Given I am an EMIS patient whose GP system is unavailable
    And I am logged in
    When I retrieve the 'Your Prescriptions' page directly
    And The EMIS GP system is still unavailable
    And I click the View Orders link
    Then I see appropriate try again shutter screen for prescriptions when there is no GP session
    When I click the 'Try again' button
    Then I see what I can do next with a repeat prescriptions error message and reference code '3p'
    And the Prescriptions_GPSessionError_OtherThingsYouCanDo page is saved to disk

  Scenario: The 'Prescription hub for users with No-Nominated Pharmacy' page is captured
    Given the scenario is submit prescription
    And I am using TPP GP System to submit my prescription
    And I have 1 historic prescriptions in this scenario
    And I am logged in
    When I retrieve the 'Your Prescriptions' page directly
    Then the PrescriptionsHub_NoNominatedPharmacy page is saved to disk

  Scenario: The 'Your orders with NO-Nominated pharmacy' page is captured
    Given I am a patient using the TPP GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And my GP Practice is EPS enabled
    And I don't have a nominated pharmacy of any type
    And I am logged in
    When I navigate to prescriptions
    And the Prescriptions Hub page is displayed
    And I click the View Orders link
    Then the Orders_NoNominatedPharmacy_ page is saved to disk

  Scenario: The 'Your orders with Nominated pharmacy' page is captured
    Given I am a patient using the TPP GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And my GP Practice is EPS enabled
    And I have a P1 typed nominated pharmacy with SW11XR OdsCode
    And I am logged in
    When I navigate to prescriptions
    And the Prescriptions Hub page is displayed
    And I click the View Orders link
    Then the Orders_NominatedPharmacy page is saved to disk

  Scenario: The 'User has no orders ' page is captured
    Given I am a patient using the EMIS GP System
    And I have 0 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And my GP Practice is EPS enabled
    And I have a P1 typed nominated pharmacy with SW11XR OdsCode
    And I am logged in
    When I navigate to prescriptions
    And the Prescriptions Hub page is displayed
    And I click the View Orders link
    Then I see no prescriptions
    And the NoOrders_WithNominatedPharmacy page is saved to disk

  Scenario: The 'Error getting prescription history and medication list (SC26)' page is captured
    Given I am a patient using the TPP GP System
    And I am logged in
    And The prescriptions endpoint is throwing a server error
    When I retrieve the 'Your Prescriptions' page directly
    And I click the View Orders link
    Then I see the appropriate error message for a prescription server error
    And the SC26_CannotShowPrescriptionInformation page is saved to disk

  Scenario: The 'Timeout error getting prescription history and medication list (SC25)' page is captured
    Given I am a patient using the TPP GP System
    And I am logged in
    And The prescriptions endpoint is timing out
    When I retrieve the 'Your Prescriptions' page directly
    And I click the View Orders link
    And I wait for 20 seconds
    Then I see the appropriate error message for a prescription timeout
    And the SC25_CannotShowPrescriptionInformation page is saved to disk
