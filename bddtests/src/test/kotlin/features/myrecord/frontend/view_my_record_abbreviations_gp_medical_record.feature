@my-record
Feature: Clinical Abbreviations Frontend - GP Medical Record

  Scenario: A patient can navigate to the clinical abbreviations page - GP Medical Record
    Given I am a EMIS user setup to use medical record version 2
    And the GP Practice has enabled allergies functionality and has a drug and non drug allergy record for VISION - GP Medical Record
    And I am logged in
    And I am on my record information page and glossary is visible - GP Medical Record
    When I click the link called 'Help with abbreviations' with a url of 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/abbreviations/'
    Then a new tab has been opened by the link
