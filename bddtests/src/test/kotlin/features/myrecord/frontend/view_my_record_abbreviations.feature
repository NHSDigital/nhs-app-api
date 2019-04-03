@my-record
Feature: My Record - Clinical Abbreviations

  Scenario: A patient can navigate to the clinical abbreviations page
    Given the my record wiremocks are initialised for EMIS
    And the GP Practice has enabled demographics functionality
    And I am on my record information page
    When I click the clinical abbreviations link
    Then a new tab opens https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/medical-abbreviations/
