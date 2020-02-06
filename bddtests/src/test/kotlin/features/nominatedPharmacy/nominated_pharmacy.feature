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
    Then I see prescriptions page loaded
    And I see the nominated pharmacy panel on the prescriptions page
    And I see my nominated pharmacy on the prescriptions page
    When I click on the nominated pharmacy panel
    Then I see nominated pharmacy page loaded
    And I see the change my nominated pharmacy link
    When I click on change your nominated pharmacy link
    Then I see the update nominated pharmacy interrupt page loaded
    When I click on the interrupt continue button
    Then I see the choose type page is loaded
    Then I select high street pharmacy
    And I click on the choose type continue button
    Then I see search nominated pharmacy page loaded
    Given searching for pharmacies with <search text> has 10 results
    When I search for a <search text> and click on search button
    Then I see list of pharmacies displayed on the result page
    And I click on item 4 pharmacy from the list of pharmacies
    Then I see confirm nominated page with selected pharmacy details
    When I click on confirm button to change my nominated pharmacy
    Then I see the change success page with my nominated pharmacy details
    When I click on the go to your repeat prescriptions link
    Then I see prescriptions page loaded
    And I see my nominated pharmacy on the prescriptions page

    Examples:
      | GP System | Pharmacy type | search text | OdsCode |
      | EMIS      | P1            | se1       | SW11XR  |

  Scenario Outline: Patient can select an online only pharmacy as their nominated pharmacy
    Given I am patient using the <GP System> GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And my GP Practice is EPS enabled
    And I have a <Pharmacy type> typed nominated pharmacy with <OdsCode> OdsCode
    And I am logged in
    And I navigate to prescriptions
    Then I see prescriptions page loaded
    And I see the nominated pharmacy panel on the prescriptions page
    And I see my nominated pharmacy on the prescriptions page
    When I click on the nominated pharmacy panel
    Then I see nominated pharmacy page loaded
    And I see the change my nominated pharmacy link
    When I click on change your nominated pharmacy link
    Then I see the update nominated pharmacy interrupt page loaded
    When I click on the interrupt continue button
    Then I see the choose type page is loaded
    Then I select online pharmacy
    And I click on the choose type continue button
    Then I see the dsp interrupt page is loaded
    And I click on the DSP Interrupt continue button
    Then I see the online choices page loaded
    Then I click the Yes radio button on the online choices page
    And I click on the continue button on the online choices page
    Then I see nominated pharmacy online only search page loaded
    Given searching for online pharmacies with <search text> has 20 results
    Then I search for an online only pharmacy with <search text> and click on search button
    Then I see list of online only pharmacies displayed on the result page
    And I click on item 4 pharmacy from the list of pharmacies
    Then I see confirm nominated page with selected online pharmacy details
    When I click on confirm button to change my nominated pharmacy to online
    Then I see the change success page with my online nominated pharmacy details
    When I click on the go to your repeat prescriptions link
    Then I see prescriptions page loaded
    And I see my nominated pharmacy on the prescriptions page
    Examples:
      | GP System | Pharmacy type | search text | OdsCode |
      | EMIS      | P1            | pharmacy       | SW11XR  |

  Scenario Outline: Patient can see no results found page when searching for online only pharmacy
    Given I am patient using the <GP System> GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And my GP Practice is EPS enabled
    And I have a <Pharmacy type> typed nominated pharmacy with <OdsCode> OdsCode
    And I am logged in
    And I navigate to prescriptions
    Then I see prescriptions page loaded
    And I see the nominated pharmacy panel on the prescriptions page
    And I see my nominated pharmacy on the prescriptions page
    When I click on the nominated pharmacy panel
    Then I see nominated pharmacy page loaded
    And I see the change my nominated pharmacy link
    When I click on change your nominated pharmacy link
    Then I see the update nominated pharmacy interrupt page loaded
    When I click on the interrupt continue button
    Then I see the choose type page is loaded
    Then I select online pharmacy
    And I click on the choose type continue button
    Then I see the dsp interrupt page is loaded
    And I click on the DSP Interrupt continue button
    Then I see the online choices page loaded
    Then I click the Yes radio button on the online choices page
    And I click on the continue button on the online choices page
    Then I see nominated pharmacy online only search page loaded
    Given searching for online pharmacies with <search text> has 0 results
    Then I search for an online only pharmacy with <search text> and click on search button
    And I see the no results found page
    Then I see the relevant information about no results for the search term <search text>
    Examples:
      | GP System | Pharmacy type | search text | OdsCode |
      | EMIS      | P1            | XXXXXXXX       | SW11XR  |

  Scenario Outline: Patient can only search nominated pharmacy with postcode
    Given I am patient using the <GP System> GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And my GP Practice is EPS enabled
    And I have a <Pharmacy type> typed nominated pharmacy with <OdsCode> OdsCode
    And I am logged in
    And I navigate to prescriptions
    Then I see prescriptions page loaded
    And I see the nominated pharmacy panel on the prescriptions page
    And I see my nominated pharmacy on the prescriptions page
    When I click on the nominated pharmacy panel
    Then I see nominated pharmacy page loaded
    And I see the change my nominated pharmacy link
    When I click on change your nominated pharmacy link
    Then I see the update nominated pharmacy interrupt page loaded
    When I click on the interrupt continue button
    Then I see the choose type page is loaded
    Then I select high street pharmacy
    And I click on the choose type continue button
    Then I see search nominated pharmacy page loaded
    When I search for a <search text> and click on search button
    Then I see an error indicating the postcode is invalid
    Examples:
      | GP System | Pharmacy type | search text | OdsCode |
      | EMIS      | P1            | boots       | SW11XR  |


  Scenario Outline: Patient sees errors when searching for online nominated pharmacy with no input text
    Given I am patient using the <GP System> GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And my GP Practice is EPS enabled
    And I have a <Pharmacy type> typed nominated pharmacy with <OdsCode> OdsCode
    And I am logged in
    And I navigate to prescriptions
    Then I see prescriptions page loaded
    And I see the nominated pharmacy panel on the prescriptions page
    And I see my nominated pharmacy on the prescriptions page
    When I click on the nominated pharmacy panel
    Then I see nominated pharmacy page loaded
    And I see the change my nominated pharmacy link
    When I click on change your nominated pharmacy link
    Then I see the update nominated pharmacy interrupt page loaded
    When I click on the interrupt continue button
    Then I see the choose type page is loaded
    Then I select online pharmacy
    And I click on the choose type continue button
    Then I see the dsp interrupt page is loaded
    And I click on the DSP Interrupt continue button
    Then I see the online choices page loaded
    Then I click the Yes radio button on the online choices page
    And I click on the continue button on the online choices page
    Then I see nominated pharmacy online only search page loaded
    Then I search for an online only pharmacy with <search text> and click on search button
    And I see an error indicating the search text is invalid
    Examples:
      | GP System | Pharmacy type | OdsCode | search text |
      | EMIS      | P1            | SW11XR  |             |

  Scenario Outline: No results messages are shown for postcode not in use
    Given I am patient using the <GP System> GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And my GP Practice is EPS enabled
    And I have a <Pharmacy type> typed nominated pharmacy with <OdsCode> OdsCode
    And I am logged in
    And I navigate to prescriptions
    Then I see prescriptions page loaded
    And I see the nominated pharmacy panel on the prescriptions page
    And I see my nominated pharmacy on the prescriptions page
    When I click on the nominated pharmacy panel
    Then I see nominated pharmacy page loaded
    And I see the change my nominated pharmacy link
    When I click on change your nominated pharmacy link
    Then I see the update nominated pharmacy interrupt page loaded
    When I click on the interrupt continue button
    Then I see the choose type page is loaded
    Then I select high street pharmacy
    And I click on the choose type continue button
    Then I see search nominated pharmacy page loaded
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
    Then I see prescriptions page loaded
    Then I see the nominated pharmacy panel on the prescriptions page
    And I see that I haven't nominated a pharmacy on the prescriptions page
    When I click on the nominated pharmacy panel
    Then I see the set nominated pharmacy interrupt page loaded
    When I click on the interrupt continue button
    Then I see the choose type page is loaded
    Then I select high street pharmacy
    And I click on the choose type continue button
    Given searching for pharmacies with <search text> has 10 results
    When I search for a <search text> and click on search button
    Then I see list of pharmacies displayed on the result page
    When I click on item 4 pharmacy from the list of pharmacies
    Then I see confirm nominated page with selected pharmacy details
    When I click on confirm button to change my nominated pharmacy
    Then I see the change success page with my nominated pharmacy details
    When I click on the go to your repeat prescriptions link
    Then I see prescriptions page loaded
    And I see my nominated pharmacy on the prescriptions page

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
    Then I see prescriptions page loaded
    And I do not see the nominated pharmacy panel

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
    Then I see prescriptions page loaded
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
    Then I see prescriptions page loaded
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
    Then I see prescriptions page loaded
    And I do not see the nominated pharmacy panel

  Scenario Outline: If patient has a P3 typed nominated pharmacy then Patient does not see nominated pharmacy
    Given I am patient using the <GP System> GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And my GP Practice is EPS enabled
    And I have a <Pharmacy Type> typed nominated pharmacy with <OdsCode> OdsCode
    And I am logged in
    When I navigate to prescriptions
    Then I see prescriptions page loaded
    And I do not see the nominated pharmacy panel
  # these steps have been commented out but not deleted as the expectation is that we will be enabling for phase 2
  # so these steps become relevant at that point

  #  And I see the nominated pharmacy panel on the prescriptions page
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
    Then I see prescriptions page loaded
    And I see the nominated pharmacy panel on the prescriptions page
    And I see my nominated pharmacy on the prescriptions page

    Examples:
      | GP System | OdsCode |
      | EMIS      | SW11XR  |
  
  Scenario Outline: The <GP System> user can see nominated pharmacy on the prescriptions summary page
    Given the scenario is submit prescription
    And I am using <GP System> GP System to submit my prescription
    And I have 1 historic prescriptions in this scenario
    And my GP Practice is EPS enabled
    And I have a <Pharmacy type> typed nominated pharmacy with <OdsCode> OdsCode
    And I am logged in
    When I retrieve the 'Your Prescriptions' page directly
    And I select 1 repeatable prescriptions to order
    And I click Continue on the Order a repeat prescription page
    Then I see nominated pharmacy information is shown and correct
    Examples:
      | GP System | Pharmacy type | OdsCode |
      | EMIS      | P1            | SW11XR  |

  Scenario Outline: Patient can view a list of random online pharmacies
    Given I am patient using the <GP System> GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And my GP Practice is EPS enabled
    And I have a <Pharmacy type> typed nominated pharmacy with <OdsCode> OdsCode
    And I am logged in
    And I navigate to prescriptions
    Then I see prescriptions page loaded
    And I see the nominated pharmacy panel on the prescriptions page
    And I see my nominated pharmacy on the prescriptions page
    When I click on the nominated pharmacy panel
    Then I see nominated pharmacy page loaded
    And I see the change my nominated pharmacy link
    When I click on change your nominated pharmacy link
    Then I see the update nominated pharmacy interrupt page loaded
    When I click on the interrupt continue button
    Then I see the choose type page is loaded
    Then I select online pharmacy
    And I click on the choose type continue button
    Then I see the dsp interrupt page is loaded
    And I click on the DSP Interrupt continue button
    Then I see the online choices page loaded
    And I click the No radio button on the online choices page
    Given searching for randomized online pharmacies has results
    Then I click on the continue button on the online choices page
    Then I see list of random online pharmacies displayed on the result page
    And I click on item 4 pharmacy from the list of online pharmacies
    Then I see confirm nominated page with selected online pharmacy details
    When I click on confirm button to change my nominated pharmacy to online
    Then I see the change success page with my online nominated pharmacy details
    When I click on the go to your repeat prescriptions link
    Then I see prescriptions page loaded
    And I see my nominated pharmacy on the prescriptions page

    Examples:
      | GP System | Pharmacy type | OdsCode |
      | EMIS      | P1            | SW11XR  |