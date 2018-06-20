Feature: View prescriptions

  A user can view information about their prescriptions after logging in

  Background:
    Given wiremock is initialised

  Scenario: A user can see the prescriptions menu button
    Given I am logged in
    Then I see the prescriptions menu button

  Scenario: A user selects the prescriptions menu button
    Given I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    When I am on the prescriptions page
    Then I see prescriptions page loaded
    And the prescriptions menu button is highlighted

  @smoketest
  Scenario: A user with no past repeat prescriptions
    Given I have 0 past repeat prescriptions
    And each repeat prescription contains 0 courses of which 0 are repeats
    When I am on the prescriptions page
    Then I see no prescriptions
    And I see a message indicating that I have no repeat prescriptions

  Scenario: A user who has prescriptions totalling more than one hundred courses
    Given I have 110 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    When I am on the prescriptions page
    Then I see 100 prescriptions

  @smoketest
  Scenario: A user who has multiple prescription each containing one course
    Given I have 3 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    When I am on the prescriptions page
    Then I see 3 prescriptions

  Scenario: A user who has multiple prescription each containing the same repeat prescription
    Given I have 3 past repeat prescriptions
    And each repeat prescription shares the same course
    When I am on the prescriptions page
    Then I see 3 prescriptions

  Scenario: A user who has only one prescription containing multiple courses
    Given I have 1 past repeat prescriptions
    And each repeat prescription contains 3 courses of which 3 are repeats
    When I am on the prescriptions page
    Then I see 3 prescriptions

  Scenario: A user who has acute prescriptions
    Given I have 1 past repeat prescriptions
    And each repeat prescription contains 3 courses of which 2 are repeats
    When I am on the prescriptions page
    Then I see 2 prescriptions

  @NHSO-556
  Scenario: The User clicks on the Prescriptions button and the service is disabled at a GP Practice level
    Given prescriptions is disabled at a GP Practice level
    When I am on the prescriptions page
    Then I see a message informing me that I don't currently have access to this service

  @NHSO-599
  @backend
  Scenario: Requesting prescriptions with correct data returns a list of prescriptions when a patient had repeat prescriptions in the last 6 months (Date 6 months ago provided)
    Given I have logged in and have a valid session cookie
    And From date is 6 months ago and I have 10 prescriptions in the last 6 months
    When I get the users prescriptions with a valid cookie
    Then I receive a list of 10 prescriptions

  @NHSO-946
  @backend
  Scenario: Repeat prescriptions in the last 6 months and no fromDate
    Given I have logged in and have a valid session cookie
    And I have a patient
    And From date is 6 months ago and I have 10 prescriptions in the last 6 months
    But I do not request a fromDate
    When I request prescriptions for the last 6 months
    Then I get a response with a list of prescriptions for the last 6 months

  @NHSO-946
  @backend
  Scenario: fromDate in the future
    Given I have logged in and have a valid session cookie
    And I have a patient
    But a fromDate in the future
    When I request prescriptions for the last 6 months
    Then I get a response with a list of prescriptions for the last 6 months

  @NHSO-946
  @backend
  Scenario: fromDate greater than 6 months ago
    Given I have logged in and have a valid session cookie
    And I have a patient
    But a fromDate greater than 6 months ago
    When I request prescriptions for the last 6 months
    Then I get a response with a list of prescriptions for the last 6 months

  @NHSO-946
  @backend
  Scenario: fromDate not in the expected format
    Given I have logged in and have a valid session cookie
    And I have a patient
    But a fromDate in an unexpected format
    When I request prescriptions for the last 6 months
    Then I receive a "Bad request" error

  @NHSO-946
  @backend
  Scenario: Missing cookie
    Given I have a patient
    But no cookie
    When I request prescriptions for the last 6 months
    Then I receive a "Unauthorized" error

  @NHSO-946
  @backend
  Scenario: NHSO-Session-Id not in the expected format
    Given I have a patient
    When I request prescriptions for the last 6 months with an invalid cookie
    Then I receive a "Unauthorized" error

  @NHSO-946
  @backend
  Scenario: Session expired
    Given I have logged in and have a valid session cookie
    And I have a patient
    But I allow my session to expire
    When I request prescriptions for the last 6 months
    Then I receive an "Unauthorized" error

  @NHSO-946
  @backend
  Scenario: GP practice has disabled prescriptions functionality
    Given I have logged in and have a valid session cookie
    And I have a patient
    But the GP System has disabled prescriptions
    When I request prescriptions for the last 6 months
    Then I receive a "Forbidden" error

  @NHSO-946
  @backend
  Scenario: GP system fails to return in a timely fashion
    Given I have logged in and have a valid session cookie
    And I have a patient
    But the GP System is too slow
    When I request prescriptions for the last 6 months
    Then I receive a "Gateway Timeout" error