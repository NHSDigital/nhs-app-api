@accessibility
  Feature: Patients Know Best Messages accessibility

    Scenario: The Patient Know Best Messages page is captured
      Given I am using the native app user agent
      And I am a user who can view their Messages and Online Consultations from Patients Know Best
      And I am logged in
      When I navigate to the redirector page with a url of '/redirector?redirect_to=https://nhsapp-test.devstacks.pkb.io/nhs-login/login?phrPath=%2Fauth%2FgetInbox.action%3Ftab%3Dmessages'
      Then I am redirected to the redirector page with the header 'Messages and online consultations'
      Then the PKB_Messages_Redirect page is saved to disk
