@my-record
Feature: Clinical Abbreviations Frontend - Medical Record v1

  Scenario: A patient can navigate to the clinical abbreviations page - Medical Record v1
    Given I am a EMIS user setup to use medical record version 1
    And I am on the medical record page
    When I click the link called 'Help with abbreviations' with a url of 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/abbreviations/'
    Then a new tab has been opened by the link
