@patientpracticemessaging
Feature: Patient to practice messaging

  Background:
    Given I am a EMIS patient
    And I am using the native app user agent
    And I am a user who can access patient practice messaging

  Scenario: A user can see their read and unread patient practice messages
    Given I have patient practice messages in my inbox
    And I am logged in
    When I navigate to the More page
    And I click the Messages link on the More page
    Then the patient to practice inbox page is displayed
    And I see a list of patient practice messages

  Scenario: A user that has no patient practice messages sees no patient practice messages information displayed
    Given I have no patient practice messages in my inbox
    And I am logged in
    When I navigate to the More page
    And I click the Messages link on the More page
    Then the patient to practice inbox page is displayed
    And I see a message indicating that I have no patient practice messages

  Scenario: A user receives an error message if there is an unknown error when trying to access their patient practice messages
    Given there is an unknown error getting patient practice messages
    And I am logged in
    When I navigate to the More page
    And I click the Messages link on the More page
    Then I see the appropriate error for patient practice messaging

  @smoketest
  Scenario: A user can see a patient practice message conversation
    Given I am logged in
    And I have patient practice messages in my inbox
    When I navigate to the More page
    And I click the Messages link on the More page
    Then the patient to practice inbox page is displayed
    When I select a patient practice message in my inbox
    Then I see my patient practice message along with the replies from the GP

  Scenario: A user can see an error message when the patient practice message cant be retrieved
    Given I am logged in
    And I have patient practice messages in my inbox
    When I navigate to the More page
    And I click the Messages link on the More page
    Then the patient to practice inbox page is displayed
    And there is an unknown error getting patient practice message details
    When I select a patient practice message in my inbox
    Then I see the appropriate error for patient practice message

