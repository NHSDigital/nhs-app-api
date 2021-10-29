@accessibility
@supplier-onboarding-accessibility
Feature: Supplier onboarding accessibility

  Scenario: 'Patients Know Best onboarding' page is captured
    Given I am using the native app user agent
    And I am a user who can view care plans from Patients Know Best
    And I am logged in
    When I navigate to the health record hub page
    Then I see the health records hub page
    And I click the menu item 'Care plans'
    And I am redirected to the redirector page with the header 'Care plans'
    And the care plans warning message on the Redirector page explains the service is from Patients Know Best
    And the Supplier_Onboarding_Patients_Know_Best page is saved to disk

  Scenario: 'Care Information Exchange onboarding' page is captured
    Given I am using the native app user agent
    And I am a user who can view care plans from Care Information Exchange
    And I am logged in
    When I navigate to the health record hub page
    Then I see the health records hub page
    And I click the menu item 'Care plans'
    And I am redirected to the redirector page with the header 'Care plans'
    And the care plan warning on the page explains the service is from Care Information Exchange
    And the Supplier_Onboarding_Care_Information_Exchange page is saved to disk

  Scenario: 'MyCareView onboarding' page is captured
    Given I am using the native app user agent
    And I am a user who can view care plans from PKB My Care View
    And I am logged in
    When I navigate to the health record hub page
    Then I see the health records hub page
    And I click the menu item 'Care plans'
    And I am redirected to the redirector page with the header 'Care plans'
    And the care plan warning on the page explains the service is from PKB My Care View
    And the Supplier_Onboarding_MyCareView page is saved to disk

  Scenario: 'Substrakt onboarding' page is captured
    Given I am using the native app user agent
    And I am a user who can view Ask Your Gp Surgery a Question from Substrakt
    And I am logged in
    When I navigate to the Messages Hub page
    Then the Messages Hub page is displayed
    When I click the Substrakt Ask Your GP Surgery a Question link on the Messages Hub page
    Then I am redirected to the redirector page with the header 'Ask your GP surgery a question'
    And the question warning message on the Redirector page explains the service is from Substrakt
    And the Supplier_Onboarding_Substrakt page is saved to disk

  Scenario: 'Engage onboarding' page is captured
    Given I am using the native app user agent
    And I am a user who can view Messages and Online Consultations from Engage
    And I am logged in
    When I navigate to the Messages Hub page
    Then the Messages Hub page is displayed
    When I click the Engage Messages link on the Messages Hub page
    Then I am redirected to the redirector page with the header 'Online consultations'
    And the messages warning message on the Redirector page explains the service is from Engage
    And the Supplier_Onboarding_Engage page is saved to disk

  Scenario: 'GNCR onboarding' page is captured
    Given I am using the native app user agent
    And I am a user who can view Appointments from GNCR
    And I am logged in
    And I navigate to the hospital and other appointments page
    Then the Hospital Appointments page is displayed
    And I can see the GNCR View Appointments link on the Appointments page
    When I click the GNCR View Appointments link on the Appointments page
    Then I am redirected to the redirector page with the header 'Hospital and other appointments'
    And the hospital and other warning message on the Redirector page explains the service is from GNCR
    And the Supplier_Onboarding_GNCR page is saved to disk

  Scenario: 'Prove your identity shutter' page is captured
    Given I am a patient with proof level 5
    And I am logged in
    When I retrieve the 'your prescriptions' page directly
    Then the page title is 'Prescriptions'
    And I am asked to prove my identity to access 'your prescriptions'
    And the Supplier_Onboarding_Prove_Your_Identity_Shutter page is saved to disk

  Scenario: 'Feature not available' page is captured
    Given I am a user who cannot view Record Sharing from Patients Know Best
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=http%3A%2F%2Fpkb.stubs.local.bitraft.io%3A8080%2Fnhs-login%2Flogin%3FphrPath%3D%2Fpatient%2FmyConsentTeam.action%3Ftab%3Dinvitations%26subTab%3DmyClinicians'
    Then the Supplier_Onboarding_Feature_Not_Available page is saved to disk
