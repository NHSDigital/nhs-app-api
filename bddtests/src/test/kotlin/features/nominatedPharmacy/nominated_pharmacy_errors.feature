@nominatedPharmacy
Feature: Error scenarios for nominated pharmacy

  Scenario Outline: Patient does not see nominated pharmacy when call to spine to retrieve nominated pharmacy fails
    Given I am patient using the <GP System> GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And my GP Practice is EPS enabled
    And the request to PDS Trace to retrieve my nominated pharmacy fails
    When I am logged in
    And I navigate to prescriptions
    Then the Prescriptions Hub page is displayed
    And I do not see the nominated pharmacy panel

    Examples:
      | GP System |
      | EMIS      |


  Scenario Outline: Patient does not see nominated pharmacy when call to NHSSearch to retrieve nominated pharmacy fails
    Given I am patient using the <GP System> GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And the request to Azure search to retrieve my nominated pharmacy fails
    When I am logged in
    And I navigate to prescriptions
    Then the Prescriptions Hub page is displayed
    And I do not see the nominated pharmacy panel

    Examples:
      | GP System |
      | EMIS      |

