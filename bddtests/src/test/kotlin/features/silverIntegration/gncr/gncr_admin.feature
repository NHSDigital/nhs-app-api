@silverIntegration
@gncr
Feature: Great North Care Record Admin

  Scenario: A user navigates to GNCR Admin and sees the warning message
    Given I am using the native app user agent
    And I am a user who can view Admin from GNCR
    And I am logged in
    And I navigate to the more page
    Then the More page for mobile devices is displayed
    And I can see the GNCR View Admin link on the More page
    When I click the GNCR View Admin link on the More page
    Then I am redirected to the redirector page with the header 'GNCR preferences'
    And the GNCR Preferences warning message on the Redirector page explains the service is from GNCR

  Scenario: A user without access to GNCR cannot see the menu item 'Great North Care Record preferences' on the More page
    Given I am a user who cannot view Admin from GNCR
    And I am logged in
    Then I see the home page
    Given I navigate to the more page
    Then the More page is displayed
    And the link to GNCR View Admin is not available on the More page

  Scenario: A user can follow the link to Find out more about personal health records
    Given I am a user who can view Admin from GNCR
    And 'NHS UK' responds to requests for '/personal-health-records'
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=http%3A%2F%2Fgncr.stubs.local.bitraft.io%3A8080%2Fpatient%2Fpreferences'
    Then I am redirected to the redirector page with the header 'GNCR preferences'
    When I click the link called 'Find out more about personal health record services' with a url of 'http://stubs.local.bitraft.io:8080/external/nhsuk/personal-health-records'
    Then a new tab has been opened by the link

  Scenario: A user navigates directly to an external partner site and will see a warning page
    Given I am a user who can view Admin from GNCR
    And GNCR responds to requests for admin
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=http%3A%2F%2Fgncr.stubs.local.bitraft.io%3A8080%2Fpatient%2Fpreferences'
    Then I am redirected to the redirector page with the header 'GNCR preferences'
    When I click the 'Continue' button on the redirector page with a url starting with 'http://gncr.stubs.local.bitraft.io:8080/patient/preferences'
    Then I am navigated to a third party site for GNCR
