@silverIntegration
@careInformationExchange
Feature: Care Information Exchange Health Tracker

  Scenario: A user navigates to CIE health tracker and sees the warning message
    Given I am using the native app user agent
    And I am a user who can view health tracker from Care Information Exchange
    And I am logged in
    When I navigate to the health record hub page
    Then I see the health records hub page
    And I click the menu item 'Track your health'
    And I am redirected to the redirector page with the header 'Track your health'
    And the warning message on the Redirector page explains the service is from Care Information Exchange

  Scenario: A user without access to CIE cannot see the menu item 'Track your health' on the Health Record Hub
    Given I am using the native app user agent
    And I am a user who cannot view health tracker from Care Information Exchange
    And I am logged in
    When I navigate to the health record hub page
    Then I see the health records hub page
    And the link to CIE Track your health is not available on the health record hub page

  Scenario: The menu item 'Track your health' is not visible on desktop
    Given I am a user who can view health tracker from Care Information Exchange
    And I am logged in
    When I navigate to the health record hub page
    Then I see the health records hub page
    And I do not see the Third Party menu item 'Track your health'

  Scenario: A user can follow the link to Find out more about personal health records
    Given I am a user who can view health tracker from Care Information Exchange
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=https%3A%2F%2Fnhsapp-test.devstacks.pkb.io%2Fnhs-login%2Flogin%3FphrPath%3D%252FpkbNhsMenu.action%26brand=cie'
    Then I am redirected to the redirector page with the header 'Track your health'
    When I click the link called 'Find out more about personal health record services.' with a url of 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/personal-health-records/'
    Then a new tab has been opened by the link

  Scenario: A user navigates to an external partner site and will see a warning page
    Given I am a EMIS patient
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=https%3A%2F%2Fnhsapp-test.devstacks.pkb.io%2Fnhs-login%2Flogin%3FphrPath%3D%252FpkbNhsMenu.action%26brand=cie'
    Then I am redirected to the redirector page with the header 'Track your health'
    When I click the 'Continue' button on the redirector page with a url starting with 'https://nhsapp-test.devstacks.pkb.io/nhs-login/login?phrPath=%2FpkbNhsMenu.action&brand=cie'
    Then I am navigated to a third party site
