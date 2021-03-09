@silverIntegration
@careInformationExchange
@sharedlinks
Feature: Care Information Exchange Shared Links

  # P5 notes - the health record hub page is not available to P5 users, preventing them from accessing any silver integration test results jump offs.

  Scenario: A user navigates to CIE shared health links and sees the warning message
    Given I am using the native app user agent
    And I am a user who can view Shared Health Links from Care Information Exchange
    And I am logged in
    When I navigate to the Health Record Hub page
    Then I see the health records hub page
    And I click the menu item 'Shared health links'
    And I am redirected to the redirector page with the header 'Shared health links'
    And the shared health warning on the page explains the service is from Care Information Exchange

  Scenario: A user without access to CIE cannot see the menu item 'Shared health links' on the health record hub page
    Given I am a user who cannot view Shared Health Links from Care Information Exchange
    And I am logged in
    When I navigate to the Health Record Hub page
    Then I see the health records hub page
    And the link to CIE shared health links is not available on the health record hub page

  Scenario: A user can follow the link to Find out more about personal health records
    Given I am a user who can view Shared Health Links from Care Information Exchange
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=http%3A%2F%2Fpkb.stubs.local.bitraft.io%3A8080%2Fnhs-login%2Flogin%3FphrPath%3D%2Flibrary%2FmanageLibrary.action%26brand=cie'
    Then I am redirected to the redirector page with the header 'Shared health links'
    When I click the link called 'Find out more about personal health record services' with a url of 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/personal-health-records/'
    Then a new tab has been opened by the link

  Scenario: A user navigates directly to an external partner site and will see a warning page
    Given I am a user who can view Shared Health Links from Care Information Exchange
    And CIE responds to requests for shared links
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=http%3A%2F%2Fpkb.stubs.local.bitraft.io%3A8080%2Fnhs-login%2Flogin%3FphrPath%3D%2Flibrary%2FmanageLibrary.action%26brand=cie'
    Then I am redirected to the redirector page with the header 'Shared health links'
    When I click the 'Continue' button on the redirector page with a url starting with 'http://pkb.stubs.local.bitraft.io:8080/nhs-login/login?phrPath=/library/manageLibrary.action&brand=cie'
    Then I am navigated to a third party site for CIE
