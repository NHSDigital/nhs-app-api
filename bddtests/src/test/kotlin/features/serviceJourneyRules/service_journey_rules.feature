@service-journey-rules
@backend
Feature: Service Journey Rules
  #This endpoint is GPSystem agnostic

  Scenario: A user fetching the service journey rules for an ODS code not configured will receive a 404
    Given I am a user whose ODS Code does not have specific journey configuration set up
    When I login but service journey rules has no configuration for my GP practice
    Then I receive a "Not Found" error

  @long-running
  Scenario: A user fetching the service journey rules but the session has expired, will receive Unauthorized
    Given I am a user whose ODS Code has a specific journey configuration set up
    And I have logged in and have a valid session cookie
    And My session has expired
    When I request the service journey rules for my ODS Code
    Then I receive a "Unauthorized" error

  @backend
  Scenario: API call for SJR will successfully return a response with appointments configured to IM1 and with nominated
  pharmacy enabled
    Given I am a user where the journey configurations are:
      | Journey            | Value   |
      | appointments       | im1     |
      | nominated pharmacy | enabled |
    And I have logged in and have a valid session cookie
    When I request the service journey rules for my ODS Code
    Then I receive an "Ok" success code
    And the service journey rules response will have appointments set to im1
    And the service journey rules response will have nominated pharmacy enabled

  @backend
  Scenario: API call for SJR will successfully return a response with appointments configured to Informatica Frontdesk
  and with nominated pharmacy disabled
    Given I am a user where the journey configurations are:
      | Journey            | Value       |
      | appointments       | informatica |
      | nominated pharmacy | disabled    |
    And I have logged in and have a valid session cookie
    When I request the service journey rules for my ODS Code
    Then I receive an "Ok" success code
    And the service journey rules response will have appointments set to informatica
    And the service journey rules response will have nominated pharmacy disabled
