@silverIntegration
@gncr
Feature: Great North Care Record Correspondence

  # P5 notes - the health record hub page is not available to P5 users, preventing them from accessing any silver integration test results jump offs.

  Scenario: A user navigates to GNCR correspondence and sees the warning message
    Given I am using the native app user agent
    And I am a user who can view Correspondence from GNCR
    And I am logged in
    When I navigate to the health record hub page
    Then I see the health records hub page
    And I click the menu item 'Hospital and other healthcare letters'
    And I am redirected to the redirector page with the header 'Hospital and other healthcare letters'
    And the warning message on the Redirector page explains the service is from GNCR

  Scenario: A user without access to GNCR cannot see the menu item 'Hospital and other healthcare letters' on the Health Record Hub page
    Given I am a user who cannot view Correspondence from GNCR
    And I am logged in
    When I navigate to the health record hub page
    Then I see the health records hub page
    And the link to GNCR 'Hospital and other healthcare letters' is not available on the Health Records Hub page

  Scenario: A user can follow the link to Find out more about hospital and other healthcare letters
    Given I am a EMIS patient
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=https%3A%2F%2Fnhs-pep-glenr.enigmadev2.net%2Fcorrespondence'
    Then I am redirected to the redirector page with the header 'Hospital and other healthcare letters'
    When I click the link called 'Find out more about personal health record services' with a url of 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/personal-health-records/'
    Then a new tab has been opened by the link

  Scenario: A user navigates to an external partner site and will see a warning page
    Given I am a EMIS patient
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=https%3A%2F%2Fnhs-pep-glenr.enigmadev2.net%2Fcorrespondence'
    Then I am redirected to the redirector page with the header 'Hospital and other healthcare letters'
    When I click the 'Continue' button on the redirector page with a url starting with 'https://nhs-pep-glenr.enigmadev2.net/correspondence'
    Then I am navigated to a third party site
