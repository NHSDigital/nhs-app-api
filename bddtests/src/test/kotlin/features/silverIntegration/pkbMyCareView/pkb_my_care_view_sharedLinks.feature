@silverIntegration
@pkbMyCareView
@sharedlinks
Feature: Patients Know Best My Care View Shared Links

  # P5 notes - the health record hub is not available to P5 users, preventing them from accessing any silver integration shared links jump offs.

  Scenario: A user navigates to PKB My Care View shared links and sees the warning message
    Given I am using the native app user agent
    And I am a user who can view Shared Links from PKB My Care View
    And I am logged in
    When I navigate to the Health Record Hub page
    Then I see the health records hub page
    And I click the menu item 'Shared health links'
    And I am redirected to the redirector page with the header 'Shared health links'
    And the shared health links warning on the page explains the service is from PKB My Care View

  Scenario: A user without access to PKB My Care View cannot see the menu item 'Shared links' on the health record hub page
    Given I am a user who cannot view Shared Links from PKB My Care View
    And I am logged in
    When I navigate to the Health Record Hub page
    Then I see the health records hub page
    And the link to PKB My Care View shared links is not available on the Health Records Hub

  Scenario: A user can follow the link to Find out more about personal health records
    Given I am a user who can view Shared Links from PKB My Care View
    And 'NHS UK' responds to requests for '/health-records'
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=http%3A%2F%2Fpkb.stubs.local.bitraft.io%3A8080%2Fnhs-login%2Flogin%3FphrPath%3D%2Flibrary%2FmanageLibrary.action%26brand=pkbMyCareView'
    Then I am redirected to the redirector page with the header 'Shared health links'
    When I click the link called 'Find out more about personal health record services' with a url of 'http://stubs.local.bitraft.io:8080/external/nhsuk/health-records'
    Then a new tab has been opened by the link

  Scenario: A user navigates to an external partner site and will see a warning page
    Given I am a user who can view Shared Links from PKB My Care View
    And My Care View responds to requests for shared links
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=http%3A%2F%2Fpkb.stubs.local.bitraft.io%3A8080%2Fnhs-login%2Flogin%3FphrPath%3D%2Flibrary%2FmanageLibrary.action%26brand=pkbMyCareView'
    Then I am redirected to the redirector page with the header 'Shared health links'
    When I click the 'Continue' button on the redirector page with a url starting with 'http://pkb.stubs.local.bitraft.io:8080/nhs-login/login?phrPath=/library/manageLibrary.action&brand=pkbMyCareView'
    Then I am navigated to a third party site for My Care View
