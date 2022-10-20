@silverIntegration
@patchs
Feature: Patchs Admin on Messages Hub

  # P5 notes - P5 users are able to access the messaging hub page directly, but should not see any silver integration messaging jump offs

  Scenario: A user with proof level 5 cannot see the menu item 'Fit notes request' on the messages hub
    Given I am a user with proof level 5 who can view Fit Notes Request from Patchs
    And I am logged in
    And I navigate to the Messages hub page
    Then the Messages Hub page is displayed
    And the link to Fit Notes Request is not available on the Messages Hub page

  Scenario: A user navigates to Patchs Fit Notes Request and sees the warning message
    Given I am using the native app user agent
    And I am a user who can view Fit Notes Request from Patchs
    And I am logged in
    When I navigate to the Messages Hub page
    Then the Messages Hub page is displayed
    When I click the Patchs Fit Notes Request link on the Messages Hub page
    Then I am redirected to the redirector page with the header 'Ask for a fit note (sick note) or other documents'
    And the Fit Notes Request warning message on the Redirector page explains the service is from Patchs

  Scenario: A user without access to Patchs cannot see the menu item 'Fit Notes Request' on the messages hub
    Given I am a user who cannot view Fit Notes Request from Patchs
    And I am logged in
    And I navigate to the Messages hub page
    And the Messages Hub page is displayed
    And the link to Fit Notes Request is not available on the Messages Hub page

  Scenario: A user can follow the link to find out more about personal health records
    Given I am a user who can view Fit Notes Request from Patchs
    And 'NHS UK' responds to requests for '/personal-health-records'
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=http%3A%2F%2Fpatchs.stubs.local.bitraft.io%3A8080%2Fnhs-app-auth%2Fadmin-hub'
    Then I am redirected to the redirector page with the header 'Ask for a fit note (sick note) or other documents'
    When I click the link called 'Find out more about personal health record services' with a url of 'http://stubs.local.bitraft.io:8080/external/nhsuk/personal-health-records'
    Then a new tab has been opened by the link

  Scenario: A user navigates to an external partner site and will see a warning page
    Given I am a user who can view Fit Notes Request from Patchs
    And Patchs responds to requests for Fit Notes Request
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=http%3A%2F%2Fpatchs.stubs.local.bitraft.io%3A8080%2Fnhs-app-auth%2Fadmin-hub'
    Then I am redirected to the redirector page with the header 'Ask for a fit note (sick note) or other documents'
    When I click the 'Continue' button on the redirector page with a url starting with 'http://patchs.stubs.local.bitraft.io:8080/nhs-app-auth/admin-hub'
    Then I am navigated to a third party site for Patchs
