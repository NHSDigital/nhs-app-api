Feature: Prescriptions submission

  A user can submit a request for a repeat prescription

  Background:
    Given wiremock is initialised
    And the scenario is submit prescription
    And I have 1 historic prescriptions in this scenario
    And I am logged in
    And I navigate to prescriptions

  @NHSO-860
  @smoketest
  Scenario Outline: The User orders a repeat prescription with 5 entries
    Given I select 5 <GP System> repeatable prescriptions to order
    And I click Continue on the Order a repeat prescription page
    When I click Confirm and order repeat prescription
    Then I see a order successful message on the Repeat prescription page with the correct prescriptions
    Examples:
    | GP System |
    | EMIS      |

  @NHSO-860
  Scenario Outline: The User orders a repeat prescription with 1 entries
    Given I select 1 <GP System> repeatable prescriptions to order
    And I click Continue on the Order a repeat prescription page
    When I click Confirm and order repeat prescription
    Then I see a order successful message on the Repeat prescription page with the correct prescriptions
    Examples:
    | GP System |
    | EMIS      |


