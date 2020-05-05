@silverIntegration
@patientsKnowBest
Feature: Patients Know Best Appointments

  Scenario: A user navigates to PKB appointments and sees the warning message
    Given I am using the native app user agent
    And I am a user who can view Appointments from Patients Know Best
    And I am logged in
    And I navigate to the hospital and other appointments page
    Then the Hospital Appointments page is displayed
    And I can see the PKB View Appointments link on the Appointments page
    When I click the PKB View Appointments link on the Appointments page
    Then I am redirected to the redirector page with the header 'View appointments'
    And the warning message on the Redirector page explains the service is from Patients Know Best

  Scenario: A user without access to PKB cannot see the menu item 'Appointments' on the appointments page
    Given I am using the native app user agent
    And I am a user who cannot view Appointments from Patients Know Best
    And I am logged in
    Then I see the home page
    Given I navigate to the hospital and other appointments page
    Then the Hospital Appointments page is displayed
    And the link to PKB View Appointments is not available on the Appointments page

  Scenario: The menu item 'View Appointments' is not visible on desktop
    Given I am a user who can view Appointments from Patients Know Best
    And I am logged in
    Then I see the home page
    Given I navigate to the hospital and other appointments page
    Then the Hospital Appointments page is displayed
    And the link to PKB View Appointments is not available on the Appointments page

  Scenario: A user can follow the link to Find out more about personal health records
    Given I am a EMIS patient
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=https%3A%2F%2Fnhsapp-test.devstacks.pkb.io%2Fnhs-login%2Flogin%3FphrPath%3D%252Fdiary%252FlistAppointments.action'
    Then I am redirected to the redirector page with the header 'View appointments'
    When I click the link called 'Find out more about personal health record services.' with a url of 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/personal-health-records/'
    Then a new tab has been opened by the link

  Scenario: A user navigates to an external partner site and will see a warning page
    Given I am a EMIS patient
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=https%3A%2F%2Fnhsapp-test.devstacks.pkb.io%2Fnhs-login%2Flogin%3FphrPath%3D%252Fdiary%252FlistAppointments.action'
    Then I am redirected to the redirector page with the header 'View appointments'
    When I click the 'Continue' button on the redirector page with a url starting with 'https://nhsapp-test.devstacks.pkb.io/nhs-login/login?phrPath=%2Fdiary%2FlistAppointments.action&assertedLoginIdentity='
    Then I am navigated to a third party site
