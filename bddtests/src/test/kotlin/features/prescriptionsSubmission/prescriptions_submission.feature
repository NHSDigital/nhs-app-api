@prescription

Feature: Prescriptions submission Frontend
  A user can submit a request for a repeat prescription

  Background:
    Given the scenario is submit prescription

  #This test covers navigation via buttons/links
  @nominatedPharmacy
  @smoketest
  Scenario: The TPP user orders a repeat prescription with 5 entries
    And I am using TPP GP System to submit my prescription
    And I have 1 historic prescriptions in this scenario
    And Azure organisation search is working
    And I am logged in
    And I navigate to prescriptions
    And I select 5 repeatable prescriptions to order
    And I enter text "As soon as possible please" for special request
    And I click Continue on the Order a repeat prescription page
    Then I cannot see any nominated pharmacy information
    When I click Confirm and order repeat prescription
    Then I see the Prescription Ordered success page
    And I click the Back to my prescriptions link
    Then I see the Repeat prescription page with 6 prescriptions

# These tests navigate directly to the pages where the features are to be tested, to save time.

  Scenario Outline: The <GP System> user orders a repeat prescription with 5 entries
    And I am using <GP System> GP System to submit my prescription
    And I have 1 historic prescriptions in this scenario
    And I am logged in
    When I retrieve the 'Your Prescriptions' page directly
    And I select 5 repeatable prescriptions to order
    And I enter text "As soon as possible please" for special request
    And I click Continue on the Order a repeat prescription page
    When I click Confirm and order repeat prescription
    Then I see the Prescription Ordered success page
    And I click the Back to my prescriptions link
    Then I see the Repeat prescription page with 6 prescriptions
    Examples:
      | GP System |
      | EMIS      |
      | VISION    |
      | MICROTEST |

  Scenario Outline: The <GP System> user orders a repeat prescription with 1 entries
    And I am using <GP System> GP System to submit my prescription
    And I have 1 historic prescriptions in this scenario
    And I am logged in
    When I retrieve the 'Your Prescriptions' page directly
    And I select 1 repeatable prescriptions to order
    And I click Continue on the Order a repeat prescription page
    When I click Confirm and order repeat prescription
    Then I see the Prescription Ordered success page
    And I click the Back to my prescriptions link
    Then I see the Repeat prescription page with 2 prescriptions
    Examples:
    | GP System |
    | EMIS      |
    | TPP       |
    | MICROTEST |
  @nativesmoketest
    Examples:
    | GP System |
    | VISION    |

  Scenario: The EMIS user tries to submit potentially dangerous text for special request
    And I am using EMIS GP System to submit my prescription
    And I have 0 historic prescriptions in this scenario
    And I am logged in
    When I retrieve the 'Your Prescriptions' page directly
    And I select 1 repeatable prescriptions to order
    And I enter text "<script>" for special request
    And I click Continue on the Order a repeat prescription page
    When I click Confirm and order repeat prescription
    Then I see a message indicating there was an error sending my order

  @nativesmoketest
  Scenario: The EMIS user should receive an error when they try to order a drug which they're ordered within the last 30 days
    And I am using EMIS GP System to submit my prescription
    And I have 1 historic prescriptions in this scenario
    And I am logged in
    When I retrieve the 'Your Prescriptions' page directly
    And I select 1 repeatable prescriptions to order
    And I click Continue on the Order a repeat prescription page
    But EMIS responds with an error indicating an included course has already been ordered in the last 30 days when submitting the repeat prescription
    When I click Confirm and order repeat prescription
    Then I see a message indicating I've previously ordered one of the selected medications within the last 30 days

  Scenario: The MICROTEST user should receive an error when all medications fail
    And I am using MICROTEST GP System to submit my prescription
    And I have 1 historic prescriptions in this scenario
    And I am logged in
    When I retrieve the 'Your Prescriptions' page directly
    And I select 1 repeatable prescriptions to order
    And I click Continue on the Order a repeat prescription page
    But GP system responds with a conflict error when a repeat prescription is submitted
    When I click Confirm and order repeat prescription
    Then I see a message indicating there was an error sending my order

  Scenario: The Microtest user see a partial success page if some of their orders fail
    And I am using MICROTEST GP System to submit my prescription
    And I have 1 historic prescriptions in this scenario
    And I am logged in
    When I retrieve the 'Your Prescriptions' page directly
    And I select 1 repeatable prescriptions to order
    And I click Continue on the Order a repeat prescription page
    But the GP system responds with an error indicating the order was partially successful
    When I click Confirm and order repeat prescription
    Then I can view which medications from my prescription order succeeded and failed

  Scenario: A user tries to navigate directly to the success page and is redirected back to the Prescriptions hub page
    Given I am a EMIS patient
    When I am logged in
    And I retrieve the 'prescriptions success' page directly
    Then I am redirected to the 'your prescriptions' page

    Scenario: A user tries to navigate directly to the microtest partial success page and is redirected back to the Prescriptions hub page
      Given I am an EMIS patient
      When I am logged in
      And I retrieve the 'partial success' page directly
      Then I am redirected to the 'your prescriptions' page
