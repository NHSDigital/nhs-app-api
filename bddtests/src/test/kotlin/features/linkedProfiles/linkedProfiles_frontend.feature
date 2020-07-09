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
    And I click the Switch to my profile button for the main user
    And I see the home page
    And I do not see the yellow banner

  Scenario Outline: A <Gp Provider> user proxying on behalf of another user will be shown appointments shutter page when appointments provider is <Appointments Provider>
    Given I am logged in as a <Gp Provider> user with linked profiles and appointments provider <Appointments Provider>
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
    And the Appointments Hub page is displayed
    And I click the GP Appointments link
    And the appointments shutter page is displayed
    Examples:
      | Gp Provider | Appointments Provider |
      | EMIS        | INFORMATICA           |
      | EMIS        | GPATHAND              |
      | TPP         | INFORMATICA           |
      | TPP         | GPATHAND              |

  Scenario Outline: A <Gp Provider> user proxying on behalf of another user will be shown Im1 Appointments page when appointments provider is <Appointments Provider>
    Given I am logged in as a <Gp Provider> user with linked profiles and appointments provider <Appointments Provider>
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
    And there are <Gp Provider> appointments available to book with a reason
    And I have no booked appointments for <Gp Provider>
    And I click on the Appointments link on the header
    And the page title is "Your GP appointments"
    Examples:
      | Gp Provider | Appointments Provider |
      | EMIS        | ECONSULT              |
      | EMIS        | IM1                   |
      | TPP         | ECONSULT              |
      | TPP         | IM1                   |

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
    And the Prescriptions Hub page is displayed
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
    And I select a linked profile with appointments enabled false, prescriptions enabled false and medical record enabled false
    And details for the selected linked profile are displayed
    When I click the Switch to this profile button for the proxy user
    Then I see the home page
    When I click the settings icon
    Then the settings shutter page is displayed
    When I navigate to Symptoms
    Then the symptoms shutter page is displayed
    When I navigate to Prescriptions
    And I click the View Orders link
    Then the prescriptions shutter page is displayed
    When I navigate to My_Record
    And I click the 'Continue' button
    Then the medical record shutter page is displayed
    When I navigate to Appointments
    Then the Appointments Hub page is displayed
    And I click the GP Appointments link
    And the appointments shutter page is displayed
    When I click the link called 'Use the 111 coronavirus service to find out what to do' with a url of 'https://111.nhs.uk/service/COVID-19/'
    Then a new tab has been opened by the link

  Scenario Outline: A <GP System> user proxying on behalf of another will see the confirmation page after booking a repeat prescription
    Given I am logged in as a <GP System> user with linked profiles and appointments provider IM1
    And the scenario is submit prescription
    Then I see the home page
    And I see the linked profiles link
    When I select the linked profiles link from the home page
    Then the linked profiles page is displayed
    And linked profiles are displayed
    When I select a linked profile
    Then details for the selected linked profile are displayed
    When I click the Switch to this profile button for the proxy user
    Then I see the home page
    And I am using <GP System> GP System to submit my prescription
    And I have 1 historic prescriptions in this scenario
    And there are 5 repeatable prescriptions available
    And I navigate to prescriptions
    And the Prescriptions Hub page is displayed
    And I select 1 repeatable prescriptions to order
    And I click Continue on the Order a repeat prescription page
    When I click Confirm and order repeat prescription
    Then I see repeat prescription confirmation page loaded
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |


  Scenario Outline: A <Gp System> user proxying on behalf of another will see the confirmation page after booking an appointment
    Given I am logged in as a <Gp System> user with linked profiles and appointments provider IM1
    Then I see the home page
    And I see the linked profiles link
    When I select the linked profiles link from the home page
    Then the linked profiles page is displayed
    And linked profiles are displayed
    When I select a linked profile
    Then details for the selected linked profile are displayed
    When I click the Switch to this profile button for the proxy user
    Then I see the home page
    And there are <Gp System> appointments available to book with a reason
    And I have no booked appointments for <Gp System>
    And I click on the Appointments link on the header
    And the Appointments Hub page is displayed
    And I click the GP Appointments link
    And the page title is "Your GP appointments"
    When I select "Book an appointment" button
    Then I am on the Appointments Guidance page
    When I select the Book an Appointment button on the guidance page
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I enter symptoms
    And I click the 'Confirm and book appointment' button
    Then The appointment booking success page is shown
    Examples:
      | Gp System |
      | EMIS      |
      | TPP       |

  Scenario: An EMIS user proxying on behalf of another will see the confirmation page after cancelling an appointment
    Given I am logged in as a EMIS user with linked profiles and appointments provider IM1
    Then I see the home page
    And I see the linked profiles link
    When I select the linked profiles link from the home page
    Then the linked profiles page is displayed
    And linked profiles are displayed
    When I select a linked profile
    Then details for the selected linked profile are displayed
    When I click the Switch to this profile button for the proxy user
    Then I see the home page
    And EMIS is available to cancel a previously booked appointment before cutoff time because No longer required
    And I click on the Appointments link on the header
    And the Appointments Hub page is displayed
    And I click the GP Appointments link
    And the page title is "Your GP appointments"
    And I select a "Cancel this appointment" link
    And I select a cancellation reason of No longer required
    When I select "Cancel appointment" button
    Then The appointment cancellation success page is shown

  Scenario: A TPP user proxying on behalf of another will see the confirmation page after cancelling an appointment
    Given I am logged in as a TPP user with linked profiles and appointments provider IM1
    Then I see the home page
    And I see the linked profiles link
    When I select the linked profiles link from the home page
    Then the linked profiles page is displayed
    And linked profiles are displayed
    When I select a linked profile
    Then details for the selected linked profile are displayed
    When I click the Switch to this profile button for the proxy user
    Then I see the home page
    And TPP is available to cancel a previously booked appointment before cutoff time because No longer required
    And I click on the Appointments link on the header
    And the Appointments Hub page is displayed
    And I click the GP Appointments link
    And the page title is "Your GP appointments"
    And I select a "Cancel this appointment" link
    When I select "Cancel appointment" button
    Then The appointment cancellation success page is shown

  Scenario Outline: A <Gp System> user proxying on behalf of another will not be able to see any options on the more page
    Given I am a <Gp System> user with linked profiles
    And I am logged in
    And I have switched to a linked profile
    When I navigate to More
    Then I see the more page header
    And the More page explains that it is not possible to access it while acting on behalf of someone else
    When I click the Switch to my profile button for the main user
    And I see the home page
    And I do not see the yellow banner
    Examples:
      | Gp System |
      | EMIS      |
      | TPP       |

  Scenario: An TPP user with proxy accounts can see proxy details and switch back to their own account
    Given I am logged in as a TPP user with linked profiles and appointments provider IM1
    When I select the linked profiles link from the home page
    And I select a linked profile
    And I click the Switch to this profile button for the proxy user
    And I see the proxy patient details of age and gp surgery
    And I click the proxy warning
    Then the switch profiles page is displayed
    And the correct proxy user details are displayed
    And I click the Switch to my profile button for the main user
    And I see the home page
    And I do not see the yellow banner

  Scenario Outline: A <GP System> user proxying on behalf of another will be able to see their medical record
    Given I am a <GP System> user with linked profiles
    And I am logged in
    And I have switched to a linked profile
    And the GP Practice has enabled all medical records for the proxy patient
    When I click on the My Record link on the header
    And I click the 'Continue' button
    Then I see the medical record v2 page
    When I click the Medicines link on my record - Medical Record v2
    Then I see the medical record v2 medicines page
    When I click the Acute medicines link - Medical Record v2
    Then I see the expected acute medicines - Medical Record v2
    When I click the Back link
    Then I see the medical record v2 medicines page
    When I click the Back link
    And I click the Consultations and events link on my record - Medical Record v2
    Then I see the expected consultations and events - Medical Record v2
    When I click the Back link
    And I click the Test results link on my record - Medical Record v2
    Then I see the correct number of test results for current the supplier - Medical Record v2
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario: An TPP user sees shutter pages when proxying and trying to access services without permissions
    Given I am logged in as a TPP user with linked profiles but no access to core services and appointments provider IM1
    And the scenario is submit prescription
    Then I see the home page
    And I see the linked profiles link
    When I select the linked profiles link from the home page
    And I select a linked profile
    And I click the Switch to this profile button for the proxy user
    And prescriptions is disabled for the proxy account at a GP Practice level
    And the GP Practice has disabled proxy access to summary care record functionality
    And the GP Practice has disabled proxy access to dcr events functionality for TPP
    And TPP user is not allowed to view appointments
    Then I see the home page
    When I click the settings icon
    Then the settings shutter page is displayed
    When I navigate to Symptoms
    Then the symptoms shutter page is displayed
    When I navigate to Prescriptions
    And the Prescriptions Hub page is displayed
    And I click the Order a repeat prescription button
    Then the prescriptions shutter page is displayed
    When I navigate to My_Record
    And I click the 'Continue' button
    Then the medical record shutter page is displayed
    When I navigate to Appointments
    Then the Appointments Hub page is displayed
    And I click the GP Appointments link
    And the appointments shutter page is displayed
    When I click the link called 'Use the 111 coronavirus service to find out what to do' with a url of 'https://111.nhs.uk/service/COVID-19/'
    Then a new tab has been opened by the link
