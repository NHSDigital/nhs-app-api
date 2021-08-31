@accessibility
@nominated-pharmacy-accessibility
Feature: Nominated pharmacy accessibility

  Scenario: The journey of six nominated pharmacy pages are captured
    Given I am a patient using the EMIS GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And my GP Practice is EPS enabled
    And I have a P1 typed nominated pharmacy with SW11XR OdsCode
    And I am logged in
    And I navigate to prescriptions
    Then the Prescriptions Hub page is displayed
    And the Prescriptions_Hub page is saved to disk
    And I see the nominated pharmacy panel on the prescriptions hub page
    And I see my nominated pharmacy on the prescriptions hub page
    When I click the nominated pharmacy link on the Prescriptions Hub
    Then I see nominated pharmacy page loaded
    And the NominatedPharmacy_Home page is saved to disk
    And I see the change my nominated pharmacy link
    When I click on change your nominated pharmacy link
    Then I see the update nominated pharmacy interrupt page loaded
    And the OutstandingPrescriptions_Interrupt page is saved to disk
    When I click on the interrupt continue button
    Then I see the choose type page is loaded
    And the NominatedPharmacy_ChooseType page is saved to disk
    And I select high street pharmacy
    And I click on the choose type continue button
    And I see search nominated pharmacy page loaded
    And the HighStreetPharmacy_Find page is saved to disk
    Given searching for pharmacies with se1 has 10 results
    When I search for a se1 and click on search button
    Then I see list of pharmacies displayed on the result page
    And I see the high street pharmacy search results page loaded for se1
    And the HighStreetPharmacy_SearchResults page is saved to disk

  Scenario: The 'High street pharmacy confirmation choice' page is captured
    Given I am a patient using the EMIS GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And my GP Practice is EPS enabled
    And I have a P1 typed nominated pharmacy with SW11XR OdsCode
    And I am logged in
    And I navigate to prescriptions
    Then the Prescriptions Hub page is displayed
    And I see the nominated pharmacy panel on the prescriptions hub page
    And I see my nominated pharmacy on the prescriptions hub page
    When I click the nominated pharmacy link on the Prescriptions Hub
    Then I see nominated pharmacy page loaded
    And I see the change my nominated pharmacy link
    When I click on change your nominated pharmacy link
    Then I see the update nominated pharmacy interrupt page loaded
    When I click on the interrupt continue button
    Then I see the choose type page is loaded
    And I select high street pharmacy
    And I click on the choose type continue button
    And I see search nominated pharmacy page loaded
    Given searching for pharmacies with se1 has 10 results
    When I search for a se1 and click on search button
    Then I see list of pharmacies displayed on the result page
    And I click on item 4 pharmacy from the list of pharmacies
    And I see confirm nominated page with selected pharmacy details
    And the HighStreetPharmacy_ConfirmChoice page is saved to disk

  Scenario: The 'Nominated pharmacy confirmation' page is captured
    Given I am a patient using the EMIS GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And my GP Practice is EPS enabled
    And I have a P1 typed nominated pharmacy with SW11XR OdsCode
    And I am logged in
    And I navigate to prescriptions
    Then the Prescriptions Hub page is displayed
    And I see the nominated pharmacy panel on the prescriptions hub page
    And I see my nominated pharmacy on the prescriptions hub page
    When I click the nominated pharmacy link on the Prescriptions Hub
    Then I see nominated pharmacy page loaded
    And I see the change my nominated pharmacy link
    When I click on change your nominated pharmacy link
    Then I see the update nominated pharmacy interrupt page loaded
    When I click on the interrupt continue button
    Then I see the choose type page is loaded
    And I select high street pharmacy
    And I click on the choose type continue button
    And I see search nominated pharmacy page loaded
    Given searching for pharmacies with se1 has 10 results
    When I search for a se1 and click on search button
    Then I see list of pharmacies displayed on the result page
    And I click on item 4 pharmacy from the list of pharmacies
    And I see confirm nominated page with selected pharmacy details
    When I click on confirm button to change my nominated pharmacy
    And I see the change success page with my nominated pharmacy details
    And the NominatedPharmacy_Confirmation page is saved to disk

  Scenario: The 'Online-only nominated pharmacy' page is captured
    Given I am a patient using the EMIS GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And my GP Practice is EPS enabled
    And I have a P1 typed nominated pharmacy with SW11XR OdsCode
    And I am logged in
    And I navigate to prescriptions
    Then the Prescriptions Hub page is displayed
    And I see the nominated pharmacy panel on the prescriptions hub page
    And I see my nominated pharmacy on the prescriptions hub page
    When I click the nominated pharmacy link on the Prescriptions Hub
    Then I see nominated pharmacy page loaded
    And I see the change my nominated pharmacy link
    When I click on change your nominated pharmacy link
    Then I see the update nominated pharmacy interrupt page loaded
    When I click on the interrupt continue button
    Then I see the choose type page is loaded
    And I select online pharmacy
    And I click on the choose type continue button
    And I see the dsp interrupt page is loaded
    And the NominatedPharmacy_OnlineOnly page is saved to disk

  Scenario: The 'Check the pharmacy this will be sent to' page is captured
    Given I am a patient using the EMIS GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And my GP Practice is EPS enabled
    And I have a P1 typed nominated pharmacy with SW11XR OdsCode
    And I am logged in
    And I navigate to prescriptions
    Then the Prescriptions Hub page is displayed
    And I click 'Order a prescription'
    And the NominatedPharmacy_CheckPharmacyThisWillBeSentTo page is saved to disk

  Scenario: The 'Nominated pharmacy interrupt' page is captured
    Given I am a patient using the EMIS GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And my GP Practice is EPS enabled
    And I don't have a nominated pharmacy of any type
    And I am logged in
    When I navigate to prescriptions
    And the Prescriptions Hub page is displayed
    Then I see the nominated pharmacy panel on the prescriptions hub page
    And I see that I haven't nominated a pharmacy on the prescriptions page
    When I click the nominated pharmacy link on the Prescriptions Hub
    Then I see the set nominated pharmacy interrupt page loaded
    And the NominatedPharmacy_Interrupt page is saved to disk

  Scenario: The 'No nominated pharmacy selected' page is captured
    Given I am a patient using the EMIS GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And my GP Practice is EPS enabled
    And I don't have a nominated pharmacy of any type
    And I am logged in
    When I navigate to prescriptions
    And the Prescriptions Hub page is displayed
    And I click 'Order a prescription'
    And the NominatedPharmacy_NoSelection page is saved to disk
