@prescription
@native-smoketest
Feature: Prescriptions submission
  A user can submit a request for a repeat prescription

  Background:
    Given the scenario is submit prescription

  #HAPPY PATH JOURNIES

  Scenario Outline: The <GP System> user orders a repeat prescription with 5 entries
    And I am using <GP System> GP System to submit my prescription
    And I have 1 historic prescriptions in this scenario
    And I am logged in
    And I navigate to prescriptions
    And I select 5 repeatable prescriptions to order
    And I enter text "As soon as possible please" for special request
    And I click Continue on the Order a repeat prescription page
    When I click Confirm and order repeat prescription
    Then I see a order successful message on the Repeat prescription page with 6 prescriptions
  @smoketest
    Examples:
    | GP System |
    | TPP       |

  #FEATURE PATH JOURNIES

  Scenario Outline: The <GP System> user orders a repeat prescription with 5 entries
    And I am using <GP System> GP System to submit my prescription
    And I have 1 historic prescriptions in this scenario
    And I am logged in
    When I retrieve the 'My Prescriptions' page directly
    And I select 5 repeatable prescriptions to order
    And I enter text "As soon as possible please" for special request
    And I click Continue on the Order a repeat prescription page
    When I click Confirm and order repeat prescription
    Then I see a order successful message on the Repeat prescription page with 6 prescriptions
    Examples:
      | GP System |
      | EMIS      |
      | VISION    |

  Scenario Outline: The <GP System> user orders a repeat prescription with 1 entries
    And I am using <GP System> GP System to submit my prescription
    And I have 1 historic prescriptions in this scenario
    And I am logged in
    When I retrieve the 'My Prescriptions' page directly
    And I select 1 repeatable prescriptions to order
    And I click Continue on the Order a repeat prescription page
    When I click Confirm and order repeat prescription
    Then I see a order successful message on the Repeat prescription page with 2 prescriptions
    Examples:
    | GP System |
    | EMIS      |
    | TPP       |
    | VISION    |

  Scenario: The EMIS user tries to submit potentially dangerous text for special request
    And I am using EMIS GP System to submit my prescription
    And I have 0 historic prescriptions in this scenario
    And I am logged in
    When I retrieve the 'My Prescriptions' page directly
    And I select 1 repeatable prescriptions to order
    And I enter text "<script>" for special request
    And I click Continue on the Order a repeat prescription page
    When I click Confirm and order repeat prescription
    Then I see a message indicating there was an error sending my order

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
