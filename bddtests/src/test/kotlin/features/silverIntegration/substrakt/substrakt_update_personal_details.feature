@silverIntegration
@substrakt
@substrakt-update-details
Feature: Substrakt Update Personal Details

  # P5 notes - the health record hub page is not available to P5 users, preventing them from accessing any silver integration test results jump offs.

  Scenario: A user navigates to Substrakt update personal details and sees the warning message
    Given I am using the native app user agent
    And I am a user who can view Update your personal details from Substrakt
    And I am logged in
    When I navigate to the health record hub page
    Then I see the health records hub page
    And I click the menu item 'Update your personal details'
    And I am redirected to the redirector page with the header 'Update your personal details'
    And the warning message on the Redirector page explains the service is from Substrakt

  Scenario: A user without access to Substrakt cannot see the menu item 'Update your personal details' on the Health Record Hub page
    Given I am a user who cannot view Update your personal details from Substrakt
    And I am logged in
    When I navigate to the health record hub page
    Then I see the health records hub page
    And the link to Substrakt 'Update your personal details' is not available on the Health Records Hub page

  Scenario: A user can follow the link to Find out more about personal health records
    Given I am a user who can view Ask Your Gp Surgery a Question from Substrakt
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=https%3A%2F%2Fjump-point.test.substrakthealth.com%2Fjump%2Fupdate-details'
    Then I am redirected to the redirector page with the header 'Update your personal details'
    When I click the link called 'Find out more about personal health record services' with a url of 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/personal-health-records/'
    Then a new tab has been opened by the link

  Scenario: A user navigates to an external partner site and will see a warning page
    Given I am a user who can view Ask Your Gp Surgery a Question from Substrakt
    And I am logged in
    When I navigate to the redirector page with a url of 'redirector?redirect_to=https%3A%2F%2Fjump-point.test.substrakthealth.com%2Fjump%2Fupdate-details'
    Then I am redirected to the redirector page with the header 'Update your personal details'
    When I click the 'Continue' button on the redirector page with a url starting with 'https://jump-point.test.substrakthealth.com/jump/update-details'
    Then I am navigated to a third party site
