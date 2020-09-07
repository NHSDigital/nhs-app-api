@silverIntegration
@patientsKnowBest
Feature: Patients Know Best Test Results

  # P5 notes - the health record hub page is not available to P5 users, preventing them from accessing any silver integration test results jump offs.

  Scenario: A user navigates to PKB test results and sees the warning message
    Given I am using the native app user agent
    And I am a user who can view test results from Patients Know Best
    And I am logged in
    When I navigate to the health record hub page
    Then I see the health records hub page
    And I click the menu item 'Test results'
    And I am redirected to the redirector page with the header 'Test results'
    And the warning message on the Redirector page explains the service is from Patients Know Best

  Scenario: A user without access to PKB cannot see the menu item 'Test results' on the Health Record Hub page
    Given I am a user who cannot view test results from Patients Know Best
    And I am logged in
    When I navigate to the health record hub page
    Then I see the health records hub page
    And the link to PKB test results is not available on the health record hub page

  Scenario: A user can follow the link to Find out more about personal health records
    Given I am a user who can view test results from Patients Know Best
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=https%3A%2F%2Fnhsapp-test.devstacks.pkb.io%2Fnhs-login%2Flogin%3FphrPath%3D%252Ftest%252FmyTests.action'
    Then I am redirected to the redirector page with the header 'Test results'
    When I click the link called 'Find out more about personal health record services' with a url of 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/personal-health-records/'
    Then a new tab has been opened by the link

  Scenario: A user navigates to an external partner site and will see a warning page
    Given I am a EMIS patient
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=https%3A%2F%2Fnhsapp-test.devstacks.pkb.io%2Fnhs-login%2Flogin%3FphrPath%3D%252Ftest%252FmyTests.action'
    Then I am redirected to the redirector page with the header 'Test results'
    When I click the 'Continue' button on the redirector page with a url starting with 'https://nhsapp-test.devstacks.pkb.io/nhs-login/login?phrPath=%2Ftest%2FmyTests.action'
    Then I am navigated to a third party site
