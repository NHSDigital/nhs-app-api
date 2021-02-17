@nominatedPharmacy
Feature: nominated pharmacy journey

  Scenario Outline: Patient can change their nominated pharmacy
    Given I am patient using the <GP System> GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And my GP Practice is EPS enabled
    And I have a <Pharmacy type> typed nominated pharmacy with <OdsCode> OdsCode
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
    Given searching for pharmacies with <search text> has 10 results
    When I search for a <search text> and click on search button
    Then I see list of pharmacies displayed on the result page
    And I click on item 4 pharmacy from the list of pharmacies
    And I see confirm nominated page with selected pharmacy details
    When I click on confirm button to change my nominated pharmacy
    Then I see the change success page with my nominated pharmacy details
    When I click on the go to your prescriptions link
    Then the Prescriptions Hub page is displayed
    And I see my updated nominated pharmacy on the prescriptions hub page
    Examples:
      | GP System | Pharmacy type | search text | OdsCode |
      | EMIS      | P1            | se1       | SW11XR  |

  Scenario Outline: Patient can change their nominated pharmacy from View Orders
    Given I am patient using the <GP System> GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And my GP Practice is EPS enabled
    And I have a <Pharmacy type> typed nominated pharmacy with <OdsCode> OdsCode
    And I am logged in
    And I navigate to prescriptions
    Then the Prescriptions Hub page is displayed
    When I click the View Orders link
    Then I click the change nominated pharmacy link on the view orders page
    And I see the update nominated pharmacy interrupt page loaded
    When I click on the interrupt continue button
    Then I see the choose type page is loaded
    And I select high street pharmacy
    And I click on the choose type continue button
    And I see search nominated pharmacy page loaded
    Given searching for pharmacies with <search text> has 10 results
    When I search for a <search text> and click on search button
    Then I see list of pharmacies displayed on the result page
    And I click on item 4 pharmacy from the list of pharmacies
    And I see confirm nominated page with selected pharmacy details
    When I click on confirm button to change my nominated pharmacy
    Then I see the change success page with my nominated pharmacy details
    When I click on the go to your prescriptions link
    Then the Prescriptions Hub page is displayed
    And I see my updated nominated pharmacy on the prescriptions hub page
    Examples:
      | GP System | Pharmacy type | search text | OdsCode |
      | EMIS      | P1            | se1       | SW11XR  |

  Scenario Outline: Patient can see the online pharmacy interrupt page and return to prescriptions home
    Given I am patient using the <GP System> GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And my GP Practice is EPS enabled
    And I have a <Pharmacy type> typed nominated pharmacy with <OdsCode> OdsCode
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
    And I click on the DSP Interrupt Prescription Home link
    And the Prescriptions Hub page is displayed
    And I see the nominated pharmacy panel on the prescriptions hub page
    And I see my nominated pharmacy on the prescriptions hub page
    Examples:
      | GP System | Pharmacy type | OdsCode |
      | EMIS      | P1            | SW11XR  |

  Scenario Outline: Patient can see the online pharmacy interrupt page and goto external site
    Given I am patient using the <GP System> GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And my GP Practice is EPS enabled
    And I have a <Pharmacy type> typed nominated pharmacy with <OdsCode> OdsCode
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
    When I click the link called 'View a list of online-only pharmacies' with a url of 'https://www.nhs.uk/Service-Search/other-services/pharmacies/internetpharmacies'
    Then a new tab has been opened by the link
    Examples:
      | GP System | Pharmacy type | OdsCode |
      | EMIS      | P1            | SW11XR  |

  Scenario Outline: Patient can only search nominated pharmacy with postcode
    Given I am patient using the <GP System> GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And my GP Practice is EPS enabled
    And I have a <Pharmacy type> typed nominated pharmacy with <OdsCode> OdsCode
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
    When I search for a <search text> and click on search button
    Then I see an error indicating the postcode is invalid
    Examples:
      | GP System | Pharmacy type | search text | OdsCode |
      | EMIS      | P1            | boots       | SW11XR  |

  Scenario Outline: No results messages are shown for postcode not in use
    Given I am patient using the <GP System> GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And my GP Practice is EPS enabled
    And I have a <Pharmacy type> typed nominated pharmacy with <OdsCode> OdsCode
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
    Given searching for pharmacies with <search text> has 0 results
    When I search for a <search text> and click on search button
    Then I see the no results found messages for <search text>
    Examples:
      | GP System | Pharmacy type | search text | OdsCode |
      | EMIS      | P1            | BT1       | SW11XR  |

  @smoketest
  Scenario Outline: Patient with no nominated pharmacy can nominate a pharmacy
    Given I am patient using the <GP System> GP System
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
    When I click on the interrupt continue button
    Then I see the choose type page is loaded
    And I select high street pharmacy
    And I click on the choose type continue button
    Given searching for pharmacies with <search text> has 10 results
    When I search for a <search text> and click on search button
    Then I see list of pharmacies displayed on the result page
    When I click on item 4 pharmacy from the list of pharmacies
    Then I see confirm nominated page with selected pharmacy details
    When I click on confirm button to change my nominated pharmacy
    Then I see the change success page with my nominated pharmacy details
    When I click on the go to your prescriptions link
    Then the Prescriptions Hub page is displayed
    And I see my updated nominated pharmacy on the prescriptions hub page
    Examples:
      | GP System | search text |
      | EMIS      | se1       |

  Scenario Outline: Patient does not see nominated pharmacy when their gp practice is not enabled for EPS
    Given I am patient using the <GP System> GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And my GP Practice is EPS disabled
    And I am logged in
    When I navigate to prescriptions
    Then the Prescriptions Hub page is displayed
    And I do not see the nominated pharmacy panel

    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: Patient with no nominated pharmacy sees relevant text in View Orders Page
    Given I am patient using the <GP System> GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And my GP Practice is EPS enabled
    And I don't have a nominated pharmacy of any type
    And I am logged in
    When I navigate to prescriptions
    And I click the View Orders link
    Then I see the help text for no set nominated pharmacy

    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: Patient does not see nominated pharmacy when gp practice is enabled but account is sensitive
    Given I am patient using the <GP System> GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And my GP Practice is EPS enabled
    And I have a P1 typed nominated pharmacy with SW11XR OdsCode and S ConfidentialityCode
    And I am logged in
    When I navigate to prescriptions
    Then the Prescriptions Hub page is displayed
    And I do not see the nominated pharmacy panel

    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: Patient does not see nominated pharmacy when gp practice is enabled but nhs number is superseded
    Given I am patient using the <GP System> GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And my GP Practice is EPS enabled
    And I have a P1 typed nominated pharmacy with SW11XR OdsCode and nhsNumber 1234567890 is returned
    And I am logged in
    When I navigate to prescriptions
    Then the Prescriptions Hub page is displayed
    And I do not see the nominated pharmacy panel

    Examples:
      | GP System |
      | EMIS      |

  Scenario: Patient does not see nominated pharmacy when SJR has disabled it for their gp practice
    Given I am a EMIS user where the journey configurations are:
      | Journey            | Value    |
      | nominated pharmacy | disabled |
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And my GP Practice is EPS enabled
    And I have a P1 typed nominated pharmacy with SW11XR OdsCode
    And I am logged in
    When I navigate to prescriptions
    Then the Prescriptions Hub page is displayed
    And I do not see the nominated pharmacy panel

  Scenario Outline: Patient with no nominated pharmacy sees relevant text in prescriptions confirmation Page
    Given I am a <GP System> patient
    And I have historic prescriptions
    And there are 5 repeatable prescriptions available
    And my GP Practice is EPS enabled
    And I don't have a nominated pharmacy of any type
    And I am logged in
    And I navigate to prescriptions
    And I select 1 repeatable prescriptions out of 5 available
    When I click Continue on the Order a repeat prescription page
    Then I see the default nominated pharmacy text
      Examples:
        | GP System |
        | EMIS      |

  Scenario: Patient does not see nominated pharmacy when SJR is disabled
    Given I am a EMIS user where the journey configurations are:
      | Journey            | Value    |
      | nominated pharmacy | disabled |
    And I have a P1 typed nominated pharmacy with SW11XR OdsCode
    And I have historic prescriptions
    And there are 5 repeatable prescriptions available
    And I am logged in
    And I navigate to prescriptions
    And I select 1 repeatable prescriptions out of 5 available
    When I click Continue on the Order a repeat prescription page
    Then I do not see a nominated pharmacy

  Scenario Outline: Pateint can see nominated pharmacy when nominated pharmacy is enabled
    Given I am patient using the <GP System> GP System
    And I have a P1 typed nominated pharmacy with SW11XR OdsCode
    And I have historic prescriptions
    And there are 5 repeatable prescriptions available
    And my GP Practice is EPS enabled
    And I am logged in
    And I navigate to prescriptions
    And I select 1 repeatable prescriptions out of 5 available
    When I click Continue on the Order a repeat prescription page
    Then I see my nominated pharmacy details
      Examples:
        | GP System |
        | EMIS      |

  Scenario Outline: If patient has a P3 typed nominated pharmacy then Patient does not see nominated pharmacy
    Given I am patient using the <GP System> GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And my GP Practice is EPS enabled
    And I have a <Pharmacy Type> typed nominated pharmacy with <OdsCode> OdsCode
    And I am logged in
    When I navigate to prescriptions
    Then the Prescriptions Hub page is displayed
    And I do not see the nominated pharmacy panel
  # these steps have been commented out but not deleted as the expectation is that we will be enabling for phase 2
  # so these steps become relevant at that point

  #  And I see the nominated pharmacy panel on the prescriptions hub page
  #  When I click on the nominated pharmacy panel
  #  Then I see nominated pharmacy page loaded with dispensing practise header
  #  And I see how to change dispensing practice instruction
  #  When I click the Back link
  #  Then I see prescriptions page loaded

    Examples:
      | GP System | Pharmacy Type | OdsCode |
      | EMIS      | P3            | SW11XR  |

  Scenario Outline: If patient has an Internet Pharmacy they can see their nominated pharmacy
    Given I am patient using the <GP System> GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And my GP Practice is EPS enabled
    And I have a P1 typed Internet pharmacy with <OdsCode> OdsCode
    And I am logged in
    When I navigate to prescriptions
    Then the Prescriptions Hub page is displayed
    And I see the nominated pharmacy panel on the prescriptions hub page
    And I see my nominated pharmacy on the prescriptions hub page

    Examples:
      | GP System | OdsCode |
      | EMIS      | SW11XR  |

  Scenario: The EMIS user can see nominated pharmacy on the prescriptions confirmation page and a playback on the order success page
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
    And I see nominated pharmacy information is shown and correct

  Scenario: The EMIS user can see advice on what happens next with their internet pharmacy on the order success page
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

  Scenario: The EMIS user can see default advice on what happens next they have no nominated pharmacy
    Given the scenario is submit prescription
    And I am using EMIS GP System to submit my prescription
    And I have 1 historic prescriptions in this scenario
    And my GP Practice is EPS enabled
    And I don't have a nominated pharmacy of any type
    And I am logged in
    When I retrieve the 'Your Prescriptions' page directly
    And I select 1 repeatable prescriptions to order
    And I click Continue on the Order a repeat prescription page
    And I click Confirm and order repeat prescription
    Then I see the Order Success page with a playback of my order and what happens next with no nominated pharmacy
    And I cannot see any nominated pharmacy information
