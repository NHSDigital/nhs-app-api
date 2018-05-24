Feature: Patient Verification

  The system validates the patient data

  @backend
  @bug @NHSO-922
  Scenario: Patient has single NHS Number
    Given I have valid credentials for a patient with one NHS Number
    When I verify patient data
    Then I receive the expected NHS Number

  @backend
  @bug @NHSO-922
  Scenario: Patient has multiple NHS Numbers
    Given I have valid credentials for a patient with multiple NHS Numbers
    When I verify patient data
    Then I receive the expected NHS Numbers

  @backend
  Scenario: Patient has no NHS Number
    Given I have valid credentials for a patient with no NHS Number
    When I verify patient data
    Then I receive no NHS Number

  @backend
  Scenario: Non-existent IM1 Connection Token
    Given I have an IM1 Connection Token that does not exist
    When I verify patient data
    Then I receive a "Bad Gateway" error

  @backend
  Scenario: IM1 Connection Token not in the expected format
    Given I have an IM1 Connection Token that is in an invalid format
    When I verify patient data
    Then I receive a "Bad Request" error

  @backend
  Scenario: No IM1 Connection Token
    Given I have no IM1 Connection Token
    When I verify patient data
    Then I receive a "Bad Request" error

  @backend
  Scenario: Non-existent ODS Code
    Given I have an ODS Code that does not exists
    When I verify patient data
    Then I receive a "Not Found" error

  @backend
  Scenario: ODS Code not in the expected format
    Given I have an ODS Code not in expected format
    When I verify patient data
    Then I receive a "Bad Request" error

  @backend
  Scenario: No ODS Code
    Given I have no ODS Code
    When I verify patient data
    Then I receive a "Bad Request" error

  @bug
  @pending
  @backend
  Scenario: EMIS is unavailable
    Given EMIS is unavailable
    When I verify patient data
    Then I receive an "Service Unavailable" error


