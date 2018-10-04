@prescription
Feature: Prescriptions submission
  A user can submit a request for a repeat prescription

  Background:
    Given the scenario is submit prescription

  @smoketest
Scenario Outline: The <GP System> user orders a repeat prescription with 5 entries
    Given <GP System> is initialised
    And I am using <GP System> GP System to submit my prescription
    And I have 1 historic prescriptions in this scenario
    And I am logged in
    And I navigate to prescriptions
    And I select 5 <GP System> repeatable prescriptions to order
    And I enter text "As soon as possible please" for special request
    And I click Continue on the Order a repeat prescription page
    When I click Confirm and order repeat prescription
    Then I see a order successful message on the Repeat prescription page with 6 prescriptions
    Examples:
    | GP System |
    | EMIS      |
    | TPP       |

  Scenario Outline: The <GP System> user orders a repeat prescription with 1 entries
    Given <GP System> is initialised
    And I am using <GP System> GP System to submit my prescription
    And I have 1 historic prescriptions in this scenario
    And I am logged in
    And I navigate to prescriptions
    And I select 1 <GP System> repeatable prescriptions to order
    And I click Continue on the Order a repeat prescription page
    When I click Confirm and order repeat prescription
    Then I see a order successful message on the Repeat prescription page with 2 prescriptions
    Examples:
    | GP System |
    | EMIS      |
    | TPP       |
