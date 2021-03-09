@silverIntegration
@gncr
Feature: Great North Care Record Appointments

  Scenario: A user navigates to GNCR appointments and sees the warning message
    Given I am using the native app user agent
    And I am a user who can view Appointments from GNCR
    And I am logged in
    And I navigate to the hospital and other appointments page
    Then the Hospital Appointments page is displayed
    And I can see the GNCR View Appointments link on the Appointments page
    When I click the GNCR View Appointments link on the Appointments page
    Then I am redirected to the redirector page with the header 'Hospital and other appointments'
    And the hospital and other warning message on the Redirector page explains the service is from GNCR

  Scenario: A user without access to GNCR cannot see the menu item 'Appointments' on the appointments page
    Given I am a user who cannot view Appointments from GNCR
    And I am logged in
    Then I see the home page
    Given I navigate to the hospital and other appointments page
    Then the Hospital Appointments page is displayed
    And the link to GNCR View Appointments is not available on the Appointments page

  Scenario: A user can follow the link to Find out more about personal health records
    Given I am a user who can view Appointments from GNCR
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=http%3A%2F%2Fgncr.stubs.local.bitraft.io%3A8080%2Fappointment'
    Then I am redirected to the redirector page with the header 'Hospital and other appointments'
    When I click the link called 'Find out more about personal health record services' with a url of 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/personal-health-records/'
    Then a new tab has been opened by the link

  Scenario: A user navigates directly to an external partner site and will see a warning page
    Given I am a user who can view Appointments from GNCR
    And GNCR responds to requests for appointments
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=http%3A%2F%2Fgncr.stubs.local.bitraft.io%3A8080%2Fappointment'
    Then I am redirected to the redirector page with the header 'Hospital and other appointments'
    When I click the 'Continue' button on the redirector page with a url starting with 'http://gncr.stubs.local.bitraft.io:8080/appointment'
    Then I am navigated to a third party site for GNCR
