@silverIntegration
@substrakt
Feature: Substrakt Messages

  # P5 notes - P5 users are able to access the messaging hub page directly, but should not see any silver integration messaging jump offs

  Scenario: A user with proof level 5 cannot see the menu item 'Ask your GP Surgery a Question' on the more page
    Given I am a user with proof level 5 who can view Ask Your Gp Surgery a Question from Substrakt
    And I am logged in
    And I navigate to the Messages hub page
    Then the Messages Hub page is displayed
    And the link to Ask your GP surgery a Question is not available on the Messages Hub page

  Scenario: A user navigates to Substrakt messages and sees the warning message
    Given I am using the native app user agent
    And I am a user who can view Ask Your Gp Surgery a Question from Substrakt
    And I am logged in
    When I navigate to the More page
    Then I am on the More Page
    And I click the Messages link on the More page
    And the Messages Hub page is displayed
    And I click the Substrakt Ask Your GP Surgery a Question link on the Messages Hub page
    And I am redirected to the redirector page with the header 'Ask your GP surgery a question'
    And the warning message on the Redirector page explains the service is from Substrakt

  Scenario: A user without access to Substrakt cannot see the menu item 'Ask Your Gp Surgery a Question' on the messages page
    Given I am a user who cannot view Ask Your Gp Surgery a Question from Substrakt
    And I am logged in
    And I navigate to the Messages hub page
    And the Messages Hub page is displayed
    And the link to Ask your GP surgery a Question is not available on the Messages Hub page

  Scenario: A user can follow the link to Find out more about personal health records
    Given I am a user who can view Ask Your Gp Surgery a Question from Substrakt
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=https%3A%2F%2Fjump-point.test.substrakthealth.com%2Fjump%2Fforms'
    Then I am redirected to the redirector page with the header 'Ask your GP surgery a question'
    When I click the link called 'Find out more about personal health record services' with a url of 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/personal-health-records/'
    Then a new tab has been opened by the link

  Scenario: A user navigates to an external partner site and will see a warning page
    Given I am a user who can view Ask Your Gp Surgery a Question from Substrakt
    And I am logged in
    When I navigate to the redirector page with a url of 'redirector?redirect_to=https%3A%2F%2Fjump-point.test.substrakthealth.com%2Fjump%2Fforms'
    Then I am redirected to the redirector page with the header 'Ask your GP surgery a question'
    When I click the 'Continue' button on the redirector page with a url starting with 'https://jump-point.test.substrakthealth.com/jump/forms'
    Then I am navigated to a third party site
