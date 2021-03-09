@silverIntegration
@patientsKnowBest
@sharedlinks
Feature: Patients Know Best Shared Links

  # P5 notes - the health record hub is not available to P5 users, preventing them from accessing any silver integration shared links jump offs.

  Scenario: A user navigates to PKB shared links and sees the warning message
    Given I am using the native app user agent
    And I am a user who can view Shared Links from Patients Know Best
    And I am logged in
    When I navigate to the Health Record Hub page
    Then I see the health records hub page
    And I click the menu item 'Shared links'
    And I am redirected to the redirector page with the header 'Shared links'
    And the shared links warning on the page explains the service is from Patients Know Best

  Scenario: A user without access to PKB cannot see the menu item 'Shared links' on the health record hub page
    Given I am a user who cannot view Shared Links from Patients Know Best
    And I am logged in
    When I navigate to the Health Record Hub page
    Then I see the health records hub page
    And the link to PKB shared links is not available on the health record hub page

  Scenario: A user can follow the link to Find out more about personal health records
    Given I am a user who can view Shared Links from Patients Know Best
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=http%3A%2F%2Fpkb.stubs.local.bitraft.io%3A8080%2Fnhs-login%2Flogin%3FphrPath%3D%2Flibrary%2FmanageLibrary.action'
    Then I am redirected to the redirector page with the header 'Shared links'
    When I click the link called 'Find out more about personal health record services' with a url of 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/personal-health-records/'
    Then a new tab has been opened by the link

  Scenario: A user navigates to an external partner site and will see a warning page
    Given I am a user who can view Shared Links from Patients Know Best
    And PKB responds to requests for shared links
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=http%3A%2F%2Fpkb.stubs.local.bitraft.io%3A8080%2Fnhs-login%2Flogin%3FphrPath%3D%2Flibrary%2FmanageLibrary.action'
    Then I am redirected to the redirector page with the header 'Shared links'
    When I click the 'Continue' button on the redirector page with a url starting with 'http://pkb.stubs.local.bitraft.io:8080/nhs-login/login?phrPath=/library/manageLibrary.action'
    Then I am navigated to a third party site for PKB
