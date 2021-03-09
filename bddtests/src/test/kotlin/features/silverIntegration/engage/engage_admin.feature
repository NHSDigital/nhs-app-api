@silverIntegration
@engage

Feature: Engage Admin

  # P5 notes - the appointments hub page is not available to P5 users, preventing them from accessing the silver integration admin jumpoff.

  Scenario: A user navigates to Engage Admin and sees the warning message
    Given I am using the native app user agent
    And I am a user who can view Admin from Engage
    And I am logged in
    When I navigate to the Appointment Hub page
    Then the Appointments Hub page is displayed
    And I wait for 30 seconds
    And I click the Engage Admin link
    And I am redirected to the redirector page with the header 'Additional GP services'
    And the additional services warning message on the Redirector page explains the service is from Engage

  Scenario: A user without access to Engage cannot see the menu item 'Additional GP services' on the Appointments Hub
    Given I am a user who cannot view Admin from Engage
    And I am logged in
    When I navigate to the Appointment Hub page
    Then the Engage Admin link is not available on the Appointments Hub

  Scenario: A user can follow the link to find out more about Admin from Engage
    Given I am a user who can view Admin from Engage
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=http%3A%2F%2Fengage.stubs.local.bitraft.io%3A8080%2F%3Fsso_route%3Dadmin'
    Then I am redirected to the redirector page with the header 'Additional GP services'
    When I click the link called 'Find out more about online consultation services' with a url of 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/online-consultations/'
    Then a new tab has been opened by the link

  Scenario: A user navigates directly to an external partner site and will see a warning page
    Given I am a user who can view Admin from Engage
    And Engage responds to requests for admin
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=http%3A%2F%2Fengage.stubs.local.bitraft.io%3A8080%2F%3Fsso_route%3Dadmin'
    Then I am redirected to the redirector page with the header 'Additional GP services'
    When I click the 'Continue' button on the redirector page with a url starting with 'http://engage.stubs.local.bitraft.io:8080/?sso_route=admin'
    Then I am navigated to a third party site for Engage
