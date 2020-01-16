@patientpracticemessaging
Feature: Patient to practice messaging

  Background:
    Given I am a EMIS patient
    And I am using the native app user agent
    And I am a user who can access patient practice messaging
    And I am logged in

  Scenario: A user can see their read and unread patient practice messages
    Given I have patient practice messages in my inbox, all of which are read
    When I navigate to the More page
    And I click the Messages link on the More page
    Then the patient to practice inbox page is displayed
    And I see a list of patient practice messages

  Scenario: A user that has no patient practice messages sees no patient practice messages information displayed
    Given I have no patient practice messages in my inbox
    When I navigate to the More page
    And I click the Messages link on the More page
    Then the patient to practice inbox page is displayed
    And I see a message indicating that I have no patient practice messages

  Scenario: A user receives an error message if there is an unknown error when trying to access their patient practice messages
    Given there is an unknown error getting patient practice messages
    When I navigate to the More page
    And I click the Messages link on the More page
    Then I see the appropriate error for patient practice messaging

  @smoketest
  Scenario: A user can see a patient practice message conversation with unread messages
    Given I have patient practice messages in my inbox, some of which are unread
    When I navigate to the More page
    And I click the Messages link on the More page
    Then the patient to practice inbox page is displayed
    And I see a list of patient practice messages
    When I select a patient practice message in my inbox
    Then I see my patient practice message along with the replies from the GP

  Scenario: A user can see a patient practice message conversation with no unread messages
    Given I have patient practice messages in my inbox, all of which are read
    When I navigate to the More page
    And I click the Messages link on the More page
    Then the patient to practice inbox page is displayed
    And I see a list of patient practice messages
    When I select a patient practice message in my inbox
    Then I see my patient practice message along with the replies from the GP

  Scenario: A user can see an error message when the patient practice message cant be retrieved
    Given I have patient practice messages in my inbox, all of which are read
    When I navigate to the More page
    And I click the Messages link on the More page
    Then the patient to practice inbox page is displayed
    And there is an unknown error getting patient practice message details
    When I select a patient practice message in my inbox
    Then I see the appropriate error for getting patient practice message details

  Scenario: A user is looking for urgent advice via patient practice messaging
    Given I have patient practice messages in my inbox, all of which are read
    And I navigate to the Patient Practice Messaging page
    When I click the Send a message button and I choose that I need urgent advice via patient practice messaging
    Then I see the patient practice messaging urgency contact your gp page
    And I see a message explaining patient practice messaging is not for urgent advice

  Scenario: A user is not looking for urgent advice via patient practice messaging
    Given I have patient practice messages in my inbox, all of which are read
    And I navigate to the Patient Practice Messaging page
    When I click the Send a message button and I choose that I do not need urgent advice via patient practice messaging
    Then I see the patient practice messaging recipients page
    And I see a list of patient practice messaging recipients
