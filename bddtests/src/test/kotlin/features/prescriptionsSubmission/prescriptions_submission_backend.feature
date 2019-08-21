@prescription
@backend
Feature: Prescriptions Submission Backend
  A user can view information about their prescriptions after logging in


  Scenario: Repeat prescription request with null body
    Given I have an empty repeat prescription request
    And I have logged into EMIS and have a valid session cookie
    When I submit the repeat prescription
    Then I receive a "Bad Request" error


  Scenario: Repeat prescription request for valid courses
    Given I have a repeat prescription request with 1 courses
    And EMIS responds with a Created success code when submitting the repeat prescription
    And I have logged into EMIS and have a valid session cookie
    When I submit the repeat prescription
    Then I receive a "Created" success code


  Scenario: Repeat prescription request with 0 courses
    Given I have a repeat prescription request with 0 courses
    And I have logged into EMIS and have a valid session cookie
    When I submit the repeat prescription
    Then I receive a "Bad Request" error


  Scenario: Repeat prescription request with an invalid course id format
    Given I have a repeat prescription request with 100 courses
    And 1 invalid courses
    And I have logged into EMIS and have a valid session cookie
    When I submit the repeat prescription
    Then I receive a "Bad Request" error


  Scenario: Repeat prescription request with an invalid course id
    Given I have a repeat prescription request with 1 courses
    But Emis responds with an error indicating a course is invalid
    And I have logged into EMIS and have a valid session cookie
    When I submit the repeat prescription
    Then I receive a "Bad Request" error


  Scenario: Repeat prescription request with a course which has been ordered within the last 30 days
    Given I have a repeat prescription request with 1 courses
    But EMIS responds with an error indicating an included course has already been ordered in the last 30 days when submitting the repeat prescription
    And I have logged into EMIS and have a valid session cookie
    When I submit the repeat prescription
    Then I receive a "466" error status code

  Scenario: Session expired (redis)
    Given I have a repeat prescription request with 1 courses
    And I have logged into EMIS and have a valid session cookie
    But I allow my session to expire
    When I submit the repeat prescription
    Then I receive a "Unauthorized" error


  Scenario: GP practice has disabled prescriptions functionality
    Given I have a repeat prescription request with 1 courses
    But EMIS responds with an error indicating prescriptions is not enabled when submitting the repeat prescription
    And I have logged into EMIS and have a valid session cookie
    When I submit the repeat prescription
    Then I receive a "Forbidden" error


  Scenario: GP system returns unknown error
    Given I have a repeat prescription request with 1 courses
    But EMIS responds with an unknown internal server error when a repeat prescription is submitted
    And I have logged into EMIS and have a valid session cookie
    When I submit the repeat prescription
    Then I receive a "Bad Gateway" error


  Scenario: GP system fails to return in a timely fashion
    Given I have a repeat prescription request with 1 courses
    But EMIS takes longer than 30 seconds to respond when a repeat prescription is submitted
    And I have logged into EMIS and have a valid session cookie
    When I submit the repeat prescription
    Then I receive a "Gateway Timeout" error with service desk reference prefixed "ze"
