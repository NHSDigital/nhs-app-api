@silverIntegration
@accurx

Feature: accuRx Medical Advice

  Scenario: A user navigates to accuRx Medical Advice and sees the warning message
    Given I am using the native app user agent
    And I am a user who can view Medical Advice from accuRx
    And I am logged in
    When I navigate to the advice page
    And the link to accuRx Medical Advice is available on the Advice page
    And I click the accuRx Medical Advice link on the Advice page
    And I am redirected to the redirector page with the header 'Ask your GP for medical advice'
    And the Medical Advice warning message on the Redirector page explains the service is from accuRx

  Scenario: A user without access to accuRx cannot see the menu item 'Medical Advice' on the Advice page
    Given I am a user who cannot view Medical Advice from accuRx
    And I am logged in
    And I navigate to the advice page
    And the link to accuRx Medical Advice is not available on the Advice page

  Scenario: A user navigates directly to an external partner site and will see a warning page
    Given I am a user who can view Medical Advice from accuRx
    And accuRx responds to requests for Medical Advice
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=http%3A%2F%2Faccurx.stubs.local.bitraft.io%3A8080%2Fapi%2FOpenIdConnect%2FAuthenticatePatientTriage%3FrequestType%3Dmedical'
    Then I am redirected to the redirector page with the header 'Ask your GP for medical advice'
    When I click the 'Continue' button on the redirector page with a url starting with 'http://accurx.stubs.local.bitraft.io:8080/api/OpenIdConnect/AuthenticatePatientTriage?requestType=medical'
    Then I am navigated to a third party site for accuRx
