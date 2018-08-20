@my-record
Feature: My Record - Clinical Abbreviations

  @NHSO-1795
  Scenario Outline: A patient can navigate to the clinical abbreviations page
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And I am on my record information page
    When I click the clinical abbreviations link
    Then a new tab opens https://beta.nhs.uk/

  Examples:
    |Service|
    |EMIS|