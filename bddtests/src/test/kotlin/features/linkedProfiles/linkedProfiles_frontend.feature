@linked-profiles
Feature: Login with proxy access

  @smoketest
  Scenario: An EMIS user with proxy accounts can see proxy details and switch back to their own account
    Given I am logged in as a EMIS user with linked profiles and appointments provider IM1
    When I select the linked profiles link from the home page
    And I select a linked profile
    And I click the Switch to this profile button for the proxy user
    And I see the proxy patient details of age and gp surgery
    And I click the proxy warning
    Then the switch profiles page is displayed
    And the correct proxy user details are displayed
    Then I click the Switch to my profile button for the main user
    And I see the home page
    And I do not see the yellow banner

  Scenario Outline: An EMIS user proxying on behalf of another user will be shown appointments shutter page when appointments provider is <Appointments Provider>
    Given I am logged in as a EMIS user with linked profiles and appointments provider <Appointments Provider>
    Then I see the home page
    And I see the linked profiles link
    When I select the linked profiles link from the home page
    Then the linked profiles page is displayed
    And linked profiles are displayed
    When I select a linked profile
    Then details for the selected linked profile are displayed
    When I click the Switch to this profile button for the proxy user
    Then I see the home page
    And I see the yellow banner
    And the yellow banner contains details for the user I am acting on behalf of
    And I do not see the home page links
    And I click on the Appointments link on the header
    Then the appointments shutter page is displayed
    Examples:
      | Appointments Provider |
      | INFORMATICA           |
      | GPATHAND              |

  Scenario Outline: An EMIS user proxying on behalf of another user will be shown Im1 Appointments page when appointments provider is <Appointments Provider>
    Given I am logged in as a EMIS user with linked profiles and appointments provider <Appointments Provider>
    Then I see the home page
    And I see the linked profiles link
    When I select the linked profiles link from the home page
    Then the linked profiles page is displayed
    And linked profiles are displayed
    When I select a linked profile
    Then details for the selected linked profile are displayed
    When I click the Switch to this profile button for the proxy user
    Then I see the home page
    And I see the yellow banner
    And the yellow banner contains details for the user I am acting on behalf of
    And I do not see the home page links
    And I have no booked appointments for EMIS
    And I click on the Appointments link on the header
    Then the page title is "Your appointments"
    Examples:
      | Appointments Provider |
      | ECONSULT              |
      | IM1                   |

  Scenario Outline: An EMIS user proxying on behalf of another user will not be shown the Nominated pharmacy on repeat prescriptions page when provider is <Appointments Provider>
    Given I am logged in as a EMIS user with linked profiles and appointments provider <Appointments Provider>
    Then I see the home page
    And I see the linked profiles link
    When I select the linked profiles link from the home page
    Then the linked profiles page is displayed
    And linked profiles are displayed
    When I select a linked profile
    Then details for the selected linked profile are displayed
    When I click the Switch to this profile button for the proxy user
    Then I see the home page
    And I see the yellow banner
    And the yellow banner contains details for the user I am acting on behalf of
    And I do not see the home page links
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And my GP Practice is EPS enabled
    And I have a P1 typed nominated pharmacy
    And I navigate to prescriptions
    Then I see prescriptions page loaded
    And I do not see the nominated pharmacy panel
    Examples:
      | Appointments Provider |
      | INFORMATICA           |
      | GPATHAND              |
      | ECONSULT              |
      | IM1                   |

  Scenario: An EMIS user sees shutter pages when proxying
    Given I am logged in as a EMIS user with linked profiles and appointments provider IM1
    Then I see the home page
    And I see the linked profiles link
    When I select the linked profiles link from the home page
    Then the linked profiles page is displayed
    And linked profiles are displayed
    Then I select a linked profile with appointments enabled false, prescriptions enabled false and medical record enabled false
    Then details for the selected linked profile are displayed
    When I click the Switch to this profile button for the proxy user
    Then I see the home page
    When I click the settings icon
    Then the settings shutter page is displayed
    When I navigate to Symptoms
    Then the symptoms shutter page is displayed
    When I navigate to Prescriptions
    Then the prescriptions shutter page is displayed
    When I navigate to Appointments
    Then the appointments shutter page is displayed
    When I navigate to My_Record
    And I click continue
    Then the medical record shutter page is displayed
