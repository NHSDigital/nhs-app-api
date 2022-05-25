@accessibility
@im1-messaging-push-notifications-accessibility
Feature: IM1 Messaging & Push Notifications

  Scenario: The 'IM Messaging' journey pages are captured
    Given I am an EMIS user who can access patient practice messaging
    And I have patient practice messages in my inbox, some of which are unread
    And I am logged in
    When I follow the Messages link from the home page
    Then the Messages Hub page is displayed
    And I navigate to the Patient Practice Messaging page
    And the IM_GPMessagesInbox page is saved to disk
    When I click the Send a message button and I choose that I do not need urgent advice via patient practice messaging
    Then I see the patient practice messaging recipients page
    And the IM_SelectWhoToMessage page is saved to disk
    And I see a list of patient practice messaging recipients
    When I click on a regular recipient
    And the IM_SendYourMessageTo page is saved to disk
    Then I am on the send message page
    And I leave the message and subject fields blank
    And I click send message
    And I see validation errors for subject and message
    And the IM_ErrorMessageSubjectAndMessageMissing page is saved to disk
    And I navigate to the Patient Practice Messaging page
    When I select a patient practice message in my inbox
    Then the IM_MessageThread page is saved to disk
    And I navigate to the Patient Practice Messaging page
    And I click the Send a message button on the patient practice messaging inbox
    And the IM_ChooseIfYouNeedUrgentAdvice page is saved to disk
    And I click continue
    And the IM_ErrorMessageNoSelection page is saved to disk

  Scenario: The 'IM03 Call Your GP Or Use NHS111' page is captured
    Given I am an EMIS user who can access patient practice messaging
    And I have patient practice messages in my inbox, some of which are unread
    And I am logged in
    When I follow the Messages link from the home page
    Then the Messages Hub page is displayed
    And I navigate to the Patient Practice Messaging page
    When I click the Send a message button and I choose that I need urgent advice via patient practice messaging
    Then the IM_CallYourGPOrUseNHS111 page is saved to disk

  Scenario: The 'IM15 Messages (inbox) - no messages' page is captured
    Given I am an EMIS user who can access patient practice messaging
    And I have no patient practice messages in my inbox
    And I am logged in
    When I follow the Messages link from the home page
    Then the Messages Hub page is displayed
    When I navigate to the Patient Practice Messaging page
    Then the IM_MessagesInbox_NoMessages page is saved to disk

  Scenario: The 'IM19 Download file' page is captured
    Given I am a TPP user who can access patient practice messaging
    And I have patient practice messages in my inbox, some of which are unread with an attachment
    And I am logged in
    When I follow the Messages link from the home page
    Then the Messages Hub page is displayed
    And I click on the patient practice Messages link on the Messages Hub page
    And the patient to practice inbox page is displayed
    And I see a list of patient practice messages without the subject and with the unread count
    When I select a patient practice message in my inbox
    And I see my patient practice message along with the replies from the GP
    And I see the view and download links on the message
    And I click on the download link
    And I see the download information page
    Then the IM_DownloadFile page is saved to disk

  Scenario: The 'IM20 File not available' page is captured
    Given I am a TPP user who can access patient practice messaging
    And I have patient practice messages in my inbox, some of which are unread with an attachment
    And I am logged in
    And that attachment is invalid
    When I follow the Messages link from the home page
    Then the Messages Hub page is displayed
    And I click on the patient practice Messages link on the Messages Hub page
    And the patient to practice inbox page is displayed
    And I see a list of patient practice messages without the subject and with the unread count
    When I select a patient practice message in my inbox
    And I see my patient practice message along with the replies from the GP
    And I see the view and download links on the message
    And I click on the view link
    Then I see the invalid attachment message
    And the IM_FileNotAvailable page is saved to disk

  Scenario: The 'IM18 Cannot send GP surgery messages' page is captured
    Given I am a TPP user who can access patient practice messaging
    And I have patient practice messages in my inbox, some of which are unread
    And I am logged in
    When I follow the Messages link from the home page
    Then the Messages Hub page is displayed
    And I navigate to the Patient Practice Messaging page
    And I have no recipients for patient practice messaging
    When I click the Send a message button on the patient practice messaging inbox
    Then I see a message indicating that I have no recipients for patient practice messaging
    And the IM_CannotSendGpSurgeryMessages page is saved to disk
