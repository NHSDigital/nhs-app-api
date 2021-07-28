@silverIntegration
@pkbSecondaryCare
Feature: Patients Know Best Secondary Care Appointments

  # P5 notes - the prescriptions hub page is not available to P5 users, preventing them from accessing any silver integration medicines jump offs.

  Scenario: A user navigates to PKB Secondary Care Appointments and sees the warning message
    Given I am using the native app user agent
    And I am a user who can view Appointments from PKB Secondary Care
    And I am logged in
    And I navigate to the hospital and other appointments page
    Then the Hospital Appointments page is displayed
    And I can see the PKB Secondary Care View Appointments link on the Appointments page
    When I click the PKB Secondary Care View Appointments link on the Appointments page
    Then I am redirected to the redirector page with the header 'View appointments'
    And the view appointments warning on the page explains the service is from PKB Secondary Care

  Scenario: A user without access to PKB cannot see the menu item 'Appointments' on the appointments page
    Given I am a user who cannot view Appointments from PKB Secondary Care
    And I am logged in
    Then I see the home page
    Given I navigate to the hospital and other appointments page
    Then the Hospital Appointments page is displayed
    And the link to PKB Secondary Care View Appointments is not available on the Appointments page

  Scenario: A user can follow the link to Find out more about personal health records
    And I am a user who can view Appointments from PKB Secondary Care
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=http%3A%2F%2Fpkb.stubs.local.bitraft.io%3A8080%2Fnhs-login%2Flogin%3FphrPath%3D%2Fdiary%2FlistAppointments.action%26brand=pkbSecondaryCare'
    Then I am redirected to the redirector page with the header 'View appointments'
    When I click the link called 'Find out more about personal health record services' with a url of 'https://www.nhs.uk/nhs-app/nhs-app-legal-and-cookies/nhs-app-privacy-policy/personal-health-records/'
    Then a new tab has been opened by the link

  Scenario: A user navigates to an external partner site and will see a warning page
    Given I am a user who can view Appointments from PKB Secondary Care
    And Secondary Care responds to requests for appointments
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=http%3A%2F%2Fpkb.stubs.local.bitraft.io%3A8080%2Fnhs-login%2Flogin%3FphrPath%3D%2Fdiary%2FlistAppointments.action%26brand%3DpkbSecondaryCare'
    Then I am redirected to the redirector page with the header 'View appointments'
    When I click the 'Continue' button on the redirector page with a url starting with 'http://pkb.stubs.local.bitraft.io:8080/nhs-login/login?phrPath=/diary/listAppointments.action&brand=pkbSecondaryCare'
    Then I am navigated to a third party site for Secondary Care

  Scenario: A user who cannot see PKB Secondary Care appointments but tries to access it is redirected
    Given I am a user who cannot view Appointments from PKB Secondary Care
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=http%3A%2F%2Fpkb.stubs.local.bitraft.io%3A8080%2Fnhs-login%2Flogin%3FphrPath%3D%2Fdiary%2FlistAppointments.action%26brand%3DpkbSecondaryCare'
    Then I see silver integration error page loaded with title View appointments
    When I select the Go to NHS App homepage link from the feature not available page
    Then I see the home page header
