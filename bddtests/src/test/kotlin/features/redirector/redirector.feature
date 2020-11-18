@redirector
Feature: Redirector

  Scenario: A user navigating to a nhs partner site via a redirector url will not see a warning page
    Given I am a EMIS patient
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=http%3A%2F%2Fsilver.local.bitraft.io%3A5000%2Fnhslogin'
    Then I am navigated to a third party site

  Scenario: A user navigating to a redirector url with no params will be directed to home page
    Given I am a EMIS patient
    And I am logged in
    When I navigate to the More page
    Then I am on the More page
    When I navigate to the redirector page with a url of '/redirector?redirect_to='
    Then I see the home page

  Scenario: A user can navigate to an nhs app web page with a redirector url
    Given I am a EMIS patient
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=appointments'
    Then the Appointments Hub page is displayed

  Scenario: A user navigating to a webpage not in the known services list will see the home page
    Given I am a EMIS patient
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=https://doesnotexistintheknownservices.com'
    Then I see the home page

  Scenario: The back button on the warning page will take the user to the previous page
    Given I am using the native app user agent
    And I am a user who can view Messages and Online Consultations from Patients Know Best
    And I am logged in
    When I navigate to the More page
    And I click the Messages link on the More page
    Then the Messages Hub page is displayed
    And I click the PKB Messages and online consultations link on the Messages Hub page
    And I am redirected to the redirector page with the header 'Messages and online consultations'
    When I click the 'Back' breadcrumb
    Then I am on the More page
    When I retrieve the 'home' page directly
    And I navigate to the redirector page with a url of '/redirector?redirect_to=https://nhsapp-test.devstacks.pkb.io/nhs-login/login?phrPath=%2Fauth%2FgetInbox.action%3Ftab%3Dmessages'
    Then I am redirected to the redirector page with the header 'Messages and online consultations'
    When I click the 'Back' breadcrumb
    Then I see the home page

  Scenario: An invalid redirector url (for ex. spelling mistake) redirects back to the home page
    Given I am a EMIS patient
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=appointmentss'
    Then I see the home page

  Scenario: A user will only see a warning page once per session per provider
    Given I am a EMIS patient
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=https%3A%2F%2Fnhsapp-test.devstacks.pkb.io%2Fnhs-login%2Flogin%3FphrPath%3D%252Fauth%252FgetInbox.action%253Ftab%253Dmessages'
    Then I am redirected to the redirector page with the header 'Messages and online consultations'
    When I click the 'Continue' button on the redirector page with a url starting with 'https://nhsapp-test.devstacks.pkb.io/nhs-login/login?phrPath=%2Fauth%2FgetInbox.action%3Ftab%3Dmessages'
    Then I am navigated to a third party site
    When I browse directly to '/more' in the NHS App
    And I am on the More page
    And I navigate to the redirector page with a url of '/redirector?redirect_to=https%3A%2F%2Fnhsapp-test.devstacks.pkb.io%2Fnhs-login%2Flogin%3FphrPath%3D%252Flibrary%252FmanageLibrary.action'
    Then I am navigated to a third party site
