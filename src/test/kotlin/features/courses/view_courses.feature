Feature: View courses

  In order to view courses associated with a user
  As a logged in user
  I want to see a list of repeat courses that I can order

  Background:
    Given wiremock is initialised
    And I have historic prescriptions
    And I am logged in
    And I navigate to prescriptions

  @pending
  Scenario: The User has repeatable prescriptions
    Given I have 10 assigned prescriptions
    And 10 of my prescriptions are of type repeat
    And 10 of my prescriptions can be requested
    When I click 'Order a repeat prescription'
    Then I see the available repeatable prescriptions

  @pending
  Scenario: The User has 0 repeatable prescriptions
    Given I have 0 assigned prescriptions
    And 0 of my prescriptions are of type repeat
    And 0 of my prescriptions can be requested
    When I click 'Order a repeat prescription'
    Then I see the available repeatable prescriptions

  @pending
  Scenario: The user has 1 repeatable prescription
    Given I have 1 assigned prescriptions
    And 1 of my prescriptions are of type repeat
    And 1 of my prescriptions can be requested
    When I click 'Order a repeat prescription'
    Then I see the available repeatable prescriptions

  @pending
  Scenario: The user should only see max 100 repeatable prescriptions
    Given I have 101 assigned prescriptions
    And 101 of my prescriptions are of type repeat
    And 101 of my prescriptions can be requested
    When I click 'Order a repeat prescription'
    Then I see the available repeatable prescriptions

  @pending
  Scenario: The user has the max number of repeatable prescriptions
    Given I have 100 assigned prescriptions
    And 100 of my prescriptions are of type repeat
    And 100 of my prescriptions can be requested
    When I click 'Order a repeat prescription'
    Then I see the available repeatable prescriptions

  @pending
  Scenario: The user has over 5 repeat dispense prescriptions
    Given I have 10 assigned prescriptions
    And 5 of my prescriptions are of type repeat
    And 3 of my prescriptions can be requested
    When I click 'Order a repeat prescription'
    Then I see the available repeatable prescriptions

  @NHSO-502
  Scenario: The User has selected repeat prescriptions to order
    And I select 5 repeatable prescriptions out of 5 available
    When I click Continue on the Order a repeat prescription page
    Then I see the previously selected prescriptions on the Confirm repeat prescription page

  @NHSO-502
  Scenario: The User has selected one repeat prescription to order
    Given I select 1 repeatable prescriptions out of 1 available
    When I click Continue on the Order a repeat prescription page
    Then I see the previously selected prescriptions on the Confirm repeat prescription page

  @NHSO-502
  Scenario: The User has selected no repeat prescriptions to order
    Given I select 0 repeatable prescriptions out of 5 available
    When I click Continue on the Order a repeat prescription page
    Then A validation message is displayed indicating the user has not selected any repeat prescriptions

  @NHSO-502
  Scenario: The User alters a repeat prescriptions selection and views previous selection
    Given I select 5 repeatable prescriptions out of 5 available
    And I click Continue on the Order a repeat prescription page
    When I click 'Change this repeat prescription' on the Prescription confirmation page
    Then I see my previously selected repeat prescriptions selected

  @NHSO-502
  Scenario: The User alters a repeat prescriptions selection and sees the updated confirmation
    Given I select 4 repeatable prescriptions out of 5 available
    And I click Continue on the Order a repeat prescription page
    When I click 'Change this repeat prescription' on the Prescription confirmation page
    And I select 1 additional repeat prescriptions
    When I click Continue on the Order a repeat prescription page
    Then I see the previously selected prescriptions on the Confirm repeat prescription page
