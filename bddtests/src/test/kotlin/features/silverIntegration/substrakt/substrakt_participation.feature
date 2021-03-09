@silverIntegration
@substrakt
@substrakt-participation
Feature: Substrakt Patient Participation Groups

  Scenario: A user with proof level 5 cannot see the menu item 'Patient participation groups' on the account page
    Given I am a user with proof level 5 who can view Participation groups from Substrakt
    And I am logged in
    And I navigate to the More page
    Then the More page is displayed
    And the link to Substrakt 'Patient participation groups' is not available on the Account page

  Scenario: A user navigates to Substrakt patient participation group and sees the warning message
    Given I am using the native app user agent
    And I am a user who can view Patient participation groups from Substrakt
    And I am logged in
    When I navigate to the More page
    Then the More Settings links are available
    And I click the account menu item 'Patient participation groups'
    And I am redirected to the redirector page with the header 'Patient participation groups'
    And the participation warning message on the Redirector page explains the service is from Substrakt

  Scenario: A user without access to Substrakt cannot see the menu item 'Patient participation groups' on the Account page
    Given I am a user who cannot view Patient participation groups from Substrakt
    And I am logged in
    When I navigate to the More page
    Then the More page is displayed
    And the link to Substrakt 'Patient participation groups' is not available on the Account page

  Scenario: A user can follow the link to Find out more about personal health records
    Given I am a user who can view Patient participation groups from Substrakt
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=http%3A%2F%2Fsubstrakt.stubs.local.bitraft.io%3A8080%2Fjump%2Fjoin-ppg'
    Then I am redirected to the redirector page with the header 'Patient participation groups'
    When I click the link called 'Find out more about personal health record services' with a url of 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/personal-health-records/'
    Then a new tab has been opened by the link

  Scenario: A user navigates to an external partner site and will see a warning page
    Given I am a user who can view Patient participation groups from Substrakt
    And Substrakt responds to requests for participation
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=http%3A%2F%2Fsubstrakt.stubs.local.bitraft.io%3A8080%2Fjump%2Fjoin-ppg'
    Then I am redirected to the redirector page with the header 'Patient participation groups'
    When I click the 'Continue' button on the redirector page with a url starting with 'http://substrakt.stubs.local.bitraft.io:8080/jump/join-ppg'
    Then I am navigated to a third party site for Substrakt
