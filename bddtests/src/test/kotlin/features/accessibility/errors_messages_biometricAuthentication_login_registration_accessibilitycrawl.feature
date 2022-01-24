@accessibility
@errors-messages-biometricAuthentication-login-registration-accessibility
Feature: Messages/Biometric authentication/Login/Registration errors accessibility

  Scenario: 'E-ME01a Error: Can't load message' page is captured
    Given I am an EMIS user who can access patient practice messaging
    And I have patient practice messages in my inbox, all of which are read
    And I am logged in
    When I follow the Messages link from the home page
    Then the Messages Hub page is displayed
    And I click on the patient practice Messages link on the Messages Hub page
    And the patient to practice inbox page is displayed
    And there is an unknown error getting patient practice message details
    When I select a patient practice message in my inbox
    Then I see the appropriate error for getting patient practice message(s)
    And the Errors_EME01a_CantLoadMessage page is saved to disk

  Scenario: 'E-ME16 Error: Messaging service disabled for user' page is captured
    Given I am a EMIS user who can access patient practice messaging
    And there is a forbidden error getting patient practice messages
    And I am logged in
    When I follow the Messages link from the home page
    Then the Messages Hub page is displayed
    And I click on the patient practice Messages link on the Messages Hub page
    And I see the appropriate forbidden error for patient practice messaging
    And the Errors_EME16_MessagingServiceDisabledForUser page is saved to disk

  Scenario: 'E-BA30 Error: biometric login failed' page is captured
    Given I am a patient using the native app
    And I am on the login logged-out page
    When I attempt biometric login and fail
    Then I see the login biometric error page is displayed
    And the Errors_EBA30_BiometricLoginFailed page is saved to disk

  Scenario: 'E-RL01 - No selection error message' page is captured
    Given I am a EMIS patient who has not already accepted terms and conditions
    And I am logged in expecting to see T&Cs
    Then the Terms and Conditions page is displayed
    When I check the agree to terms and conditions checkbox
    And I click the 'Continue' button
    Then the User Research page is displayed
    And I click the 'Continue' button
    And the Errors_ERL01_NoSelectionErrorMessage page is saved to disk

  Scenario: '400 bad request - Supplied OAuth details are incomplete or invalid' page is captured
    Given I am logged into Citizen ID but am receiving invalid data
    And 'NHS UK' responds to requests for '/nhs-app-contact-us'
    Then I see an error 'Contact us if you keep seeing this message, quoting error code' link with a url of 'http://stubs.local.bitraft.io:8080/external/nhsuk/nhs-app-contact-us?errorcode=3a' and error prefix of '3a'
    And the Errors_400_BadRequest_SuppliedOAuthDetailsIncompleteOrInvalid page is saved to disk

  Scenario: '464 ODS code associated with a user is not supported or no NHS number found' page is captured
    Given I attempt to log in as a EMIS user with invalid ODS Code
    And 'NHS UK' responds to requests for '/nhs-app-contact-us'
    And 'My Health Online' responds to requests for type '/myhealthonline'
    And '111' responds to requests for '/wales'
    And 'COVID Pass or proof' responds to requests for type '/conditions/coronavirus-covid-19/covid-pass'
    And 'Northern Ireland' responds to requests for type '/www-nidirect-gov-uk/articles/gp-out-hours-service'
    And I see the Contact us link with a url of 'http://stubs.local.bitraft.io:8080/external/nhsuk/nhs-app-contact-us?errorcode=3f'
    And the Errors_464_ODSCodeAssociatedWithUserNotSupportedOrNoNHSNumberFound page is saved to disk

  Scenario: '465 User does not meet the minimum age requirement' page is captured
    Given I attempt to log in as a TPP user with an age under 13
    And 'NHS COVID Pass' responds to requests for type '/'
    And 'COVID Pass or proof' responds to requests for type '/get-your-covid-pass-letter'
    And '111' responds to requests for '/home'
    Then I see a message informing me I cannot log in as I am under the minimum age
    And the Errors_465_UserDoesNotMeetTheMinimumAgeRequirement page is saved to disk

  Scenario: '504 Timeout when calling upstream system' page is captured
    Given I attempt to log in as an EMIS and the CID request timeout
    And 'NHS UK' responds to requests for '/nhs-app-contact-us'
    Then I click an error 'Contact us if you keep seeing this message, quoting error code' link with a url of 'http://stubs.local.bitraft.io:8080/external/nhsuk/nhs-app-contact-us?errorcode=zn' and error prefix of 'zn'
    And the Errors_504_TimeoutWhenCallingUpstreamSystem page is saved to disk
