@silverIntegration
@careInformationExchange
Feature: Care Information Exchange Appointments

  # P5 notes - the hospital and other appointments page is not available to P5 users, preventing them from accessing any silver integration secondary appointments jump offs.

  Scenario: A user navigates to CIE appointments and sees the warning message
    Given I am using the native app user agent
    And I am a user who can view Appointments from Care Information Exchange
    And I am logged in
    And I navigate to the hospital and other appointments page
    Then the Hospital Appointments page is displayed
    And I can see the CIE View Appointments link on the Appointments page
    When I click the CIE View Appointments link on the Appointments page
    Then I am redirected to the redirector page with the header 'View appointments'
    And the view appointments warning on the page explains the service is from Care Information Exchange

  Scenario: A user without access to CIE cannot see the menu item 'Appointments' on the appointments page
    Given I am a user who cannot view Appointments from Care Information Exchange
    And I am logged in
    Then I see the home page
    Given I navigate to the hospital and other appointments page
    Then the Hospital Appointments page is displayed
    And the link to CIE View Appointments is not available on the Appointments page


  Scenario: A user can follow the link to Find out more about personal health records
    Given I am a user who can view Appointments from Care Information Exchange
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=http%3A%2F%2Fpkb.stubs.local.bitraft.io%3A8080%2Fnhs-login%2Flogin%3FphrPath%3D%2Fdiary%2FlistAppointments.action%26brand=cie'
    Then I am redirected to the redirector page with the header 'View appointments'
    When I click the link called 'Find out more about personal health record services' with a url of 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/personal-health-records/'
    Then a new tab has been opened by the link

  Scenario: A user navigates directly to an external partner site and will see a warning page
    Given I am a user who can view Appointments from Care Information Exchange
    And CIE responds to requests for appointments
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=http%3A%2F%2Fpkb.stubs.local.bitraft.io%3A8080%2Fnhs-login%2Flogin%3FphrPath%3D%2Fdiary%2FlistAppointments.action%26brand=cie'
    Then I am redirected to the redirector page with the header 'View appointments'
    When I click the 'Continue' button on the redirector page with a url starting with 'http://pkb.stubs.local.bitraft.io:8080/nhs-login/login?phrPath=/diary/listAppointments.action&brand=cie'
    Then I am navigated to a third party site for CIE
