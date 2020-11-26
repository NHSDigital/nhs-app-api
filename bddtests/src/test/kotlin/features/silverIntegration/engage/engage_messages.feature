@silverIntegration
@engage
Feature: Engage Messages

  # P5 notes - P5 users are able to access the messaging hub page directly, but should not see any silver integration messaging jump offs

  Scenario: A user with proof level 5 cannot see the menu item 'Messages and online consultations' on the more page
    Given I am a user with proof level 5 who can view Messages and Online Consultations from Engage
    And I am logged in
    And I navigate to the Messages hub page
    Then the Messages Hub page is displayed
    And the link to Engage Messages and consultations is not available on the Messages Hub page

  Scenario: A user navigates directly to Engage messages and sees the warning message
    Given I am using the native app user agent
    And I am a user who can view Messages and Online Consultations from Engage
    And I am logged in
    When I navigate to the More page
    Then I am on the More page
    And I click the Messages link on the More page
    And the Messages Hub page is displayed
    And I click the Engage Messages link on the Messages Hub page
    And I am redirected to the redirector page with the header 'Messages'
    And the messages warning message on the Redirector page explains the service is from Engage

  Scenario: A user without access to Engage cannot see the menu item 'Messages and online consultations' on the more page
    Given I am a user who cannot view Messages and Online Consultations from Engage
    And I am logged in
    And I navigate to the Messages hub page
    And the Messages Hub page is displayed
    And the link to Messages and consultations is not available on the Messages Hub page

  Scenario: A user can follow the link to Find out more about personal health records
    Given I am a user who can view Messages and Online Consultations from Engage
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=https%3A%2F%2Fnhs1-nhsapp.engage.gp%2F%3Fsso_route%3Dmessages'
    Then I am redirected to the redirector page with the header 'Messages'
    When I click the link called 'Find out more about online consultation services' with a url of 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/online-consultations/'
    Then a new tab has been opened by the link

  Scenario: A user navigates directly to an external partner site and will see a warning page
    Given I am a user who can view Messages and Online Consultations from Engage
    And I am logged in
    When I navigate to the redirector page with a url of '/redirector?redirect_to=https%3A%2F%2Fnhs1-nhsapp.engage.gp%2F%3Fsso_route%3Dmessages'
    Then I am redirected to the redirector page with the header 'Messages'
    When I click the 'Continue' button on the redirector page with a url starting with 'https://nhs1-nhsapp.engage.gp/?sso_route=messages'
    Then I am navigated to a third party site
