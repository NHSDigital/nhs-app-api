Feature: View prescriptions

  A user can view information about their prescriptions after logging in

  @backend
  @NHSO-945
  Scenario: Repeat prescription request with null body
    Given I have an empty repeat prescription request
    When I submit the repeat prescription
    Then I receive a "Bad Request" error

  @backend
  @NHSO-945
  Scenario: Repeat prescription request for valid courses
    Given I have a repeat prescription request with 1 courses
    And EMIS responds with a Created success code when submitting the repeat prescription
    When I submit the repeat prescription
    Then I receive a "Created" success code

  @backend
  @NHSO-945
  Scenario: Repeat prescription request with 0 courses
    Given I have a repeat prescription request with 0 courses
    When I submit the repeat prescription
    Then I receive a "Bad Request" error

  @backend
  @NHSO-945
  Scenario: Repeat prescription request with an invalid course id format
    Given I have a repeat prescription request with 100 courses
    And 1 invalid courses
    When I submit the repeat prescription
    Then I receive a "Bad Request" error

  @backend
  @NHSO-945
  Scenario: Repeat prescription request with an invalid course id
    Given I have a repeat prescription request with 1 courses
    But Emis responds with an error indicating a course is invalid
    When I submit the repeat prescription
    Then I receive a "Bad Request" error

  @backend
  @NHSO-945
  Scenario: Repeat prescription request with a course which has been ordered within the last 30 days
    Given I have a repeat prescription request with 1 courses
    But EMIS responds with an error indicating an included course has already been ordered in the last 30 days when submitting the repeat prescription
    When I submit the repeat prescription
    Then I receive a "Conflict" error

  @backend
  @NHSO-945
  Scenario: Session expired (redis)
    Given I have a repeat prescription request with 1 courses
    But I allow my session to expire
    When I submit the repeat prescription
    Then I receive a "Unauthorized" error

  @backend
  @NHSO-945
  Scenario: GP practice has disabled prescriptions functionality
    Given I have a repeat prescription request with 1 courses
    But EMIS responds with an error indicating prescriptions is not enabled when submitting the repeat prescription
    When I submit the repeat prescription
    Then I receive a "Forbidden" error

  @backend
  @NHSO-945
  Scenario: GP system returns unknown error
    Given I have a repeat prescription request with 1 courses
    But EMIS responds with an unknown internal server error when a repeat prescription is submitted
    When I submit the repeat prescription
    Then I receive a "Bad Gateway" error

  @backend
  @NHSO-945
  Scenario: GP system fails to return in a timely fashion
    Given I have a repeat prescription request with 1 courses
    But EMIS takes longer than 30 seconds to respond when a repeat prescription is submitted
    When I submit the repeat prescription
    Then I receive a "Gateway Timeout" error
