@silverIntegration
Feature: Silver integration errors

  Scenario: A user who navigates directly to a silver integration
            which is not enabled for them in SJR
            is redirected to the feature not available page
    Given I am a user who cannot view Medicines from Patients Know Best
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=http%3A%2F%2Fpkb.stubs.local.bitraft.io%3A8080%2Fnhs-login%2Flogin%3FphrPath%3D%252Fauth%252FmanageMedications.action%253Ftab%253Dtreatments'
    Then I see silver integration error page loaded with title Hospital and other prescriptions
    When I select the Go to NHS App homepage link from the feature not available page
    Then I see the home page header

  Scenario: A P5 user who navigates directly to a silver integration
            is redirected to the silver integration uplift page
    Given I am a user with proof level 5 who can view Messages and Online Consultations from Patients Know Best
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=http%3A%2F%2Fpkb.stubs.local.bitraft.io%3A8080%2Fnhs-login%2Flogin%3FphrPath%3D%252Fauth%252FgetInbox.action%253Ftab%253Dmessages'
    Then the page title is 'Messages and consultations with a doctor or health professional'
    And I am asked to prove my identity to access 'silver integration feature'
    When I click the 'Continue' button
    Then the uplift journey starts
