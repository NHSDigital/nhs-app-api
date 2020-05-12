@patientpracticemessaging
Feature: Patient to practice messaging

  Scenario Outline: A <GP System> user that has no messages sees no patient practice messages information displayed
    Given I am a <GP System> user who can access patient practice messaging
    And I am logged in
    And I have no patient practice messages in my inbox
    When I follow the Messages link from the home page
    Then the Messages Hub page is displayed
    And I click on the patient practice Messages link on the Messages Hub page
    Then the patient to practice inbox page is displayed
    And I see a message indicating that I have no patient practice messages
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario Outline: A user receives a service unavailable message if they do not have access to their messages
    Given I am a <GP System> user who can access patient practice messaging
    And I am logged in
    And there is a forbidden error getting patient practice messages
    When I follow the Messages link from the home page
    Then the Messages Hub page is displayed
    And I click on the patient practice Messages link on the Messages Hub page
    Then I see the appropriate forbidden error for patient practice messaging
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario: A user receives a validation error if they enter invalid data when sending a patient practice message
    Given I am an EMIS user who can access patient practice messaging
    And I am logged in
    And I have patient practice messages in my inbox, some of which are unread
    And I navigate to the Patient Practice Messaging page
    When I click the Send a message button and I choose that I do not need urgent advice via patient practice messaging
    Then I see the patient practice messaging recipients page
    And I see a list of patient practice messaging recipients
    When I click on a regular recipient
    Then I am on the send message page
    And I leave the message and subject fields blank
    And I click send message
    Then I see validation errors for subject and message

  Scenario Outline: A user sees a message indicating that they have no recipients for patient practice messaging
    Given I am a <GP System> user who can access patient practice messaging
    And I am logged in
    And I have patient practice messages in my inbox, some of which are unread
    And I navigate to the Patient Practice Messaging page
    And I have no recipients for patient practice messaging
    When I click the Send a message button on the patient practice messaging inbox
    Then I see a message indicating that I have no recipients for patient practice messaging
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario: A TPP patient can view a list of their recipients
    Given I am a TPP user who can access patient practice messaging
    And I am logged in
    And I have patient practice messages in my inbox, some of which are unread
    When I follow the Messages link from the home page
    Then the Messages Hub page is displayed
    And I click on the patient practice Messages link on the Messages Hub page
    Then the patient to practice inbox page is displayed
    When I click the Send a message button and I choose that I do not need urgent advice via patient practice messaging
    Then I see the patient practice messaging recipients page
    And I see a list of patient practice messaging recipients


  @smoketest
  Scenario: A patient can send a patient practice message to their GP
    Given I am an EMIS user who can access patient practice messaging
    And I am logged in
    And I have patient practice messages in my inbox, some of which are unread
    When I follow the Messages link from the home page
    Then the Messages Hub page is displayed
    And I click on the patient practice Messages link on the Messages Hub page
    Then the patient to practice inbox page is displayed
    When I click the Send a message button and I choose that I do not need urgent advice via patient practice messaging
    Then I see the patient practice messaging recipients page
    And I see a list of patient practice messaging recipients
    When I click on a regular recipient
    Then I am on the send message page
    And I insert a subject
    And I insert a message
    And I click send message
    Then I see my new message after it has been sent

  Scenario: A patient can send a patient practice message to their GP from the message details page
    Given I am an EMIS user who can access patient practice messaging
    And I am logged in
    And I have patient practice messages in my inbox, some of which are unread
    When I follow the Messages link from the home page
    Then the Messages Hub page is displayed
    And I click on the patient practice Messages link on the Messages Hub page
    Then the patient to practice inbox page is displayed
    When I select a patient practice message in my inbox
    Then I see my patient practice message along with the replies from the GP
    When I click the send message link on the message details page and I do not need urgent advice
    Then I see the patient practice messaging recipients page
    And I see a list of patient practice messaging recipients
    When I click on a regular recipient
    Then I am on the send message page
    And I insert a subject
    And I insert a message
    And I click send message
    Then I see my new message after it has been sent

  Scenario Outline: A TPP patient can send a patient practice message to a <Recipient Type> recipient
    Given I am a TPP user who can access patient practice messaging
    And I am logged in
    And I want to send a message to a <Recipient Type> recipient and have unread messages in my inbox
    When I follow the Messages link from the home page
    Then the Messages Hub page is displayed
    And I click on the patient practice Messages link on the Messages Hub page
    Then the patient to practice inbox page is displayed
    When I click the Send a message button and I choose that I do not need urgent advice via patient practice messaging
    Then I see the patient practice messaging recipients page
    And I see a list of patient practice messaging recipients
    When I click on a <Recipient Type> recipient
    Then I am on the send message page
    And I insert a message
    And I click send message
    Then I see my new message after it has been sent
    Examples:
      | Recipient Type |
      | unit           |
      | regular        |

  Scenario: A TPP patient can view their patient messaging inbox and the unread count is displayed
    Given I am a TPP user who can access patient practice messaging
    And I am logged in
    And I have patient practice messages in my inbox, some of which are unread
    When I follow the Messages link from the home page
    Then the Messages Hub page is displayed
    And I click on the patient practice Messages link on the Messages Hub page
    Then the patient to practice inbox page is displayed
    And I see a list of patient practice messages without the subject and with the unread count

  Scenario: A TPP patient viewing a message marks it as read
    Given I am a TPP user who can access patient practice messaging
    And I am logged in
    And I have patient practice messages in my inbox, some of which are unread
    When I follow the Messages link from the home page
    Then the Messages Hub page is displayed
    And I click on the patient practice Messages link on the Messages Hub page
    Then the patient to practice inbox page is displayed
    When I select a patient practice message in my inbox
    Then the message is marked as read

  Scenario: A TPP patient can view a patient message they started
    Given I am a TPP user who can access patient practice messaging
    And I am logged in
    And I have patient practice messages in my inbox, all of which are read
    When I follow the Messages link from the home page
    Then the Messages Hub page is displayed
    And I click on the patient practice Messages link on the Messages Hub page
    Then the patient to practice inbox page is displayed
    And I see a list of patient practice messages without the subject and without the unread count
    When I select a patient practice message in my inbox
    Then I see my patient practice message along with the replies from the GP

  Scenario: A TPP patient can view an attachment on a message
    Given I am a TPP user who can access patient practice messaging
    And I am logged in
    And I have patient practice messages in my inbox, some of which are unread with an attachment
    When I follow the Messages link from the home page
    Then the Messages Hub page is displayed
    And I click on the patient practice Messages link on the Messages Hub page
    Then the patient to practice inbox page is displayed
    And I see a list of patient practice messages without the subject and without the unread count
    When I select a patient practice message in my inbox
    And I see my patient practice message along with the replies from the GP
    And I see the view and download links on the message
    And I click on the view link
    Then I can view the message attachment

  Scenario: A TPP user can view a message attachment without access to documents
    Given I am a TPP user who can access patient practice messaging
    And the Patient has no access to Documents
    And I am logged in
    And I have patient practice messages in my inbox, some of which are unread with an attachment
    When I follow the Messages link from the home page
    Then the Messages Hub page is displayed
    And I click on the patient practice Messages link on the Messages Hub page
    Then the patient to practice inbox page is displayed
    And I see a list of patient practice messages without the subject and with the unread count
    When I select a patient practice message in my inbox
    And I see my patient practice message along with the replies from the GP
    And I see the view and download links on the message
    And I click on the view link
    Then I can view the message attachment

  Scenario: A TPP patient can download an attachment on a message
    And I am a TPP user who can access patient practice messaging
    And I am logged in
    And I have patient practice messages in my inbox, some of which are unread with an attachment
    When I follow the Messages link from the home page
    Then the Messages Hub page is displayed
    And I click on the patient practice Messages link on the Messages Hub page
    Then the patient to practice inbox page is displayed
    And I see a list of patient practice messages without the subject and with the unread count
    When I select a patient practice message in my inbox
    And I see my patient practice message along with the replies from the GP
    And I see the view and download links on the message
    And I click on the download link
    And I see the download information page
    When I click on the download button
    Then the attachment has been downloaded

  Scenario: A TPP patient cannot view an invalid attachment on a message
    Given I am a TPP user who can access patient practice messaging
    And I am logged in
    And I have patient practice messages in my inbox, some of which are unread with an attachment
    And that attachment is invalid
    When I follow the Messages link from the home page
    Then the Messages Hub page is displayed
    And I click on the patient practice Messages link on the Messages Hub page
    Then the patient to practice inbox page is displayed
    And I see a list of patient practice messages without the subject and with the unread count
    When I select a patient practice message in my inbox
    And I see my patient practice message along with the replies from the GP
    And I see the view and download links on the message
    And I click on the view link
    Then I see the invalid attachment message

  Scenario: A TPP patient can view a patient message the gp sent
    Given I am a TPP user who can access patient practice messaging
    And I am logged in
    And I have patient practice messages in my inbox, one of which came from the GP
    When I follow the Messages link from the home page
    Then the Messages Hub page is displayed
    And I click on the patient practice Messages link on the Messages Hub page
    Then the patient to practice inbox page is displayed
    And I see a list of patient practice messages from the GP
    When I select a patient practice message in my inbox
    Then I see my patient practice message along with the replies from the GP

  Scenario: A TPP patient can view their patient messaging inbox and the unread count is not displayed
    Given I am a TPP user who can access patient practice messaging
    And I am logged in
    And I have patient practice messages in my inbox, all of which are read
    When I follow the Messages link from the home page
    Then the Messages Hub page is displayed
    And I click on the patient practice Messages link on the Messages Hub page
    Then the patient to practice inbox page is displayed
    And I see a list of patient practice messages without the subject and without the unread count

  @pending
  @NHSO-8671
  Scenario: A user can see their unread messages, view one and then delete the patient practice conversation
    Given I am an EMIS user who can access patient practice messaging
    And I am logged in
    And I have patient practice messages in my inbox, some of which are unread
    When I follow the Messages link from the home page
    Then the Messages Hub page is displayed
    And I click on the patient practice Messages link on the Messages Hub page
    Then the patient to practice inbox page is displayed
    And I see a list of patient practice messages
    When I select a patient practice message in my inbox
    Then I see my patient practice message along with the replies from the GP
    When I select delete conversation on the view conversation page
    Then I am prompted to confirm my intention to delete the conversation
    When I click delete conversation on the delete page to confirm my decision
    Then I see a page indicating my patient practice message has been deleted
    When I click go back to patient practice messages
    Then the patient to practice inbox page is displayed

  @pending
  @NHSO-8671
  Scenario: A user cant delete a patient practice conversation and is shown an error
    Given I am an EMIS user who can access patient practice messaging
    And I am logged in
    And I have patient practice messages in my inbox, some of which are unread
    When I follow the Messages link from the home page
    Then the Messages Hub page is displayed
    And I click on the patient practice Messages link on the Messages Hub page
    Then the patient to practice inbox page is displayed
    And I see a list of patient practice messages
    When I select a patient practice message in my inbox
    Then I see my patient practice message along with the replies from the GP
    When I select delete conversation on the view conversation page
    Then I am prompted to confirm my intention to delete the conversation
    And there is a bad request deleting the patient practice conversation
    When I click delete conversation on the delete page to confirm my decision
    Then I see the appropriate error for deleting patient practice message(s)

  Scenario: A user can see a patient practice message conversation with no unread messages
    Given I am an EMIS user who can access patient practice messaging
    And I am logged in
    And I have patient practice messages in my inbox, all of which are read
    When I follow the Messages link from the home page
    Then the Messages Hub page is displayed
    And I click on the patient practice Messages link on the Messages Hub page
    Then the patient to practice inbox page is displayed
    And I see a list of patient practice messages
    When I select a patient practice message in my inbox
    Then I see my patient practice message along with the replies from the GP

    @pending
    @NHSO-8913
  Scenario: A user can see an error message when the patient practice message cant be retrieved
    Given I am an EMIS user who can access patient practice messaging
    And I am logged in
    And I have patient practice messages in my inbox, all of which are read
    When I follow the Messages link from the home page
    Then the Messages Hub page is displayed
    And I click on the patient practice Messages link on the Messages Hub page
    Then the patient to practice inbox page is displayed
    And there is an unknown error getting patient practice message details
    When I select a patient practice message in my inbox
    Then I see the appropriate error for getting patient practice message(s)

    @pending
    @NHSO-8913
  Scenario: A user receives an error message if there is an unknown error when trying to access their patient practice messages
    Given I am an EMIS user who can access patient practice messaging
    And I am logged in
    And there is an unknown error getting patient practice messages
    When I follow the Messages link from the home page
    Then the Messages Hub page is displayed
    And I click on the patient practice Messages link on the Messages Hub page
    Then I see the appropriate error for listing patient practice message(s)

  Scenario: A user is looking for urgent advice via patient practice messaging
    Given I am an EMIS user who can access patient practice messaging
    And I am logged in
    And I have patient practice messages in my inbox, all of which are read
    And I navigate to the Patient Practice Messaging page
    When I click the Send a message button and I choose that I need urgent advice via patient practice messaging
    Then I see the patient practice messaging urgency contact your gp page
    And I see a message explaining patient practice messaging is not for urgent advice

  Scenario: A user is not looking for urgent advice via patient practice messaging
    Given I am an EMIS user who can access patient practice messaging
    And I am logged in
    And I have patient practice messages in my inbox, all of which are read
    And I navigate to the Patient Practice Messaging page
    When I click the Send a message button and I choose that I do not need urgent advice via patient practice messaging
    Then I see the patient practice messaging recipients page
    And I see a list of patient practice messaging recipients

