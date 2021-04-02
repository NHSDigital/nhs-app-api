@silverIntegration
@patientsKnowBest
@sharedlinks
Feature: Patients Know Best Secondary Care Record Sharing

  # P5 notes - the health record hub is not available to P5 users, preventing them from accessing any silver integration shared links jump offs.

  Scenario: A user navigates to PKB Secondary Care Record Sharing and sees the warning message
    Given I am using the native app user agent
    And I am a user who can view Record Sharing from PKB Secondary Care
    And I am logged in
    When I navigate to the Health Record Hub page
    Then I see the health records hub page
    And I click the menu item 'Record sharing'
    And I am redirected to the redirector page with the header 'Record Sharing'
    And the record sharing warning on the page explains the service is from PKB Secondary Care

  Scenario: A user without access to PKB Secondary Care cannot see the menu item 'Record Sharing' on the health record hub page
    Given I am a user who cannot view Record Sharing from PKB Secondary Care
    And I am logged in
    When I navigate to the Health Record Hub page
    Then I see the health records hub page
    And the link to PKB Secondary Care record sharing is not available on the Health Records Hub

  Scenario: A user navigates to an external partner site and will see a warning page
    Given I am a user who can view Record Sharing from PKB Secondary Care
    And Secondary Care responds to requests for record sharing
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=http%3A%2F%2Fpkb.stubs.local.bitraft.io%3A8080%2Fnhs-login%2Flogin%3FphrPath%3D%2Fpatient%2FmyConsentTeam.action%3Ftab%3Dinvitations%26subTab%3DmyClinicians%26brand%3DpkbSecondaryCare'
    Then I am redirected to the redirector page with the header 'Record Sharing'
    When I click the 'Continue' button on the redirector page with a url starting with 'http://pkb.stubs.local.bitraft.io:8080/nhs-login/login?phrPath=/patient/myConsentTeam.action?tab=invitations&subTab=myClinicians&brand=pkbSecondaryCare'
    Then I am navigated to a third party site for Secondary Care

  Scenario: A user can follow the link to Find out more about personal health records
    Given I am a user who can view Record Sharing from PKB Secondary Care
    And 'NHS UK' responds to requests for '/health-records'
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=http%3A%2F%2Fpkb.stubs.local.bitraft.io%3A8080%2Fnhs-login%2Flogin%3FphrPath%3D%2Fpatient%2FmyConsentTeam.action%3Ftab%3Dinvitations%26subTab%3DmyClinicians%26brand%3DpkbSecondaryCare'
    Then I am redirected to the redirector page with the header 'Record Sharing'
    When I click the link called 'Find out more about personal health record services' with a url of 'http://stubs.local.bitraft.io:8080/external/nhsuk/health-records'
    Then a new tab has been opened by the link

  Scenario: A user who cannot see PKB record sharing but tries to access it is redirected
    Given I am a user who cannot view Record Sharing from PKB Secondary Care
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=http%3A%2F%2Fpkb.stubs.local.bitraft.io%3A8080%2Fnhs-login%2Flogin%3FphrPath%3D%2Fpatient%2FmyConsentTeam.action%3Ftab%3Dinvitations%26subTab%3DmyClinicians%26brand%3DpkbSecondaryCare'
    Then I see silver integration error page loaded with title Record sharing
    When I select the Go to NHS App homepage link from the feature not available page
    Then I see the home page header
