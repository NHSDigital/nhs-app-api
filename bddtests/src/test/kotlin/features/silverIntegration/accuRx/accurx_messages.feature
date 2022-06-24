@silverIntegration
@accurx
Feature: Accurx Messages

  # P5 notes - P5 users are able to access the messaging hub page directly, but should not see any silver integration messaging jump offs

  Scenario: A user with proof level 5 cannot see the menu item 'Ask your GP Surgery a Question' on the messages hub
    Given I am a user with proof level 5 who can view Ask Your Gp Surgery a Question from Accurx
    And I am logged in
    And I navigate to the Messages hub page
    Then the Messages Hub page is displayed
    And the link to Ask your GP surgery a Question is not available on the Messages Hub page

  Scenario: A user navigates to Accurx messages and sees the warning message
    Given I am using the native app user agent
    And I am a user who can view Ask Your Gp Surgery a Question from Accurx
    And I am logged in
    When I navigate to the Messages Hub page
    Then the Messages Hub page is displayed
    When I click the Accurx Ask Your GP Surgery a Question link on the Messages Hub page
    Then I am redirected to the redirector page with the header 'Ask your GP surgery a question'
    And the question warning message on the Redirector page explains the service is from Accurx

  Scenario: A user without access to Accurx cannot see the menu item 'Ask Your Gp Surgery a Question' on the messages hub
    Given I am a user who cannot view Ask Your Gp Surgery a Question from Accurx
    And I am logged in
    And I navigate to the Messages hub page
    And the Messages Hub page is displayed
    And the link to Ask your GP surgery a Question is not available on the Messages Hub page

  Scenario: A user can follow the link to Find out more about personal health records
    Given I am a user who can view Ask Your Gp Surgery a Question from Accurx
    And 'NHS UK' responds to requests for '/online-consultations'
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=http%3A%2F%2Faccurx.stubs.local.bitraft.io%3A8080%2Fapi%2FOpenIdConnect%2FAuthenticatePatientTriage%3FrequestType%3Dadmin'
    Then I am redirected to the redirector page with the header 'Ask your GP surgery a question'
    When I click the link called 'Find out more about online consultation services' with a url of 'http://stubs.local.bitraft.io:8080/external/nhsuk/online-consultations'
    Then a new tab has been opened by the link

  Scenario: A user navigates to an external partner site and will see a warning page
    Given I am a user who can view Ask Your Gp Surgery a Question from Accurx
    And Accurx responds to requests for messages
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=http%3A%2F%2Faccurx.stubs.local.bitraft.io%3A8080%2Fapi%2FOpenIdConnect%2FAuthenticatePatientTriage%3FrequestType%3Dadmin'
    Then I am redirected to the redirector page with the header 'Ask your GP surgery a question'
    When I click the 'Continue' button on the redirector page with a url starting with 'http://accurx.stubs.local.bitraft.io:8080/api/OpenIdConnect/AuthenticatePatientTriage?requestType=admin'
    Then I am navigated to a third party site for Accurx
