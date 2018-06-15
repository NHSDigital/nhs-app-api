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
