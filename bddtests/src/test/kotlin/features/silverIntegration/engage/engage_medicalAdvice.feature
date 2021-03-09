@silverIntegration
@engage

Feature: Engage Medical Advice

  Scenario: A user navigates to Engage medical advice and sees the warning message
    Given I am using the native app user agent
    And I am a user who can view Medical Advice from Engage
    And I am logged in
    When I navigate to the advice page
    And the link to Engage Medical Advice is available on the Advice page
    And I click the Engage Medical Advice link on the Advice page
    And I am redirected to the redirector page with the header 'Ask your GP for advice'
    And the advice warning message on the Redirector page explains the service is from Engage

  Scenario: A user without access to Engage cannot see the menu item 'Medical Advice' on the Advice page
    Given I am a user who cannot view Medical Advice from Engage
    And I am logged in
    And I navigate to the advice page
    And the link to Engage Medical Advice is not available on the Advice page

  Scenario: A user navigates directly to an external partner site and will see a warning page
    Given I am a user who can view Medical Advice from Engage
    And Engage responds to requests for medical advice
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=http%3A%2F%2Fengage.stubs.local.bitraft.io%3A8080%2F%3Fsso_route%3Dmedical'
    Then I am redirected to the redirector page with the header 'Ask your GP for advice'
    When I click the 'Continue' button on the redirector page with a url starting with 'http://engage.stubs.local.bitraft.io:8080/?sso_route=medical'
    Then I am navigated to a third party site for Engage
