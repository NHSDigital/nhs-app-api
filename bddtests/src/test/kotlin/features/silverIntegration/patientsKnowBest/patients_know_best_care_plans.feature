@silverIntegration
@patientsKnowBest
Feature: Patients Know Best Care Plans

  # P5 notes - the health record hub page is not available to P5 users, preventing them from accessing any silver integration care plans jump offs.

  Scenario: A user navigates to PKB care plans aGncrStepDefinitions.ktnd sees the warning message
    Given I am using the native app user agent
    And I am a user who can view care plans from Patients Know Best
    And I am logged in
    When I navigate to the health record hub page
    Then I see the health records hub page
    And I click the menu item 'Care plans'
    And I am redirected to the redirector page with the header 'Care plans'
    And the care plans warning message on the Redirector page explains the service is from Patients Know Best

  Scenario: A user without access to PKB cannot see the menu item 'Care plans' on the Health Record Hub page
    Given I am a user who cannot view care plans from Patients Know Best
    And I am logged in
    When I navigate to the health record hub page
    Then I see the health records hub page
    And the link to PKB Care plans is not available on the health record hub page

  Scenario: A user can follow the link to Find out more about personal health records
    Given I am a user who can view care plans from Patients Know Best
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=http%3A%2F%2Fpkb.stubs.local.bitraft.io%3A8080%2Fnhs-login%2Flogin%3FphrPath%3D%2Fauth%2FlistPlans.action'
    Then I am redirected to the redirector page with the header 'Care plans'
    When I click the link called 'Find out more about personal health record services' with a url of 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/personal-health-records/'
    Then a new tab has been opened by the link

  Scenario: A user navigates to an external partner site and will see a warning page
    Given I am a user who can view care plans from Patients Know Best
    And PKB responds to requests for care plans
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=http%3A%2F%2Fpkb.stubs.local.bitraft.io%3A8080%2Fnhs-login%2Flogin%3FphrPath%3D%2Fauth%2FlistPlans.action'
    Then I am redirected to the redirector page with the header 'Care plans'
    When I click the 'Continue' button on the redirector page with a url starting with 'http://pkb.stubs.local.bitraft.io:8080/nhs-login/login?phrPath=/auth/listPlans.action'
    Then I am navigated to a third party site for PKB

