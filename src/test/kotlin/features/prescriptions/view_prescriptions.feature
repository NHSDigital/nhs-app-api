Feature: View prescriptions

  A user can view information about their prescriptions after logging in

  Background:
    Given wiremock is initialised

  Scenario: A user can see the prescriptions menu button
    Given I am logged in
    Then I see the prescriptions menu button

  @bug @NHSO-922
  Scenario: A user selects the prescriptions menu button
    Given I am logged in
    When I navigate to prescriptions
    Then I see prescriptions page loaded
    And the prescriptions menu button is highlighted

  @pending
  Scenario: An EMIS user with no past repeat prescriptions opens the page
    Given I am logged in
    When I navigate to prescriptions
    Then I see a message indicating that I have no repeat prescriptions

  @pending
  Scenario: An EMIS user with 1 or more past repeat prescriptions > 6 months old opens the page
    Given I am logged in
    When I navigate to prescriptions
    Then I see a message indicating that I have no repeat prescriptions


  @pending
  @NHSO-599
  @backend
  Scenario: No repeat prescriptions in the last 6 months
    Given I have a patient
    And the patient has no prescriptions in the last 6 months
    When I request prescriptions for the last 6 months
    Then I get a response with an empty list of prescriptions

  @pending
  @NHSO-599
  @backend
  Scenario: Repeat prescriptions in the last 6 months
    Given I have a patient
    And the patient has repeat prescriptions in the last 6 months
    When I request prescriptions for the last 6 months
    Then I get a response with a list of prescriptions for the last 6 months

  @pending
  @NHSO-599
  @backend
  Scenario: Repeat prescriptions outside of the last 6 months
    Given I have a patient
    And the patient has repeat prescriptions outside the last 6 months
    When I request prescriptions for the last 6 months
    Then I get a response with an empty list of prescriptions

  @pending
  @NHSO-599
  @backend
  Scenario: No repeat prescriptions in the last 3 months and a fromDate
    Given I have a patient
    And the patient has no prescriptions in the last 3 months
    And I request a fromDate of <now-3 months>
    When I request prescriptions for the last 3 months
    Then I get a response with an empty list of prescriptions

  @pending
  @NHSO-599
  @backend
  Scenario: Rrepeat prescriptions in the last 3 months and a fromDate
    Given I have a patient
    And the patient has repeat prescriptions in the last 3 months
    And I request a fromDate of <now-2 months>
    When I request prescriptions for the last 3 months
    Then I get a response with a list of prescriptions for the last 2 months

  @pending
  @NHSO-599
  @backend
  Scenario: No repeat prescriptions in the last 6 months and no fromDate
    Given I have a patient
    And the patient has no prescriptions in the last 6 months
    But I do not request a fromDate
    When I request prescriptions for the last 6 months
    Then I get a response with an empty list of prescriptions


  @pending
  @NHSO-599
  @backend
  Scenario: Repeat prescriptions in the last 6 months and no fromDate
    Given I have a patient
    And the patient has no prescriptions in the last 6 months
    But I do not request a fromDate
    When I request prescriptions for the last 6 months
    Then I get a response with a list of prescriptions for the last 6 months


  @pending @NHSO-498
  @backend
  Scenario: fromDate not in the expected format
    Given I have a patient
    But a fromDate in an unexpected format
    When I request prescriptions for the last 6 months
    Then I get a "Bad request" error

  @pending @NHSO-498
  @backend
  Scenario: Missing cookie
    Given I have a patient
    But no cookie
    When I request prescriptions for the last 6 months
    Then I get a "Bad request" error

  @pending @NHSO-498
  @backend
  Scenario: NHSO-Session-Id not in the expected format
    Given I have a patient
    But a NHSO-Session-Id in an unexpected format
    When I request prescriptions for the last 6 months
    Then I get a "Bad request" error

  @pending @NHSO-498
  @backend
  Scenario: fromDate in the future
    Given I have a patient
    But a fromDate in the future
    When I request prescriptions for the last 6 months
    Then I get a "Bad request" error

  @pending @NHSO-498
  @backend
  Scenario: fromDate greater than 6 months ago
    Given I have a patient
    But a fromDate greater than 6 months ago
    When I request prescriptions for the last 6 months
    Then I get a "Bad request" error

  @manual @NHSO-498
  @backend
  Scenario: Cache is unavailable
    Given I have a patient
    But the cache is unavailable
    When I request prescriptions for the last 6 months
    Then I get a "Server Error" error

  @manual @NHSO-498
  @backend
  Scenario: Cache is unavailable
    Given I have a patient
    But there is a cache issue
    When I request prescriptions for the last 6 months
    Then I get a "Server Error" error

  @pending @NHSO-498
  @backend
  Scenario: Missing data
    Given I have a patient
    But missing data
    When I request prescriptions for the last 6 months
    Then I get a "Bad Gateway" error

  @pending @NHSO-498
  @backend
  Scenario: Invalid Data
    Given I have a patient
    But invalid data
    When I request prescriptions for the last 6 months
    Then I get a "Server Error" error

  @pending @NHSO-498
  @backend
  Scenario: Missing headers
    Given I have a patient
    But missing headers
    When I request prescriptions for the last 6 months
    Then I get a "Bad Gateway" error

  @pending @NHSO-498
  @backend
  Scenario: Request prescriptions with malformed data from EMIS handled appropriately (Review when NHSO-498 has been implemented)

  @pending @NHSO-498
  @backend
  Scenario: Session expired
    Given I have a patient
    But their session has expired
    When I request prescriptions for the last 6 months
    Then I get a "Unauthorized" error


  @pending @NHSO-498
  @backend
  Scenario: GP system is unavailable
    Given I have a patient
    But the GP System is unavailable
    When I request prescriptions for the last 6 months
    Then I get a "Service Unavailable" error

  @pending @NHSO-498
  @backend
  Scenario: GP practice has disabled prescriptions functionality
    Given I have a patient
    But the GP System has disabled prescriptions
    When I request prescriptions for the last 6 months
    Then I get a "Forbidden" error

  @pending @NHSO-498
  @backend
  Scenario: GP system fails to return in a timely fashion
    Given I have a patient
    But the GP System is too slow
    When I request prescriptions for the last 6 months
    Then I get a "Gateway Timeout" error