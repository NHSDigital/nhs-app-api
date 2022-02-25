@silverIntegration
Feature: Wellness and Prevention

  # P5 notes - the health record hub is not available to P5 users, preventing them from accessing any silver integration shared links jump offs.

  Scenario: A user navigates to Wellness and Prevention and sees the warning message
    Given I am using the native app user agent
    And I am a user who can view Wellness and Prevention from Health Record Hub
    And I am logged in
    When I navigate to the Health Record Hub page
    Then I see the health records hub page
    And I click the menu item 'Wellness and Prevention'
    And I am redirected to the redirector page with the header 'Third Party Feature Name'
    And the warning on the page explains the service is from Wellness and Prevention

  Scenario: A user without access to Wellness and Prevention cannot see the menu item 'Wellness and Prevention' on the health record hub page
    Given I am a user who cannot view Wellness and Prevention from Health Record Hub
    And I am logged in
    When I navigate to the Health Record Hub page
    Then I see the health records hub page
    And the link to Wellness and Prevention is not available on the Health Record Hub page

  Scenario: A user can follow the link to Find out more about Service Type Plural
    Given I am a user who can view Wellness and Prevention from Health Record Hub
    And Wellness and Prevention responds to privacy requests
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=http%3A%2F%2FwellnessAndPrevention.stubs.local.bitraft.io%3A8080%2Fsso'
    Then I am redirected to the redirector page with the header 'Third Party Feature Name'
    When I click the link called 'Find out more about Service Type Plural' with a url of 'http://stubs.local.bitraft.io:8080/external/wellnessAndPrevention/privacy'
    Then a new tab has been opened by the link

  Scenario: A user navigates to an external partner site and will see a warning page
    Given I am a user who can view Wellness and Prevention from Health Record Hub
    And Wellness and Prevention responds to requests
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=http%3A%2F%2FwellnessAndPrevention.stubs.local.bitraft.io%3A8080%2Fsso'
    And I am redirected to the redirector page with the header 'Third Party Feature Name'
    And I click the 'Continue' button on the redirector page with a url starting with 'http://wellnessandprevention.stubs.local.bitraft.io:8080/sso'
    Then I am navigated to a third party site for Wellness and Prevention
