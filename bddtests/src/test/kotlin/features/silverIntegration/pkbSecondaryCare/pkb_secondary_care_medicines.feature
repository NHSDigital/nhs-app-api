@silverIntegration
@pkbSecondaryCare
Feature: Patients Know Best Secondary Care Medicines

  # P5 notes - the prescriptions hub page is not available to P5 users, preventing them from accessing any silver integration medicines jump offs.

  Scenario: A user navigates to PKB Secondary Care Medicines and sees the warning message
    Given I am using the native app user agent
    And I am a user who can view Medicines from PKB Secondary Care
    And I am logged in
    And I navigate to the your prescriptions page
    Then the Prescriptions Hub page is displayed
    And the PKB Secondary Care View Medicines link is available on the Prescriptions Hub
    When I click the PKB Secondary Care View Medicines link on the Prescriptions hub
    Then I am redirected to the redirector page with the header 'Hospital and other medicines'
    And the hospital and medicines warning on the page explains the service is from PKB Secondary Care

  Scenario: The menu item 'Hospital and other prescriptions' is visible on desktop
    Given I am a user who can view Medicines from PKB Secondary Care
    And I am logged in
    Then I see the home page
    Given I navigate to the your prescriptions page
    Then the Prescriptions Hub page is displayed
    And the PKB Secondary Care View Medicines link is available on the Prescriptions Hub

  Scenario: A user navigates directly to an external partner site and will see a warning page
    Given I am a user who can view Medicines from PKB Secondary Care
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=https%3A%2F%2Fnhsapp-test.devstacks.pkb.io%2Fnhs-login%2Flogin%3FphrPath%3D%252Fauth%252FmanageMedications.action%253Ftab%253Dtreatments%26brand=pkbSecondaryCare'
    Then I am redirected to the redirector page with the header 'Hospital and other medicines'
    When I click the 'Continue' button on the redirector page with a url starting with 'https://nhsapp-test.devstacks.pkb.io/nhs-login/login?phrPath=%2Fauth%2FmanageMedications.action%3Ftab%3Dtreatments&brand=pkbSecondaryCare'
    Then I am navigated to a third party site

  Scenario: A user can follow the link to Find out more about personal health records
    Given I am a user who can view Medicines from PKB Secondary Care
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=https%3A%2F%2Fnhsapp-test.devstacks.pkb.io%2Fnhs-login%2Flogin%3FphrPath%3D%252Fauth%252FmanageMedications.action%253Ftab%253Dtreatments%26brand=pkbSecondaryCare'
    Then I am redirected to the redirector page with the header 'Hospital and other medicines'
    When I click the link called 'Find out more about personal health record services' with a url of 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/personal-health-records/'
    Then a new tab has been opened by the link

  Scenario: A user who cannot see PKB Secondary Care medications but tries to access it is redirected
    Given I am a user who cannot view Medicines from PKB Secondary Care
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=https%3A%2F%2Fnhsapp-test.devstacks.pkb.io%2Fnhs-login%2Flogin%3FphrPath%3D%252Fauth%252FmanageMedications.action%253Ftab%253Dtreatments%26brand=pkbSecondaryCare'
    Then I see silver integration error page loaded with title Hospital and other medicines
    When I select the Go to NHS App homepage link from the feature not available page
    Then I see the home page header
