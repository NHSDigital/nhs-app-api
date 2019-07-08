@my-record
Feature: My Record - Clinical Abbreviations

  Scenario: A patient can navigate to the clinical abbreviations page
    Given the my record wiremocks are initialised for EMIS
    And the GP Practice has enabled demographics functionality
    And I am on my record information page
    When I click the link called 'Help with abbreviations' with a url of 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/abbreviations/'
    Then a new tab has been opened by the link
