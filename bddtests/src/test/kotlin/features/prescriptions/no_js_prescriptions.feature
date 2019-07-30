@prescriptions
@noJs
Feature: Prescriptions Service With Javascript Disabled

  Background:
    Given I have disabled javascript
    And the scenario is submit prescription

  Scenario: The EMIS user orders a repeat prescription with 5 entries without javascript
    And I am using EMIS GP System to submit my prescription
    And I have 1 historic prescriptions in this scenario
    And I am logged in
    And I navigate to prescriptions
    And I select 5 repeatable prescriptions to order
    And I enter text "As soon as possible please" for special request
    And I click Continue on the Order a repeat prescription page
    When I click Confirm and order repeat prescription
    Then I see a order successful message on the Repeat prescription page with 6 prescriptions

  # just a single test to make sure errors are handled for prescription submission when javascript is not enabled
  Scenario: The EMIS user should receive an error when they try to order a drug which they're ordered within the last 30 days
    And I am using EMIS GP System to submit my prescription
    And I have 1 historic prescriptions in this scenario
    And I am logged in
    When I retrieve the 'My Prescriptions' page directly
    And I select 1 repeatable prescriptions to order
    And I click Continue on the Order a repeat prescription page
    But EMIS responds with an error indicating an included course has already been ordered in the last 30 days when submitting the repeat prescription
    When I click Confirm and order repeat prescription
    Then I see a message indicating I've previously ordered one of the selected medications within the last 30 days
