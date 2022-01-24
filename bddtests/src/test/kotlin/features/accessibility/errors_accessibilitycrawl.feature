@accessibility
@errors-accessibility
Feature: errors accessibility

  Scenario: 404 page not found is captured
    Given I am using the native app user agent
    And '111' responds to requests for '/home'
    And I am a user wishing to view my messages with content
      | /appointments/sausages  |
    And I am logged in
    When I navigate to the Messages Hub page
    And I click the App Messages link on the messages hub page
    And the Message Senders page is displayed
    And the senders are displayed on the Messages Inbox page
    And I click on Sender One sender
    Then the Sender Messages page is displayed
    When I click on message 1 message on the Sender Messages page
    Then the Message page is displayed
    When I click on the '/appointments/sausages' link in the message
    Then the Page not found error is displayed
    And the Errors_404_PageNotFound page is saved to disk

  Scenario: System or server error is captured
    Given I am logged in as a EMIS user with no linked profiles
    And I have access to online consultations gp advice journey and the response is delayed by 31 seconds
    When I navigate to Advice
    Then the Advice page is displayed
    When I click Ask your GP for Advice
    And I accept demographics and terms and conditions question
    And I am submitting the questionnaire for myself
    And I see a condition list for myself
    And I click on a condition
    And I am not in an emergency
    And I select my gender and click continue
    And I insert my symptoms and click continue
    And I insert my date of birth
    And I select how much alcohol I drink weekly
    And I insert how long I have felt the pain
    Then I see the appropriate error message for an online consultation timeout
    And the Errors_SC05a_SystemOrServerError page is saved to disk

  Scenario: Session expired is captured
    Given there are EMIS appointments available to book
    And I am logged in
    When I retrieve the 'Appointment Booking' page directly
    And I am idle long enough for the session expiry dialog box to appear
    Then I am idle for a short time
    And I see a dialog box prompting to extend the session
    When I am idle long enough for the session to expire
    Then I see the login page with the session expiry notification
    And the Errors_SC05e_SessionExpired page is saved to disk

  Scenario: Service unavailable too young is captured
    Given I attempt to log in as a TPP user with an age under 13
    And 'NHS COVID Pass' responds to requests for type '/'
    And 'COVID Pass or proof' responds to requests for type '/get-your-covid-pass-letter'
    And '111' responds to requests for '/home'
    Then I see a message informing me I cannot log in as I am under the minimum age
    And the Errors_SC05g_ServiceUnavailable_TooYoung page is saved to disk
