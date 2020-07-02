@silverIntegration
@careInformationExchange
Feature: Care Information Exchange Shared Links

  # P5 notes - the more page is not available to P5 users, preventing them from accessing any silver integration shared links jump offs.

  Scenario: A user navigates to CIE shared links and sees the warning message
    Given I am using the native app user agent
    And I am a user who can view Shared Links from Care Information Exchange
    And I am logged in
    When I navigate to the More page
    Then I am on the More Page
    And the link to Shared links is available on the More page
    And I click the Shared links link on the More page
    And I am redirected to the redirector page with the header 'Shared links'
    And the warning message on the Redirector page explains the service is from Care Information Exchange

  Scenario: A user without access to CIE cannot see the menu item 'Shared links' on the more page
    Given I am a user who cannot view Shared Links from Care Information Exchange
    And I am logged in
    And I navigate to the More page
    Then I am on the More Page
    And the link to Shared links is not available on the More page

  Scenario: A user can follow the link to Find out more about personal health records
    Given I am a user who can view Shared Links from Care Information Exchange
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=https%3A%2F%2Fnhsapp-test.devstacks.pkb.io%2Fnhs-login%2Flogin%3FphrPath%3D%252Flibrary%252FmanageLibrary.action%26brand=cie'
    Then I am redirected to the redirector page with the header 'Shared links'
    When I click the link called 'Find out more about personal health record services.' with a url of 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/personal-health-records/'
    Then a new tab has been opened by the link

  Scenario: A user navigates to an external partner site and will see a warning page
    Given I am a EMIS patient
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=https%3A%2F%2Fnhsapp-test.devstacks.pkb.io%2Fnhs-login%2Flogin%3FphrPath%3D%252Flibrary%252FmanageLibrary.action%26brand=cie'
    Then I am redirected to the redirector page with the header 'Shared links'
    When I click the 'Continue' button on the redirector page with a url starting with 'https://nhsapp-test.devstacks.pkb.io/nhs-login/login?phrPath=%2Flibrary%2FmanageLibrary.action&brand=cie'
    Then I am navigated to a third party site