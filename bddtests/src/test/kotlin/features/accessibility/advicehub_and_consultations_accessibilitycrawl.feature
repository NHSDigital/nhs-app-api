@accessibility
@advice-hub-and-online-consultations-accessibility
Feature: Advice hub & Online Consultations accessibility

  Scenario: The Online consultations-GP Advice journey pages are captured
    Given I am logged in as a EMIS user with no linked profiles
    And I have access to online consultations gp advice journey and it is not an emergency
    When I navigate to Advice
    And the Advice page is displayed
    And the AdviceHub_LoggedIn page is saved to disk
    And I click Ask your GP for Advice
    And the OLC_ConsentToShareDemographicData page is saved to disk
    And I accept demographics and terms and conditions question
    And I am submitting the questionnaire for myself
    And I see a condition list for myself
    And the OLC_ChooseCondition page is saved to disk
    And I click on a condition
    And the OLC_NonEmergencyChoice page is saved to disk
    And I am not in an emergency
    And the OLC_SelectGender page is saved to disk
    And I select my gender and click continue
    And the OLC_InsertSymptoms page is saved to disk
    And I insert my symptoms and click continue
    And the OLC_InsertDOB page is saved to disk
    And I insert my date of birth
    And the OLC_SelectAlcoholConsumption page is saved to disk
    And I select how much alcohol I drink weekly
    And I insert how long I have felt the pain
    Then I see a care plan for myself
    And the OLC_CarePlan page is saved to disk

  Scenario: The 'Ask your GP for advice - Choose your child's conditions' page is captured
    Given I am logged in as a EMIS user with no linked profiles
    And I have access to online consultations gp advice journey for my child
    When I navigate to Advice
    Then the Advice page is displayed
    And I click Ask your GP for Advice
    And I accept demographics and terms and conditions question
    And I am submitting the questionnaire for my child
    And I see a condition list for my child
    And the OLC_ChooseChildsCondition page is saved to disk

  Scenario: The 'Advice proxy shutter' page is captured
    Given I am logged in as a EMIS user with linked profiles and appointments provider IM1
    And I see the home page
    And I can see and follow the Linked profiles link
    And the linked profiles page is displayed
    And linked profiles are displayed
    And I select a linked profile with appointments enabled false, prescriptions enabled false and medical record enabled false
    And details for the selected linked profile are displayed
    When I click the Switch to this profile button for the proxy user
    And I see the proxy home page
    And I navigate to Advice
    Then the advice shutter page is displayed
    And the Advice_ProxyShutter page is saved to disk

  Scenario: The 'Advice (logged out)' page is captured
    Given I am using the native app user agent
    And I am not logged in
    And I click the 'Get health advice' button
    Then the page title is 'Advice'
    And the AdviceHub_LoggedOut page is saved to disk

  Scenario: The 'GP Advice - Optional consent to OLC privacy notice for adult triage' pages are captured
    Given I am logged in as a EMIS user with no linked profiles
    And I have access to online consultations gp advice journey and it is an emergency
    And I navigate to Advice
    And the Advice page is displayed
    When I click Ask your GP for Advice
    And I accept demographics and navigate to terms consent page
    Then the OLC_ConsentToOLCPrivacyNotice page is saved to disk

  Scenario: The 'Additional GP Services - Optional consent to share demographic data and OLC privacy notice' pages are captured
    Given I am logged in as a EMIS user with no linked profiles
    And I have access to online consultations gp advice journey and it is an emergency
    When I navigate to Appointments
    And I click the Additional GP services link on the Appointments Hub
    Then the OLC_ConsentToShareDemographicData_AdditionalGPServices page is saved to disk
    And I accept demographics and navigate to terms consent page
    And the OLC_ConsentToOLCPrivacyNotice_AdditionalGPServices page is saved to disk
