@silverIntegration
@pkbSecondaryCare
Feature: Patients Know Best My Care View Messages

  # P5 notes - the prescriptions hub page is not available to P5 users, preventing them from accessing any silver integration medicines jump offs.

  Scenario: A user with proof level 5 cannot see the menu item 'Messages and online consultations' on the messages hub
    Given I am a user with proof level 5 who can view Messages and Online Consultations from PKB My Care View
    And I am logged in
    And I navigate to the Messages hub page
    Then the Messages Hub page is displayed
    And the link to PKB My Care View Messages and consultations is not available on the Messages Hub page

  Scenario: A user navigates to PKB My Care View messages and sees the warning message
    Given I am using the native app user agent
    And I am a user who can view Messages and Online Consultations from PKB My Care View
    And I am logged in
    When I navigate to the Messages hub page
    Then the Messages Hub page is displayed
    When I click the PKB My Care View Messages and online consultations link on the Messages Hub page
    Then I am redirected to the redirector page with the header 'Consultations, events and messages'
    And the consultations warning on the page explains the service is from PKB My Care View

  Scenario: A user without access to PKB My Care View cannot see the menu item 'Consultations, events and messages' on the messages hub
    Given I am a user who cannot view Messages and Online Consultations from PKB My Care View
    And I am logged in
    And I navigate to the Messages hub page
    Then the Messages Hub page is displayed
    And the link to PKB My Care View Messages and consultations is not available on the Messages Hub page

  Scenario: A user can follow the link to Find out more about personal health records
    Given I am a user who can view Messages and Online Consultations from PKB My Care View
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=http%3A%2F%2Fpkb.stubs.local.bitraft.io%3A8080%2Fnhs-login%2Flogin%3FphrPath%3D%2Fauth%2FgetInbox.action%3Ftab%3Dmessages%26brand=pkbMyCareView'
    Then I am redirected to the redirector page with the header 'Consultations, events and messages'
    When I click the link called 'Find out more about personal health record services' with a url of 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/personal-health-records/'
    Then a new tab has been opened by the link

  Scenario: A user navigates directly to an external partner site and will see a warning page
    Given I am a user who can view Messages and Online Consultations from PKB My Care View
    And My Care View responds to requests for messages
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=http%3A%2F%2Fpkb.stubs.local.bitraft.io%3A8080%2Fnhs-login%2Flogin%3FphrPath%3D%2Fauth%2FgetInbox.action%3Ftab%3Dmessages%26brand=pkbMyCareView'
    Then I am redirected to the redirector page with the header 'Consultations, events and messages'
    When I click the 'Continue' button on the redirector page with a url starting with 'http://pkb.stubs.local.bitraft.io:8080/nhs-login/login?phrPath=/auth/getInbox.action?tab=messages&brand=pkbMyCareView'
    Then I am navigated to a third party site for My Care View

  Scenario: A user with no access to PKB My Care View cannot see the Medicines link on the Messages Hub
    Given I am a user who cannot view Medicines from PKB My Care View
    And I am logged in
    Then I see the home page
    And I navigate to the Messages hub page
    And the Messages Hub page is displayed
    And the link to PKB My Care View Messages and consultations is not available on the Messages Hub page
