@service-journey-rules
@backend
Feature: Service Journey Rules Backend
  #This endpoint is GPSystem agnostic
  Scenario: A user fetching the service journey rules for an ODS code not configured will receive a 404
    Given I am a user whose ODS Code does not have specific journey configuration set up
    When I login but service journey rules has no configuration for my GP practice
    Then I receive an "ODS Code Not Found" error with service desk reference prefixed "3f"

  @long-running
  Scenario: A user fetching the service journey rules but the session has expired, will receive Unauthorized
    Given I am a EMIS user where the journey configurations are:
      | Journey            | Value       |
      | appointments       | informatica |
    And I have logged in and have a valid session cookie
    And My session has expired
    When I request the service journey rules for my ODS Code
    Then I receive a "Unauthorized" error

  Scenario: API call for SJR can return a response with nominated pharmacy enabled
    Given I am a EMIS user where the journey configurations are:
      | Journey            | Value   |
      | nominated pharmacy | enabled |
    And I have logged in and have a valid session cookie
    When I request the service journey rules for my ODS Code
    Then I receive an "Ok" success code
    And the service journey rules response will have nominated pharmacy enabled

  Scenario: API call for SJR can return a response with nominated pharmacy disabled
    Given I am a EMIS user where the journey configurations are:
      | Journey            | Value    |
      | nominated pharmacy | disabled |
    And I have logged in and have a valid session cookie
    When I request the service journey rules for my ODS Code
    Then I receive an "Ok" success code
    And the service journey rules response will have nominated pharmacy disabled

  Scenario: API call for SJR can return a response with appointments, medical record and prescriptions configured to IM1
    Given I am a EMIS user where the journey configurations are:
      | Journey            | Value   |
      | appointments       | im1     |
      | medical record     | im1     |
      | prescriptions      | im1     |
    And I have logged in and have a valid session cookie
    When I request the service journey rules for my ODS Code
    Then I receive an "Ok" success code
    And the service journey rules response will have appointments set to im1
    And the service journey rules response will have medical record set to im1
    And the service journey rules response will have prescriptions set to im1

  Scenario: API call for SJR can return a response with appointments configured to Informatica Frontdesk
    Given I am a EMIS user where the journey configurations are:
      | Journey            | Value       |
      | appointments       | informatica |
    And I have logged in and have a valid session cookie
    When I request the service journey rules for my ODS Code
    Then I receive an "Ok" success code
    And the service journey rules response will have appointments set to informatica

  Scenario: API call for SJR can return a response with appointments, medical record and prescriptions configured to GP at Hand
    Given I am a EMIS user where the journey configurations are:
      | Journey            | Value     |
      | appointments       | gpAtHand  |
      | medical record     | gpAtHand  |
      | prescriptions      | gpAtHand  |
    And I have logged in and have a valid session cookie
    When I request the service journey rules for my ODS Code
    Then I receive an "Ok" success code
    And the service journey rules response will have appointments set to gpAtHand
    And the service journey rules response will have medical record set to gpAtHand
    And the service journey rules response will have prescriptions set to gpAtHand

  Scenario: API call for SJR can return a response with medical records configured to version 2
    Given I am a EMIS user setup to use medical record version 2
    And I have logged in and have a valid session cookie
    When I request the service journey rules for my ODS Code
    Then I receive an "Ok" success code
    And the service journey rules response will have medical record version set to 2
