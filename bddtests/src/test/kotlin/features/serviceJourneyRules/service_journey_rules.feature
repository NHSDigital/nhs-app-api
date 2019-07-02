@service-journey-rules
@backend
Feature: Service Journey Rules
  #This endpoint is GPSystem agnostic

  Scenario: A user can fetch the service journey rules for their ODS code
    Given I am a user whose ODS Code has a specific journey configuration set up
    And I have logged in and have a valid session cookie
    When I request the service journey rules for my ODS Code
    Then I receive an "Ok" success code
    And I receive the service journey rules response

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
  Scenario: API call for SJR will successfully return a response with appointments configured to IM1
    Given I am a user where the journey configuration for appointments is set to Im1
    And I have logged in and have a valid session cookie
    When I request the service journey rules for my ODS Code
    Then I receive an "Ok" success code
    And the service journey rules response will have appointments set to im1

  @backend
  Scenario: API call for SJR will successfully return a response with appointments configured to Informatica Frontdesk
    Given I am a user where the journey configuration for appointments is set to Informatica
    And I have logged in and have a valid session cookie
    When I request the service journey rules for my ODS Code
    Then I receive an "Ok" success code
    And the service journey rules response will have appointments set to informatica
