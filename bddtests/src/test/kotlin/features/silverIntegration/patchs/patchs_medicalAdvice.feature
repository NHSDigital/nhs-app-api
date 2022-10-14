@silverIntegration
@patchs

Feature: Patchs Medical Advice

  Scenario: A user navigates to Patchs Medical Advice and sees the warning message
    Given I am using the native app user agent
    And I am a user who can view Medical Advice from Patchs
    And I am logged in
    When I navigate to the advice page
    And the link to Patchs Medical Advice is available on the Advice page
    And I click the Patchs Medical Advice link on the Advice page
    And I am redirected to the redirector page with the header 'Ask your GP for advice about a health problem'
    And the Medical Advice warning message on the Redirector page explains the service is from Patchs

  Scenario: A user without access to Patchs cannot see the menu item 'Medical Advice' on the Advice page
    Given I am a user who cannot view Medical Advice from Patchs
    And I am logged in
    And I navigate to the advice page
    And the link to Patchs Medical Advice is not available on the Advice page

  Scenario: A user navigates directly to the Patchs external partner site and will see a warning page
    Given I am a user who can view Medical Advice from Patchs
    And Patchs responds to requests for Medical Advice
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=http%3A%2F%2Fpatchs.stubs.local.bitraft.io%3A8080%2Fnhs-app-auth%2Fsubmit-clinical-request'
    Then I am redirected to the redirector page with the header 'Ask your GP for advice about a health problem'
    When I click the 'Continue' button on the redirector page with a url starting with 'http://patchs.stubs.local.bitraft.io:8080/nhs-app-auth/submit-clinical-request'
    Then I am navigated to a third party site for Patchs
