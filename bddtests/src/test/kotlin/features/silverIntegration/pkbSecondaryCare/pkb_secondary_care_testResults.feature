@silverIntegration
@pkbSecondaryCare
Feature: Patients Know Best Secondary Care Test Results and Imaging

  # P5 notes - the health record hub page is not available to P5 users, preventing them from accessing any silver integration test results jump offs.

  Scenario: A user navigates to PKB Secondary Care test results and imaging and sees the warning message
    Given I am using the native app user agent
    And I am a user who can view test results and imaging from PKB Secondary Care
    And I am logged in
    When I navigate to the health record hub page
    Then I see the health records hub page
    And I click the menu item 'Test results and imaging'
    And I am redirected to the redirector page with the header 'Test results and imaging'
    And the test results and imaging warning on the page explains the service is from PKB Secondary Care

  Scenario: A user without access to PKB Secondary Care cannot see the menu item 'Test results and imaging' on the Health Record Hub page
    Given I am a user who cannot view test results and imaging from PKB Secondary Care
    And I am logged in
    When I navigate to the health record hub page
    Then I see the health records hub page
    And the link to PKB Secondary Care test results and imaging is not available on the Health Records Hub

  Scenario: A user can follow the link to Find out more about personal health records
    Given I am a user who can view test results and imaging from PKB Secondary Care
    And 'NHS UK' responds to requests for '/health-records'
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=http%3A%2F%2Fpkb.stubs.local.bitraft.io%3A8080%2Fnhs-login%2Flogin%3FphrPath%3D%2FpkbNhsResultsMenu.action%26brand=pkbSecondaryCare'
    Then I am redirected to the redirector page with the header 'Test results and imaging'
    When I click the link called 'Find out more about personal health record services' with a url of 'http://stubs.local.bitraft.io:8080/external/nhsuk/personal-health-records'
    Then a new tab has been opened by the link

  Scenario: A user navigates to directly an external partner site and will see a warning page
    Given I am a user who can view test results and imaging from PKB Secondary Care
    And Secondary Care responds to requests for test results and imaging
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=http%3A%2F%2Fpkb.stubs.local.bitraft.io%3A8080%2Fnhs-login%2Flogin%3FphrPath%3D%2FpkbNhsResultsMenu.action%26brand=pkbSecondaryCare'
    Then I am redirected to the redirector page with the header 'Test results and imaging'
    When I click the 'Continue' button on the redirector page with a url starting with 'http://pkb.stubs.local.bitraft.io:8080/nhs-login/login?phrPath=/pkbNhsResultsMenu.action&brand=pkbSecondaryCare'
    Then I am navigated to a third party site for Secondary Care
